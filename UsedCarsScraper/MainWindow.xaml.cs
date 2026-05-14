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
    private CancellationTokenSource _cts;
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
    StandvirtualGrid.CommitEdit(DataGridEditingUnit.Row, true);
    StandvirtualGrid.CancelEdit();

    var models = new List<InputModel>();
    foreach (var t in Filters)
    {
        models.Add(t);
    }

    var hasNullProp = models.Any(x => x?.Make == null || x?.Model == null);
    if (hasNullProp)
    {
        MessageBox.Show("Please make sure you selected Make and model for each input", "Reminding");
        return;
    }

    StartBtn.IsEnabled = false;
    StopBtn.IsEnabled = true;

    _cts = new CancellationTokenSource();
    var token = _cts.Token;

    var json = JsonConvert.SerializeObject(models, Formatting.Indented);
    await File.WriteAllTextAsync($"Standvirtual configs/{DateTime.Now:dd_MM_yyyy_mm_ss}_StandVirtual_config_file.json", json, token);

    var standVirtualService = new StandVirtualService();
    
    var progressReporter = new Progress<(int Percentage, string Message)>(info =>
    {
        MainProgressBar.Value = info.Percentage;
        ProgressLabel.Text = info.Message;
        var timeStamp = DateTime.Now.ToString("HH:mm:ss");
        LogTextBox.AppendText($"[{timeStamp}] {info.Message}\r\n");
        LogTextBox.ScrollToEnd();
    });

    try
    {
        do
        {
            // Throw if user clicked Stop while we were waiting
            token.ThrowIfCancellationRequested(); 

            MainProgressBar.Value = 0;
            var d2 = new DateTime();
            var d1 = DateTime.Now;
            var days = 1;
            
            if (Daily.IsChecked == true) d2 = DateTime.Now.AddDays(1);
            if (ThreeDays.IsChecked == true)
            {
                days = 3;
                d2 = DateTime.Now.AddDays(3);
            }

            // PASS THE TOKEN TO THE SERVICE
            await standVirtualService.Start(models, progressReporter, token);
            
            MainProgressBar.Value = 100;
            var nextRunMsg = $"Work done for today. Next run will be {DateTime.Now.AddDays(days):dd/MM/yyyy hh:mm:ss}";
            ProgressLabel.Text = nextRunMsg;
            LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {nextRunMsg}\r\n");
            var delay = (int)(d2 - d1).TotalSeconds;
            StartBtn.IsEnabled = true;
            await Task.Delay(TimeSpan.FromSeconds(delay), token);
            
        } while (true);
    }
    catch (OperationCanceledException)
    {
        // THIS CATCHES THE STOP BUTTON CLICK
        ProgressLabel.Text = "Scraping stopped by user.";
        LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] Scraping stopped by user.\r\n");
        MainProgressBar.Value = 0;
    }
    catch (Exception ex)
    {
        ProgressLabel.Text = $"Error: {ex.Message}";
        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
    finally
    {
        // RESET BUTTONS
        StartBtn.IsEnabled = true;
        StopBtn.IsEnabled = false;
        _cts?.Dispose();
        _cts = null;
    }
}

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // var configsScraper= new ConfigsScraper();
        // await configsScraper.StandVirtualConfigurationsScraper();
        // return;
        if (!Directory.Exists("Standvirtual configs"))
        {
            Directory.CreateDirectory("Standvirtual configs");
        }
        if (!Directory.Exists("Standvirtual json outputs"))
        {
            Directory.CreateDirectory("Standvirtual json outputs");
        }
        if (!Directory.Exists("Standvirtual outputs"))
        {
            Directory.CreateDirectory("Standvirtual outputs");
        }
        var json = await File.ReadAllTextAsync("StandVirtual config file");
        AppConfig = JsonConvert.DeserializeObject<Config>(json);
        OnPropertyChanged(nameof(AppConfig));
        Daily.IsChecked = true;

        if (File.Exists("last_StandVirtual_config_file"))
        {
            LoadCache();
        }
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
    private void Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var modelCombo = sender as ComboBox;
        var rowData = modelCombo?.DataContext as InputModel;
    
        // Ensure we are getting the actual Model object
        if (modelCombo?.SelectedItem is Model selectedModel && rowData != null)
        {
            rowData.Model = selectedModel; // Assign full object
        
            System.Diagnostics.Debug.WriteLine($"Successfully assigned Model {selectedModel.Name}. SubModels count: {selectedModel.SubModels?.Count ?? 0}");
        
            // Force the SubModel dropdown to refresh
            rowData.OnPropertyChanged(nameof(rowData.SubModels));
        }
    }
private void SaveCache()
{
    try
    {
        // Force the DataGrid to commit any cell currently being edited
        StandvirtualGrid.CommitEdit(DataGridEditingUnit.Row, true);
        StandvirtualGrid.CancelEdit();

        // Serialize and save to a local file
        var json = JsonConvert.SerializeObject(Filters, Formatting.Indented);
        File.WriteAllText("last_StandVirtual_config_file", json);
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Failed to save cache: {ex.Message}");
    }
}

private void LoadCache()
{
    try
    {
        if (File.Exists("last_StandVirtual_config_file"))
        {
            var json = File.ReadAllText("last_StandVirtual_config_file");
            var cachedFilters = JsonConvert.DeserializeObject<List<InputModel>>(json);

            if (cachedFilters != null && cachedFilters.Count > 0)
            {
                Filters.Clear();

                foreach (var cacheItem in cachedFilters)
                {
                    // --- THE FIX: Save the names BEFORE the setters wipe them out ---
                    string cachedMakeName = cacheItem.Make?.Name;
                    string cachedModelName = cacheItem.Model?.Name;
                    string cachedSubModelName = cacheItem.SubModel?.Name;

                    if (cachedMakeName != null && AppConfig?.Makes != null)
                    {
                        // 1. Find and assign Make (This will trigger Model = null internally)
                        var realMake = AppConfig.Makes.FirstOrDefault(m => m.Name == cachedMakeName);
                        if (realMake != null)
                        {
                            cacheItem.Make = realMake;

                            // 2. Find and assign Model using our saved string (This will trigger SubModel = null)
                            if (cachedModelName != null)
                            {
                                var realModel = realMake.Models.FirstOrDefault(m => m.Name == cachedModelName);
                                if (realModel != null)
                                {
                                    cacheItem.Model = realModel;

                                    // 3. Find and assign SubModel using our saved string
                                    if (cachedSubModelName != null)
                                    {
                                        var realSubModel = realModel.SubModels.FirstOrDefault(s => s.Name == cachedSubModelName);
                                        if (realSubModel != null)
                                        {
                                            cacheItem.SubModel = realSubModel;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Filters.Add(cacheItem);
                }
            }
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Failed to load cache: {ex.Message}");
    }

    if (Filters.Count == 0)
    {
        Filters.Add(new InputModel());
    }
}
    private void Window_Closing(object? sender, CancelEventArgs e)
    {
       SaveCache();
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            ProgressLabel.Text = "Stopping scraper... please wait.";
            StopBtn.IsEnabled = false; // Prevent user from clicking STOP multiple times
            
            // This triggers the OperationCanceledException in the Start thread
            _cts.Cancel(); 
        }
    }
}