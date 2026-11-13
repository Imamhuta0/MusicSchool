using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Ktpt.MusicSchool.Services;

public class CourseCard
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string Category { get; set; } = "";
    public string Content { get; set; } = "";
    public int Seats { get; set; }
    public string SeatsText { get; set; } = "";
    public decimal Price { get; set; }
    public string PriceText { get; set; } = "";
    public IBrush? Background { get; set; }
    public Bitmap? Image { get; set; }
}