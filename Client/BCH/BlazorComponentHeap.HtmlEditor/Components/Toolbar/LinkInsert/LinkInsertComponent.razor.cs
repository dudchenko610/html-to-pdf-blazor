using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.HtmlEditor.Components.Toolbar.LinkInsert;

public partial class LinkInsertComponent : IDisposable
{
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    [Parameter] public EventCallback<string> OnOkayClicked { get; set; }
    [Parameter] public bool IsImage { get; set; }

    private string _value = string.Empty;
    private ElementReference _inputRef;
    private bool _hidden = true;

    private DotNetObjectReference<LinkInsertComponent> _dotNetRef = null!;

    private readonly string _containerId = $"_id{Guid.NewGuid()}";
    private readonly string _inputId = $"_id{Guid.NewGuid()}";

    protected override void OnInitialized()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JsRuntime.InvokeVoidAsync("addOnOuterFocusOut", _inputId, _containerId, _dotNetRef, "OnContainerFocusOutAsync");

        if (!_hidden) await _inputRef.FocusAsync();
    }

    public void Dispose()
    {
        _dotNetRef?.Dispose();
    }

    private void OnIconClicked()
    {
        _hidden = !_hidden;
        StateHasChanged();
    }

    private async Task OnOkayClickedAsync()
    {
        await OnOkayClicked.InvokeAsync(_value);

        _value = string.Empty;
        _hidden = true;
        StateHasChanged();
    }

    [JSInvokable] public Task OnContainerFocusOutAsync()
    {
        _hidden = true;
        StateHasChanged();

        return Task.CompletedTask;
    }
}
