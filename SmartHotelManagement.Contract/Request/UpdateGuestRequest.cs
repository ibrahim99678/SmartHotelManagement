using System;
using System.ComponentModel.DataAnnotations;

namespace SmartHotelManagement.Contract.Request
{
    public class UpdateGuestRequest
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
        public string? Gender { get; set; }

        [StringLength(60)]
        public string? Nationality { get; set; }

        [StringLength(50)]
        public string? IDProofType { get; set; }

        [StringLength(80)]
        public string? IDProofNumber { get; set; }

        public string? GuestImage { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
