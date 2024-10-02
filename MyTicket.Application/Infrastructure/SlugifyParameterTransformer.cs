using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace MyTicket.Application.Infrastructure;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object value)
    {
        return value != null
            ? Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2")
            : null;
    }
}