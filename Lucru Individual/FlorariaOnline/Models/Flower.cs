using System.ComponentModel.DataAnnotations;

namespace FlorariaOnline.Models;

public class Flower
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = "";

    [StringLength(40)]
    public string? Color { get; set; }

    [Range(0, 100000)]
    public decimal PricePerStem { get; set; }

    [Range(0, 100000)]
    public int Stock { get; set; }

    [Range(0, 100000)]
    public int MinimumStock { get; set; } = 10;

    [StringLength(400)]
    public string? ImageUrl { get; set; }
}