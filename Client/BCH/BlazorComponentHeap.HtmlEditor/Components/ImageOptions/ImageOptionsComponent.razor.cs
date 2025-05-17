using Microsoft.AspNetCore.Components;
using BlazorComponentHeap.Core.Extensions;
using BlazorComponentHeap.HtmlEditor.Attributes;
using BlazorComponentHeap.HtmlEditor.Models;

namespace BlazorComponentHeap.HtmlEditor.Components.ImageOptions;

public partial class ImageOptionsComponent
{
    [Parameter] public EventCallback OnImageReplaceClicked { get; set; }
    [Parameter] public int CommandsActivated { get; set; }
    [Parameter] public EventCallback<(ControlButton, string)> OnButtonClick { get; set; }

    private Task OnImageReplaceClickedAsync()
    {
        return OnImageReplaceClicked.InvokeAsync();
    }

    private Task OnAlignTextSelectedAsync(string? text)
    {
        if (string.IsNullOrEmpty(text)) return Task.CompletedTask;
        
        var control = text switch
        {
            "left" => ControlButton.JustifyLeft,
            "right" => ControlButton.JustifyRight,
            "justify" => ControlButton.JustifyCenter,
            _ => ControlButton.FormatBlock
        };

        if (IsActive(control)) return Task.CompletedTask;

        return OnButtonClickedAsync(control);
    }

    private async Task OnButtonClickedAsync(ControlButton control, string value = null!)
    {
        await OnButtonClick.InvokeAsync((control, value));
    }

    private bool IsActive(ControlButton button)
    {
        return IsKthBitSet(CommandsActivated, button.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));
    }

    private bool IsKthBitSet(int n, int k) => (n & (1 << k)) != 0;
}