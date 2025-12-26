using ReservesOfRussia.DAL;
using ReservesOfRussia.DAL.Models;
using System;
using System.Collections.Generic;

namespace ReservesOfRussia.BLL.Services
{
    public class ReserveService
    {
        private readonly ReserveRepository _repository;

        public ReserveService(string connectionString)
        {
            _repository = new ReserveRepository(connectionString);
        }

        public List<Reserve> GetAllReserves()
        {
            try
            {
                return _repository.GetAllReserves();
            }
            catch (Exception ex)
            {
                // In a real application, you would log this exception
                throw new Exception("An error occurred while fetching the reserves.", ex);
            }
        }

        public List<Region> GetAllRegions()
        {
            try
            {
                return _repository.GetAllRegions();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the regions.", ex);
            }
        }

        public void SaveReserve(Reserve reserve)
        {
            // --- Business Logic: Validation ---
            if (reserve == null)
            {
                throw new ArgumentNullException(nameof(reserve), "Reserve cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(reserve.Name))
            {
                throw new ArgumentException("The reserve name cannot be empty.", nameof(reserve.Name));
            }

            if (reserve.Area <= 0)
            {
                throw new ArgumentException("The area must be a positive number.", nameof(reserve.Area));
            }

            if (reserve.RegionId <= 0)
            {
                throw new ArgumentException("A region must be selected.", nameof(reserve.RegionId));
            }

            try
            {
                if (reserve.Id == 0)
                {
                    _repository.AddReserve(reserve);
                }
                else
                {
                    _repository.UpdateReserve(reserve);
                }
            }
            catch (Exception ex)
            {
                // In a real application, you would log this exception
                throw new Exception($"An error occurred while saving the reserve '{reserve.Name}'.", ex);
            }
        }

        public void DeleteReserve(int reserveId)
        {
            if (reserveId <= 0)
            {
                throw new ArgumentException("Invalid reserve ID.", nameof(reserveId));
            }

            try
            {
                _repository.DeleteReserve(reserveId);
            }
            catch (Exception ex)
            {
                // In a real application, you would log this exception
                throw new Exception($"An error occurred while deleting the reserve with ID {reserveId}.", ex);
            }
        }
    }
}
