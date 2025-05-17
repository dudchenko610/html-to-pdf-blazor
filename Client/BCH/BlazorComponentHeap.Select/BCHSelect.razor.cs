using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using BlazorComponentHeap.Core.Models;
using BlazorComponentHeap.Core.Models.Events;
using BlazorComponentHeap.Core.Services;

namespace BlazorComponentHeap.Select;

public partial class BCHSelect<TItem> : ComponentBase, IAsyncDisposable
{
    [Inject] public required IJsUtilsService JsUtilsService { get; set; }
    [Inject] public required IJSRuntime JsRuntime { get; set; }

    private class Element
    {
        public string Name { get; set; } = string.Empty;
        public TItem Item { get; set; } = default!;
    }

    private class Group
    {
        public bool Expanded { get; set; } = true;
        public string Name { get; set; } = string.Empty;
        public object Key { get; set; } = null!;
        public List<Element> Elements { get; set; } = new();
    }
    
    [Parameter] public EventCallback OnFocusOut { get; set; }
    [Parameter] public bool Filtering { get; set; } = false;
    [Parameter] public bool MultipleValues { get; set; } = false;
    [Parameter] public bool UpperSide { get; set; } = false;
    [Parameter] public bool ScrollToSelected { get; set; } = true;
    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public string ContentId { get; set; } = $"_id_{Guid.NewGuid()}";
    [Parameter] public string NoItemsText { get; set; } = "No items found";

    [Parameter] public int ItemHeight { get; set; } = 40;
    [Parameter] public int ContentHeight { get; set; } = 200;
    [Parameter] public int Height { get; set; } = 56;
    [Parameter] public int Width { get; set; } = 200;
    [Parameter] public TItem? DefaultValue { get; set; } = default(TItem);
    [Parameter] public string DefaultText { get; set; } = "Please Select";
    [Parameter] public IEnumerable<TItem> Options { get; set; } = new List<TItem>();
    [Parameter] public Func<TItem, string>? OptionNamePredicate { get; set; } = x => $"{x}";
    [Parameter] public Func<TItem, string>? FilterByPredicate { get; set; } = null;
    [Parameter] public Func<TItem, object>? GroupPredicate { get; set; } = null;
    [Parameter] public Func<TItem, string>? GroupNamePredicate { get; set; } = null;
    [Parameter] public Func<TItem, string>? CssItemPredicate { get; set; } = x => string.Empty;
    [Parameter] public EventCallback<KeyboardEventArgs> OnFilterKeyDown { get; set; }
    [Parameter] public EventCallback<TItem?> SelectedChanged { get; set; }
    [Parameter] public TItem? Selected
    {
        get => _selectedValue;
        set
        {
            if ((_selectedValue != null && _selectedValue.Equals(value)) || (_selectedValue == null && value == null) ) 
                return;
            _selectedValue = value;

            SelectedChanged.InvokeAsync(value);
        }
    }

    [Parameter] public EventCallback<string> FilterChanged { get; set; }
    [Parameter] public string Filter
    {
        get => _typedFilterValue;
        set
        {
            if (_typedFilterValue == value) return;
            _typedFilterValue = value;
            OnFilterType();
            FilterChanged.InvokeAsync(value);
        }
    }

    [Parameter] public EventCallback<bool> IsOpenedChanged { get; set; }
    [Parameter] public bool IsOpened
    {
        get => _isOpened; set {}
    }

    [Parameter] public IList<TItem> SelectedItems { get; set; } = new List<TItem>();
    [Parameter] public EventCallback<TItem> OnSelectItem { get; set; }
    [Parameter] public EventCallback<TItem> OnDeselectItem { get; set; }
    [Parameter] public RenderFragment<TItem> OptionTemplate { get; set; } = null!;
    [Parameter] public RenderFragment<object> GroupTemplate { get; set; } = null!;

    private readonly string _containerId = $"_id_{Guid.NewGuid()}";
    private readonly string _inputId = $"_id_{Guid.NewGuid()}";
    private string ScrollerId => $"{ContentId}_scroller";
    private readonly string _subscriptionKey = $"_key_{Guid.NewGuid()}";
    private readonly string _contentClass = $"_class_{Guid.NewGuid()}";

    private TItem? _selectedValue = default(TItem);
    private bool _isOpened = false;
    private ElementReference _inputRef;
    private string _typedFilterValue = string.Empty;
    private string _placeholder = "";

    private List<Group> _groups = new();

    private int _prevCount = -1;
    private bool _scrolled = false;
    private Vec2 _containerPos = new ();
    private float _contentWidth = 0;
    private NumberFormatInfo _nF = new () { NumberDecimalSeparator = "." };

    private bool _isHybridApp = false;

    protected override void OnInitialized()
    {
        _isHybridApp = JsRuntime.GetType().Name.Contains("WebView");
        
        OptionNamePredicate ??= x => $"{x}";
        
        FilterByPredicate ??= OptionNamePredicate;

        if (!MultipleValues && DefaultValue != null && Options.Contains(DefaultValue))
            Selected = DefaultValue;

        //var isClass = default(TItem) == null; // Only true if T is a reference type or nullable value type
        _placeholder = (Selected == null) ? DefaultText : OptionNamePredicate.Invoke(Selected);
        
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        await JsUtilsService.RemoveDocumentListenerAsync<ExtMouseEventArgs>("mousedown", _subscriptionKey);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsUtilsService.AddDocumentListenerAsync<ExtMouseEventArgs>("mousedown", _subscriptionKey,
                OnDocumentMouseDownAsync);
        }
        
