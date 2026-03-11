using System.ComponentModel.DataAnnotations;

namespace FlorariaOnline.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    [Required, StringLength(20)]
    public string ItemType { get; set; } = "Standard"; // Standard / Custom

    [Required, StringLength(200)]
    public string Name { get; set; } = "";

    [Range(0, 100000)]
    public decimal UnitPrice { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }

    [Range(0, 100000)]
    public decimal LineTotal { get; set; }

    public CustomBouquet? CustomBouquet { get; set; }
}