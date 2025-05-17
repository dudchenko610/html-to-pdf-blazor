using Microsoft.JSInterop;
using BlazorComponentHeap.Core.Models.Markup;

namespace BlazorComponentHeap.Core.Services;

public class JsUtilsService : IJsUtilsService
{
    private readonly IJSRuntime _jsRuntime;
    // private readonly IJSInProcessRuntime _jsProcessRuntime;

    private class DocumentListenerHolder<T>
    {
        public Dictionary<string, Func<T, Task>> Functions { get; set; } = new();
        public DotNetObjectReference<DocumentListenerHolder<T>> DotNetRef { get; }
        
        public DocumentListenerHolder()
        {
            DotNetRef = DotNetObjectReference.Create(this);
        }

        [JSInvokable]
        public void Callback(T e)
        {
            foreach (var keyValue in Functions)
            {
                keyValue.Value.Invoke(e);
            }
        }
    }
    
    private readonly Dictionary<string, object> _docListeners = new();
    
    public JsUtilsService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        // _jsProcessRuntime = (IJSInProcessRuntime) jsRuntime;
    }

    public async Task<BoundingClientRect?> GetBoundingClientRectAsync(string id)
    {
        return await _jsRuntime.InvokeAsync<BoundingClientRect>("bchGetBoundingClientRectById", id);
    }

    // public BoundingClientRect GetBoundingClientRect(string id)
    // {
    //     return _jsProcessRuntime.Invoke<BoundingClientRect>("bchGetBoundingClientRectById", id);
    // }
    //
    // public void ScrollTo(string id, string x, string y, string behavior = "smooth")
    // {
    //     _jsProcessRuntime.InvokeVoid("bchScrollElementTo", id, x, y, behavior);
    // }

    public async Task ScrollToAsync(string id, string x, string y, string behavior = "smooth")
    {
        await _jsRuntime.InvokeVoidAsync("bchScrollElementTo", id, x, y, behavior);
    }

    public ValueTask FocusAsync(string elementId)
    {
        return _jsRuntime.InvokeVoidAsync("bchFocusElement", elementId);
    }

    public async Task AddDocumentListenerAsync<T>(string eventName, string key, Func<T, Task> callback, 
        bool preventDefault = false,
        bool stopPropagation = false,
        bool passive = true)
    {
        var eventKey = $"{eventName}{typeof(T).FullName}";
        var subscriberKey = $"{eventName}{key}";
        
        // Console.WriteLine($"AddDocumentListenerAsync {key} {eventKey}");
        
        // Console.WriteLine($"AddDocumentListenerAsync {eventName} typeof(T) {} {preventDefault} {stopPropagation} {passive}");
        // Console.WriteLine($"_docListeners Contains {_docListeners.ContainsKey(eventName)} ");
        // Console.WriteLine($"_docListeners TryGetValue {_docListeners.TryGetValue(eventName, out var docL)} isNull {docL == null}");
        //
        if (!(_docListeners.TryGetValue(eventKey, out var docListenerObj) 
            && docListenerObj is DocumentListenerHolder<T> docListener))
        {
            docListener = new DocumentListenerHolder<T>();
            docListener.Functions.Add(subscriberKey, callback);
            _docListeners.Add(eventKey, docListener);

            var jsListenerKey = eventKey;
            await _jsRuntime.InvokeVoidAsync("bchAddDocumentListener", jsListenerKey, eventName, 
                docListener.DotNetRef, "Callback", preventDefault, stopPropagation, passive);
            
            return;
        }
        
        docListener.Functions.TryAdd(subscriberKey, callback);
    }

    public async Task RemoveDocumentListenerAsync<T>(string eventName, string key)
    {
        // Console.WriteLine($"RemoveDocumentListenerAsync {eventName}");
        var eventKey = $"{eventName}{typeof(T).FullName}";
        var subscriberKey = $"{eventName}{key}";
        
        // Console.WriteLine($"RemoveDocumentListenerAsync {key} {eventKey}");
        
        if (_docListeners.TryGetValue(eventKey, out var docListenerObj) 
              && docListenerObj is DocumentListenerHolder<T> docListener)
        {
            docListener.Functions.Remove(subscriberKey);

            if (docListener.Functions.Count == 0)
            {
                _docListeners.Remove(eventKey);
                var jsListenerKey = eventKey;

                // https://stackoverflow.com/questions/72488563/blazor-server-side-application-throwing-system-invalidoperationexception-javas
                try
                {
                    await _jsRuntime.InvokeVoidAsync("bchRemoveDocumentListener", jsListenerKey, eventName);
                }
                catch (JSDisconnectedException ex)
                {
                    // Ignore
                }
            }
        }
    }
}