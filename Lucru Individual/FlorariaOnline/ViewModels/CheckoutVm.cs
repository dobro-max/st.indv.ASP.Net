using System.ComponentModel.DataAnnotations;

namespace FlorariaOnline.ViewModels;

public class CheckoutVm
{
    [Required, StringLength(80)]
    public string DeliveryName { get; set; } = "";

    [Required, StringLength(30)]
    public string Phone { get; set; } = "";

    [Required, StringLength(200)]
    public string Address { get; set; } = "";

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = "Cash";
}