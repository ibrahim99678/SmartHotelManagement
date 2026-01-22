using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Model;

public class Room : Entity
{
    public int RoomId { get; set; }
    [Required,StringLength(10)]
    public string RoomNumber { get; set; } = string.Empty;
    public int RoomTypeId { get; set; }
    public RoomType? RoomType { get; set; }
    public int? FloorNo { get; set; }
    public int? Capacity { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal BaseRate { get; set; } = 0;
    public string Status { get; set; }= "Available";// Available, Occupied, Maintenance
    public string? RoomImage { get; set; }
    [StringLength(500)]
    public string? Notes { get; set; }
    public bool IsActive { get; set; }=true;
}




