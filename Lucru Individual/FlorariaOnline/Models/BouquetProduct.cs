using System.ComponentModel.DataAnnotations;

namespace FlorariaOnline.Models;

public class BouquetProduct
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(0, 100000)]
    public decimal BasePrice { get; set; }

    [StringLength(400)]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public List<BouquetProductItem> Items { get; set; } = new();
}