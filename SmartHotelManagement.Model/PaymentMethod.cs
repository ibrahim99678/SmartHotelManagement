using System.ComponentModel.DataAnnotations;

namespace SmartHotelManagement.Model;

public class PaymentMethod : Entity
{
    public int PaymentMethodId { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Provider { get; set; }
    public string? MaskedAccount { get; set; }
    public bool IsActive { get; set; } = true;
}
