using Microsoft.AspNetCore.Components.Web;
using BlazorComponentHeap.Core.Models.Markup;

namespace BlazorComponentHeap.HtmlEditor.Events;

public class IFrameMouseDownEvent : MouseEventArgs
{
    public CoordsHolder? CoordsHolder { get; set; }
    public BoundingClientRect? Rect { get; set; }
    public bool HasTableTag { get; set; }
}
