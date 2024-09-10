using DelegacjePracownicze.Pages;
namespace DelegacjePracownicze
{
    public partial class App : Application
    {
        public App()
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("pl-PL");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("pl-PL");

            InitializeComponent();

            MainPage = new AppShell();

            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

            if (isLoggedIn)
            {
                // Jeśli użytkownik jest zalogowany, pokaż główne zakładki
                ((AppShell)MainPage).ShowMainTabBar();
                ((AppShell)MainPage).GoToAsync("//MainPage");
            }
            else
            {
                // Jeśli użytkownik nie jest zalogowany, pokaż stronę logowania
                ((AppShell)MainPage).GoToAsync("//LoginPage");
            }
        }
    }
}
