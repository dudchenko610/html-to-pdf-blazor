using BlazorComponentHeap.Core.Extensions;
using BlazorComponentHeap.HtmlEditor.Attributes;
using BlazorComponentHeap.HtmlEditor.Events;
using BlazorComponentHeap.HtmlEditor.Models;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.HtmlEditor;

public partial class HtmlEditor
{
    [JSInvokable]
    public async Task OnKeyPressInCodeMirrorAsync()
    {
        await OnChange.InvokeAsync();
    }

    [JSInvokable]
    public async Task OnIFrameKeyDownAsync(UndoRedoStateModel undoRedoState)
    {
        if (undoRedoState == null!) // immediate key down
        {
            if (_showImageRect || _showFloatingToolbar || _showImageOptions)
            {
                _showImageRect = false;
                _showFloatingToolbar = false;
                _showImageOptions = false;
                _showTableOptions = false;

                StateHasChanged();
            }

            await OnChange.InvokeAsync();
            return;
        }

        var commandBitMask = _commandButtonsBitMask;

        if (undoRedoState.Undo)
            commandBitMask |= 1 << ControlButton.Undo.GetValue<int, DocumentCommandAttribute>(x => x.BitShift);
        else commandBitMask &= ~(1 << ControlButton.Undo.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));

        if (undoRedoState.Redo)
            commandBitMask |= 1 << ControlButton.Redo.GetValue<int, DocumentCommandAttribute>(x => x.BitShift);
        else commandBitMask &= ~(1 << ControlButton.Redo.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));

        if (_commandButtonsBitMask != commandBitMask || _showFloatingToolbar || _showImageOptions)
        {
            _commandButtonsBitMask = commandBitMask;
            _showFloatingToolbar = false;
            _showImageOptions = false;
            _showTableOptions = false;

            StateHasChanged();
        }
    }

    [JSInvokable]
    public Task OnIFrameContextMenuAsync(IFrameMouseDownEvent evt)
    {
        if (evt.CoordsHolder?.TagName.ToLower() == "img")
        {
            _imageOptionsPos.Set(evt.PageX, evt.PageY);
            _showImageOptions = true;

            StateHasChanged();

            return Task.CompletedTask;
        }

        if (evt.HasTableTag)
        {
            _tableOptionsPos.Set(evt.PageX, evt.PageY);
            _showTableOptions = true;

            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnIFrameMouseDownAsync(IFrameMouseDownEvent evt)
    {
        _showFloatingToolbar = false;
        _showImageOptions = false;
        _showTableOptions = false;

        if (evt.CoordsHolder?.TagName.ToLower() == "img" && evt.Rect is not null)
        {
            _showImageRect = true;
            _imageRectPos.Set(evt.Rect.X, evt.Rect.Y);
            _imageRectSize.Set(evt.Rect.Width * ScaleFactor, evt.Rect.Height * ScaleFactor);
            _imageRectSizeBeforeDragging.Set(_imageRectSize);
            _imageRatio = evt.Rect.Width / evt.Rect.Height;
            OnSelectImage.InvokeAsync();
        }
        else
        {
            _showImageRect = false;
        }

        StateHasChanged();

        return Task.CompletedTask;
    }

    [JSInvokable]
    public void OnIFrameScroll()
    {
        if (_showImageRect && !_rectHandleDragged || _showFloatingToolbar || _showImageOptions)
        {
            _showImageRect = false;
            _showFloatingToolbar = false;
            _showImageOptions = false;
            _showTableOptions = false;

            StateHasChanged();
        }
    }

    [JSInvokable]
    public void OnIFrameMouseUp(IFrameMouseUpEvent e)
    {
        if (e.StartIndex != e.EndIndex)
        {
            _floatingToolbarPos.Set(e.PageX, e.PageY - 60);
            _showFloatingToolbar = true;

            StateHasChanged();
        }
    }

    [JSInvokable]
    public Task OnSelectionChangedNotEditableAsync(SelectionEventArgs e)
    {
        return OnSelectionChange.InvokeAsync(e);
    }

    [JSInvokable]
    public Task OnSelectionChangedAsync(SelectionEventArgs e)
    {
        var commandBitMask = _commandButtonsBitMask;

        commandBitMask &= ~(1 << ControlButton.Bold.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));
        commandBitMask &= ~(1 << ControlButton.Italic.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));
        commandBitMask &= ~(1 << ControlButton.Underline.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));

        commandBitMask &= ~(1 << ControlButton.JustifyLeft.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));
        commandBitMask &= ~(1 << ControlButton.JustifyRight.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));
        commandBitMask &= ~(1 << ControlButton.JustifyCenter.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));
        commandBitMask &= ~(1 << ControlButton.FormatBlock.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));

        commandBitMask &= ~(1 << ControlButton.OrderedList.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));
        commandBitMask &= ~(1 << ControlButton.UnorderedList.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));
        commandBitMask &= ~(1 << ControlButton.RemoveLink.GetValue<int, DocumentCommandAttribute>(x => x.BitShift));

        foreach (var coordHolder in e.PathCoordinates)
        {
            var tagName = coordHolder.TagName.ToLower();

            commandBitMask = tagName switch
            {
                "b" => commandBitMask |=
                    1 << ControlButton.Bold.GetValue<int, DocumentCommandAttribute>(x => x.BitShift),
                "strong" => commandBitMask |=
                    1 << ControlButton.Bold.GetValue<int, DocumentCommandAttribute>(x => x.BitShift),

                "i" => commandBitMask |=
                    1 << ControlButton.Italic.GetValue<int, DocumentCommandAttribute>(x => x.BitShift),
                "em" => commandBitMask |=
                    1 << ControlButton.Italic.GetValue<int, DocumentCommandAttribute>(x => x.BitShift),

                "u" => commandBitMask |=
                    1 << ControlButton.Underline.GetValue<int, DocumentCommandAttribute>(x => x.BitShift),
                "ol" => commandBitMask |=
                    1 << ControlButton.OrderedList.GetValue<int, DocumentCommandAttribute>(x => x.BitShift),
                "ul" => commandBitMask |=
                    1 << ControlButton.UnorderedList.GetValue<int, DocumentCommandAttribute>(x => x.BitShift),
                "a" => commandBitMask |=
                    1 << ControlButton.RemoveLink.GetValue<int, DocumentCommandAttribute>(x => x.BitShift),
                _ => commandBitMask
            };
        }

        var prevColorText = _colorText;
        var prevParagraph = _paragraph;

        if (e.PathCoordinates.Count != 0)
        {
            var bottomElement = e.PathCoordinates[^1].StyleMap;

            if (bottomElement.TryGetValue("textAlign", out var cssValue))
            {
                var controlVal = cssValue switch
                {
                    "left" => ControlButton.JustifyLeft,
                    "right" => ControlButton.JustifyRight,
                    "center" => ControlButton.JustifyCenter,
                    _ => ControlButton.FormatBlock
                };

                commandBitMask |= 1 << controlVal.GetValue<int, DocumentCommandAttribute>(x => x.BitShift);
            }

            var topElement = e.PathCoordinates[0];
            _colorText = topElement.StyleMap.TryGetValue("color", out var color) ? color : string.Empty;

            _paragraph = $"<{topElement.TagName.ToLower()}>";
        }
        else
        {
            _colorText = "#000000";
        }

        if (_commandButtonsBitMask != commandBitMask || prevColorText != _colorText || prevParagraph != _paragraph)
        {
            _commandButtonsBitMask = commandBitMask;
            StateHasChanged();
        }

        OnSelectionChange.InvokeAsync(e);

        return Task.CompletedTask;
    }
}