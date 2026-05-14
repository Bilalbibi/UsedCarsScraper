using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using UsedCarsScraper.Models;
using UsedCarsScraper.Services;

namespace UsedCarsScraper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private List<InputModel> inputModels = [];
    private List<Car> cars = [];

    public Config? AppConfig
    {
        get;
        set
        {
            field = value;
            NotifyPropertyChanged();
        }
    }


    private ObservableCollection<InputModel> Filters { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        if (Filters == null)
            Filters = [];

        // 2. Set the DataContext (Best Practice)

        // 3. Set the Grid's ItemsSource
        try
        {
            StandvirtualGrid.Items.Clear();

            // 4. Set the ItemsSource
            StandvirtualGrid.ItemsSource = Filters;
            Filters.Add(new InputModel());
            DataContext = this;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        if (btn != null) btn.IsEnabled = false;

        foreach (var t in StandvirtualGrid.Items)
        {
            if (t is InputModel input) inputModels.Add(input);
        }

        var hasNullProp = inputModels.Any(x => x?.Make == null || x?.Model == null);
        if (hasNullProp)
        {
            CustomMessageBox.Show("Please make sure you selected Make and model for each input", "Reminding");
            btn?.IsEnabled = true;
            return;
        }
        var json = JsonConvert.SerializeObject(inputModels,Formatting.Indented);
        await File.WriteAllTextAsync($"configs/{DateTime.Now:dd_MM_yyyy mm_ss}_StandVirtual config file", json);
        var standVirtualService = new StandVirtualService();
        //await standVirtualService.GetDetails("https://www.standvirtual.com/carros/anuncio/citroen-ds5-ver-5-2-0-hdi-hybrid4-sport-chic-cmp6-ID8PVTye.html");

        // NEW: Create the Progress object (this safely updates the UI thread)
        var progressReporter = new Progress<(int Percentage, string Message)>(info =>
        {
            MainProgressBar.Value = info.Percentage;
            ProgressLabel.Text = info.Message;
            var timeStamp = DateTime.Now.ToString("HH:mm:ss");
            LogTextBox.AppendText($"[{timeStamp}] {info.Message}\r\n");
            LogTextBox.ScrollToEnd();
        });

        do
        {
            MainProgressBar.Value = 0;

            inputModels = [];
            for (var i = 0; i < StandvirtualGrid.Items.Count; i++)
            {
                var input = StandvirtualGrid.Items[i] as InputModel;
                if (input != null) inputModels.Add(input);
            }

            var d2 = new DateTime();
            var d1 = DateTime.Now;
            var days = 1;
            if (Daily.IsChecked == true)
            {
                d2 = DateTime.Now.AddDays(1);
            }

            if (ThreeDays.IsChecked == true)
            {
                days = 3;
                d2 = DateTime.Now.AddDays(3);
            }

            await standVirtualService.Start(inputModels, progressReporter);
            MainProgressBar.Value = 100;
            LogTextBox.AppendText($"work done for today next run will be {DateTime.Now.AddDays(days):dd/MM/yyyy hh:mm:ss}");
            btn?.IsEnabled = true;
            var delay = (int)(d2 - d1).TotalSeconds;
            await Task.Delay(TimeSpan.FromSeconds(delay));
        } while (true);
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var configsScraper= new ConfigsScraper();
        await configsScraper.StandVirtualConfigurationsScraper();
        return;
        if (!Directory.Exists("configs"))
        {
            Directory.CreateDirectory("configs");
        }
        if (!Directory.Exists("json outputs"))
        {
            Directory.CreateDirectory("json outputs");
        }
        if (!Directory.Exists("outputs"))
        {
            Directory.CreateDirectory("outputs");
        }
        var json = await File.ReadAllTextAsync("StandVirtual config file");
        AppConfig = JsonConvert.DeserializeObject<Config>(json);
        OnPropertyChanged(nameof(AppConfig));
        Daily.IsChecked = true;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void NotifyPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void AddRow_Click(object sender, RoutedEventArgs e)
    {
        Filters.Add(new InputModel());

        StandvirtualGrid.ScrollIntoView(Filters.Last());
    }

    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
        if (StandvirtualGrid.SelectedItem is InputModel selectedItem)
        {
            Filters.Remove(selectedItem);
        }
        else if (Filters.Count > 0)
        {
            Filters.RemoveAt(Filters.Count - 1);
        }
    }

    private void Make_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var makeCombo = sender as ComboBox;
        var rowData = makeCombo?.DataContext as InputModel;

        // Check what was actually selected
        if (makeCombo.SelectedItem is Make selectedMake)
        {
            rowData.Make = selectedMake;
            rowData.OnPropertyChanged(nameof(rowData.Models));
        }
    }
}