using BlazorComponentHeap.Core.Models.Markup;

namespace BlazorComponentHeap.HtmlEditor.Events;

public class SelectionEventArgs : EventArgs
{
    public List<CoordsHolder> PathCoordinates { get; set; } = new();
    public string? TextContent { get; set; }
}