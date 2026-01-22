using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.Model;

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
        public static RoomType MapToRoomType(this CreateRoomTypeRequest request)
        {
            return new RoomType
            {
                RoomTypeName = request.RoomTypeName,
                DefaultRate = request.DefaultRate,
                CreatedAt = DateTime.Now,
                CreatedBy = 1,

            };
            
        }
        public static RoomType MapToRoomType(this UpdateRoomTypeRequest request)
        {
            if (request == null) return null!;
            return new RoomType
            {
                RoomTypeId = request.RoomTypeId,
                RoomTypeName = request.RoomTypeName,
                DefaultRate = request.DefaultRate,
                ModifiedAt = DateTime.Now,
                ModifiedBy = 1

            };
        }
        public static Room MapToRoom(this CreateRoomRequest request)
        {
            return new Room
            {
                RoomNumber = request.RoomNumber,
                RoomTypeId = request.RoomTypeId,
                FloorNo = request.FloorNo,
                Capacity = request.Capacity,
                BaseRate = request.BaseRate,
                Status = request.Status,
                RoomImage = request.RoomImage,
                Notes = request.Notes,
                IsActive = request.IsActive
            };
        }
        public static Reservation MapToReservation(this CreateReservationRequest request)
        {
            return new Reservation
            {
                GuestId = request.GuestId,
                RoomId = request.RoomId,
                CheckInDate = request.CheckInDate,
                CheckOutDate = (DateTime)request.CheckOutDate,                
                TotalAmount = request.TotalAmount,
                Status = request.Status,
                CreatedAt = DateTime.Now,
                CreatedBy = 1
            };
        }
       
    }
}