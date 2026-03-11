using System.ComponentModel.DataAnnotations;

namespace FlorariaOnline.Models;

public class BouquetProductItem
{
    public int Id { get; set; }

    public int BouquetProductId { get; set; }
    public BouquetProduct? BouquetProduct { get; set; }

    public int FlowerId { get; set; }
    public Flower? Flower { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }
}