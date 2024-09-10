using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DelegacjePracownicze.Models;
using DelegacjePracownicze.Services;
using System.Data.SqlTypes;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Data.SqlClient;

namespace DelegacjePracownicze.ViewModels
{
    public partial class AddSettlementViewModel : ObservableObject
    {
        private readonly IAlertService alertService;
        private readonly UserService userService;

        
        [ObservableProperty] private ObservableCollection<Delegation>? delegations = new();
        [ObservableProperty] private Delegation? selectedDelegation = null;
        [ObservableProperty] private ObservableCollection<Category>? categories = new();
        [ObservableProperty] private Category? selectedCategory = null;
        [ObservableProperty] private ObservableCollection<Bill> savedBills = new();
        [ObservableProperty] private string settlementId = string.Empty;
        [ObservableProperty] private string idBill = string.Empty;
        [ObservableProperty] private bool isBillFormVisible = false;
        [ObservableProperty] private bool isAddBillButtonVisible = true;


        // Form Field Properties
        [ObservableProperty] private string sellerName = string.Empty;
        [ObservableProperty] private string sellerAddress = string.Empty;
        [ObservableProperty] private string sellerCountry = string.Empty;
        [ObservableProperty] private string sellerCity = string.Empty;
        [ObservableProperty] private string sellerPostalCode = string.Empty;
        [ObservableProperty] private string sellerTaxId = string.Empty;
        [ObservableProperty] private string vatRate = string.Empty;
        [ObservableProperty] private string? netValue = string.Empty;
        [ObservableProperty] private string? vat = string.Empty;
        [ObservableProperty] private string? grossValue = string.Empty;

        // Error properties
        [ObservableProperty] private string? delegationErrorMessage = string.Empty;
        [ObservableProperty] private bool isDelegationErrorVisible = false;

        [ObservableProperty] private string? settlementErrorMessage = string.Empty;
        [ObservableProperty] private bool isSettlementErrorVisible = false;


        [ObservableProperty] private string? selectedCategoryErrorMessage = string.Empty;
        [ObservableProperty] private bool isSelectedCategoryErrorVisible = false;

        [ObservableProperty] private string billErrorMessage = string.Empty;
        [ObservableProperty] private bool isBillErrorVisible = false;

        [ObservableProperty] private string sellerNameErrorMessage = string.Empty;
        [ObservableProperty] private bool isSellerNameErrorVisible = false;

        [ObservableProperty] private string sellerAddressErrorMessage = string.Empty;
        [ObservableProperty] private bool isSellerAddressErrorVisible = false;

        [ObservableProperty] private string sellerCountryErrorMessage = string.Empty;
        [ObservableProperty] private bool isSellerCountryErrorVisible = false;

        [ObservableProperty] private string sellerCityErrorMessage = string.Empty;
        [ObservableProperty] private bool isSellerCityErrorVisible = false;

        [ObservableProperty] private string sellerPostalCodeErrorMessage = string.Empty;
        [ObservableProperty] private bool isSellerPostalCodeErrorVisible = false;

        [ObservableProperty] private string sellerTaxIdErrorMessage = string.Empty;
        [ObservableProperty] private bool isSellerTaxIdErrorVisible = false;

        [ObservableProperty] private string vatRateErrorMessage = string.Empty;
        [ObservableProperty] private bool isVatRateErrorVisible = false;

        [ObservableProperty] private string netValueErrorMessage = string.Empty;
        [ObservableProperty] private bool isNetValueErrorVisible = false;

        [ObservableProperty] private string vatValueErrorMessage = string.Empty;
        [ObservableProperty] private bool isVatValueErrorVisible = false;

        [ObservableProperty] private string grossValueErrorMessage = string.Empty;
        [ObservableProperty] private bool isGrossValueErrorVisible = false;

        public Bill? SelectedBill { get; set; } = null;

        public IRelayCommand<Bill> EditBillCommand { get; }
        public IRelayCommand AddBillCommand { get; }
        public IRelayCommand SaveBillCommand { get; }
        public IRelayCommand CancelBillCommand { get; }
        public IRelayCommand SaveSettlementCommand { get; }
        public IRelayCommand TestConnectionCommand { get; }

