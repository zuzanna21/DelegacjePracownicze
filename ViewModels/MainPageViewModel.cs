using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DelegacjePracownicze.Models;
using DelegacjePracownicze.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using DelegacjePracownicze.Pages;
using System.Windows.Input;

namespace DelegacjePracownicze.ViewModels;


public partial class MainPageViewModel : ObservableObject
{
    private readonly UserService userService;

    [ObservableProperty]
    private ObservableCollection<Settlement> settlements;

    [ObservableProperty]
    private bool isDescending = true;

    [ObservableProperty]
    private string email = string.Empty;

    public IRelayCommand ToggleSortCommand { get; }

    public IRelayCommand<Settlement> ShowBillsCommand { get; }

    public ICommand AddSettlementCommand { get; }

    public MainPageViewModel(UserService userService)
    {
        this.userService = userService;

        Email = Preferences.Get("UserEmail", string.Empty);
        Debug.WriteLine($"Loaded email from Preferences: {Email}");

        settlements = new ObservableCollection<Settlement>();
        ToggleSortCommand = new RelayCommand(ToggleSort);

        ShowBillsCommand = new RelayCommand<Settlement>(ShowBills);

        AddSettlementCommand = new Command(OnAddSettlement);

        LoadRozliczenia();
    }

    public MainPageViewModel() : this(new UserService())
    {
    }

    private async void LoadRozliczenia()
    {
        if (string.IsNullOrEmpty(Email))
        {
            return;
        }

        var list = await userService.GetSettlementsAsync(Email);
        Settlements = new ObservableCollection<Settlement>(list);
        SortRozliczenia();
    }

    private void SortRozliczenia()
    {
        if (IsDescending)
        {
            Settlements = new ObservableCollection<Settlement>(Settlements.OrderByDescending(r => r.IssueDate));
        }
        else
        {
            Settlements = new ObservableCollection<Settlement>(Settlements.OrderBy(r => r.IssueDate));
        }
    }

    private void ToggleSort()
    {
        IsDescending = !IsDescending;
        SortRozliczenia();
    }

    private async void ShowBills(Settlement? settlement)
    {
        if (settlement == null || settlement.SettlementId == 0)
        {
            Debug.WriteLine("Invalid Settlement or SettlementId");
            return;
        }


        await Shell.Current.GoToAsync($"{nameof(BillsPage)}?settlementId={settlement?.SettlementId}");
    }

    private async void OnAddSettlement()
    {

        await Application.Current.MainPage.Navigation.PushAsync(new AddSettlementPage());
    }
}
