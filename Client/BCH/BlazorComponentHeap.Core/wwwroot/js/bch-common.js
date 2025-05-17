// window.addEventListener("resize", () => {
//     DotNet.invokeMethodAsync("BlazorComponentHeap.Core", 'OnBrowserResizeAsync').then(data => data);
// });
//
// window.addEventListener('scroll',function(event) {
//     const pathCoordinates = getPathCoordinates(event);
//
//     DotNet.invokeMethodAsync("BlazorComponentHeap.Core", 'OnBrowserGlobalScrollAsync', {
//         pathCoordinates: pathCoordinates
//     });
// }, true);

function bchGetBoundingClientRectById(id, param) {

    const element = document.getElementById(id);
    if (!element) return null;

    const rect = element.getBoundingClientRect();

    return {
        width: rect.width,
        height: rect.height,
        bottom: rect.bottom,
        left: rect.left,
        right: rect.right,
        top: rect.top,
        x: rect.x,
        y: rect.y,
        offsetTop: element.offsetTop,
        offsetLeft: element.offsetLeft,
        clientWidth: element.clientWidth,
        clientHeight: element.clientHeight,
        offsetWidth: element.offsetWidth,
        offsetHeight: element.offsetHeight
    };
}

function bchScrollElementTo(id, x, y, behavior) {
    const element = document.getElementById(id);

    if (!element) {
        return;
    }

    element.scrollTo({
        left: x,
        top: y,
        behavior: behavior // only 'auto' or 'smooth'
    });
}

const bchListeners = {};

function bchAddDocumentListener(key, eventName, dotnetReference, methodName,
                                preventDefault = false,
                                stopPropagation = false,
                                passive = true) {

    bchListeners[key + eventName] = function (event) {

        const preventDefaultAttr = event.target && event.target.hasAttribute(`${eventName}-prevent-default`);
        const stopPropagationAttr = event.target && event.target.hasAttribute(`${eventName}-stop-propagation`);

        if (preventDefault || preventDefaultAttr) event.preventDefault();
        if (stopPropagation || stopPropagationAttr) event.stopPropagation();

        let response = {};

        switch (eventName) {
            case "touchstart":
            case "touchmove":
            {
                const touches = Object.entries(event.touches).map((value, key) => {
                    const touch = value[1];
                    const pathCoordinates = getPathCoordinatesByPos(touch.pageX, touch.pageY);

                    return {
                        clientX: touch.clientX,
                        clientY: touch.clientY,
                        pageX: touch.pageX,
                        pageY: touch.pageY,
                        pathCoordinates: pathCoordinates
                    }
                });

                response = {
                    touches: touches
                };
                break;
            }
            case "touchend":
            {
                const touches = Object.entries(event.changedTouches).map((value, key) => {
                    const touch = value[1];
                    const pathCoordinates = getPathCoordinatesByPos(touch.pageX, touch.pageY);

                    return {
                        clientX: touch.clientX,
                        clientY: touch.clientY,
                        pageX: touch.pageX,
                        pageY: touch.pageY,
                        pathCoordinates: pathCoordinates
                    }
                });

                response = {
                    touches: touches
                };
                break;
            }
            case "mousewheel":
            {
                const x = event.clientX - event.target.offsetLeft;
                const y = event.clientY - event.target.offsetTop;
                const pathCoordinates = getPathCoordinates(event);

                response = {
                    x: x,
                    y: y,
                    deltaX: event.deltaX,
                    deltaY: event.deltaY,
                    pathCoordinates: pathCoordinates
                };

                break;
            }
            default:
                const pathCoordinates = getPathCoordinates(event);

                response = {
                    offsetX: event.offsetX,
                    offsetY: event.offsetY,
                    pageX: event.pageX,
                    pageY: event.pageY,
                    screenX: event.screenX,
                    screenY: event.screenY,
                    clientX: event.clientX,
                    clientY: event.clientY,
                    pathCoordinates: pathCoordinates
                };
                break;
        }

        dotnetReference.invokeMethodAsync(methodName, response);

        if (eventName === 'mousewheel') return false;
    };

    document.addEventListener(eventName, bchListeners[key + eventName], { passive: passive });
}

function bchRemoveDocumentListener(key, eventName) {
    if (bchListeners[key + eventName]) {
        document.removeEventListener(eventName, bchListeners[key + eventName]);
        delete bchListeners[key + eventName];
    }
}

function bchSetLeftTopToElement(id, x, y) {
    const element = document.getElementById(id);
    if (!element) return;

    element.style.left = x;
    element.style.top = y;
}

function bchFocusElement(elementId) {
    const element = document.getElementById(elementId);
    if (element === document.activeElement) return;
    element.focus();
}