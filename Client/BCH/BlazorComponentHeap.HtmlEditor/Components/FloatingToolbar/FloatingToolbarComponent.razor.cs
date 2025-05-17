using Microsoft.AspNetCore.Components;
using BlazorComponentHeap.Core.Extensions;
using BlazorComponentHeap.HtmlEditor.Attributes;
using BlazorComponentHeap.HtmlEditor.Models;

namespace BlazorComponentHeap.HtmlEditor.Components.FloatingToolbar;

public partial class FloatingToolbarComponent
{
    [Parameter] public int CommandsActivated { get; set; }
    [Parameter] public string Paragraph { get; set; } = string.Empty;
    [Parameter] public EventCallback<(ControlButton, string)> OnButtonClick { get; set; }
    
    private async Task OnAlignTextSelectedAsync(string? text)
    {
        if (string.IsNullOrEmpty(text)) return;
        
        var control = text switch
        {
            "left" => ControlButton.JustifyLeft,
            "right" => ControlButton.JustifyRight,
            "justify" => ControlButton.JustifyCenter,
            _ => ControlButton.FormatBlock
        };

        if (IsActive(control)) return;

        await OnButtonClickedAsync(control);
    }
    
    private async Task OnButtonClickedAsync(ControlButton control, string value = null!)
    {
        await OnButtonClick.InvokeAsync((control, value));
    }
    
    private bool IsActive(ControlButton button)
    {
        return IsKthBitSet(CommandsActivated, button.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));
    }
    
    private async Task OnParagraphSelectedAsync(string? text)
    {
        if (string.IsNullOrEmpty(text)) return; 
        
        var parameters = text switch
        {
            "H1" => "<h1>",
            "H2" => "<h2>",
            "H3" => "<h3>",
            "H4" => "<h4>",
            "H5" => "<h5>",
            "H6" => "<h6>",
            "Code" => "<pre>",
            "Quotation" => "<blockquote>",
            _ => "<p>"
        };

        await OnButtonClick.InvokeAsync((ControlButton.FormatBlock, parameters));
    }
    
    private bool IsKthBitSet(int n, int k) => (n & (1 << k)) != 0;
}