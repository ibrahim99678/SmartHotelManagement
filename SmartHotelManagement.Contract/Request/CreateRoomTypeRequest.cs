using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Contract.Request
{
    public class CreateRoomTypeRequest
    {
        public int RoomTypeId { get; set; }
        [Required, StringLength(100)]
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal DefaultRate { get; set; } = 0;
        
    }
}
