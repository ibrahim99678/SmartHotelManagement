using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Contract.Request;

public class CreateReservationRequest
{
    public int ReservationId { get; set; }
    public int GuestId { get; set; }
    public Guest? Guest { get; set; }
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    [StringLength(100)]
    public string ReferenceName { get; set; } = string.Empty;
    [Phone]
    public string? ReferencePhone { get; set; }
    [StringLength(100)]
    public string SpouseName { get; set; } = string.Empty;
    [Required]
    public DateTime CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int? StayInNight { get; set; } = 0;
    public decimal TotalAmount { get; set; } = 0;
    public string Status { get; set; } = "Booked"; // Booked, CheckedIn, CheckedOut, Cancelled
    public bool IsCheckedIn { get; set; }
    public bool IsCheckedOut { get; set; }
}

