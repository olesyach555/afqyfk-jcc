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
                Title = "Edit Reserve";
                // Populate fields with existing data
                txtName.Text = _currentReserve.Name;
                cmbRegion.SelectedValue = _currentReserve.RegionId;
                txtArea.Text = _currentReserve.Area.ToString();
                dpFoundationDate.SelectedDate = _currentReserve.FoundationDate;
                txtDescription.Text = _currentReserve.Description;
            }
            else
            {
                Title = "Add New Reserve";
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
                App.ShowError("Failed to load regions.", ex);
                this.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // --- Input Gathering & Basic Validation ---
            if (cmbRegion.SelectedValue == null)
            {
                MessageBox.Show("Please select a region.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtArea.Text, out decimal area))
            {
                MessageBox.Show("Please enter a valid number for the area.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show(argEx.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // Catches other errors (e.g., database)
                App.ShowError("An error occurred while saving.", ex);
            }
        }
    }
}