        if (IsOpened)
        {
            //if (Filtering) await _inputRef.FocusAsync();

            if (Filtering) await JsUtilsService.FocusAsync(_inputId);

            if (ScrollToSelected && Selected != null! && !MultipleValues && !_scrolled)
            {
                _scrolled = true;

                var index = GroupPredicate is not null ? -1 : -2;

                for (var i = 0; i < _groups.Count; i++)
                {
                    foreach (var element in _groups[i].Elements)
                    {
                        if (element.Item != null && element.Item.Equals(Selected))
                        {
                            i = _groups.Count;
                            break;
                        }

                        index++;
                    }

                    index++;
                }
                
                var offset = index * ItemHeight;
                await JsUtilsService.ScrollToAsync(ScrollerId, "0", $"{offset}", "auto");
            }
        }

        if (_prevCount != Options.Count())
        {
            FilterData();
            StateHasChanged();
        }
    }

    private async Task OnGlobalScrollAsync(ScrollEventArgs e)
    {
        var scrollContainer = e.PathCoordinates.FirstOrDefault();
        if (scrollContainer?.Id == ScrollerId) return;
        
        await SetOpenedAsync(false);
        
        _scrolled = false;
        Filter = string.Empty;
        await OnFocusOut.InvokeAsync();
        StateHasChanged();
    }

    private async Task OnOptionClickedAsync(TItem option)
    {
        if (!MultipleValues)
        {
            await SetOpenedAsync(false);
            
            Selected = option;
            Filter = string.Empty;

            if (Selected != null!)
            {
                _placeholder = OptionNamePredicate.Invoke(Selected);
            }
        }
        else
        {
            if (SelectedItems.Contains(option))
            {
                SelectedItems.Remove(option);
                await OnDeselectItem.InvokeAsync(option);
            }
            else
            {
                SelectedItems.Add(option);
                await OnSelectItem.InvokeAsync(option);
            }
        }

        _scrolled = false;

        StateHasChanged();
    }

    private bool IsSelected(TItem option)
    {
        if (MultipleValues)
        {
            return SelectedItems.Contains(option);
        }

        return _selectedValue != null && _selectedValue.Equals(option);
    }

    private async Task OnSelectClickedAsync()
    {
        if (!IsOpened)
        {
            var containerRect = await JsUtilsService.GetBoundingClientRectAsync(_containerId);
            _containerPos.Set(containerRect.X, containerRect.Y);
            _contentWidth = (float) containerRect.Width;
        }
        
        await SetOpenedAsync(!_isOpened);
        
        _scrolled = false;
        Filter = string.Empty;

        FilterData();
        StateHasChanged();
    }

    private void OnFilterType()
    {
        FilterData(_typedFilterValue);

        StateHasChanged();
    }

    private void FilterData(string filter = "")
    {
        _groups = new();

        var groupPredicate = GroupPredicate ?? (x => string.Empty);
        var groupNamePredicate = GroupNamePredicate ?? (x => $"{groupPredicate.Invoke(x)}");

        var groups = Options.GroupBy(groupPredicate);
        _prevCount = Options.Count();
        
        foreach (var group in groups)
        {
            if (!group.Any()) continue;

            var first = group.First();

            var gr = new Group
            {
                Key = groupPredicate.Invoke(first),
                Name = groupNamePredicate.Invoke(first)
            };

            foreach (var item in group)
            {
                var name = OptionNamePredicate == null ? string.Empty : OptionNamePredicate.Invoke(item);
                var filterElementValue = FilterByPredicate?.Invoke(item);

                if (!string.IsNullOrEmpty(filterElementValue) && filterElementValue.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    gr.Elements.Add(new Element
                    {
                        Item = item,
                        Name = name
                    });
                }
            }

            if (gr.Elements.Count == 0) continue;

            _groups.Add(gr);
        }
    }

    private void OnGroupClicked(Group group)
    {
        group.Expanded = !group.Expanded;
        StateHasChanged();
    }

    private async Task OnInputKeyDownAsync(KeyboardEventArgs e)
    {
        if (e.Code is "Enter" or "NumpadEnter")
        {
            _typedFilterValue = string.Empty;
            StateHasChanged();
        }
        else
        {
            if (!IsOpened)
            {
                await SetOpenedAsync(true);
                
                _scrolled = false;

                FilterData();
                StateHasChanged();
            }
        }

        await OnFilterKeyDown.InvokeAsync(e);
    }

    private async Task OnDocumentMouseDownAsync(ExtMouseEventArgs e)
    {
        var container = e.PathCoordinates
            .FirstOrDefault(x => x.Id == _containerId || x.Id == ContentId);

        if (container == null) // outside of select
        {
            await SetOpenedAsync(false);
            
            _scrolled = false;
            Filter = string.Empty;
            await OnFocusOut.InvokeAsync();
            StateHasChanged();
        }
    }

    private async Task SetOpenedAsync(bool isOpened)
    {
        if (_isOpened == isOpened) return;
        
        _isOpened = isOpened;
        await IsOpenedChanged.InvokeAsync(isOpened);
    }

    private int GetContentHeight()
    {
        var renderGroupCount = GroupPredicate is null ? 0 : 1;
        return _groups.Sum(group => group.Elements.Count + renderGroupCount) * ItemHeight;
    }
}
