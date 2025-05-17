using BlazorComponentHeap.HtmlEditor;
using BlazorComponentHeap.HtmlEditor.Models;

namespace HtmlToPdf.WebWasm.Pages.Main;

public partial class MainPage
{
    private HtmlEditor? _htmlEditor;

    private readonly List<ToolbarItemModel> _toolbarKeys = new()
    {
        new ToolbarItemModel { Key = "save-to-pdf", Hint = "Save To Pdf" },

        new ToolbarItemModel { Key = nameof(ControlButton.Separator).ToLower() },
        new ToolbarItemModel { Key = nameof(ControlButton.Undo).ToLower(), Hint = "Undo" },
        new ToolbarItemModel { Key = nameof(ControlButton.Redo).ToLower(), Hint = "Redo" },
        new ToolbarItemModel { Key = nameof(ControlButton.Separator).ToLower() },
        new ToolbarItemModel { Key = nameof(ControlButton.Bold).ToLower(), Hint = "Bold" },
        new ToolbarItemModel { Key = nameof(ControlButton.Italic).ToLower(), Hint = "Italic" },
        new ToolbarItemModel { Key = nameof(ControlButton.Underline).ToLower(), Hint = "Underline" },
        new ToolbarItemModel { Key = nameof(ControlButton.Separator).ToLower() },
        new ToolbarItemModel { Key = nameof(ControlButton.FontSize).ToLower(), Hint = "Font Size" },
        new ToolbarItemModel { Key = "headings", Hint = "Headings" },
        new ToolbarItemModel { Key = "align", Hint = "Align" },
        new ToolbarItemModel { Key = nameof(ControlButton.ColorText).ToLower(), Hint = "Text Color" },
        new ToolbarItemModel { Key = nameof(ControlButton.Separator).ToLower() },
        new ToolbarItemModel { Key = "table", Hint = "Table" },
        new ToolbarItemModel { Key = nameof(ControlButton.Separator).ToLower() },
        new ToolbarItemModel { Key = nameof(ControlButton.OrderedList).ToLower(), Hint = "Ordered List" },
        new ToolbarItemModel { Key = nameof(ControlButton.UnorderedList).ToLower(), Hint = "Unordered List" },
        new ToolbarItemModel { Key = nameof(ControlButton.Separator).ToLower() },
        new ToolbarItemModel { Key = nameof(ControlButton.Indent).ToLower(), Hint = "Indent" },
        new ToolbarItemModel { Key = nameof(ControlButton.Outdent).ToLower(), Hint = "Outdent" },
        new ToolbarItemModel { Key = nameof(ControlButton.Separator).ToLower() },
        new ToolbarItemModel { Key = "gallery", Hint = "Gallery" },
        new ToolbarItemModel { Key = nameof(ControlButton.Separator).ToLower() },
        new ToolbarItemModel { Key = nameof(ControlButton.RemoveLink).ToLower(), Hint = "Remove Link" },
        new ToolbarItemModel { Key = nameof(ControlButton.InsertLink).ToLower(), Hint = "Insert Link" },

        new ToolbarItemModel { Key = nameof(ControlButton.Separator).ToLower() },

        new ToolbarItemModel { Key = "mode", Hint = "View Mode" },
        new ToolbarItemModel { Key = nameof(ControlButton.Separator).ToLower() },

        new ToolbarItemModel { Key = "doc-template" },
    };

    private async Task OnChangeInEditorAsync()
    {
        
    }
    
    private async Task OnViewModeChangedAsync(EditorViewMode viewMode)
    {
        
    }

    private async Task OnEditorLoadedAsync()
    {
        
    }

    private async Task OnReplaceImageClickedAsync()
    {
        
    }

    private async Task OnSelectImageAsync()
    {
        
    }
}