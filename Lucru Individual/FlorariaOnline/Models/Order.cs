using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FlorariaOnline.Models;

public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";
    public IdentityUser? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.New;

    [Required, StringLength(80)]
    public string DeliveryName { get; set; } = "";

    [Required, StringLength(30)]
    public string Phone { get; set; } = "";

    [Required, StringLength(200)]
    public string Address { get; set; } = "";

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required, StringLength(30)]
    public string PaymentMethod { get; set; } = "Cash";

    public decimal Total { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}