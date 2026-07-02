using HelloRinnegan.Services.Data;
using RinneganCore;
using System.ComponentModel.DataAnnotations;

namespace HelloRinnegan.Services.Areas.Todos;

public class New : Rinnegan<New.Request, New.Response>
{
    protected override Result<Response>? Execute()
    {
        var item = new TodoItem
        {
            id = TodoStore.NextId(),
            title = request!.title!,
            createdAt = DateTimeOffset.UtcNow
        };

        TodoStore.Items.Add(item);

        return OK(new Response
        {
            id = item.id,
            title = item.title,
            createdAt = item.createdAt
        });
    }

    public class Request
    {
        [Required]
        [MinLength(2)]
        public string? title { get; set; }
    }

    public class Response
    {
        public long id { get; set; }
        public string title { get; set; } = string.Empty;
        public DateTimeOffset createdAt { get; set; }
    }
}
