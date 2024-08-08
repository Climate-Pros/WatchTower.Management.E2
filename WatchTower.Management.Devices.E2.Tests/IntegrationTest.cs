using Funq;
using ServiceStack;
using NUnit.Framework;
using WatchTower.Management.Devices.E2.ServiceInterface;

namespace WatchTower.Management.Devices.E2.Tests;

public class IntegrationTest
{
    const string BaseUri = "http://localhost:2000/";
    private readonly ServiceStackHost appHost;

    class AppHost : AppSelfHostBase
    {
        public AppHost() : base(nameof(IntegrationTest), typeof(CommandServices).Assembly) { }

        public override void Configure(Container container)
        {
        }
    }

    public IntegrationTest()
    {
        appHost = new AppHost()
            .Init()
            .Start(BaseUri);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => appHost.Dispose();

    public IServiceClient CreateClient() => new JsonServiceClient(BaseUri);

    /*[Test]
    public void Can_call_Hello_Service()
    {
        var client = CreateClient();

        var response = client.Get(new Hello { Name = "World" });

        Assert.That(response.Result, Is.EqualTo("Hello, World!"));
    }*/
    
    [Test]
    public void Can_get_controller_list()
    {
        /*var client = CreateClient();

        var response = client.Get(new Hello { Name = "World" });

        Assert.That(response.Result, Is.EqualTo("Hello, World!"));*/
    }
}