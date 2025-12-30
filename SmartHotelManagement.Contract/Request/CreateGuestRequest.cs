using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Contract.Request;

public class CreateGuestRequest
{
    public int GuestId { get; set; }
    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(15)]
    public string? Gender { get; set; } // Later we can convert to enum

    [StringLength(60)]
    public string? Nationality { get; set; }

    [StringLength(50)]
    public string? IDProofType { get; set; }

    [StringLength(80)]
    public string? IDProofNumber { get; set; }

    public IFormFile? ImageFile { get; set; }
    public string? GuestImage { get; set; }

    public bool IsActive { get; set; } = true;
}
