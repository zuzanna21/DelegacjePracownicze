using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegacjePracownicze.Models
{
    public class Delegation
    {
        public int DelegationId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int DaysCount { get; set; }
        public int HoursCount { get; set; }
        public string DelegationCountry { get; set; } = string.Empty;
        public string DelegationCity { get; set; } = string.Empty;
        public string TravelGoal { get; set; } = string.Empty;
    }
}
