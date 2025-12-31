using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.Model;
using System;

namespace SmartHotelManagement.BLL.Mapping
{
    public static class ContractMapping
    {
        public static Guest MapToGuest(this CreateGuestRequest request)
        {
            return new Guest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Nationality = request.Nationality,
                IDProofNumber = request.IDProofNumber,
                IDProofType = request.IDProofType,
                GuestImage = request.GuestImage,
                IsActive = request.IsActive,
                CreatedAt = DateTime.Now,
                CreatedBy = 1
            };

        }

        public static UpdateGuestRequest MapToUpdateGuestRequest(this Guest guest)
        {
            if (guest == null) return null!;
            return new UpdateGuestRequest
            {
                GuestId = guest.GuestId,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                PhoneNumber = guest.PhoneNumber,
                Email = guest.Email,
                Address = guest.Address,
                DateOfBirth = guest.DateOfBirth,
                Gender = guest.Gender,
                Nationality = guest.Nationality,
                IDProofType = guest.IDProofType,
                IDProofNumber = guest.IDProofNumber,
                GuestImage = guest.GuestImage,
                IsActive = guest.IsActive
            };
        }

        public static Guest MapToGuest(this UpdateGuestRequest request)
        {
            if (request == null) return null!;
            return new Guest
            {
                GuestId = request.GuestId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Nationality = request.Nationality,
                IDProofType = request.IDProofType,
                IDProofNumber = request.IDProofNumber,
                GuestImage = request.GuestImage,
                IsActive = request.IsActive
            };
        }
    }
}
