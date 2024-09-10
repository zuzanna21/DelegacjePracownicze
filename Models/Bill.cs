using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegacjePracownicze.Models
{
    public class Bill : INotifyPropertyChanged
    {
        private int billId;
        private int delegationId;
        private int categoryId;
        private string sellerName = string.Empty;
        private string sellerAddress = string.Empty;
        private string sellerCountry = string.Empty;
        private string sellerCity = string.Empty;
        private string sellerPostalCode = string.Empty;
        private string sellerTaxId = string.Empty;
        private string vatRate = string.Empty;
        private SqlMoney netValue;
        private SqlMoney vat;
        private SqlMoney grossValue;
        private Category category;
        private Return returnStatus;

        public Bill()
        {
            returnStatus = new Return(); 
            category = new Category(); 
        }

        public int BillId
        {
            get => billId;
            set
            {
                billId = value;
                OnPropertyChanged(nameof(BillId));
            }
        }

        public int DelegationId
        {
            get => delegationId;
            set
            {
                delegationId = value;
                OnPropertyChanged(nameof(DelegationId));
            }
        }

        public int CategoryId
        {
            get => categoryId;
            set
            {
                categoryId = value;
                OnPropertyChanged(nameof(CategoryId));
            }
        }

        public string SellerName
        {
            get => sellerName;
            set
            {
                sellerName = value;
                OnPropertyChanged(nameof(SellerName));
            }
        }

        public string SellerAddress
        {
            get => sellerAddress;
            set
            {
                sellerAddress = value;
                OnPropertyChanged(nameof(SellerAddress));
            }
        }

        public string SellerCountry
        {
            get => sellerCountry;
            set
            {
                sellerCountry = value;
                OnPropertyChanged(nameof(SellerCountry));
            }
        }

        public string SellerCity
        {
            get => sellerCity;
            set
            {
                sellerCity = value;
                OnPropertyChanged(nameof(SellerCity));
            }
        }

        public string SellerPostalCode
        {
            get => sellerPostalCode;
            set
            {
                sellerPostalCode = value;
                OnPropertyChanged(nameof(SellerPostalCode));
            }
        }

        public string SellerTaxId
        {
            get => sellerTaxId;
            set
            {
                sellerTaxId = value;
                OnPropertyChanged(nameof(SellerTaxId));
            }
        }

        public string VATRate
        {
            get => vatRate;
            set
            {
                vatRate = value;
                OnPropertyChanged(nameof(VATRate));
            }
        }

        public SqlMoney NetValue
        {
            get => netValue;
            set
            {
                netValue = value;
                OnPropertyChanged(nameof(NetValue));
            }
        }

        public SqlMoney VAT
        {
            get => vat;
            set
            {
                vat = value;
                OnPropertyChanged(nameof(VAT));
            }
        }

        public SqlMoney GrossValue
        {
            get => grossValue;
            set
            {
                grossValue = value;
                OnPropertyChanged(nameof(GrossValue));
            }
        }

        public Category Category
        {
            get => category;
            set
            {
                category = value;
                OnPropertyChanged(nameof(Category));
            }
        }

        public Return Return
        {
            get => returnStatus;
            set
            {
                returnStatus = value;
                OnPropertyChanged(nameof(Return));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}