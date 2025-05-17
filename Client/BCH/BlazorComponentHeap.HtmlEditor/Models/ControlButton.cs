using BlazorComponentHeap.HtmlEditor.Attributes;

namespace BlazorComponentHeap.HtmlEditor.Models;

public enum ControlButton
{
    [DocumentCommand("bold", 0)] Bold,
    [DocumentCommand("italic", 1)] Italic,
    [DocumentCommand("underline", 2)] Underline,

    [DocumentCommand("undo", 3)] Undo,
    [DocumentCommand("redo", 4)] Redo,

    [DocumentCommand("justifyLeft", 5)] JustifyLeft,
    [DocumentCommand("justifyRight", 6)] JustifyRight,
    [DocumentCommand("justifyCenter", 7)] JustifyCenter,
    [DocumentCommand("formatBlock", 8)] FormatBlock,

    [DocumentCommand("insertOrderedList", 9)] OrderedList,
    [DocumentCommand("insertUnorderedList", 10)] UnorderedList,

    [DocumentCommand("indent", 11)] Indent,
    [DocumentCommand("outdent", 12)] Outdent,

    [DocumentCommand("foreColor")] ColorText,
    [DocumentCommand("insertImage", 13)] InsertImage,
    [DocumentCommand("unlink", 14)] RemoveLink,
    [DocumentCommand("createLink")] InsertLink,
    [DocumentCommand("fontSize")] FontSize,
    [DocumentCommand("insertHTML")] InsertHTML,

    Insert3Images,
    Insert2Images,
    Insert1Image,

    Separator,

    Source,
    Preview
}