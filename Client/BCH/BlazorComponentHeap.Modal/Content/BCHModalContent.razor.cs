using BlazorComponentHeap.Modal.Models;
using BlazorComponentHeap.Modal.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.Modal.Content;

public partial class BCHModalContent : IDisposable
{
    [Inject] public required IModalService ModalService { get; set; }
    [Parameter] public required ModalModel ModalModel { get; set; }

    protected override void OnInitialized()
    {
        ModalModel.OnUpdate += StateHasChanged;
    }
    
    public void Dispose()
    {
        ModalModel.OnUpdate -= StateHasChanged;
    }
    
    private void OnOverlayClicked()
    {
        ModalService.FireOverlayClicked(ModalModel);
    }
    
    private bool IsCenter()
    {
        return string.IsNullOrWhiteSpace(ModalModel.X) || string.IsNullOrWhiteSpace(ModalModel.Y);
    }
}