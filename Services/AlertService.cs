using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DelegacjePracownicze.Services;

public class AlertService : IAlertService
{
    public async Task<bool> ShowConfirmationDialog(string title, string message, string accept, string cancel)
    {
       
        if (Application.Current?.MainPage != null)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
        else
        {
        
            Debug.WriteLine("MainPage is null, cannot show alert.");
            return false; 
        }
    }
}