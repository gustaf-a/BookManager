namespace BookApi.Data;

public class GetValueRequest
{
    public string ColumnName { get; set; } = string.Empty;

    public bool GetMaxValue { get; set; } = false;

    public int IgnoreFirstCharacters { get; set; } = 0;
}
