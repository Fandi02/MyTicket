namespace MyTicket.Application.Models;

public class ResultApi
{
    public ResultApi()
    {
        IsSuccess = true;
        StatusCode = 200;
        Message = "Ok";
        InnerMessage = null;
        Path = null;
        Payload = null;
        Method = null;
    }

    /// <summary>
    /// Default value is true.
    /// </summary>
    /// <value></value>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Default value is 200OK.
    /// </summary>
    /// <value></value>
    public int StatusCode { get; set; }

    /// <summary>
    /// Default value is "Ok".
    /// </summary>
    /// <value></value>
    public string Message { get; set; }

    /// <summary>
    /// Default value is null.
    /// </summary>
    /// <value></value>
    public string InnerMessage { get; set; }

    /// <summary>
    /// Default value is null.
    /// </summary>
    /// <value></value>
    public string Path { get; set; }

    /// <summary>
    /// Default value is null.
    /// </summary>
    /// <value></value>
    public string Method { get; set; }

    /// <summary>
    /// Default value is null.
    /// </summary>
    /// <value></value>
    public object Payload { get; set; }
}