        public AddSettlementViewModel() : this(new AlertService(), new UserService()) { }

        public AddSettlementViewModel(IAlertService alertService, UserService userService)
        {
            this.alertService = alertService;
            this.userService = userService;

            AddBillCommand = new RelayCommand(ShowBillForm);
            SaveBillCommand = new AsyncRelayCommand(SaveBillAsync);
            CancelBillCommand = new RelayCommand(ClearBillForm);
            SaveSettlementCommand = new AsyncRelayCommand(SaveSettlementAsync);
            EditBillCommand = new RelayCommand<Bill>(OnEditBill);
            TestConnectionCommand = new AsyncRelayCommand(TestConnectionAsync);

            LoadData();
        }

        private async void LoadData()
        {
            IsAddBillButtonVisible = true;

            var delegations = await userService.GetDelegationsAsync();
            if (delegations.Any())
            {
                //Debug.WriteLine("Delegations fetched successfully.");
                Delegations = new ObservableCollection<Delegation>(
                    delegations.Select(d =>
                        new Delegation
                        {
                            DelegationId = d.DelegationId,
                            DelegationCountry = $"{d.DelegationCountry} {d.DepartureDate.ToString("dd.MM.yyyy")} - {d.ReturnDate.ToString("dd.MM.yyyy")}"
                        })
                );
            }
            //else
            //{
            //    Debug.WriteLine("No delegations found.");
            //}

            var categories = await userService.GetCategoriesAsync();

            if (categories.Any())  
            {
                //Debug.WriteLine("Categories fetched successfully.");
                Categories = new ObservableCollection<Category>(
                    categories.Select(c =>
                        new Category
                        {
                            CategoryId = c.CategoryId,
                            CategoryName = $"{c.CategoryName} - {c.AllowanceCountry}"
                        })
                );
            }
            //else
            //{
            //    Debug.WriteLine("No categoriess found.");
            //}

        }

        private async void ShowBillForm()
        {
            if (await ValidateSettlementAsync())
            {
                IsBillFormVisible = true;
                IsAddBillButtonVisible = false;
            }
        }

        private void PopulateBillData(Bill bill)
        {

            bill.SellerName = SellerName;
            bill.SellerAddress = SellerAddress;
            bill.SellerCountry = SellerCountry;
            bill.SellerCity = SellerCity;
            bill.SellerPostalCode = SellerPostalCode;
            bill.SellerTaxId = SellerTaxId;
            bill.VATRate = VatRate;
            if (NetValue != null) { bill.NetValue = new SqlMoney(decimal.Parse(NetValue)); }
            if (Vat != null) { bill.VAT = new SqlMoney(decimal.Parse(Vat)); }
            if (GrossValue != null) { bill.GrossValue = new SqlMoney(decimal.Parse(GrossValue)); }

        }

        private void OnEditBill(Bill? bill)
        {
            if (bill == null) return;

            IdBill = bill.BillId.ToString();
            SellerName = bill.SellerName;
            SellerAddress = bill.SellerAddress;
            SellerCountry = bill.SellerCountry;
            SellerCity = bill.SellerCity;
            SellerPostalCode = bill.SellerPostalCode;
            SellerTaxId = bill.SellerTaxId;
            VatRate = bill.VATRate;
            NetValue = bill.NetValue.ToString();
            Vat = bill.VAT.ToString();
            GrossValue = bill.GrossValue.ToString();
            SelectedCategory = bill.Category;

            IsBillFormVisible = true;
            IsAddBillButtonVisible = false;
            SelectedBill = bill;
        }

        private void ClearBillForm()
        {
            IsBillFormVisible = false;
            IsAddBillButtonVisible = true;

            IdBill = string.Empty;
            SellerName = string.Empty;
            SellerAddress = string.Empty;
            SellerCountry = string.Empty;
            SellerCity = string.Empty;
            SellerPostalCode = string.Empty;
            SellerTaxId = string.Empty;
            VatRate = string.Empty;
            NetValue = string.Empty;
            Vat = string.Empty;
            GrossValue = string.Empty;
            SelectedCategory = null;
            SelectedBill = null;
        }


