using DelegacjePracownicze.Pages;

namespace DelegacjePracownicze
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("BillsPage", typeof(BillsPage));

            MainTabBar.IsVisible = false;

            // Sprawdzanie, czy użytkownik jest zalogowany
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

            if (isLoggedIn)
            {
                ShowMainTabBar();
                GoToAsync("//MainPage");
            }
            else
            {
                GoToAsync("//LoginPage");
            }
        }

        public void ShowMainTabBar()
        {
            MainTabBar.IsVisible = true;
        }

        public void HideMainTabBar()
        {
            MainTabBar.IsVisible = false;
        }
    }
}
