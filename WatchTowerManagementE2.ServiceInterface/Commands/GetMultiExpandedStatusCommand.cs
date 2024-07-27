using System.Diagnostics;
using ServiceStack;
using ServiceStack.Html;
using ServiceStack.Text;
using WatchTowerManagementE2.ServiceInterface.Commands.Types;
using WatchTowerManagementE2.ServiceModel;
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

    public async Task ExecuteAsync(GetMultiExpandedStatus request)
    {
        var sw = Stopwatch.StartNew();
        //var cells = ((GetCellListResponse)await Any(new GetCellList() { LocationId = request.LocationId, ControllerName = request.ControllerName })).Result;
        var cells = await CommandExecutor.ExecuteWithResultAsync(new GetCellListCommand(), new GetCellList { LocationId = request.LocationId, ControllerName = request.ControllerName });
        
        var points = new List<string>();

        cells.Each(cell =>
        {
            var applicationGroupName = ApplicationTypes.SingleOrDefault(_ => _.Type.ToString() == cell.CellType.ToString());

            if (applicationGroupName?.Name is null) return;
            
            var applicationGroup = ApplicationGroups.SingleOrDefault(_ => _.Name == applicationGroupName.Name);

            if (applicationGroup?.Name == null) return;

            var variables = applicationGroup.Variables;

            variables.Each(variable => { request.Points.Add($"{request.ControllerName}:{cell.CellName}:{variable}"); });
        });

        var batchResults = new List<MultiExpandStatus>();
        var pointBatches = request.Points.BatchesOf(10);

        //await Parallel.ForEachAsync( pointBatches.AsParallel(), new ParallelOptions(){MaxDegreeOfParallelism = 2}, async (batch, idx) =>
        await pointBatches.EachAsync( async (batch, i) => 
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
            
            batchResults.AddRange(result.Result.Results);
        });

        sw.Stop();

        Result = batchResults.ToList(); /*new GetMultiExpandedStatusResponse
        {
            Stopwatch = sw.Elapsed.ToString(),
            Result = [batchResults.Where(_ => _.Value is not null).ToList()]
        };*/

        /*var result = await ExecuteE2Command<GetMultiExpandedStatusResult>
        (
            GetEndpointByLocationId(request.LocationId),
            "E2.GetMultiExpandedStatus",
            json => json.FromJson<GetMultiExpandedStatusResult>(),
            [request.Points]
        );

        Result = result.Result.Results;*/
    }
}