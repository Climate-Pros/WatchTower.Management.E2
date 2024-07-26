namespace WatchTowerManagementE2.Jobs;

public class RecurringJobs 
{
    #region Examples
    
    /*
    /// <summary>
    /// 
    /// </summary>
    [RecurringJob("#1#15 * * * *", "UTC", "cache", RecurringJobId = "update_accounts")]
    public void UpdateAccounts()
    {
        try
        {
            var result = HostContext.AppHost.ExecuteService(new ExportAccounts());

            Console.WriteLine("Successfully initiated product catalog export");
            Console.WriteLine($"Results: {result.ToJson()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to initiated product catalog export");
            ex.ToExceptionless().Submit();
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    [RecurringJob("#1#15 * * * *", "UTC", "cache", RecurringJobId = "update_opportunities")]
    public void UpdateOpportunities()
    {
        try
        {
            var result = HostContext.AppHost.ExecuteService(new ExportOpportunitiesV2());

            Console.WriteLine("Successfully initiated opportunity export");
            Console.WriteLine($"Results: {result.ToJson()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to initiated opportunity export");
            ex.ToExceptionless().Submit();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [RecurringJob("#1#15 * * * *", "UTC", "cache", RecurringJobId = "update_subscriptions")]
    public void UpdateSubscriptions()
    {
        try
        {
            var result = HostContext.AppHost.ExecuteService(new ExportSubscriptionsV2());

            Console.WriteLine("Successfully initiated subscription export");
            Console.WriteLine($"Results: {result.ToJson()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to initiated opportunity export");
            ex.ToExceptionless().Submit();
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    [RecurringJob("0 * * * *", "UTC", "cache", RecurringJobId = "update_product_catalog")]
    public void UpdateProductCatalog()
    {
        try
        {
            HostContext
                .GetPlugin<NatsPlugin>()
                .Publish
                (
                    "webhooks.callback",
                    new WebhookEvent()
                    {
                        Id          = Guid.NewGuid(),
                        Topic       = "product-catalog",
                        Event       = "changed",
                        Sender      = "PWS Catalog Service",
                        PublishedAt = DateTime.UtcNow.ToString("u"),
                        Country     = "US",
                        Environment = "",
                        Payload = new
                        {
                            Message    = "Catalog was updated",
                            ModifiedAt = DateTime.UtcNow.ToString("u"),
                        }
                    }
                )
                .GetResult();

            Console.WriteLine("Successfully initiated product catalog cache loading");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to initiated product catalog cache loading");
            ex.ToExceptionless().Submit();
        }
    }*/

    #endregion
}