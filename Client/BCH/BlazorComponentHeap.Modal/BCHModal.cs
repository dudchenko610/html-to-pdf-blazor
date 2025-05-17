using BlazorComponentHeap.Modal.Models;
using BlazorComponentHeap.Modal.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorComponentHeap.Modal;

public class BCHModal : ComponentBase, IDisposable
{
    [Inject] public required IModalService ModalService { get; set; }
    [Inject] public required IJSRuntime JsRuntime { get; set; }
    
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback OnOverlayClicked { get; set; }
    
    [Parameter] public string Id { get; set; } = "";
    [Parameter] public string OverlayId { get; set; } = "";
    [Parameter] public string Width { get; set; } = "200px;";
    [Parameter] public string Height { get; set; } = "200px;";
    [Parameter] public string X { get; set; } = "";
    [Parameter] public string Y { get; set; } = "";
    [Parameter] public int ZIndex { get; set; } = 999999;
    [Parameter] public string CssClass { get; set; } = string.Empty;
    [Parameter] public string CssStyles { get; set; } = string.Empty;
    [Parameter] public bool CloseOnOverlayClicked { get; set; }
    [Parameter] public bool ShowOverlay { get; set; }

    private ModalModel _modalModel = new();

    private bool _show { get; set; }
    
    [Parameter] public EventCallback<bool> ShowChanged { get; set; }
    [Parameter] public bool Show
    {
        get => _show;
        set
        {
            if (_show == value) return;
            _show = value;

            ResetValuesFromParameters();

            if (_show) ModalService.Open(_modalModel);
            else ModalService.Close(_modalModel);
            
            ShowChanged.InvokeAsync(value);
        }
    }
    
    private string _prevX = string.Empty;
    private string _prevY = string.Empty;
    private int _prevZIndex = 999999;

    protected override void OnInitialized()
    {
        ModalService.OnOverlayClicked += OnOverlayClickedFired;
        
        ResetValuesFromParameters();
        
        if (Show) ModalService.Open(_modalModel);
        else ModalService.Close(_modalModel);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        _modalModel.OnUpdate?.Invoke();
    }

    protected override void OnParametersSet()
    {
        if (_prevX != X || _prevY != Y || _prevZIndex != ZIndex) Update();
    }
    
    public void Dispose()
    {
        ModalService.Close(_modalModel);
        ModalService.OnOverlayClicked -= OnOverlayClickedFired;
    }

    private void OnOverlayClickedFired(ModalModel modalModel)
    {
        OnOverlayClicked.InvokeAsync();

        if (!CloseOnOverlayClicked) return;
        
        Show = false;
        StateHasChanged();
    }

    public void Update()
    {
        ResetValuesFromParameters();
        _modalModel.OnUpdate?.Invoke();
    }
    
    private void ResetValuesFromParameters()
    {
        if (!string.IsNullOrEmpty(Id)) _modalModel.Id = Id;
        if (!string.IsNullOrEmpty(OverlayId)) _modalModel.OverlayId = OverlayId;
        
        _modalModel.Fragment = ChildContent;
        _modalModel.Height = Height;
        _modalModel.Width = Width;
        _modalModel.X = X;
        _modalModel.Y = Y;
        _modalModel.CssClass = CssClass;
        _modalModel.CssStyles = CssStyles;
        _modalModel.Overlay = ShowOverlay;
        _modalModel.ZIndex = ZIndex;

        _prevX = _modalModel.X;
        _prevY = _modalModel.Y;
    }
    
    public async Task SetPositionAsync(string x, string y)
    {
        await JsRuntime.InvokeVoidAsync("bchSetLeftTopToElement", _modalModel.Id, x, y);
    }
}