using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Contract.Request.Account;

public class RegisterModel
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }= string.Empty;
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    [StringLength(50)]
    [DataType(DataType.Password)]
    [Compare("Password",ErrorMessage = "The Password and Confirm Password don not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

}
