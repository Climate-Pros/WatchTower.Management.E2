using System.Diagnostics;
using ServiceStack;
using ServiceStack.Script;
using WatchTowerManagementE2.ServiceInterface.Commands;
using WatchTowerManagementE2.ServiceModel;
using WatchTowerManagementE2.ServiceModel.Commands.GetMultiExpandedStatus;

namespace WatchTowerManagementE2.ServiceInterface;

public class CommandServices() : Service
{
    public ICommandExecutor CommandExecutor { get; set; }

    /*public async Task<object> Any(GetControllerList request)
    {
        var result = await CommandExecutor.ExecuteWithResultAsync(new GetControllerListCommand { LocationId = request.LocationId}, request);

        return new GetControllerListResponse
        {
            Result = result
        };
    }*/

    /*public async Task<object> Any(GetCellList request)
    {
        await request.ExecuteAsync(new GetCellListCommand(request.LocationId) { ControllerName = request.ControllerName });

        return new GetCellListResponse
        {
            Result = request.Result
        };
    }*/
    
    public async Task<object> Any(GetMultiExpandedStatus request)
    {
        var sw = Stopwatch.StartNew();
        var applicationTypes = ((GetApplicationTypesResponse) await ResolveService<ReferenceDataServices>().Any(new GetApplicationTypes())).Result;
        var applicationGroups = ((GetApplicationGroupsResponse) await ResolveService<ReferenceDataServices>().Any(new GetApplicationGroups())).Result;
        //var cells = ((GetCellListResponse)await Any(new GetCellList() { LocationId = request.LocationId, ControllerName = request.ControllerName })).Result;
        var cells = await CommandExecutor.ExecuteWithResultAsync(new GetCellListCommand { LocationId = request.LocationId }, new GetCellList { LocationId = request.LocationId, ControllerName = request.ControllerName });
        
        var points = new List<string>();
        
        cells.Each(cell =>
        {
            var applicationGroupName = applicationTypes.SingleOrDefault(_ => _.Value.ToString() == cell.CellType.ToString()).Key;
            var applicationGroup = applicationGroups.SingleOrDefault(_ => _.Key == applicationGroupName);
           
            if (applicationGroup.Key == null) return;
            
            var variables = applicationGroup.Value;
            
            variables.Each(variable =>
            {
                points.Add($"{request.ControllerName}:{cell.CellName}:{variable}");
            });                
        });

        var batchResults = new List<MultiExpandStatus>();
        var pointBatches = points.BatchesOf(10);
        
        await pointBatches.EachAsync (async (batch, idx) =>
        {
            request.Points = batch.ToList();
            
            var values = await CommandExecutor.ExecuteWithResultAsync(new GetMultiExpandedStatusCommand { LocationId = request.LocationId }, request);
            
            batchResults.AddRange(values);
        });
        
        sw.Stop();
        
        return new GetMultiExpandedStatusResponse
        {
            Stopwatch = sw.Elapsed.ToString(),
            Result = [batchResults.Where(_ => _.Value is not null).ToList()]
        };
    }
}