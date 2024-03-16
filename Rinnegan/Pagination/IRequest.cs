using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rinnegan.Pagination
{
    public interface IRequest
    {

        [Range(1, int.MaxValue, ErrorMessage = "{0} را بین {1} و {2} وارد کنید")]
        [Display(Name = "شماره صفحه")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        int pageNumber { get; set; }


        [Display(Name = "تعداد نمایش در صفحه")]
        [Required(ErrorMessage = "{0} را وارد کنید")]
        int pageSize { get; set; }
    }
}
