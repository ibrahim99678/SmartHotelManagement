using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Contract.Request
{
    public class UpdateRoomTypeRequest
    {
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal DefaultRate { get; set; } = 0;
    }
}
