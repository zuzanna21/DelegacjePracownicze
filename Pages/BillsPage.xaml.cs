using DelegacjePracownicze.ViewModels;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace DelegacjePracownicze.Pages;

[QueryProperty(nameof(SettlementId), "settlementId")]
public partial class BillsPage : ContentPage
{
    public int SettlementId { get; set; }

    public BillsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Debug.WriteLine($"BillsPage is appearing with SettlementId: {SettlementId}");

        BindingContext = new BillsViewModel(SettlementId);
    }
}