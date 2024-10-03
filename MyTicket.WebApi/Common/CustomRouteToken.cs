using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;

namespace MyTicket.WebApi.Common;

public partial class CustomRouteToken : IApplicationModelConvention
{
    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex MyRegex();

    private readonly string _tokenRegex;
    private readonly Func<ControllerModel, string?> _valueGenerator;

    public CustomRouteToken(string tokenName, Func<ControllerModel, string?> valueGenerator)
    {
        _tokenRegex = $@"(\[{tokenName}])(?<!\[\1(?=]))";
        _valueGenerator = valueGenerator;
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var tokenValue = _valueGenerator(controller);
            tokenValue = MyRegex().Replace(tokenValue!, "$1-$2").ToLower();
            UpdateSelectors(controller.Selectors, tokenValue);
            UpdateSelectors(controller.Actions.SelectMany(a => a.Selectors), tokenValue);
        }
    }

    private void UpdateSelectors(IEnumerable<SelectorModel> selectors, string? tokenValue)
    {
        foreach (var selector in selectors.Where(s => s.AttributeRouteModel != null))
        {
            if (selector.AttributeRouteModel == null) continue;
            selector.AttributeRouteModel.Template = InsertTokenValue(selector.AttributeRouteModel.Template, tokenValue);
            selector.AttributeRouteModel.Name = InsertTokenValue(selector.AttributeRouteModel.Name, tokenValue);
        }
    }

    private string? InsertTokenValue(string? template, string? tokenValue)
    {
        return template is null ? template : Regex.Replace(template, _tokenRegex, tokenValue!);
    }
}