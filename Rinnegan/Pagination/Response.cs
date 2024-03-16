using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinnegan.Pagination
{
    public class Response<T>
    {
        int pageNumber { get; set; }
        int pageSize { get; set; }
        int total { get; set; }
        List<T> items { get; set; }
    }
}
