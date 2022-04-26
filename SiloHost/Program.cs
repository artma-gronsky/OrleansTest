using System.Net;
using Grains;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Configuration;
using Orleans.Hosting;
using SiloHost.Configurations.Models;
using SiloHost.Core.Models;
using SiloHost.Filters;

await RunSilo();

static async Task<int> RunSilo()
{
    try
    {
        await StartSilo();

        Console.WriteLine("Silo started");

        Console.WriteLine("Press enter to terminate");
        Console.ReadLine();

        return 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return -1;
    }
}

static async Task<ISiloHost> StartSilo()
{
    var configurationRoot = LoadConf();
    var builder = new SiloHostBuilder()

        // Clustering information
        .Configure<ClusterOptions>(options =>
        {
            // for clients and silo
            options.ClusterId = "dev";

            // uniq id used for some providers...?
            options.ServiceId = "HelloApp";
        })

        // Clustering provider
        // for development
        .UseLocalhostClustering()
        .ConfigureLogging(logging => logging.AddConsole())
        .AddAdoNetGrainStorage("Npgsql", options =>
        {
            var settings = GetOrleansConfiguration(configurationRoot);

            options.Invariant = settings.Invariant;
            options.ConnectionString = settings.ConnectionString;
            options.UseJsonFormat = settings.UseJsonFormat;
        })
        .UseDashboard()
        .ConfigureServices(service =>
        {
            service.AddSingleton(s => CreateGrainMethodsList());
            service.AddSingleton(s => new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
                TypeNameHandling = TypeNameHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
        })
        .AddIncomingGrainCallFilter<LoggingFilter>()
        .ConfigureApplicationParts((Action<IApplicationPartManager>)(parts =>
        {
            parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences();
        }))
        // Endpoints
        .Configure<EndpointOptions>(options =>
        {
            // for silo to silo communications
            options.SiloPort = 11111;
            // client to silo   
            options.GatewayPort = 30000;
            options.AdvertisedIPAddress = IPAddress.Loopback;
        });

    var host = builder.Build();
    await host.StartAsync();
    return host;
}

static IConfigurationRoot LoadConf()
{
    var configurationBuilder = new ConfigurationBuilder();
    configurationBuilder.AddJsonFile("appsettings.json");

    return configurationBuilder.Build();
}

static OrleansConfiguration GetOrleansConfiguration(IConfigurationRoot conf)
{
    var orleansConf = conf.GetSection(nameof(OrleansConfiguration));

    var settings = new OrleansConfiguration();
    orleansConf.Bind(settings);

    return settings;
}

static GrainInfo CreateGrainMethodsList()
{
    var grainInterfaces = typeof(IHello)
        .Assembly.GetTypes()
        .Where(type => type.IsInterface)
        .SelectMany(x => x.GetMethods())
        .Select(m => m.Name).ToList();

    return new GrainInfo{Methods = grainInterfaces};
}