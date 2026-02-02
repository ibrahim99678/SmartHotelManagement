namespace SmartHotelManagement.Web.Models;

public class RoomViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string ImageUrl { get; set; }
    public double Rating { get; set; }
    public bool IsNew { get; set; } = false;
}