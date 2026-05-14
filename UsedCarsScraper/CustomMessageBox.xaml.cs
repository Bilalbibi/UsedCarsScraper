using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UsedCarsScraper
{
    public partial class CustomMessageBox : Window
    {
        private MessageBoxResult _result;

        public CustomMessageBox() => InitializeComponent();

        /// <summary>
        /// Shows a custom message box.
        /// </summary>
        public static MessageBoxResult Show(string message, string title = "Information", 
            MessageBoxImage icon = MessageBoxImage.Information, MessageBoxButton buttons = MessageBoxButton.OK,
            Window owner = null)
        {
            var dialog = new CustomMessageBox
            {
                Title = title,
                MessageText = { Text = message },
                Owner = owner ?? Application.Current.MainWindow
            };

            dialog.SetIcon(icon);
            dialog.CreateButtons(buttons);
            dialog.ShowDialog();
            return dialog._result;
        }

        private void SetIcon(MessageBoxImage icon)
        {
            var path = new Path { Fill = GetIconBrush(icon), Data = GetIconGeometry(icon) };
            MessageIcon.Content = path;
        }

        private Brush GetIconBrush(MessageBoxImage icon) => icon switch
        {
            MessageBoxImage.Error => Brushes.Red,
            MessageBoxImage.Warning => Brushes.Orange,
            MessageBoxImage.Question => Brushes.Blue,
            _ => Brushes.Gray
        };

        private Geometry GetIconGeometry(MessageBoxImage icon) => icon switch
        {
            // Simple geometric icons (can replace with your own PathData)
            MessageBoxImage.Error   => Geometry.Parse("M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z"),
            MessageBoxImage.Warning => Geometry.Parse("M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z"),
            MessageBoxImage.Question=> Geometry.Parse("M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 17h-2v-2h2v2zm2.07-7.75l-.9.92C13.45 12.9 13 13.5 13 15h-2v-.5c0-1.1.45-2.1 1.17-2.83l1.24-1.26c.37-.36.59-.86.59-1.41 0-1.1-.9-2-2-2s-2 .9-2 2H8c0-2.21 1.79-4 4-4s4 1.79 4 4c0 .88-.36 1.68-.93 2.25z"),
            _ => Geometry.Parse("M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z")
        };

        private void CreateButtons(MessageBoxButton buttons)
        {
            ButtonPanel.Children.Clear();
            void AddButton(string text, MessageBoxResult result, bool isDefault = false, bool isCancel = false)
            {
                var btn = new Button
                {
                    Content = text,
                    MinWidth = 80,
                    Margin = new Thickness(5, 0, 0, 0),
                    Padding = new Thickness(10),
                    IsDefault = isDefault,
                    IsCancel = isCancel
                };
                btn.Click += (s, e) => { _result = result; Close(); };
                ButtonPanel.Children.Add(btn);
            }

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AddButton("OK", MessageBoxResult.OK, isDefault: true);
                    break;
                case MessageBoxButton.OKCancel:
                    AddButton("Cancel", MessageBoxResult.Cancel, isCancel: true);
                    AddButton("OK", MessageBoxResult.OK, isDefault: true);
                    break;
                case MessageBoxButton.YesNo:
                    AddButton("No", MessageBoxResult.No, isCancel: true);
                    AddButton("Yes", MessageBoxResult.Yes, isDefault: true);
                    break;
                case MessageBoxButton.YesNoCancel:
                    AddButton("Cancel", MessageBoxResult.Cancel, isCancel: true);
                    AddButton("No", MessageBoxResult.No);
                    AddButton("Yes", MessageBoxResult.Yes, isDefault: true);
                    break;
            }

            // Focus the default button
            if (ButtonPanel.Children.Count > 0)
                ((Button)ButtonPanel.Children[^1]).Focus();
        }
    }
}