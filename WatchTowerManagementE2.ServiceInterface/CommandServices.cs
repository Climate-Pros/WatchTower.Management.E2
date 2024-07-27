using System.Diagnostics;
using ServiceStack;
using ServiceStack.Script;
using ServiceStack.Text;
using WatchTowerManagementE2.ServiceInterface.Commands;
using WatchTowerManagementE2.ServiceInterface.Commands.Types;
using WatchTowerManagementE2.ServiceModel;
using WatchTowerManagementE2.ServiceModel.Commands.Enums;
using WatchTowerManagementE2.ServiceModel.Commands.GetMultiExpandedStatus;
using WatchTowerManagementE2.ServiceModel.Types;

namespace WatchTowerManagementE2.ServiceInterface;

public class CommandServices() : Service
{
    public ICommandExecutor CommandExecutor => HostContext.Resolve<ICommandExecutor>();

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
        var applicationTypes = Gateway.Send(new GetApplicationTypes()).Result;
        var applicationGroups = Gateway.Send(new GetApplicationGroups()).Result;
        //var cells = ((GetCellListResponse)await Any(new GetCellList() { LocationId = request.LocationId, ControllerName = request.ControllerName })).Result;
        var cells = await CommandExecutor.ExecuteWithResultAsync(new GetCellListCommand { LocationId = request.LocationId }, new GetCellList { LocationId = request.LocationId, ControllerName = request.ControllerName });
        
        var points = new List<string>();
        
        cells.Each(cell =>
        {
            var applicationGroupName = applicationTypes.SingleOrDefault(_ => _.Type.ToString() == cell.CellType.ToString())?.Name;

            if (applicationGroupName is null)
            {
                $"Cannot find an application group for cell type: {cell.CellType}".Print();
                return;
            }

            var applicationGroup = applicationGroups.SingleOrDefault(_ => _.Name == applicationGroupName);
           
            if (applicationGroup?.Name is null)
            {
                $"Cannot find an application group name for {applicationGroupName}".Print();
                return;
            }
            
            var variables = applicationGroup?.Variables;

            if (variables is not null)
            {
                variables.Each(variable =>
                {
                    points.Add($"{request.ControllerName}:{cell.CellName}:{variable}");
                });
            }
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