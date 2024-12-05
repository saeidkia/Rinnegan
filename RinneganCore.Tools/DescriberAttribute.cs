using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore.Tools
{
    public class DescriberAttribute : Attribute
    {
        private string _name;
        private string? _category;
        public DescriberAttribute(string name, string? category = null)
        {
            this._name = name;
            this._category = category;
        }
    }
}
