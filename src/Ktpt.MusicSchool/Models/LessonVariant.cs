using System;
using System.Collections.Generic;

namespace Ktpt.MusicSchool.Models;

public partial class LessonVariant
{
    public int VariantId { get; set; }

    public int LessonId { get; set; }

    public int TeacherId { get; set; }

    public string VariantName { get; set; } = null!;

    public int AvailableSeats { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User Teacher { get; set; } = null!;
}
