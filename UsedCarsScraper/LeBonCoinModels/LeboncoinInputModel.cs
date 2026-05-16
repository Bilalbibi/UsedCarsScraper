using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UsedCarsScraper.LeBonCoinModels;

 public class LeboncoinInputModel : INotifyPropertyChanged
    {
        // --- 1. MAKE & MODEL CASCADING LOGIC ---
        private LeBonCoinCarBrand _make;
        public LeBonCoinCarBrand Make
        {
            get => _make;
            set
            {
                if (Equals(_make, value)) return; // Guard clause for UI loading
                
                _make = value;
                Model = null; // Clear Model when Make changes
                
                OnPropertyChanged();
                OnPropertyChanged(nameof(Models)); // Trigger UI to refresh Models list
            }
        }

        // Helper property for the Model ComboBox
        public List<LeBonCoinCarModel> Models => Make?.Models ?? new List<LeBonCoinCarModel>();

        private LeBonCoinCarModel _model;
        public LeBonCoinCarModel Model
        {
            get => _model;
            set
            {
                if (Equals(_model, value)) return;
                _model = value;
                OnPropertyChanged();
            }
        }

        // --- 2. ENERGY TYPE ---
        private Fuel _fuelType;
        public Fuel FuelType
        {
            get => _fuelType;
            set
            {
                if (Equals(_fuelType, value)) return;
                _fuelType = value;
                OnPropertyChanged();
            }
        }

        // --- 3. STANDARD TEXT FIELDS ---
        private string _priceFrom;
        public string PriceFrom { get => _priceFrom; set { _priceFrom = value; OnPropertyChanged(); } }

        private string _priceTo;
        public string PriceTo { get => _priceTo; set { _priceTo = value; OnPropertyChanged(); } }

        private string _mileAgeFrom;
        public string MileAgeFrom { get => _mileAgeFrom; set { _mileAgeFrom = value; OnPropertyChanged(); } }

        private string _mileAgeTo;
        public string MileAgeTo { get => _mileAgeTo; set { _mileAgeTo = value; OnPropertyChanged(); } }

        private string _dinPowerFrom;
        public string DinPowerFrom { get => _dinPowerFrom; set { _dinPowerFrom = value; OnPropertyChanged(); } }

        private string _dinPowerTo;
        public string DinPowerTo { get => _dinPowerTo; set { _dinPowerTo = value; OnPropertyChanged(); } }

        private DateTime? _advertisingDate;
        public DateTime? AdvertisingDate { get => _advertisingDate; set { _advertisingDate = value; OnPropertyChanged(); } }

        private string _registrationYear;
        public string RegistrationYear { get => _registrationYear; set { _registrationYear = value; OnPropertyChanged(); } }

        private string _companySeller;
        public string CompanySeller { get => _companySeller; set { _companySeller = value; OnPropertyChanged(); } }


        // --- INOTIFYPROPERTYCHANGED IMPLEMENTATION ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }