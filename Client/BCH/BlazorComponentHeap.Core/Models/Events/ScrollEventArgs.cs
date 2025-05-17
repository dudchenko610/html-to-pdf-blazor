using BlazorComponentHeap.Core.Models.Markup;

namespace BlazorComponentHeap.Core.Models.Events;

public class ScrollEventArgs : EventArgs
{
    public double ClientHeight { get; set; }
    public double ScrollHeight { get; set; }
    public double ScrollTop { get; set; }
    public double ClientWidth { get; set; }
    
    public List<CoordsHolder> PathCoordinates { get; set; } = new();
}
