namespace Ktpt.MusicSchool.Models;

public partial class User
{
    public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    public override string ToString() => $"{Login} — {FullName}";
}

public partial class LessonVariant
{
    public override string ToString() => $"{VariantName} — свободно мест: {AvailableSeats}";
}

public partial class Order
{
    public override string ToString() => $"Заказ № {OrderId} · {OrderDate:dd.MM.yyyy} · {Client?.FullName}";
}

public partial class OrderItem
{
    public decimal Total => Quantity * UnitPrice;

    public override string ToString() =>
        $"{Variant?.Lesson?.Title} / {Variant?.VariantName} / " +
        $"Дата: {LessonDate:dd.MM.yyyy} / " +
        $"{Quantity} × {UnitPrice:N2} = {Total:N2} руб.";
}

public partial class Category
{
    public override string ToString() => CategoryName;
}