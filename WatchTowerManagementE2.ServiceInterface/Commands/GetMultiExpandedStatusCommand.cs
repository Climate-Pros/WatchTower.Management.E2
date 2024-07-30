using System.Collections.Concurrent;
using System.Diagnostics;
using ServiceStack;
using ServiceStack.Html;
using ServiceStack.Text;
using WatchTowerManagementE2.ServiceInterface.Commands.Types;
using WatchTowerManagementE2.ServiceModel;
using WatchTowerManagementE2.ServiceModel.Commands.GetCellList;
using WatchTowerManagementE2.ServiceModel.Commands.GetControllerList;
using WatchTowerManagementE2.ServiceModel.Commands.GetMultiExpandedStatus;

namespace WatchTowerManagementE2.ServiceInterface.Commands;

public class GetMultiExpandedStatusCommand : E2Command<GetMultiExpandedStatus, List<MultiExpandStatus>>
{
    [Input(Type=Input.Types.Text, Value = "10753")]
    public int LocationId { get; set; }
    
    [Input(Type=Input.Types.Text, Value = "E2 Unit01")]
    public string ControllerName { get; set; }
    
    [Input(Type = Input.Types.Hidden)]
    public List<string> Points { get; set; }

    public override async Task ExecuteAsync(GetMultiExpandedStatus request)
    {
        var sw = Stopwatch.StartNew();
        var cells = await CommandExecutor.ExecuteWithResultAsync(new GetCellListCommand(), new GetCellList { LocationId = request.LocationId, ControllerName = request.ControllerName });

        cells.Each(cell =>
        {
            var applicationGroupName = ApplicationTypes.SingleOrDefault(_ => _.Type.ToString() == cell.CellType.ToString());

            if (applicationGroupName?.Name is null) return;
            
            var applicationGroup = ApplicationGroups.SingleOrDefault(_ => _.Name == applicationGroupName.Name);

            if (applicationGroup?.Name == null) return;

            var variables = applicationGroup.Variables;

            variables.Each(variable => { request.Points.Add($"{request.ControllerName}:{cell.CellName}:{variable}"); });
        });

        var batchResults = new ConcurrentBag<MultiExpandStatus>();
        var pointBatches = request.Points.BatchesOf(20);

        await Parallel.ForEachAsync( pointBatches.AsParallel(), new ParallelOptions(){  MaxDegreeOfParallelism = 1 }, async (batch, idx) =>
        //await pointBatches.EachAsync( async (batch, i) => 
        {
            request.Points = batch.ToList();

            //var values = await CommandExecutor.ExecuteWithResultAsync(new GetMultiExpandedStatusCommand(), request);
            var result = await ExecuteE2Command<GetMultiExpandedStatusResult>
            (
                GetEndpointByLocationId(request.LocationId),
                "E2.GetMultiExpandedStatus",
                json => json.FromJson<GetMultiExpandedStatusResult>(),
                [batch.ToObjects()]
            );

            var innerCellList = cells;
            
            result.Result.Results.Each(innerCell =>
            {
                FormatCell(ref innerCell, innerCellList);

                batchResults.Add(innerCell);
            });

            innerCellList = null;
            result = null;
        });

        sw.Stop();

        Result = batchResults.OrderBy(_ => _.Prop).ToList();

        cells = null;
        sw = null;
        batchResults = null;
        
        GC.Collect();
    }

    private static void FormatCell(ref MultiExpandStatus _, List<Cell> cellList)
    {
        var parts = _.Prop.Split(":");
        var cell = cellList.SingleOrDefault(cell => cell.CellName == parts[1]);
        var cellType = cell is not null ? $"{cell?.CellType}:" : "";

        _.Prop = $"{parts[0]}:{cellType}{parts[1]}:{parts[2]}";
    }
}