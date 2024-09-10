using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegacjePracownicze.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public SqlMoney AllowanceRate { get; set; }
        public string AllowanceCountry { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
