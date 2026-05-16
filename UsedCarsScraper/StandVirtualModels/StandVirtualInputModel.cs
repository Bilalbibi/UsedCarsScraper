using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UsedCarsScraper.StandVirtualModels
{
    public class StandVirtualInputModel : INotifyPropertyChanged
    {
        // Use private backing fields so setters can trigger OnPropertyChanged()
        private StandVirtualMake _standVirtualMake;

        public StandVirtualMake StandVirtualMake
        {
            get => _standVirtualMake;
            set
            {
                // 1. THE GUARD CLAUSE: Prevent WPF from wiping data on UI load
                if (Equals(_standVirtualMake, value)) return;

                _standVirtualMake = value;
                StandVirtualModel = null; // Now this ONLY triggers when you actually click a NEW Make
                OnPropertyChanged();
                OnPropertyChanged(nameof(Models));
            }
        }

        public List<StandVirtualModel> Models => StandVirtualMake?.Models ?? new List<StandVirtualModel>();

        // --- UPDATED MODEL LOGIC ---
        private StandVirtualModel _standVirtualModel;

        public StandVirtualModel StandVirtualModel
        {
            get => _standVirtualModel;
            set
            {
                // 2. THE GUARD CLAUSE
                if (Equals(_standVirtualModel, value)) return;

                _standVirtualModel = value;
                StandVirtualSubModel = null; // Only wipes SubModel if the Model ACTUALLY changed
                OnPropertyChanged();
                OnPropertyChanged(nameof(SubModels));
            }
        }

        // --- NEW: SUBMODEL LOGIC ---
        public List<StandVirtualSubModel> SubModels => StandVirtualModel?.SubModels ?? new List<StandVirtualSubModel>();

        private StandVirtualSubModel _standVirtualSubModel;

        public StandVirtualSubModel StandVirtualSubModel
        {
            get => _standVirtualSubModel;
            set
            {
                // 3. THE GUARD CLAUSE
                if (Equals(_standVirtualSubModel, value)) return;

                _standVirtualSubModel = value;
                OnPropertyChanged();
            }
        }

        // --- NEW: ENERGY TYPE (Fuel) LOGIC ---
        // Since FuelTypes is a Dictionary<string, string>, we can just store the Key (e.g., "gasoline")
        public bool IsElectric => _fuelType?.Equals("electric", StringComparison.OrdinalIgnoreCase) ?? false;


        private string _fuelType;

        public string FuelType
        {
            get => _fuelType;
            set
            {
                if (Equals(_fuelType, value)) return; // Guard clause

                _fuelType = value;

                // 2. Clear battery inputs if they choose something like Diesel or Gas
                if (!IsElectric)
                {
                    BatteryCapacityFrom = null;
                    BatteryCapacityTo = null;
                    OnPropertyChanged(nameof(BatteryCapacityFrom));
                    OnPropertyChanged(nameof(BatteryCapacityTo));
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsElectric)); // 3. Notify the UI to disable/enable the cells
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? PriceFrom { get; set; }
        public string? PriceTo { get; set; }
        public string? MileAgeFrom { get; set; }
        public string? MileAgeTo { get; set; }
        public string? BatteryCapacityFrom { get; set; }
        public string? BatteryCapacityTo { get; set; }
        public bool Vat { get; set; }
        public string? CompanySeller { get; set; }
        public DateTime? AdvertisingDate { get; set; }

        public int? RegistrationYear { get; set; }
    }
}