        private async Task SaveBillAsync()
        {
            if (!await ValidateBillAsync()) return;

            if (SelectedBill != null)
            {
                SelectedBill.BillId = int.Parse(IdBill);
                PopulateBillData(SelectedBill);
            }
            else
            {
                if (SelectedCategory != null)
                {
                    var newBill = new Bill
                    {
                        BillId = int.Parse(IdBill),
                        Category = SelectedCategory
                    };
                    PopulateBillData(newBill);
                    SavedBills.Add(newBill);
                }
            }

            ClearBillForm();
        }

        private async Task SaveSettlementAsync()
        {
            if (!await ValidateSettlementAsync()) return;

            if (await alertService.ShowConfirmationDialog("Potwierdzenie", "Czy napewno chcesz zapisać rozliczenie?", "Tak", "Nie"))
            {
                var email = Preferences.Get("UserEmail", string.Empty);
                var user = await userService.GetUserByEmailAsync(email);
                var newSettlement = new Settlement
                {
                    SettlementId = int.Parse(SettlementId),
                    DelegationId = SelectedDelegation!.DelegationId,
                    IssueDate = DateTime.Now,
                    Bills = SavedBills.ToList()
                };

                try
                {
                    await userService.SaveSettlementWithBillsAsync(newSettlement, user.EmployeeId);
                    await Shell.Current.GoToAsync("..");
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd przy zapisie rozliczenia: {ex.Message}");
                }
            }
        }



        private async Task<bool> ValidateSettlementAsync()
        {
            if (SelectedDelegation == null)
            {
                DelegationErrorMessage = "Musisz wybrać delegację.";
                IsDelegationErrorVisible = true;
                return false;
            }

            if (string.IsNullOrWhiteSpace(SettlementId) || !int.TryParse(SettlementId, out _))
            {
                SettlementErrorMessage = "Musisz wprowadzić poprawny numer rozliczenia.";
                IsSettlementErrorVisible = true;
                return false;
            }

            if (await userService.CheckIfSettlementExistsAsync(SettlementId))
            {
                SettlementErrorMessage = "Podane rozliczenie już istnieje.";
                IsSettlementErrorVisible = true;
                return false;
            }

            //ClearError();
            return true;
        }

        private async Task<bool> ValidateBillAsync()
        {

            bool isValid = true;

            isValid &= await ValidateBillId();
            isValid &= ValidateSelectedCategory();
            isValid &= ValidateSellerName();
            isValid &= ValidateSellerAddress();
            isValid &= ValidateSellerCountry();
            isValid &= ValidateSellerCity();
            isValid &= ValidateSellerPostalCode();
            isValid &= ValidateSellerTaxId();
            isValid &= ValidateVatRate();
            isValid &= ValidateNetValue();
            isValid &= ValidateVatValue();
            isValid &= ValidateGrossValue();

            return isValid;
        }


        //private void ClearError()
        //{
        //    IsErrorVisible = false;
        //    ErrorMessage = string.Empty;

        //}


        private async Task<bool> ValidateBillId()
        {
            if (string.IsNullOrWhiteSpace(IdBill))
            {
                BillErrorMessage = "Musisz wprowadzić numer rachunku.";
                IsBillErrorVisible = true;
                return false;
            }
            else if (!int.TryParse(IdBill, out int billIdInt))
            {
                BillErrorMessage = "Numer rachunku musi być liczbą całkowitą";
                IsBillErrorVisible = true;
                return false;
            }
            else if (SavedBills.Any(bill => bill.BillId == billIdInt) && (SelectedBill == null || SelectedBill.BillId != billIdInt))
            {
                BillErrorMessage = "Dodałeś już ten rachunek do rozliczenia.";
                IsBillErrorVisible = true;
                return false;
            }
            else if (await userService.CheckIfBillExistsAsync(billIdInt))
            {
                BillErrorMessage = "Podany rachunek już istnieje.";
                IsBillErrorVisible = true;
                return false;
            }

            IsBillErrorVisible = false;
            return true;
        }

        private bool ValidateSelectedCategory()
        {
            if (SelectedCategory == null)
            {
                SelectedCategoryErrorMessage = "Musisz wybrać kategorię.";
                IsSelectedCategoryErrorVisible = true;
                return false;
            }
            else
            {
                IsSelectedCategoryErrorVisible = false;
                return true;
            }
        }

        private bool ValidateSellerName()
        {
            if (string.IsNullOrEmpty(SellerName))
            {
                SellerNameErrorMessage = "To pole nie może być puste.";
                IsSellerNameErrorVisible = true;
                return false;
            }

            IsSellerNameErrorVisible = false;
            return true;
        }

        private bool ValidateSellerAddress()
        {
            if (string.IsNullOrEmpty(SellerAddress))
            {
                SellerAddressErrorMessage = "To pole nie może być puste.";
                IsSellerAddressErrorVisible = true;
                return false;
            }

            IsSellerAddressErrorVisible = false;
            return true;
        }

        private bool ValidateSellerCountry()
        {
            if (string.IsNullOrEmpty(SellerCountry))
            {
                SellerCountryErrorMessage = "To pole nie może być puste.";
                IsSellerCountryErrorVisible = true;
                return false;
            }
            else if (!char.IsUpper(SellerCountry[0]))
            {
                SellerCountryErrorMessage = "Nazwa kraju musi zaczynać się z dużej litery.";
                IsSellerCountryErrorVisible = true;
                return false;
            }
            else if (SellerCountry.Length < 4)
            {
                SellerCountryErrorMessage = "Nazwa kraju musi składać się z przynajmniej 4 znaków.";
                IsSellerCountryErrorVisible = true;
                return false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(SellerCountry, @"^[a-zA-Z\s]+$"))
            {
                SellerCountryErrorMessage = "Nazwa kraju nie może zawierać cyfr lub znaków specjalnych.";
                IsSellerCountryErrorVisible = true;
                return false;
            }

            IsSellerCountryErrorVisible = false;
            return true;
        }

        private bool ValidateSellerCity()
        {
            if (string.IsNullOrEmpty(SellerCity))
            {
                SellerCityErrorMessage = "To pole nie może być puste.";
                IsSellerCityErrorVisible = true;
                return false;
            }
            else if (!char.IsUpper(SellerCity[0]))
            {
                SellerCityErrorMessage = "Nazwa miasta musi zaczynać się z dużej litery.";
                IsSellerCityErrorVisible = true;
                return false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(SellerCity, @"^[a-zA-Z\s]+$"))
            {
                SellerCityErrorMessage = "Nazwa miasta nie może zawierać cyfr lub znaków specjalnych.";
                IsSellerCityErrorVisible = true;
                return false;
            }

            IsSellerCityErrorVisible = false;
            return true;
        }

        private bool ValidateSellerPostalCode()
        {
            if (string.IsNullOrEmpty(SellerPostalCode))
            {
                SellerPostalCodeErrorMessage = "To pole nie może być puste.";
                IsSellerPostalCodeErrorVisible = true;
                return false;
            }
            else if (SellerPostalCode.Length > 11)
            {
                SellerPostalCodeErrorMessage = "Kod pocztowy nie może przekraczać 11 znaków.";
                IsSellerPostalCodeErrorVisible = true;
                return false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(SellerPostalCode, @"^[a-zA-Z0-9\s-]+$"))
            {
                SellerPostalCodeErrorMessage = "Dopuszczalne są tylko cyfry, litery, - oraz spacje.";
                IsSellerPostalCodeErrorVisible = true;
                return false;
            }

            IsSellerPostalCodeErrorVisible = false;
            return true;
        }

        private bool ValidateSellerTaxId()
        {
            if (string.IsNullOrEmpty(SellerTaxId))
            {
                SellerTaxIdErrorMessage = "To pole nie może być puste.";
                IsSellerTaxIdErrorVisible = true;
                return false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(SellerTaxId, @"^[A-Z]{2}[0-9A-Za-z]{2,14}$"))
            {
           
                SellerTaxIdErrorMessage = "Numer NIP musi być poprzedzony oznaczeniem kraju składającym się z dwóch znaków i łącznie mieć od 4 do 16 znaków.";
                IsSellerTaxIdErrorVisible = true;
                return false;
            }

            IsSellerTaxIdErrorVisible = false;
            return true;
        }

