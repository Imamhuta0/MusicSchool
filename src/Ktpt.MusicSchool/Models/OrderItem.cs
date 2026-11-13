using System;
using System.Collections.Generic;

namespace Ktpt.MusicSchool.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int VariantId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public DateOnly LessonDate { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual LessonVariant Variant { get; set; } = null!;
}
