namespace Ktpt.MusicSchool.Services;

public static class PriceRules
{
    public static decimal Price(decimal basePrice, bool hadOrders = false)
    {
        return basePrice;
    }

    public static string Availability(int seats) => seats > 5 ? "много" : "мало";

    public static bool LowStock(int seats) => seats <= 3;
}