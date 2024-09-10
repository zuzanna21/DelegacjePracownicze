using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegacjePracownicze.Models
{
    public class Settlement
    {
        public int SettlementId { get; set; }
        public int BillId { get; set; }
        public int EmployeeId { get; set; }
        public int DelegationId { get; set; }
        public SqlMoney GrossValue { get; set; }
        public DateTime IssueDate { get; set; }

        public Delegation Delegation { get; set; } = new Delegation();
        public List<Bill> Bills { get; set; } = new List<Bill>();
    }
}
