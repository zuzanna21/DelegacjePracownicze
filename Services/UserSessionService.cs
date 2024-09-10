using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegacjePracownicze.Services
{
    public class UserSessionService
    {
        public string? Email { get; private set; }

        public void SetUser(string email)
        {
            Email = email;
        }

        public void ClearUser()
        {
            Email = null;
        }
    }
}
