using BlazorComponentHeap.Core.Models.Markup;

namespace BlazorComponentHeap.Core.Services;

public interface IJsUtilsService
{
    Task<BoundingClientRect?> GetBoundingClientRectAsync(string id);
    Task ScrollToAsync(string id, string x, string y, string behavior = "smooth"); // auto

    ValueTask FocusAsync(string elementId);

    Task AddDocumentListenerAsync<T>(string eventName, string key, Func<T, Task> callback,
        bool preventDefault = false,
        bool stopPropagation = false,
        bool passive = true);
    Task RemoveDocumentListenerAsync<T>(string eventName, string key);
}