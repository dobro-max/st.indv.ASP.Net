using System.ComponentModel.DataAnnotations;

namespace FlorariaOnline.Models;

public class CustomBouquetItem
{
    public int Id { get; set; }

    public int CustomBouquetId { get; set; }
    public CustomBouquet? CustomBouquet { get; set; }

    public int FlowerId { get; set; }
    public Flower? Flower { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }
}