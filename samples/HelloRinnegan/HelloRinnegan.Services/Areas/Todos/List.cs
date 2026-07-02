using HelloRinnegan.Services.Data;
using RinneganCore;
using RinneganCore.Tools;

namespace HelloRinnegan.Services.Areas.Todos;

public class List : Rinnegan<List.Request, PagedResult<List.Response>>
{
    protected override Result<PagedResult<Response>>? Execute()
    {
        var query = TodoStore.Items
            .OrderByDescending(x => x.id)
            .Select(x => new Response
            {
                id = x.id,
                title = x.title,
                createdAt = x.createdAt
            })
            .AsQueryable();

        return OK(query.ToPaged(request!));
    }

    public class Request : IPagedRequest
    {
        public int? page { get; set; } = 1;
        public int? pageSize { get; set; } = 10;
    }

    public class Response
    {
        public long id { get; set; }
        public string title { get; set; } = string.Empty;
        public DateTimeOffset createdAt { get; set; }
    }
}
