using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DelegacjePracownicze.Models;
using DelegacjePracownicze.Services;

namespace DelegacjePracownicze.ViewModels
{
    public partial class BillsViewModel : ObservableObject
    {
        private readonly UserService userService;

 

        private ObservableCollection<Bill> _bills = new ObservableCollection<Bill>();

        public ObservableCollection<Bill> Bills
        {
            get => _bills;
            private set => SetProperty(ref _bills, value);
        }

        public BillsViewModel(int settlementId)
        {
            userService = new UserService();
            LoadBills(settlementId);
        }

        private async void LoadBills(int settlementId)
        {
            try
            {
                var list = await userService.GetBillsBySettlementIdAsync(settlementId);
                if (list != null && list.Count > 0)
                {
                    Bills = new ObservableCollection<Bill>(list);
                    Console.WriteLine($"Loaded {Bills.Count} bills for settlement ID {settlementId}");
                }
                else
                {
                    Console.WriteLine("No bills found or list is null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading bills: {ex.Message}");
            }
        }

    }
}
