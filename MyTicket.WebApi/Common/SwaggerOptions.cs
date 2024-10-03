namespace MyTicket.WebApi.Common;

public class SwaggerOptions
{
    public SwaggerOptions()
    {
        Title = "API Management Bank Soal";
        Version = "v1";
        SpecUrl = "/swagger/v1/swagger.json";
        RoutePrefix = "docs";
    }

    public string Title { get; set; }
    public string Version { get; set; }
    public string SpecUrl { get; set; }
    public string RoutePrefix { get; set; }
}