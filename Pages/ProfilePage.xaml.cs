using DelegacjePracownicze.ViewModels;
using DelegacjePracownicze.Services;
using System.Diagnostics;
namespace DelegacjePracownicze.Pages;



public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public ProfilePage()
    {
        
        InitializeComponent();
        var userService = new UserService(); 
        var viewModel = new ProfileViewModel(userService);
        BindingContext = viewModel;

      
        viewModel.Email = Preferences.Get("UserEmail", string.Empty);

        if (!string.IsNullOrEmpty(viewModel.Email))
        {
            viewModel.LoadUserData();
        }
        else
        {
            Debug.WriteLine("Email is not set in Preferences");
        }
    }

}