using Microsoft.JSInterop;

namespace BlazorComponentHeap.HtmlEditor;

public partial class HtmlEditor
{
    public async Task InsertRowAboveTheSelectionAsync()
    {
        _showTableOptions = false;
        StateHasChanged();
        await JsRuntime.InvokeVoidAsync("insertRowAboveTheSelection", _previewId);
    }

    public async Task InsertRowBelowTheSelectionAsync()
    {
        _showTableOptions = false;
        StateHasChanged();
        await JsRuntime.InvokeVoidAsync("insertRowBelowTheSelection", _previewId);
    }

    public async Task RemoveSelectedRowAsync()
    {
        _showTableOptions = false;
        StateHasChanged();
        await JsRuntime.InvokeVoidAsync("removeRowOfTheSelection", _previewId);
    }

    public async Task OnInsertColumnToTheLeftAsync()
    {
        _showTableOptions = false;
        StateHasChanged();
        await JsRuntime.InvokeVoidAsync("insertColumnLeftOfSelection", _previewId);
    }

    public async Task OnInsertColumnToTheRightAsync()
    {
        _showTableOptions = false;
        StateHasChanged();
        await JsRuntime.InvokeVoidAsync("insertColumnRightOfSelection", _previewId);
    }

    public async Task RemoveSelectedColumnAsync()
    {
        _showTableOptions = false;
        StateHasChanged();
        await JsRuntime.InvokeVoidAsync("removeColumnOfTheSelection", _previewId);
    }
}