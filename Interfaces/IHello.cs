using Orleans;

namespace Interfaces;

public interface IHello: IGrainWithIntegerCompoundKey
{
    Task<string> SayHello(string greeting);
}