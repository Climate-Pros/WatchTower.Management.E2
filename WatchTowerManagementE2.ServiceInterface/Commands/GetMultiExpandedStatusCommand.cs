using System.Diagnostics;
using ServiceStack;
using ServiceStack.Html;
using WatchTowerManagementE2.ServiceInterface.Commands.Types;
using WatchTowerManagementE2.ServiceModel;
using WatchTowerManagementE2.ServiceModel.Commands.GetControllerList;
using WatchTowerManagementE2.ServiceModel.Commands.GetMultiExpandedStatus;

namespace WatchTowerManagementE2.ServiceInterface.Commands;

public class GetMultiExpandedStatusCommand : E2Command<GetMultiExpandedStatus, List<MultiExpandStatus>>
{
    public ICommandExecutor CommandExecutor => HostContext.Resolve<ICommandExecutor>();
    public JsonApiClient Client { get; set; }

    public int LocationId { get; set; }
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
            var applicationGroupName = ApplicationTypes.SingleOrDefault(_ => _.Value.ToString() == cell.CellType.ToString()).Key;
            var applicationGroup = ApplicationGroups.SingleOrDefault(_ => _.Key == applicationGroupName);

            if (applicationGroup.Key == null) return;

            var variables = applicationGroup.Value;

            variables.Each(variable => { points.Add($"{request.ControllerName}:{cell.CellName}:{variable}"); });
        });

        var batchResults = new List<MultiExpandStatus>();
        var pointBatches = points.BatchesOf(10);

        //await Parallel.ForEachAsync( pointBatches.AsParallel(), new ParallelOptions(){MaxDegreeOfParallelism = 2}, async (batch, idx) =>
        await pointBatches.EachAsync( async (batch, i) => 
        {
            request.Points = batch.ToList();

            var values = await CommandExecutor.ExecuteWithResultAsync(new GetMultiExpandedStatusCommand(), request);

            batchResults.AddRange(values);
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
        );*/

        //Result = result.Result.Results;
    }
}