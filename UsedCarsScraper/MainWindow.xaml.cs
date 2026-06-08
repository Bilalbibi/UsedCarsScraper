using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using UsedCarsScraper.GeneralModels;
using UsedCarsScraper.LaCentraleModels;
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
    private CancellationTokenSource? _cts;

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

    public LaCentraleConfig? LcConfig
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
    public ObservableCollection<LaCentraleInputModel> LcFilters { get; set; } = [];


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

        try
        {
            LaCentraleGrid.Items.Clear();
            LaCentraleGrid.ItemsSource = LcFilters;
            LcFilters.Add(new LaCentraleInputModel());
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
            StandvirtualProgressBar.Value = info.Percentage;
            StandvirtualProgressLabel.Text = info.Message;
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

                StandvirtualProgressBar.Value = 0;
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

                StandvirtualProgressBar.Value = 100;
                var nextRunMsg =
                    $"Work done for today. Next run will be {DateTime.Now.AddDays(days):dd/MM/yyyy hh:mm:ss}";
                StandvirtualProgressLabel.Text = nextRunMsg;
                LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {nextRunMsg}\r\n");
                var delay = (int)(d2 - d1).TotalSeconds;
                StartBtn.IsEnabled = true;
                await Task.Delay(TimeSpan.FromSeconds(delay), token);
            } while (true);
        }
        catch (OperationCanceledException)
        {
            // THIS CATCHES THE STOP BUTTON CLICK
            StandvirtualProgressLabel.Text = "Scraping stopped by user.";
            LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] Scraping stopped by user.\r\n");
            StandvirtualProgressBar.Value = 0;
        }
        catch (Exception ex)
        {
            StandvirtualProgressLabel.Text = $"Error: {ex.Message}";
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
        CreateLaCentraleFolders();
        var svJson = await File.ReadAllTextAsync("StandVirtual config file");
        StandVirtualConfig = JsonConvert.DeserializeObject<StandVirtualConfig>(svJson);
    
        var lbJson = await File.ReadAllTextAsync("Leboncoin config file");
        LbConfig = JsonConvert.DeserializeObject<LeBonCoinConfig>(lbJson);
        
        if (File.Exists("LaCentrale config file"))
        {
            var lcJson = await File.ReadAllTextAsync("LaCentrale config file");
            LcConfig = JsonConvert.DeserializeObject<LaCentraleConfig>(lcJson);
            NormalizeLaCentraleFuelTypes();
        }

        // Notify the UI
        OnPropertyChanged(nameof(StandVirtualConfig));
        OnPropertyChanged(nameof(LbConfig));
        OnPropertyChanged(nameof(LcConfig));

        // NOW load the caches and populate the grids!
        LoadAllCaches();
    }

    private void NormalizeLaCentraleFuelTypes()
    {
        if (LcConfig?.FuelTypes == null)
        {
            return;
        }

        LcConfig.FuelTypes = LcConfig.FuelTypes.ToDictionary(
            fuel => fuel.Key,
            fuel => HumanizeFuelType(fuel.Value),
            StringComparer.OrdinalIgnoreCase);
    }

    private static string HumanizeFuelType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Replace('_', ' ').ToLowerInvariant());
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

    private static void CreateLaCentraleFolders()
    {
        Directory.CreateDirectory("lacentral configs");
        Directory.CreateDirectory("lacentral json outputs");
        Directory.CreateDirectory("lacentral outputs");
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
        DeleteSelectedRows(StandvirtualGrid, StandVirtualFilters);
    }
    private void LbDeleteRow_Click(object sender, RoutedEventArgs e)
    {
        DeleteSelectedRows(LeboncoinGrid, LbFilters);
    }

    private static void DeleteSelectedRows<T>(DataGrid grid, ObservableCollection<T> filters) where T : new()
    {
        grid.CommitEdit(DataGridEditingUnit.Row, true);
        grid.CancelEdit();

        var selectedItems = grid.SelectedItems
            .OfType<T>()
            .ToList();

        if (selectedItems.Count > 0)
        {
            foreach (var selectedItem in selectedItems)
            {
                filters.Remove(selectedItem);
            }
        }
        else if (filters.Count > 0)
        {
            filters.RemoveAt(filters.Count - 1);
        }

        if (filters.Count == 0)
        {
            filters.Add(new T());
        }
    }
    private void RowSelectCheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not CheckBox checkBox)
        {
            return;
        }

        var row = FindParent<DataGridRow>(checkBox);
        if (row == null)
        {
            return;
        }

        row.IsSelected = !row.IsSelected;
        e.Handled = true;
    }

    private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(child);
        while (parent != null && parent is not T)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parent as T;
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

        // Save La Centrale
        SaveCache(LaCentraleGrid, LcFilters, "last_LaCentrale_config_file");
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

    // 3. LOAD LA CENTRALE
    if (File.Exists("last_LaCentrale_config_file"))
    {
        LoadCache("last_LaCentrale_config_file", LcFilters, cacheItem =>
        {
            var cachedMakeName = cacheItem.Make?.Name;
            var cachedModelName = cacheItem.Model?.Name;
            var cachedSubModelName = cacheItem.SubModel?.Name;

            if (cachedMakeName != null && LcConfig?.Makes != null)
            {
                var realMake = LcConfig.Makes.FirstOrDefault(m => m.Name == cachedMakeName);
                if (realMake != null)
                {
                    cacheItem.Make = realMake;
                    if (cachedModelName != null)
                    {
                        var realModel = realMake.Models.FirstOrDefault(m => m.Name == cachedModelName);
                        if (realModel != null)
                        {
                            cacheItem.Model = realModel;
                            if (cachedSubModelName != null)
                            {
                                var realSubModel = realModel.SubModels.FirstOrDefault(s => s.Name == cachedSubModelName);
                                if (realSubModel != null) cacheItem.SubModel = realSubModel;
                            }
                        }
                    }
                }
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
            StandvirtualProgressLabel.Text = "Stopping scraper... please wait.";
            StopBtn.IsEnabled = false; // Prevent user from clicking STOP multiple times

            // This triggers the OperationCanceledException in the Start thread
            _cts.Cancel();
        }
    }

    private async void LbStartButton_Click(object sender, RoutedEventArgs e)
    {
        LeboncoinGrid.CommitEdit(DataGridEditingUnit.Row, true);
        LeboncoinGrid.CancelEdit();

        var models = LbFilters.ToList();
        var hasNullProp = models.Any(x => x?.Make == null);
        if (hasNullProp)
        {
            MessageBox.Show("Please make sure you selected Make for each Leboncoin input", "Reminding");
            return;
        }

        LbStartBtn.IsEnabled = false;
        LbStopBtn.IsEnabled = true;

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        var json = JsonConvert.SerializeObject(models, Formatting.Indented);
        await File.WriteAllTextAsync(
            $"leboncoin configs/{DateTime.Now:dd_MM_yyyy_HH_mm_ss}_Leboncoin_config_file.json", json, token);

        var leBonCoinService = new LeBonCoinService();
        var progressReporter = new Progress<(int Percentage, string Message)>(info =>
        {
            LeboncoinProgressBar.Value = info.Percentage;
            LeboncoinProgressLabel.Text = info.Message;
            LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {info.Message}\r\n");
            LogTextBox.ScrollToEnd();
        });

        try
        {
            do
            {
                token.ThrowIfCancellationRequested();

                LeboncoinProgressBar.Value = 0;
                var start = DateTime.Now;
                var days = LbThreeDays.IsChecked == true ? 3 : 1;

                await leBonCoinService.Start(models, progressReporter, token);

                LeboncoinProgressBar.Value = 100;
                var nextRunMsg =
                    $"Leboncoin work done. Next run will be {DateTime.Now.AddDays(days):dd/MM/yyyy HH:mm:ss}";
                LeboncoinProgressLabel.Text = nextRunMsg;
                LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {nextRunMsg}\r\n");

                LbStartBtn.IsEnabled = true;
                await Task.Delay(DateTime.Now.AddDays(days) - start, token);
            } while (true);
        }
        catch (OperationCanceledException)
        {
            LeboncoinProgressLabel.Text = "Leboncoin scraping stopped by user.";
            LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] Leboncoin scraping stopped by user.\r\n");
            LeboncoinProgressBar.Value = 0;
        }
        catch (Exception ex)
        {
            LeboncoinProgressLabel.Text = $"Leboncoin error: {ex.Message}";
            MessageBox.Show(ex.Message, "Leboncoin error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            LbStartBtn.IsEnabled = true;
            LbStopBtn.IsEnabled = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void LbStopButton_Click(object sender, RoutedEventArgs e)
    {
        if (_cts == null || _cts.IsCancellationRequested) return;

        LeboncoinProgressLabel.Text = "Stopping Leboncoin scraper... please wait.";
        LbStopBtn.IsEnabled = false;
        _cts.Cancel();
    }

    private void LcStopButton_Click(object sender, RoutedEventArgs e)
    {
        if (_cts == null || _cts.IsCancellationRequested) return;

        LaCentraleProgressLabel.Text = "Stopping LaCentrale scraper... please wait.";
        LcStopBtn.IsEnabled = false;
        _cts.Cancel();
    }

    private async void LcStartButton_Click(object sender, RoutedEventArgs e)
    {
        LaCentraleGrid.CommitEdit(DataGridEditingUnit.Row, true);
        LaCentraleGrid.CancelEdit();

        var models = LcFilters.ToList();
        var hasNullProp = models.Any(x => x.Make == null);
        if (hasNullProp)
        {
            MessageBox.Show("Please make sure you selected Make for each LaCentrale input", "Reminding");
            return;
        }

        LcStartBtn.IsEnabled = false;
        LcStopBtn.IsEnabled = true;

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        var json = JsonConvert.SerializeObject(models, Formatting.Indented);
        await File.WriteAllTextAsync(
            $"lacentral configs/{DateTime.Now:dd_MM_yyyy_HH_mm_ss}_LaCentrale_config_file.json", json, token);

        var laCentraleService = new LaCentraleService();
        var progressReporter = new Progress<(int Percentage, string Message)>(info =>
        {
            LaCentraleProgressBar.Value = info.Percentage;
            LaCentraleProgressLabel.Text = info.Message;
            LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {info.Message}\r\n");
            LogTextBox.ScrollToEnd();
        });

        try
        {
            do
            {
                token.ThrowIfCancellationRequested();

                LaCentraleProgressBar.Value = 0;
                var start = DateTime.Now;
                var days = LcThreeDays.IsChecked == true ? 3 : 1;

                await laCentraleService.Start(models, progressReporter, token);

                LaCentraleProgressBar.Value = 100;
                var nextRunMsg =
                    $"LaCentrale work done. Next run will be {DateTime.Now.AddDays(days):dd/MM/yyyy HH:mm:ss}";
                LaCentraleProgressLabel.Text = nextRunMsg;
                LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {nextRunMsg}\r\n");

                LcStartBtn.IsEnabled = true;
                await Task.Delay(DateTime.Now.AddDays(days) - start, token);
            } while (true);
        }
        catch (OperationCanceledException)
        {
            LaCentraleProgressLabel.Text = "LaCentrale scraping stopped by user.";
            LogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] LaCentrale scraping stopped by user.\r\n");
            LaCentraleProgressBar.Value = 0;
        }
        catch (Exception ex)
        {
            LaCentraleProgressLabel.Text = $"LaCentrale error: {ex.Message}";
            MessageBox.Show(ex.Message, "LaCentrale error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            LcStartBtn.IsEnabled = true;
            LcStopBtn.IsEnabled = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void LcAddRow_Click(object sender, RoutedEventArgs e)
    {
        LcFilters.Add(new LaCentraleInputModel());
        LaCentraleGrid.ScrollIntoView(LcFilters.Last());
    }

    private void LcDeleteRow_Click(object sender, RoutedEventArgs e)
    {
        DeleteSelectedRows(LaCentraleGrid, LcFilters);
    }
}




