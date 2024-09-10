using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegacjePracownicze.Services;

public interface IAlertService
{
    Task<bool> ShowConfirmationDialog(string title, string message, string accept, string cancel);
}
