using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyTicket.Application.Models;

namespace MyTicket.Application.Infrastructure;

public class ResultManipulator : IResultFilter
{
    public void OnResultExecuted(ResultExecutedContext context)
    {
        // do nothing
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        //run code immediately before and after the execution of action results. They run only when the action method has executed successfully. They are useful for logic that must surround view or formatter execution.
        if (context.Result is ObjectResult result)
        {
            var resultObj = result.Value;

            var actionMetadata = context.ActionDescriptor.EndpointMetadata;
            if (actionMetadata.Any(metadataItem => metadataItem is IgnoreResultManipulatorAttribute))
            {
                context.Result = new JsonResult(resultObj, new JsonSerializerOptions()
                {
                    //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
            }
            else
            {
                var resp = new ResultApi
                {
                    Path = context.HttpContext.Request.Path.HasValue ? context.HttpContext.Request.Path.Value : "",
                    Method = context.HttpContext.Request.Method
                };

                if (resultObj is not null && resultObj is not Unit)
                    resp.Payload = resultObj;

                context.Result = new JsonResult(resp, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    Converters = { new JsonStringEnumConverter() }
                });
            }
        }
    }
}
