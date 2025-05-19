//Preload the image
var drgimg = document.createElement("img");
drgimg.src = 'data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs=';

if (!Event.prototype.hasOwnProperty('path')) {
    Object.defineProperty(Event.prototype, 'path', {
        get() { return this.composedPath(); }
    });
}

if (!Event.prototype.hasOwnProperty('path')) {
    Object.defineProperty(Event.prototype, 'path', {
        get() { return this.composedPath(); }
    });
}

// for Safari and Firefox
if (!("path" in MouseEvent.prototype)) {
    Object.defineProperty(MouseEvent.prototype, "path", {
        get: function () {

            var path = [];
            var currentElem = this.target;
            while (currentElem) {
                path.push(currentElem);
                currentElem = currentElem.parentElement;
            }
            if (path.indexOf(window) === -1 && path.indexOf(document) === -1)
                path.push(document);
            if (path.indexOf(window) === -1)
                path.push(window);
            return path;
        }
    });
}

function getPathCoordinates(event) {
    const pageX = event.pageX | 0;
    const pageY = event.pageY | 0;

    const pathCoordinates = event.composedPath().map(element => {
        if (element.getBoundingClientRect) {
            const viewportOffset = element.getBoundingClientRect();

            return {
                x: pageX - viewportOffset.left,
                y: pageY - viewportOffset.top,
                clientWidth: element.clientWidth,
                clientHeight: element.clientHeight,
                scrollTop: element.scrollTop,
                classList: element.classList.value,
                id: element.id
            };
        }
    }).filter(x => x);

    return pathCoordinates;
}

function getPathCoordinatesByPos(x, y) {
    let element = document.elementFromPoint(x, y);
    const path = [];

    while (element && element !== document.body) {
        path.push(element);
        element = element.parentElement;
    }

    const pathCoordinates = getPathCoordinates({
        pageX: x,
        pageY: y,
        path: path
    });

    return pathCoordinates;
}

const bchGhostElement = document.createElement('div');
bchGhostElement.style.opacity = 0;
var bchGhostElementAppended = false;

Blazor.registerCustomEventType('extscroll', {
    browserEventName: 'scroll',
    createEventArgs: event => {

        return {
            clientHeight: event.target.clientHeight,
            scrollHeight: event.target.scrollHeight,
            scrollTop: event.target.scrollTop,
            clientWidth: event.target.clientWidth
        };
    }
});

Blazor.registerCustomEventType('extdragstart', {
    browserEventName: 'dragstart',
    createEventArgs: event => {

        event.dataTransfer.effectAllowed = "copyMove";

        if (!bchGhostElementAppended) {
            document.body.appendChild(bchGhostElement);
            bchGhostElementAppended = true;
        }

        var isSafari = /constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || (typeof safari !== 'undefined' && safari.pushNotification));

        if (!isSafari) {
            event.dataTransfer.setDragImage(bchGhostElement, 0, 0);
        } else {
            event.dataTransfer.setDragImage(drgimg, 0, 0);
        }

        setTimeout(function () {
            event.target.setAttribute('dragging', '');
        }, 0);

        const pathCoordinates = getPathCoordinates(event);

        return {
            offsetX: event.offsetX,
            offsetY: event.offsetY,
            pageX: event.pageX,
            pageY: event.pageY,
            screenX: event.screenX,
            screenY: event.screenY,

            pathCoordinates: pathCoordinates
        };
    }
});

Blazor.registerCustomEventType('extdragend', {
    browserEventName: 'dragend',
    createEventArgs: event => {
        event.target.removeAttribute('dragging');
        return event;
    }
});

Blazor.registerCustomEventType('extmousewheel', {
    browserEventName: 'mousewheel',
    createEventArgs: event => {

        // event.preventDefault();
        // event.stopPropagation();
        // event.returnValue = false;

        const x = event.clientX - event.target.offsetLeft;
        const y = event.clientY - event.target.offsetTop;
        const pathCoordinates = getPathCoordinates(event);

        return {
            x: x,
            y: y,
            deltaX: event.deltaX,
            deltaY: event.deltaY,
            pathCoordinates: pathCoordinates
        };
    }
});

// Blazor.registerCustomEventType('mouseleave', {
//     browserEventName: 'mouseleave',
//     createEventArgs: event => {
//
//         const pathCoordinates = getPathCoordinates(event);
//
//         return {
//             offsetX: event.offsetX,
//             offsetY: event.offsetY,
//             pageX: event.pageX,
//             pageY: event.pageY,
//             screenX: event.screenX,
//             screenY: event.screenY,
//
//             pathCoordinates: pathCoordinates
//         };
//     }
// });

Blazor.registerCustomEventType('extmousedown', {
    browserEventName: 'mousedown',
    createEventArgs: event => {

        const x = event.clientX - event.target.offsetLeft;
        const y = event.clientY - event.target.offsetTop;
        const pathCoordinates = getPathCoordinates(event);

        return {
            x: x,
            y: y,
            deltaX: event.deltaX,
            deltaY: event.deltaY,
            pageX: event.pageX,
            pageY: event.pageY,
            clientWidth: event.target.clientWidth,
            clientHeight: event.target.clientHeight,
            pathCoordinates: pathCoordinates
        };
    }
});

Blazor.registerCustomEventType('exttouchstart', {
    browserEventName: 'touchstart',
    createEventArgs: event => {

        const touches = Object.entries(event.touches).map((value, key) => {
            const touch = value[1];

            return {
                x: touch.clientX - event.target.offsetLeft,
                y: touch.clientY - event.target.offsetTop,
                clientWidth: event.target.clientWidth,
                clientHeight: event.target.clientHeight,
                clientX: touch.clientX,
                clientY: touch.clientY,
                pageX: touch.pageX,
                pageY: touch.pageY
            }
        });

        let element = document.elementFromPoint(touches[0].pageX, touches[0].pageY);
        const path = [];

        while (element && element !== document.body) {
            path.push(element);
            element = element.parentElement;
        }

        const pathCoordinates = getPathCoordinates({
            pageX: touches[0].pageX,
            pageY: touches[0].pageY,
            path: path
        });

        return {
            touches: touches,
            pathCoordinates: pathCoordinates
        };
    }
});