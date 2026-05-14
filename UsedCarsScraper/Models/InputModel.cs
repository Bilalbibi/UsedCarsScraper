using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UsedCarsScraper.Models
{
    
    public class InputModel : INotifyPropertyChanged
    {
        // Use private backing fields so setters can trigger OnPropertyChanged()
        private Make _make;
        private Model? _model;
        public Make Make 
        { 
            get => _make; 
            set { 
                _make = value; 
                _model = null; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(Models)); // Tells the Model ComboBox to refresh its list
            } 
        }

        // This Helper Property is key!
        
        public List<Model> Models 
        {
            get 
            {
                var list = Make?.Models ?? [];
               // Console.WriteLine($"Models getter called. Found {list.Count} models.");
                return list;
            }
        }

       
        public Model? Model { get => _model; set { _model = value; OnPropertyChanged(); } }

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