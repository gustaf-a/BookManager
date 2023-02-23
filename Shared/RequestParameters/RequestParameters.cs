namespace Shared.RequestParameters;

public abstract class RequestParameters
{
    private const int MaxPageSize = 50;

    public int PageNumber { get; set; } = 1;

    private int _pageSize = 20;

    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            if (value > 0)
                _pageSize = value > MaxPageSize
                    ? MaxPageSize
                    : value;
        }
    }
}
