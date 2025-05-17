using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.HtmlEditor.Components.Toolbar.Item;

public class ToolbarItemComponent : ComponentBase
{
    [Parameter] public string Key { get; set; } = string.Empty;
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;

    [CascadingParameter(Name = "HtmlTextEditorComponent")] public HtmlEditor OwnerContainer { get; set; } = null!;

    protected override void OnInitialized()
    {
        OwnerContainer.AddCustomToolbarItem(this);
    }
}