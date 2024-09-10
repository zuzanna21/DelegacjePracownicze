using DelegacjePracownicze.ViewModels;
using DelegacjePracownicze.Services;

namespace DelegacjePracownicze.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var email = Preferences.Get("UserEmail", string.Empty);

            var viewModel = new MainPageViewModel(new UserService())
            {
                Email = email
            };

            BindingContext = viewModel;
        }
    }
}