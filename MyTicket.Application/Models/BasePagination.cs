namespace MyTicket.Application.Models;

public class BasePagination
{
    public const int BasePage = 1;
    public const int BaseSize = _defaultSize;

    private const int _defaultSize = 10;

    public BasePagination()
    {
        Page = 1;
        Size = _defaultSize;
    }

    public BasePagination(int page, int size) : this()
    {
        Page = page;
        Size = size;
    }

    private int _page;
    private int _size;

    /// <summary>
    /// Page 1~n. Formula (1 <= x <= n, where n is unlimited)
    /// </summary>
    public int Page
    {
        set => _page = value;
        get => _page < 1 ? (_page = 1) : _page;
    }

    /// <summary>
    /// Size 10~100. Formula (10 <= x <= 100)
    /// </summary>
    public int Size
    {
        set => _size = value;
        get => _size < _defaultSize ? (_size = _defaultSize) : _size > 100 ? (_size = 100) : _size;
    }

    /// <summary>
    /// Query to search
    /// </summary>
    /// <value></value>
    public string Query { get; set; }

    public int CalculateOffset()
    {
        return (Page - 1) == 0 || (Page - 1) < 0 ? 0 : (Page - 1) * Size;
    }

    public int CalculatePageCount(int itemCount)
    {
        return (int)Math.Ceiling((decimal)itemCount / Size);
    }
}
