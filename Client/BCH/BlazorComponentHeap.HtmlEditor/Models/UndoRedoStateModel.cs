using BlazorComponentHeap.Core.Models.Markup;

namespace BlazorComponentHeap.HtmlEditor.Models;

public class UndoRedoStateModel
{
    public bool Undo { get; set; }
    public bool Redo { get; set; }
    public List<CoordsHolder> PathCoordinates { get; set; } = new();
}