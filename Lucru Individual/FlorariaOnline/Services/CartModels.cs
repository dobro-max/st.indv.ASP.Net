namespace FlorariaOnline.Services;

public class CartLine
{
    public string Key { get; set; } = "";          // ex: "STD-3" sau "CUS-<guid>"
    public string ItemType { get; set; } = "Standard";
    public int? ProductId { get; set; }
    public string Name { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;

    // Pentru custom
    public string? WrapType { get; set; }
    public string? GreetingCardMessage { get; set; }
    public decimal? AssemblyFee { get; set; }
    public List<CustomFlowerLine>? Flowers { get; set; }
}

public class CustomFlowerLine
{
    public int FlowerId { get; set; }
    public string Name { get; set; } = "";
    public decimal PricePerStem { get; set; }
    public int Quantity { get; set; }
}