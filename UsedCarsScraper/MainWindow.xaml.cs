using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using UsedCarsScraper.GeneralModels;
using UsedCarsScraper.LeBonCoinModels;
using UsedCarsScraper.Services;
using UsedCarsScraper.StandVirtualModels;

namespace UsedCarsScraper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private List<StandVirtualInputModel> inputModels = [];
    private List<Car> cars = [];
    private CancellationTokenSource _cts;

    public StandVirtualConfig? StandVirtualConfig
    {
        get;
        set
        {
            field = value;
            NotifyPropertyChanged();
        }
    }

    public LeBonCoinConfig? LbConfig
    {
        get;
        set
        {
            field = value;
            NotifyPropertyChanged();
        }
    }


    private ObservableCollection<StandVirtualInputModel> StandVirtualFilters { get; set; }
    public ObservableCollection<LeboncoinInputModel> LbFilters { get; set; } = [];


    public MainWindow()
    {
        InitializeComponent();
        if (StandVirtualFilters == null)
            StandVirtualFilters = [];
        try
        {
            StandvirtualGrid.Items.Clear();
            StandvirtualGrid.ItemsSource = StandVirtualFilters;
            StandVirtualFilters.Add(new StandVirtualInputModel());
            DataContext = this;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        try
        {
            LeboncoinGrid.Items.Clear();
            LeboncoinGrid.ItemsSource = LbFilters;
            LbFilters.Add(new LeboncoinInputModel());
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

        var models = new List<StandVirtualInputModel>();
        foreach (var t in StandVirtualFilters)
        {
            models.Add(t);
        }

        var hasNullProp = models.Any(x => x?.StandVirtualMake == null || x?.StandVirtualModel == null);
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
        await File.WriteAllTextAsync(
            $"Standvirtual configs/{DateTime.Now:dd_MM_yyyy_mm_ss}_StandVirtual_config_file.json", json, token);

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
                var nextRunMsg =
                    $"Work done for today. Next run will be {DateTime.Now.AddDays(days):dd/MM/yyyy hh:mm:ss}";
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
        Daily.IsChecked = true;
        await CreateStandVirtualFolders();
        await CreateLbnFolders();
        var svJson = await File.ReadAllTextAsync("StandVirtual config file");
        StandVirtualConfig = JsonConvert.DeserializeObject<StandVirtualConfig>(svJson);
    
        var lbJson = await File.ReadAllTextAsync("Leboncoin config file");
        LbConfig = JsonConvert.DeserializeObject<LeBonCoinConfig>(lbJson);

        // Notify the UI
        OnPropertyChanged(nameof(StandVirtualConfig));
        OnPropertyChanged(nameof(LbConfig));

        // NOW load the caches and populate the grids!
        LoadAllCaches();
    }

    private async Task CreateLbnFolders()
    {
        // var leBonCoinConfigFiller = new LeBonCoinCarDataExtractor();
        // await leBonCoinConfigFiller.ExtractCarData();
        if (!Directory.Exists("leboncoin configs"))
        {
            Directory.CreateDirectory("leboncoin configs");
        }

        if (!Directory.Exists("leboncoin json outputs"))
        {
            Directory.CreateDirectory("leboncoin json outputs");
        }

        if (!Directory.Exists("leboncoin outputs"))
        {
            Directory.CreateDirectory("leboncoin outputs");
        }

        // var json = await File.ReadAllTextAsync("leboncoin config file");
        // LbConfig = JsonConvert.DeserializeObject<LeBonCoinConfig>(json);
        // OnPropertyChanged(nameof(LbConfig));
    }

    private async Task CreateStandVirtualFolders()
    {
        // var standVirtualConfigsScraper= new StandVirtualConfigsScraper();
        // await standVirtualConfigsScraper.StandVirtualConfigurationsScraper();
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

        // var json = await File.ReadAllTextAsync("StandVirtual config file");
        // StandVirtualConfig = JsonConvert.DeserializeObject<StandVirtualConfig>(json);
        // OnPropertyChanged(nameof(StandVirtualConfig));
        //
        // if (File.Exists("last_StandVirtual_config_file"))
        // {
        //     LoadCacheForStandVirtual();
        // }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void NotifyPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void AddRow_Click(object sender, RoutedEventArgs e)
    {
        StandVirtualFilters.Add(new StandVirtualInputModel());
        StandvirtualGrid.ScrollIntoView(StandVirtualFilters.Last());
    }
    private void LbAddRow_Click(object sender, RoutedEventArgs e)
    {
        LbFilters.Add(new LeboncoinInputModel());
        LeboncoinGrid.ScrollIntoView(LbFilters.Last());
    }
    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
        if (StandvirtualGrid.SelectedItem is StandVirtualInputModel selectedItem)
        {
            StandVirtualFilters.Remove(selectedItem);
        }
        else if (StandVirtualFilters.Count > 0)
        {
            StandVirtualFilters.RemoveAt(StandVirtualFilters.Count - 1);
        }
    }
    private void LbDeleteRow_Click(object sender, RoutedEventArgs e)
    {
        if (LeboncoinGrid.SelectedItem is LeboncoinInputModel selectedItem)
        {
            LbFilters.Remove(selectedItem);
        }
        else if (LbFilters.Count > 0)
        {
            LbFilters.RemoveAt(LbFilters.Count - 1);
        }
    }
    private void Make_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var makeCombo = sender as ComboBox;
        var rowData = makeCombo?.DataContext as StandVirtualInputModel;

        // Check what was actually selected
        if (makeCombo.SelectedItem is StandVirtualMake selectedMake)
        {
            rowData.StandVirtualMake = selectedMake;
            rowData.OnPropertyChanged(nameof(rowData.Models));
        }
    }

    private void Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var modelCombo = sender as ComboBox;
        var rowData = modelCombo?.DataContext as StandVirtualInputModel;

        // Ensure we are getting the actual Model object
        if (modelCombo?.SelectedItem is StandVirtualModel selectedModel && rowData != null)
        {
            rowData.StandVirtualModel = selectedModel; // Assign full object

            System.Diagnostics.Debug.WriteLine(
                $"Successfully assigned Model {selectedModel.Name}. SubModels count: {selectedModel.SubModels?.Count ?? 0}");

            // Force the SubModel dropdown to refresh
            rowData.OnPropertyChanged(nameof(rowData.SubModels));
        }
    }
    private void SaveAllCaches()
    {
        // Save Standvirtual
        SaveCache(StandvirtualGrid, StandVirtualFilters, "last_StandVirtual_config_file");
    
        // Save Leboncoin
        SaveCache(LeboncoinGrid, LbFilters, "last_Leboncoin_config_file");
    }
    private void SaveCache(DataGrid grid, object filtersCollection, string filePath)
    {
        try
        {
            // Force the specific DataGrid to commit edits
            grid.CommitEdit(DataGridEditingUnit.Row, true);
            grid.CancelEdit();

            // Serialize and save
            var json = JsonConvert.SerializeObject(filtersCollection, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save cache to {filePath}: {ex.Message}");
        }
    }
    private void LoadCache<T>(string filePath, ObservableCollection<T> targetCollection, Action<T> relinkAction) where T : new()
    {
        try
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var cachedFilters = JsonConvert.DeserializeObject<List<T>>(json);

                if (cachedFilters != null && cachedFilters.Count > 0)
                {
                    targetCollection.Clear();

                    foreach (var cacheItem in cachedFilters)
                    {
                        // Invoke the custom re-linking rules passed into the method
                        relinkAction?.Invoke(cacheItem);
                    
                        targetCollection.Add(cacheItem);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load cache from {filePath}: {ex.Message}");
        }

        // Always ensure there is at least one empty row to start
        if (targetCollection.Count == 0)
        {
            targetCollection.Add(new T());
        }
    }
private void LoadAllCaches()
{
    // 1. LOAD STANDVIRTUAL
    if (File.Exists("last_StandVirtual_config_file"))
    {
        LoadCache("last_StandVirtual_config_file", StandVirtualFilters, cacheItem =>
        {
            var cachedMakeName = cacheItem.StandVirtualMake?.Name;
            var cachedModelName = cacheItem.StandVirtualModel?.Name;
            var cachedSubModelName = cacheItem.StandVirtualSubModel?.Name;

            if (cachedMakeName != null && StandVirtualConfig?.Makes != null)
            {
                var realMake = StandVirtualConfig.Makes.FirstOrDefault(m => m.Name == cachedMakeName);
                if (realMake != null)
                {
                    cacheItem.StandVirtualMake = realMake;
                    if (cachedModelName != null)
                    {
                        var realModel = realMake.Models.FirstOrDefault(m => m.Name == cachedModelName);
                        if (realModel != null)
                        {
                            cacheItem.StandVirtualModel = realModel;
                            if (cachedSubModelName != null)
                            {
                                var realSubModel = realModel.SubModels.FirstOrDefault(s => s.Name == cachedSubModelName);
                                if (realSubModel != null) cacheItem.StandVirtualSubModel = realSubModel;
                            }
                        }
                    }
                }
            }
        });
    }

    // 2. LOAD LEBONCOIN
    if (File.Exists("last_Leboncoin_config_file"))
    {
        LoadCache("last_Leboncoin_config_file", LbFilters, cacheItem =>
        {
            var cachedMakeName = cacheItem.Make?.Name;
            var cachedModelName = cacheItem.Model?.Name;
            var cachedFuelName = cacheItem.FuelType?.Name;

            // Re-link Make and Model
            if (cachedMakeName != null && LbConfig?.BonCoinCarBrands != null)
            {
                var realMake = LbConfig.BonCoinCarBrands.FirstOrDefault(m => m.Name == cachedMakeName);
                if (realMake != null)
                {
                    cacheItem.Make = realMake;
                    if (cachedModelName != null)
                    {
                        var realModel = realMake.Models.FirstOrDefault(m => m.Name == cachedModelName);
                        if (realModel != null) cacheItem.Model = realModel;
                    }
                }
            }

            // Re-link FuelType
            if (cachedFuelName != null && LbConfig?.Fuels != null)
            {
                var realFuel = LbConfig.Fuels.FirstOrDefault(f => f.Name == cachedFuelName);
                if (realFuel != null) cacheItem.FuelType = realFuel;
            }
        });
    }
}
    private void LoadCacheForStandVirtual()
    {
        try
        {
            if (File.Exists("last_StandVirtual_config_file"))
            {
                var json = File.ReadAllText("last_StandVirtual_config_file");
                var cachedFilters = JsonConvert.DeserializeObject<List<StandVirtualInputModel>>(json);

                if (cachedFilters != null && cachedFilters.Count > 0)
                {
                    StandVirtualFilters.Clear();

                    foreach (var cacheItem in cachedFilters)
                    {
                        // --- THE FIX: Save the names BEFORE the setters wipe them out ---
                        var cachedMakeName = cacheItem.StandVirtualMake?.Name;
                        var cachedModelName = cacheItem.StandVirtualModel?.Name;
                        var cachedSubModelName = cacheItem.StandVirtualSubModel?.Name;

                        if (cachedMakeName != null && StandVirtualConfig?.Makes != null)
                        {
                            // 1. Find and assign Make (This will trigger Model = null internally)
                            var realMake = StandVirtualConfig.Makes.FirstOrDefault(m => m.Name == cachedMakeName);
                            if (realMake != null)
                            {
                                cacheItem.StandVirtualMake = realMake;

                                // 2. Find and assign Model using our saved string (This will trigger SubModel = null)
                                if (cachedModelName != null)
                                {
                                    var realModel = realMake.Models.FirstOrDefault(m => m.Name == cachedModelName);
                                    if (realModel != null)
                                    {
                                        cacheItem.StandVirtualModel = realModel;

                                        // 3. Find and assign SubModel using our saved string
                                        if (cachedSubModelName != null)
                                        {
                                            var realSubModel =
                                                realModel.SubModels.FirstOrDefault(s => s.Name == cachedSubModelName);
                                            if (realSubModel != null)
                                            {
                                                cacheItem.StandVirtualSubModel = realSubModel;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        StandVirtualFilters.Add(cacheItem);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load cache: {ex.Message}");
        }

        if (StandVirtualFilters.Count == 0)
        {
            StandVirtualFilters.Add(new StandVirtualInputModel());
        }
    }
    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        SaveAllCaches();
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

    private void LbStartButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void LbStopButton_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
    
}