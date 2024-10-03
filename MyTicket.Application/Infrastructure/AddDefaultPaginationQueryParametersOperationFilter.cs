using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyTicket.Application.Infrastructure;

public class AddDefaultPaginationQueryParametersOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var hasDefaultPaginationAttribute = actionMetadata.Any(metadataItem => metadataItem is HasDefaultPaginationAttribute);
        if (!hasDefaultPaginationAttribute) return;

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "p",
            Description = "Page number",
            In = ParameterLocation.Query,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "number"
            }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "s",
            Description = "Total data ",
            In = ParameterLocation.Query,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "number"
            }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "q",
            Description = "Search all by text, minimum 3 charaters, like \"budi\"",
            In = ParameterLocation.Query,
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });
    }
}
