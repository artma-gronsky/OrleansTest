using System.Diagnostics;
using Interfaces;
using Orleans;
using Orleans.Providers;

namespace Grains;

[StorageProvider(ProviderName = "Npgsql")]
public class HelloGrain: Grain<GreetingArchive>, IHello
{
    public override Task OnActivateAsync()
    {
        
        Console.WriteLine($"HEllo grain pk={this.GetPrimaryKeyLong(out _)} activated");
        return base.OnActivateAsync();
    }

    public override Task OnDeactivateAsync()
    {
        Console.WriteLine($"HEllo grain pk={this.GetPrimaryKeyLong(out _)} deactivated");
        return base.OnDeactivateAsync();
    }

    public async Task<string> SayHello(string greeting)
    {

        State.Greetings.Add(greeting);
        await WriteStateAsync();
        
        var pKey = this.GetPrimaryKeyLong(out var extensioKey);
        
        this.DeactivateOnIdle();
        
        Console.WriteLine($"This is primary key: {pKey}  ext: {extensioKey}");
        return $"You said {greeting}, I say Hello!";
    }
}


public class GreetingArchive
{
    public List<string> Greetings { get; private set; } = new List<string>();
}