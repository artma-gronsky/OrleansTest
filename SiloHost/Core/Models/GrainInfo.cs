namespace SiloHost.Core.Models;

public class GrainInfo
{
    public GrainInfo()
    {
        Methods = new List<string>();
    }
    public List<string> Methods { get; set; }
}