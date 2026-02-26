using System;
using System.Collections.Generic;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.Web.Models;

public class DashboardViewModel
{
    public IList<Reservation> TodayReservations { get; set; } = new List<Reservation>();
    public IList<Reservation> Last7DaysReservations { get; set; } = new List<Reservation>();
    public int AvailableRoomsCount { get; set; }
    public int EmployeeCount { get; set; }
    public decimal MonthlySalesTotal { get; set; }
    public IDictionary<string, decimal> MonthlySalesByRoomType { get; set; } = new Dictionary<string, decimal>();
}
