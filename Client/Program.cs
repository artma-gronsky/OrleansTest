using Interfaces;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using Polly;

await RunMainAsync();

static async Task RunMainAsync()
{
    try
    {
        await using var client = StartClient();
        var grain = client.GetGrain<IHello>(0, "my-extension-key");

        var response = await grain.SayHello("Good morning");

        Console.WriteLine(response);
        Console.WriteLine($"Client is initialized: {client.IsInitialized}");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}

static IClusterClient StartClient()
{
    return Policy<IClusterClient>.Handle<SiloUnavailableException>()
        .Or<OrleansMessageRejectionException>()
        .WaitAndRetry(new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2) })
        .Execute(() =>
        {
            var client = new ClientBuilder()

                // Clustering information 
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloApp";
                })

                // Clustering provider
                .UseLocalhostClustering()
                .Build();

            client.Connect().GetAwaiter().GetResult();
            Console.WriteLine("Client connected");

            return client;
        });
}