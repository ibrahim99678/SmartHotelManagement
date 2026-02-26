using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Model;

public class RoomChange: Entity
{
    public int RoomChangeId { get; set; }

    public int ReservationId { get; set; }
    public int OldRoomId { get; set; }
    public int NewRoomId { get; set; }

    public DateTime ChangedOn { get; set; } = DateTime.Now;
    public string Reason { get; set; } = "";
}

