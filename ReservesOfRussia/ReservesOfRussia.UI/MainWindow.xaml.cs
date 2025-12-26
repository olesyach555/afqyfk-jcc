using ReservesOfRussia.BLL.Services;
using ReservesOfRussia.DAL;
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
                MessageBox.Show("Строка подключения 'ReservesDbConnection' не найдена в App.config.", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            // --- Initialize the database on startup ---
            try
            {
                DatabaseSetup.InitializeDatabase(connectionString);
            }
            catch (Exception ex)
            {
                App.ShowError("Произошла критическая ошибка при настройке базы данных.", ex);
                Application.Current.Shutdown();
                return;
            }
            // ------------------------------------------

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
                App.ShowError("Не удалось загрузить заповедники.", ex);
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
                MessageBox.Show("Пожалуйста, выберите заповедник для изменения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDeleteReserve_Click(object sender, RoutedEventArgs e)
        {
            if (reservesListBox.SelectedItem is Reserve selectedReserve)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить '{selectedReserve.Name}'?", "Подтвердите удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _reserveService.DeleteReserve(selectedReserve.Id);
                        LoadReserves(); // Refresh the list
                    }
                    catch (Exception ex)
                    {
                        App.ShowError($"Не удалось удалить заповедник '{selectedReserve.Name}'.", ex);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заповедник для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    doc.Blocks.Add(new Paragraph(new Run($"Регион: {selectedReserve.Region?.Name}")));
                    doc.Blocks.Add(new Paragraph(new Run($"Площадь: {selectedReserve.Area:N2} кв. км")));
                    doc.Blocks.Add(new Paragraph(new Run($"Дата основания: {selectedReserve.FoundationDate:dd MMMM yyyy}")));
                    doc.Blocks.Add(new Paragraph(new Run(selectedReserve.Description)));

                    IDocumentPaginatorSource idpSource = doc;
                    printDialog.PrintDocument(idpSource.DocumentPaginator, $"Информация о заповеднике: {selectedReserve.Name}");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заповедник для печати.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
