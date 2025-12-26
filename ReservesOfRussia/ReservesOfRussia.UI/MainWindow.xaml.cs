using ReservesOfRussia.BLL.Services;
using ReservesOfRussia.DAL.Models;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ReservesOfRussia.UI
{
    public partial class MainWindow : Window
    {
        private readonly ReserveService _reserveService;

        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["ReservesDbConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string 'ReservesDbConnection' not found in App.config.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            _reserveService = new ReserveService(connectionString);
            LoadReserves();
        }

        private void LoadReserves()
        {
            try
            {
                var reserves = _reserveService.GetAllReserves();
                reservesListBox.ItemsSource = reserves;
                ClearDetails();
            }
            catch (Exception ex)
            {
                App.ShowError("Failed to load reserves.", ex);
            }
        }

        private void ClearDetails()
        {
            detailsPanel.DataContext = null;
        }

        private void reservesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            detailsPanel.DataContext = reservesListBox.SelectedItem as Reserve;
        }

        private void btnAddReserve_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditReserveWindow(_reserveService);
            if (addEditWindow.ShowDialog() == true)
            {
                LoadReserves(); // Refresh the list
            }
        }

        private void btnEditReserve_Click(object sender, RoutedEventArgs e)
        {
            if (reservesListBox.SelectedItem is Reserve selectedReserve)
            {
                var addEditWindow = new AddEditReserveWindow(_reserveService, selectedReserve);
                if (addEditWindow.ShowDialog() == true)
                {
                    LoadReserves(); // Refresh the list
                }
            }
            else
            {
                MessageBox.Show("Please select a reserve to edit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDeleteReserve_Click(object sender, RoutedEventArgs e)
        {
            if (reservesListBox.SelectedItem is Reserve selectedReserve)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedReserve.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _reserveService.DeleteReserve(selectedReserve.Id);
                        LoadReserves(); // Refresh the list
                    }
                    catch (Exception ex)
                    {
                        App.ShowError($"Failed to delete reserve '{selectedReserve.Name}'.", ex);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a reserve to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnPrintInfo_Click(object sender, RoutedEventArgs e)
        {
            if (reservesListBox.SelectedItem is Reserve selectedReserve)
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    FlowDocument doc = new FlowDocument();
                    doc.Blocks.Add(new Paragraph(new Run(selectedReserve.Name) { FontSize = 20, FontWeight = FontWeights.Bold }));
                    doc.Blocks.Add(new Paragraph(new Run($"Region: {selectedReserve.Region?.Name}")));
                    doc.Blocks.Add(new Paragraph(new Run($"Area: {selectedReserve.Area:N2} sq. km")));
                    doc.Blocks.Add(new Paragraph(new Run($"Founded: {selectedReserve.FoundationDate:dd MMMM yyyy}")));
                    doc.Blocks.Add(new Paragraph(new Run(selectedReserve.Description)));

                    IDocumentPaginatorSource idpSource = doc;
                    printDialog.PrintDocument(idpSource.DocumentPaginator, $"Reserve Info: {selectedReserve.Name}");
                }
            }
            else
            {
                MessageBox.Show("Please select a reserve to print.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
