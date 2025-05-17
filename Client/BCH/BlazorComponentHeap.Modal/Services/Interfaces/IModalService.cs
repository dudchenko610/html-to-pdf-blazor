using BlazorComponentHeap.Modal.Models;

namespace BlazorComponentHeap.Modal.Services.Interfaces;

public interface IModalService
{
    IReadOnlyList<ModalModel> Modals { get; }

    event Action? OnUpdate;
    event Action<ModalModel>? OnOverlayClicked;

    void Open(ModalModel modalModel);
    void Close(ModalModel modalModel);
    void FireOverlayClicked(ModalModel modalModel);
}
