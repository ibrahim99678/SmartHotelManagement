using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.BLL.Mapping;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Implementations
{
    public class GuestService : IGuestService
    {
        private readonly IGuestUnitOfWork _guestUnitOfWork;

        public GuestService(IGuestUnitOfWork guestUnitOfWork)
        {
            _guestUnitOfWork = guestUnitOfWork;
        }

        public async Task<Result<int>> AddAsync(CreateGuestRequest guest)
        {
            if(guest is null)
            {
                return Result<int>.FailResult("Guest can not be null.");
            }
           

            //var existingGuest = await _guestUnitOfWork.GuestRepository.GetAsync(
            //    x => x.IDProofType, x => x.IDProofNumber == guest.IDProofNumber, null, null, false);

            //if (existingGuest.Any())
            //{
            //    return Result<int>.FailResult("The Guest with the same ID number has already exist.");
            //}

            try
            {
                var newGuest = guest.MapToGuest();
                
                await _guestUnitOfWork.GuestRepository.AddAsync(newGuest);
                var saved = await _guestUnitOfWork.SaveChangesAsync();

                if(!saved)
                {
                    return Result<int>.FailResult("Failed to save the guest.");
                }
                return Result<int>.SuccessResult(newGuest.GuestId);
            }
            catch (Exception)
            {
                return Result<int>.FailResult("An Error Occured While adding the Guest.");
            }
            
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var guest = await _guestUnitOfWork.GuestRepository.GetByIdAsync(id);

            if(guest is null)
            {
                return Result<bool>.FailResult("Guest is not found");
            }
            await _guestUnitOfWork.GuestRepository.DeleteAsync(guest);
            var saved = await _guestUnitOfWork.SaveChangesAsync();

            if (!saved)
            {
                return Result<bool>.FailResult("Fail to Delete the Guest.");
            }
            
            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<IList<Guest>>> GetAllAsync()
        {
            var guests = await _guestUnitOfWork.GuestRepository.GetAsync(
                x => x, null, 
                x => x.OrderByDescending(x => x.GuestId), null, true);
            return Result<IList<Guest>>.SuccessResult(guests);
        }

        public async Task<Result<Guest?>> GetByIdAsync(int id)
        {
            var guest = await _guestUnitOfWork.GuestRepository.GetByIdAsync(id);

            if(guest is null)
            {
                return Result<Guest>.FailResult($"Guest with id {guest.GuestId} is not found.");
            }
            return Result<Guest>.SuccessResult(guest);
        }

        public Task GetByIdAsync(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<int>> UpdateAsync(Guest guest)
        {
            if(guest is null)
            {
                return Result<int>.FailResult("Guest Data Can not be null.");
            }
            var existGuest = await _guestUnitOfWork.GuestRepository.GetByIdAsync(guest.GuestId);

            if(existGuest is null)
            {
                return Result<int>.FailResult($"Guest with id {guest.GuestId} is not found");
            }

            existGuest.FirstName = guest.FirstName;
            existGuest.LastName = guest.LastName;           
            existGuest.Address = guest.Address;         
            existGuest.GuestImage = guest.GuestImage;
            existGuest.IDProofType = guest.IDProofType;

            await _guestUnitOfWork.GuestRepository.UpdateAsync(guest);
            var saved = await _guestUnitOfWork.SaveChangesAsync();

            if (!saved)
            {
                return Result<int>.FailResult("Fail to update guest.");
            }

            return Result<int>.SuccessResult(existGuest.GuestId);
            
        }

        public async Task<Result<int>> UpdateAsync(UpdateGuestRequest guest)
        {
            if (guest is null)
            {
                return Result<int>.FailResult("Guest data cannot be null.");
            }

            // Assuming IDProofNumber is used to identify the guest to update
            var existingGuests = await _guestUnitOfWork.GuestRepository.GetAsync(
                x => x, x => x.IDProofNumber == guest.IDProofNumber, null, null, false);

            var existGuest = existingGuests.FirstOrDefault();
            if (existGuest is null)
            {
                return Result<int>.FailResult($"Guest with IDProofNumber {guest.IDProofNumber} is not found.");
            }

            existGuest.FirstName = guest.FirstName;
            existGuest.LastName = guest.LastName;
            existGuest.Address = guest.Address;
            existGuest.GuestImage = guest.GuestImage;
            existGuest.IDProofType = guest.IDProofType;
            existGuest.IsActive = guest.IsActive;

            await _guestUnitOfWork.GuestRepository.UpdateAsync(existGuest);
            var saved = await _guestUnitOfWork.SaveChangesAsync();

            if (!saved)
            {
                return Result<int>.FailResult("Fail to update guest.");
            }

            return Result<int>.SuccessResult(existGuest.GuestId);
        }
    }
}
