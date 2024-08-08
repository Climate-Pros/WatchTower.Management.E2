using System.Diagnostics;
using ServiceStack;
using ServiceStack.Text;
using WatchTower.Management.Devices.E2.ServiceInterface.Commands;
using WatchTower.Management.Devices.E2.ServiceModel;
using WatchTower.Management.Devices.E2.ServiceModel.Commands.GetMultiExpandedStatus;

namespace WatchTower.Management.Devices.E2.ServiceInterface;

public class CommandServices() : Service
{
    public ICommandExecutor CommandExecutor => HostContext.Resolve<ICommandExecutor>();

    public async Task<object> Any(GetMultiExpandedStatus request)
    {
        var sw = Stopwatch.StartNew();
        var applicationTypes = Gateway.Send(new GetApplicationTypes()).Result;
        var applicationGroups = Gateway.Send(new GetApplicationGroups()).Result;

        var cellListRequest = new GetCellListCommand();
        
        await cellListRequest.ExecuteAsync( new GetCellList
        {
            LocationId = request.LocationId,
            ControllerName = request.ControllerName
        });
        
        var response = cellListRequest.Result
            ?.Result
            ?.Data
        ;
        
        var points = new List<string>();
        
        response?.Each(cell =>
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
            
            batchResults.AddRange(values.Result.Data);
        });
        
        sw.Stop();
        
        return new GetMultiExpandedStatusResponse
        {
            Stopwatch = sw.Elapsed.ToString(),
            Result = new GetMultiExpandedStatusResult
            {
                Result = new Result 
                {
                    Data = batchResults.Where(_ => _.Value is not null).ToList()
                }
            }
        };
    }
}