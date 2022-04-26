using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using SiloHost.Core.Models;

namespace SiloHost.Filters;

public class LoggingFilter : IIncomingGrainCallFilter
{
    private readonly GrainInfo grainInfo;
    private readonly ILogger<LoggingFilter> logger;
    private readonly JsonSerializerSettings jsonSettings;

    public LoggingFilter(GrainInfo grainInfo, ILogger<LoggingFilter> logger, JsonSerializerSettings jsonSettings)
    {
        this.grainInfo = grainInfo;
        this.logger = logger;
        this.jsonSettings = jsonSettings;
    }

    public async Task Invoke(IIncomingGrainCallContext context)
    {
        try
        {
            if (grainInfo.Methods.Contains(context.InterfaceMethod.Name))
            {
                var arguments = JsonConvert.SerializeObject(context.Arguments, jsonSettings);
                logger.LogInformation(
                    $"LOGGING FILTER {context.Grain.GetType()}.{context.InterfaceMethod.Name} arguments: {arguments}");
            }

            await context.Invoke();

            if (grainInfo.Methods.Contains(context.InterfaceMethod.Name))
            {
                var result = JsonConvert.SerializeObject(context.Result, jsonSettings);
                logger.LogInformation(
                    $"LOGGING FILTER {context.Grain.GetType()}.{context.InterfaceMethod.Name} result: {result}");
            }
        }
        catch (Exception e)
        { 
            var arguments = JsonConvert.SerializeObject(context.Arguments, jsonSettings);
            var result = JsonConvert.SerializeObject(context.Result, jsonSettings);
            
            logger.LogError($"LOGGING FILTER {context.Grain.GetType()}.{context.InterfaceMethod.Name} arguments: {arguments} result: {result} threw new exaption {nameof(e)}",e);
        }
    }
}