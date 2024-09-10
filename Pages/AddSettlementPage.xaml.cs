using System;
using Microsoft.Maui.Controls;
using DelegacjePracownicze.ViewModels;
using DelegacjePracownicze.Services;
namespace DelegacjePracownicze.Pages;

public partial class AddSettlementPage : ContentPage, IAlertService
{
    public AddSettlementPage()
    {
        InitializeComponent();
    }

    public async Task<bool> ShowConfirmationDialog(string title, string message, string accept, string cancel)
    {
        return await DisplayAlert(title, message, accept, cancel);
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry.Text.Contains("."))
        {
            entry.Text = entry.Text.Replace(".", ",");
        }
    }
}