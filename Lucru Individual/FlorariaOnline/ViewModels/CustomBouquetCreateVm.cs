using System.ComponentModel.DataAnnotations;

namespace FlorariaOnline.ViewModels;

public class CustomBouquetCreateVm
{
    [Required]
    public string WrapType { get; set; } = "Classic";

    [StringLength(200)]
    public string? GreetingCardMessage { get; set; }

    [Range(0, 100000)]
    public decimal AssemblyFee { get; set; } = 30;

    // cheie = FlowerId, value = cantitate
    public Dictionary<int, int> Quantities { get; set; } = new();

    public List<FlowerRow> AvailableFlowers { get; set; } = new();

    public class FlowerRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Color { get; set; }
        public decimal PricePerStem { get; set; }
        public int Stock { get; set; }
    }
}