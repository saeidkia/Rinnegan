namespace HelloRinnegan.Services.Data;

public static class TodoStore
{
    private static long _seed = 2;

    public static List<TodoItem> Items { get; } =
    [
        new TodoItem { id = 1, title = "Read Rinnegan docs", createdAt = DateTimeOffset.UtcNow.AddMinutes(-20) },
        new TodoItem { id = 2, title = "Create first endpoint", createdAt = DateTimeOffset.UtcNow.AddMinutes(-10) }
    ];

    public static long NextId()
    {
        _seed++;
        return _seed;
    }
}

public class TodoItem
{
    public long id { get; set; }
    public string title { get; set; } = string.Empty;
    public DateTimeOffset createdAt { get; set; }
}
