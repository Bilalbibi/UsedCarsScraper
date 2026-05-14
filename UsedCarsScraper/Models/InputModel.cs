using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UsedCarsScraper.Models
{
    public class InputModel : INotifyPropertyChanged
    {
        // Use private backing fields so setters can trigger OnPropertyChanged()
        private Make _make;

        public Make Make
        {
            get => _make;
            set
            {
                // 1. THE GUARD CLAUSE: Prevent WPF from wiping data on UI load
                if (Equals(_make, value)) return;

                _make = value;
                Model = null; // Now this ONLY triggers when you actually click a NEW Make
                OnPropertyChanged();
                OnPropertyChanged(nameof(Models));
            }
        }

        public List<Model> Models => Make?.Models ?? new List<Model>();

        // --- UPDATED MODEL LOGIC ---
        private Model _model;

        public Model Model
        {
            get => _model;
            set
            {
                // 2. THE GUARD CLAUSE
                if (Equals(_model, value)) return;

                _model = value;
                SubModel = null; // Only wipes SubModel if the Model ACTUALLY changed
                OnPropertyChanged();
                OnPropertyChanged(nameof(SubModels));
            }
        }

        // --- NEW: SUBMODEL LOGIC ---
        public List<SubModel> SubModels => Model?.SubModels ?? new List<SubModel>();

        private SubModel _subModel;

        public SubModel SubModel
        {
            get => _subModel;
            set
            {
                // 3. THE GUARD CLAUSE
                if (Equals(_subModel, value)) return;

                _subModel = value;
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