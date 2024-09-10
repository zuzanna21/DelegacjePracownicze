using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DelegacjePracownicze.Models;
using DelegacjePracownicze.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DelegacjePracownicze.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly UserService userService;
        

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public ICommand LoginCommand { get; }

  
        public LoginViewModel()
        {
            userService = new UserService();
            
            LoginCommand = new AsyncRelayCommand(LoginAsync);
        }

        private async Task LoginAsync()
        {
      
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Niepełne dane";
                return;
            }

       
            User? user = await userService.GetUserByEmailAsync(Email);

            if (user == null)
            {
                ErrorMessage = "Użytkownik nie istnieje";
            }
            else if (!user.VerifyPassword(Password))
            {
                ErrorMessage = "Błędne hasło";
            }
            else
            {
                Preferences.Set("IsLoggedIn", true);
                Preferences.Set("UserEmail", user.Email);

                Debug.WriteLine($"User email saved to Preferences: {user.Email}");

                ErrorMessage = string.Empty; 
                ((AppShell)Shell.Current).ShowMainTabBar();
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
    }
}