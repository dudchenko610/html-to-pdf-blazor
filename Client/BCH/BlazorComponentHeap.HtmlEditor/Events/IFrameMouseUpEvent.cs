namespace BlazorComponentHeap.HtmlEditor.Events;

public class IFrameMouseUpEvent
{
    public float PageX { get; set; }
    public float PageY { get; set; }

    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
}