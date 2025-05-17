using BlazorComponentHeap.Modal.Models;
using BlazorComponentHeap.Modal.Services.Interfaces;

namespace BlazorComponentHeap.Modal.Services;

public class ModalService : IModalService
{
    public IReadOnlyList<ModalModel> Modals { get; }
    public event Action? OnUpdate;
    public event Action<ModalModel>? OnOverlayClicked;

    private readonly List<ModalModel> _modals = new();

    public ModalService()
    {
        Modals = _modals;
    }

    public void Open(ModalModel modalModel)
    {
        if (_modals.Contains(modalModel)) return;
        
        _modals.Add(modalModel);
        OnUpdate?.Invoke();
    }

    public void Close(ModalModel modalModel)
    {
        if (!_modals.Contains(modalModel)) return;
        
        _modals.Remove(modalModel);
        OnUpdate?.Invoke();
    }

    public void FireOverlayClicked(ModalModel modalModel)
    {
        OnOverlayClicked?.Invoke(modalModel);
    }
}
