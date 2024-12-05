using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore.Tools
{
    public abstract class PagedResult
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }

        public int pageCount => (int)Math.Ceiling((double)this.total / this.pageSize);
        public int firstIndex => (page - 1) * pageSize + 1;
        public int lastIndex => Math.Min(page * pageSize, total);
    }

    public class PagedResult<T> : PagedResult where T : class
    {
        public T[] items { get; set; }

    }

    public static class PagingExtension
    {
        public static PagedResult<T> ToPaged<T>(this IQueryable<T> query, IPagedRequest request) where T : class
        {
            return new PagedResult<T>
            {
                page = request.page.Value,
                pageSize = request.pageSize.Value,
                total = query.Count(),
                items = query.Skip((request.page.Value - 1) * request.pageSize.Value).Take(request.pageSize.Value).ToArray(),
            };
        }
    }

    public interface IPagedRequest
    {
        int? page { get; set; }
        int? pageSize { get; set; }
    }
}