        private bool ValidateVatRate()
        {
            if (string.IsNullOrEmpty(VatRate))
            {
                VatRateErrorMessage = "To pole nie może być puste.";
                IsVatRateErrorVisible = true;
                return false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(VatRate, @"^\d{1,3},?\d{0,2}%?$"))
            {
                VatRateErrorMessage = "Stawka VAT może mieć od 1 do 3 cyfr, opcjonalny przecinek i % na końcu.";
                IsVatRateErrorVisible = true;
                return false;
            }
            else if (decimal.TryParse(VatRate.TrimEnd('%'), out decimal vatRateValue) && vatRateValue <= 0)
            {
                VatRateErrorMessage = "Stawka VAT musi być większa od 0.";
                IsVatRateErrorVisible = true;
                return false;
            }

            IsVatRateErrorVisible = false;
            return true;
        }

        private bool ValidateNetValue()
        {
            if (string.IsNullOrEmpty(NetValue))
            {
                NetValueErrorMessage = "To pole nie może być puste";
                IsNetValueErrorVisible = true;
                return false;
            }
            if (!decimal.TryParse(NetValue, out decimal netValue) || netValue <= 0)
            {
                NetValueErrorMessage = "Wartość netto musi być większa od 0.";
                IsNetValueErrorVisible = true;
                return false;
            }

            IsNetValueErrorVisible = false;
            return true;
        }

        private bool ValidateVatValue()
        {
            if (string.IsNullOrEmpty(Vat))
            {
                VatValueErrorMessage = "To pole nie może być uste";
                IsVatValueErrorVisible = true;
                return false;
            }
            else
            if (!decimal.TryParse(Vat, out decimal vatValue) || vatValue <= 0)
            {
                VatValueErrorMessage = "Kwota VAT musi być większa od 0.";
                IsVatValueErrorVisible = true;
                return false;
            }

            IsVatValueErrorVisible = false;
            return true;
        }

        private bool ValidateGrossValue()
        {
            if (string.IsNullOrEmpty(GrossValue))
            {
                GrossValueErrorMessage = "To pole nie może być puste";
                IsGrossValueErrorVisible = true;
                return false;
            }
            else
            if (!decimal.TryParse(GrossValue, out decimal grossValue) || grossValue <= 0)
            {
                GrossValueErrorMessage = "Wartość brutto musi być większa od 0.";
                IsGrossValueErrorVisible = true;
                return false;
            }

            IsGrossValueErrorVisible = false;
            return true;
        }



        partial void OnSelectedDelegationChanged(Delegation? value)
        {
            if (SelectedDelegation != null)
            {
                IsDelegationErrorVisible = false;
            }
        }

        partial void OnSelectedCategoryChanged(Category? value)
        {
            if (SelectedCategory != null)
            {
                IsSelectedCategoryErrorVisible = false;
            }
        }

        partial void OnSettlementIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(SettlementId))
            {
                IsSettlementErrorVisible = false; 
            }
        }

        partial void OnIdBillChanged(string value)
        {
            if (!string.IsNullOrEmpty(IdBill))
            {
                IsBillErrorVisible = false; 
            }
        }

        partial void OnSellerNameChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IsSellerNameErrorVisible = false; 
            }
        }

        partial void OnSellerAddressChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IsSellerAddressErrorVisible = false; 
            }
        }

        partial void OnSellerCountryChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IsSellerCountryErrorVisible = false; 
            }
        }

        partial void OnSellerCityChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IsSellerCityErrorVisible = false;
            }
        }

        partial void OnSellerPostalCodeChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IsSellerPostalCodeErrorVisible = false; 
            }
        }

        partial void OnSellerTaxIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IsSellerTaxIdErrorVisible = false; 
            }
        }

        partial void OnVatRateChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IsVatRateErrorVisible = false; 
            }
        }

        partial void OnNetValueChanged(string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IsNetValueErrorVisible = false; 
            }
        }

        partial void OnVatChanged(string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IsVatValueErrorVisible = false; 
            }
        }

        partial void OnGrossValueChanged(string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                IsGrossValueErrorVisible = false; 
            }
        }


        public async Task TestConnectionAsync()
        {
            try
            {
                await userService.TestConnectionAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd połączenia: {ex.Message}");
            }
        }
    }
}