using Microsoft.AspNetCore.Components;
using BlazorComponentHeap.Modal.Services.Interfaces;

namespace BlazorComponentHeap.Modal.Root;

public partial class BCHRootModal : IDisposable
{
    [Inject] public required IModalService ModalService { get; set; }

    protected override void OnInitialized()
    {
        ModalService.OnUpdate += StateHasChanged;
    }

    public void Dispose()
    {
        ModalService.OnUpdate -= StateHasChanged;
    }
}
