using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Model;

public class Reservation : Entity
{
    public int ReservationId { get; set; }
    public int GuestId { get; set; }
    public Guest? Guest { get; set; }
    public int RoomId { get; set; } 
    public Room? Room { get; set; }
    public string ReferenceName { get; set; } = string.Empty;
    public int? ReferencePhone { get; set; }
    public string SpouseName { get; set; }= string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int? StayInNight { get; set; } = 0;
    public decimal TotalAmount { get; set; } = 0;
    public string Status { get; set; } = "Booked"; // Booked, CheckedIn, CheckedOut, Cancelled
    public bool IsCheckedIn { get; set; }
    public bool IsCheckedOut { get; set; }
}
