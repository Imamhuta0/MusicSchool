using System;
using System.Collections.Generic;

namespace Ktpt.MusicSchool.Models;

public partial class Lesson
{
    public int LessonId { get; set; }

    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public decimal BasePrice { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<LessonVariant> LessonVariants { get; set; } = new List<LessonVariant>();
}
