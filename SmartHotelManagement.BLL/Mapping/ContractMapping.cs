using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
                CreatedAt=DateTime.Now,
                CreatedBy =1
            };
       
        }
    }
}
