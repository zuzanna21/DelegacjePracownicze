using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegacjePracownicze.Models
{
    public class Return
    {
        public int ReturnId { get; set; }
        public int BillId { get; set; }
        public int EmployeeId { get; set; }
        public string ReturnStatus { get; set; } = string.Empty;
    }
}
