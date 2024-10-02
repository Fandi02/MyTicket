namespace MyTicket.Application.Extensions;
public class WebAppConfig
{
    public WebAppConfig()
    {
        MultipartBodyLengthLimitInMb = 10;
        DoMigrationCheck = false;
        UseDiagnosticHandler = true;
    }

    /// <summary>
    /// Always do migration check when app is running first time. Default false
    /// </summary>
    /// <value></value>
    public bool DoMigrationCheck { get; set; }

    /// <summary>
    /// Multipart body length configuration in web api config. Default is 10
    /// </summary>
    /// <value></value>
    public int MultipartBodyLengthLimitInMb { get; set; }

    /// <summary>
    /// Secret key for authentication.
    /// </summary>
    /// <value></value>
    public string SecretKey { get; set; }

    /// <summary>
    /// Use diagnostic handler middlware. Default is true;
    /// </summary>
    /// <value></value>
    public bool UseDiagnosticHandler { get; set; }
}
