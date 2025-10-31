using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access.Models
{
    public interface IProduct
    {
        int Id { get; set; }
        string Name { get; set; }
        string Author { get; set; }
        decimal Price { get; set; }

        public string GetBasicInfo();
    }
}
    