using System.ComponentModel.DataAnnotations;

namespace FlorariaOnline.Models;

public class CustomBouquet
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }
    public OrderItem? OrderItem { get; set; }

    [Required, StringLength(40)]
    public string WrapType { get; set; } = "Classic";

    [StringLength(200)]
    public string? GreetingCardMessage { get; set; }

    [Range(0, 100000)]
    public decimal AssemblyFee { get; set; } = 30;

    public List<CustomBouquetItem> Items { get; set; } = new();
}