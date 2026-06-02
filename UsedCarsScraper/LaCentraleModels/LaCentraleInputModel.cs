using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UsedCarsScraper.LaCentraleModels;

public class LaCentraleInputModel : INotifyPropertyChanged
{
    private LaCentraleMake? _make;
    private LaCentraleModel? _model;
    private LaCentraleSubModel? _subModel;

    public LaCentraleMake? Make
    {
        get => _make;
        set
        {
            if (Equals(_make, value)) return;
            _make = value;
            Model = null;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Models));
        }
    }

    public List<LaCentraleModel> Models => Make?.Models ?? [];

    public LaCentraleModel? Model
    {
        get => _model;
        set
        {
            if (Equals(_model, value)) return;
            _model = value;
            SubModel = null;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SubModels));
        }
    }

    public List<LaCentraleSubModel> SubModels => Model?.SubModels ?? [];

    public LaCentraleSubModel? SubModel
    {
        get => _subModel;
        set
        {
            if (Equals(_subModel, value)) return;
            _subModel = value;
            OnPropertyChanged();
        }
    }

    public string? FuelType { get; set; }
    public string? PriceFrom { get; set; }
    public string? PriceTo { get; set; }
    public string? MileAgeFrom { get; set; }
    public string? MileAgeTo { get; set; }
    public string? DinPowerFrom { get; set; }
    public string? DinPowerTo { get; set; }
    public DateTime? AdvertisingDate { get; set; }
    public int? RegistrationYear { get; set; }
    public string? CompanySeller { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
