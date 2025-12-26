using ReservesOfRussia.BLL.Services;
using ReservesOfRussia.DAL.Models;
using System;
using System.Windows;

namespace ReservesOfRussia.UI
{
    public partial class AddEditReserveWindow : Window
    {
        private readonly ReserveService _reserveService;
        private readonly Reserve _currentReserve;

        public AddEditReserveWindow(ReserveService reserveService, Reserve reserveToEdit = null)
        {
            InitializeComponent();
            _reserveService = reserveService;
            _currentReserve = reserveToEdit ?? new Reserve();

            LoadRegions();

            if (reserveToEdit != null)
            {
                Title = "Изменить заповедник";
                // Populate fields with existing data
                txtName.Text = _currentReserve.Name;
                cmbRegion.SelectedValue = _currentReserve.RegionId;
                txtArea.Text = _currentReserve.Area.ToString();
                dpFoundationDate.SelectedDate = _currentReserve.FoundationDate;
                txtDescription.Text = _currentReserve.Description;
            }
            else
            {
                Title = "Добавить новый заповедник";
            }
        }

        private void LoadRegions()
        {
            try
            {
                cmbRegion.ItemsSource = _reserveService.GetAllRegions();
            }
            catch (Exception ex)
            {
                App.ShowError("Не удалось загрузить регионы.", ex);
                this.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // --- Input Gathering & Basic Validation ---
            if (cmbRegion.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите регион.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtArea.Text, out decimal area))
            {
                MessageBox.Show("Пожалуйста, введите корректное число для площади.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- Update the object ---
            _currentReserve.Name = txtName.Text;
            _currentReserve.RegionId = (int)cmbRegion.SelectedValue;
            _currentReserve.Area = area;
            _currentReserve.FoundationDate = dpFoundationDate.SelectedDate;
            _currentReserve.Description = txtDescription.Text;

            try
            {
                // --- Pass to BLL for saving ---
                _reserveService.SaveReserve(_currentReserve);
                DialogResult = true; // Signals success to the main window
            }
            catch (ArgumentException argEx)
            {
                // Catches validation errors from BLL
                MessageBox.Show(argEx.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // Catches other errors (e.g., database)
                App.ShowError("Произошла ошибка при сохранении.", ex);
            }
        }
    }
}
