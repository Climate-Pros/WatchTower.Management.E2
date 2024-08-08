using System.Collections.Concurrent;
using System.Diagnostics;
using ServiceStack;
using ServiceStack.Html;
using WatchTower.Management.Devices.E2.ServiceModel;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetCellList;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetMultiExpandedStatus;
using WatchTower.Management.Devices.Shared;
using static WatchTower.Management.Devices.E2.ServiceModel.Commands.Types;
using Result = WatchTower.Management.Devices.E2.ServiceModel.Commands.GetMultiExpandedStatus.Result;

namespace WatchTower.Management.Devices.E2.ServiceInterface.Commands;

public class GetMultiExpandedStatusCommand : GetMultiExpandedStatus
{
    public override async Task ExecuteAsync(GetMultiExpandedStatus request)
    {
        var sw = Stopwatch.StartNew();
        var results = await CommandExecutor.ExecuteWithResultAsync(new GetCellListCommand(), new GetCellList { LocationId = request.LocationId, ControllerName = request.ControllerName });
        var cells = results.Result.Data;
        
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

        await Parallel.ForEachAsync(pointBatches.AsParallel(), new ParallelOptions() { MaxDegreeOfParallelism = 1 }, async (batch, idx) =>
        {

            request.Points = batch.ToList();

            var response = await ExecuteCommand
            (
                GetEndpointByLocationId(request.LocationId),
                "E2.GetMultiExpandedStatus",
                [batch.ToObjects()],
                json => json.FromJson<GetMultiExpandedStatusResult>()
            );

            var variables = response.Result.Data;
            
            variables.Each(_variable =>
            {
                FormatCell(ref _variable, cells.ToList());

                batchResults.Add(_variable);
            });
            
            response = null;

        });
        sw.Stop();
        
        Result = new GetMultiExpandedStatusResult
        {
            Result = new Result
            {
                Data = batchResults.OrderBy(_ => _.Prop).ToList()
            }
        };
        
        batchResults = null;
   }

    private static void FormatCell(ref MultiExpandStatus _, List<Cell> cellList)
    {
        var parts = _.Prop.Split(":");
        var cell = cellList.SingleOrDefault(cell => cell.CellName == parts[1]);
        var cellType = cell is not null ? $"{cell?.CellType}:" : "";

        _.Prop = $"{parts[0]}:{cellType}{parts[1]}:{parts[2]}";
    }
}