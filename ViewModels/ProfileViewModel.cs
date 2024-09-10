using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelegacjePracownicze.Pages;
using System.Windows.Input;
using DelegacjePracownicze.Services;
using System.Diagnostics;

namespace DelegacjePracownicze.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly UserService userService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string firstName =string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private string address = string.Empty;

    [ObservableProperty]
    private string city = string.Empty;

    [ObservableProperty]
    private string postalCode = string.Empty;

    [ObservableProperty]
    private string country = string.Empty;

    [ObservableProperty]
    private string position = string.Empty;
    
    public ICommand LogoutCommand { get; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public string FullAddress => $"{Address}\n{PostalCode} {City}\n{Country}".Trim();


    // Konstruktor z wstrzykiwaniem zależności
    public ProfileViewModel(UserService userService)
    {
        this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        LogoutCommand = new Command(OnLogoutButtonClicked);

      
    }

    public async void LoadUserData()
    {
        Debug.WriteLine("LoadUserData called");

        if (string.IsNullOrEmpty(Email))
        {
            Debug.WriteLine("Email is empty, aborting load");
            return;
        }

        var user = await userService.GetUserByEmailAsync(Email);

        if (user != null)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            PhoneNumber = user.PhoneNumber;
            Address = user.Address;
            City = user.City;
            Country = user.Country;
            PostalCode = user.PostalCode;
            Position = user.Position;

            Debug.WriteLine($"Loaded user: {FirstName} {LastName}, {PhoneNumber}, {Address}");

       
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(FullAddress));
        }
        else
        {
            Debug.WriteLine("User not found");
        }
    }

    private async void OnLogoutButtonClicked()
    {

        Preferences.Set("IsLoggedIn", false);


        ((AppShell)Shell.Current).HideMainTabBar();

        await Shell.Current.GoToAsync("//LoginPage");
    }
}
