using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace FlorariaOnline.Services;

public class CartService
{
    private const string SessionKey = "CART_V1";
    private readonly IHttpContextAccessor _http;

    public CartService(IHttpContextAccessor http) => _http = http;

    private ISession Session => _http.HttpContext!.Session;

    public List<CartLine> GetCart()
    {
        var json = Session.GetString(SessionKey);
        return string.IsNullOrWhiteSpace(json)
            ? new List<CartLine>()
            : (JsonSerializer.Deserialize<List<CartLine>>(json) ?? new List<CartLine>());
    }

    public void SaveCart(List<CartLine> cart)
    {
        Session.SetString(SessionKey, JsonSerializer.Serialize(cart));
    }

    public void Clear() => Session.Remove(SessionKey);

    public decimal Total(List<CartLine> cart) => cart.Sum(x => x.UnitPrice * x.Quantity);

    public void AddOrIncrease(CartLine line)
    {
        var cart = GetCart();
        var existing = cart.FirstOrDefault(x => x.Key == line.Key);
        if (existing != null)
            existing.Quantity += line.Quantity;
        else
            cart.Add(line);

        SaveCart(cart);
    }

    public void SetQuantity(string key, int qty)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.Key == key);
        if (item == null) return;

        if (qty <= 0) cart.Remove(item);
        else item.Quantity = qty;

        SaveCart(cart);
    }

    public void Remove(string key)
    {
        var cart = GetCart();
        cart.RemoveAll(x => x.Key == key);
        SaveCart(cart);
    }
}