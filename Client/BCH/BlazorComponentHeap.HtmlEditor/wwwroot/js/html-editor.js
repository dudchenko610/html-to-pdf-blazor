/*IFRAME*/

const iframeGlobalListeners = {};

async function addIFrameDocumentListener(iframeId, key, eventName, dotnetReference, methodName) {
    const iframeElement = document.getElementById(iframeId);
    const iframeDocument = iframeElement.contentDocument || iframeElement.contentWindow.document;
    const iframeWindow = iframeElement.contentWindow;

    let keyDownCounter = 0;
    let lastCounterValue = 0;
    let iframeTextOnFirstInput = '';

    // Options for the observer (which mutations to observe)
    const config = { attributes: true, childList: true, subtree: true };

    // Create an observer instance linked to the callback function
    let observer = null;

    iframeGlobalListeners[key + eventName] = async function (event) {
        var response = {}
        let returnData = true;

        switch (eventName) {
            case 'keydown': {

                if (event.ctrlKey && (event.keyCode == 89 || event.keyCode == 90)) event.preventDefault();

                if (lastCounterValue == keyDownCounter) {
                    iframeTextOnFirstInput = iframeDocument.documentElement.outerHTML;
                }

                const counter = keyDownCounter++;

                dotnetReference.invokeMethodAsync(methodName, null);

                await sleep(400);

                if (counter + 1 == keyDownCounter) {
                    onEditorIFrameKeyDown(iframeId, iframeTextOnFirstInput);
                    lastCounterValue = keyDownCounter;
                }

                const history = editorsHistory[iframeId];
                response = getUndoRedoButtonsState(history);

                break;
            }
            case 'selectionchange': {
                const iframeBody = iframeDocument.body;
                let selectedElement = getSelectedNode(iframeDocument, iframeWindow);

                const textContent = getSelectedText(iframeDocument, iframeWindow);

                if (!iframeBody || !iframeBody.contains(selectedElement)) return;

                const pathCoordinates = [];

                while (selectedElement &&
                selectedElement.tagName?.toLowerCase() != 'body' &&
                selectedElement.tagName?.toLowerCase() != 'html') {

                    const styleMap = {};

                    for (var key in selectedElement.style) {

                        if (/^\d+$/.test(key)) continue;

                        const styleValue = selectedElement.style[key];

                        if (styleValue && typeof styleValue !== 'function') {
                            styleMap[key] = styleValue;
                        }
                    }

                    const attrColor = selectedElement.getAttribute('color')

                    if (attrColor) styleMap['color'] = attrColor;

                    delete styleMap['length'];
                    delete styleMap['cssText'];

                    pathCoordinates.push({
                        tagName: selectedElement.tagName,
                        styleMap: styleMap
                    })

                    selectedElement = selectedElement.parentNode;
                }

                response = {
                    pathCoordinates: pathCoordinates,
                    textContent: textContent
                }

                break;
            }
            case 'contextmenu': {
                const pathCoordinates = getPathCoordinates(event);
                const iframeElement = document.getElementById(iframeId);
                const iframeAABB = iframeElement.getBoundingClientRect();

                if (pathCoordinates.length) {
                    const coordsHolder = pathCoordinates[0];
                    const composedPath = event.composedPath();
                    const veryBottomElement = composedPath[0];

                    console.log('contextmenu veryBottomElement = ', veryBottomElement);
                    const hasTableTag = composedPath.some(x => x.tagName.toLowerCase() === 'table');

                    if (veryBottomElement.tagName.toLowerCase() === 'img') {
                        event.preventDefault();
                    } else if (hasTableTag) {
                        event.preventDefault();
                    }

                    response = {
                        pageX: event.clientX + iframeAABB.left,
                        pageY: event.clientY + iframeAABB.top,
                        coordsHolder: coordsHolder,
                        rect: veryBottomElement.getBoundingClientRect(),
                        hasTableTag: hasTableTag
                    }
                }

                break;
            }
            case 'mousedown': {
                const pathCoordinates = getPathCoordinates(event);

                if (pathCoordinates.length) {
                    const coordsHolder = pathCoordinates[0];
                    const veryBottomElement = event.composedPath()[0];

                    if (selectionStates[iframeId] && selectionStates[iframeId].tagName.toLowerCase() === 'img' && observer) { // remove observer from old element
                        observer.disconnect();
                    }

                    selectionStates[iframeId] = veryBottomElement;

                    if (veryBottomElement.tagName.toLowerCase() === 'img') {

                        // select img

                        {
                            var sel = iframeWindow.getSelection();
                            var range = iframeDocument.createRange();
                            range.selectNodeContents(veryBottomElement);
                            sel.removeAllRanges();
                            sel.addRange(range);
                        }

                        observer = new MutationObserver((mutationList, observer) => {

                            const selected = selectionStates[iframeId];

                            if (selected && selected.tagName.toLowerCase() === 'img') {

                                dotnetReference.invokeMethodAsync('OnIFrameImgObservedAsync', {
                                    rect: selected.getBoundingClientRect()
                                });
                            }
                        });

                        observer.observe(veryBottomElement, config);
                    }

                    response = {
                        coordsHolder: coordsHolder,
                        rect: veryBottomElement.getBoundingClientRect()
                    }
                }

                break;
            }
            case 'dragover': {
                const selected = selectionStates[iframeId];
                returnData = selected && selected.tagName.toLowerCase() == 'img';

                selectionStates[iframeId] = null;
            }
            case 'scroll': {
                break;
            }

            case 'mouseup': {

                const iframeElement = document.getElementById(iframeId);
                const iframeAABB = iframeElement.getBoundingClientRect();

                const selectedRange = getSelectedRange(iframeDocument, iframeWindow);

                response = {
                    pageX: event.clientX + iframeAABB.left,
                    pageY: event.clientY + iframeAABB.top,
                    startIndex: selectedRange.startOffset,
                    endIndex: selectedRange.endOffset
                }

                break;
            }

            case 'paste': {
                console.log("hitting the paste case...");
                const iframeElement = document.getElementById(iframeId);
                const iframeDocument = iframeElement.contentDocument || iframeElement.contentWindow.document;

                event.preventDefault(); //Stop browser from pasting rich text (default behavior)

                const text = (event.clipboardData || window.clipboardData).getData('text/plain');

                //Check first if we can use the brwoser built-in method to insert text:
                if (iframeDocument.queryCommandSupported && iframeDocument.queryCommandSupported('insertText')) {
                    iframeDocument.execCommand("insertText", false, text);
                } else {
                    const selection = iFrameWindow.getSelection();
                    if (!selection.rangeCount) return;

                    const range = selection.getRangeAt(0);
                    range.deleteContents(); //If something selected we delet it to replace it with pasted text (like in Office365)
                    range.insertNode(iFrameDocument.createTextNode(text)); //insert plaintext manually if the browser default method is not working
                }
                returnData = false;
                break;
            }

        }

        if (returnData) dotnetReference.invokeMethodAsync(methodName, response);
    };

    iframeDocument.addEventListener(eventName, iframeGlobalListeners[key + eventName]);
}

function removeIFrameDocumentListener(iframeId, key, eventName) {
    const iframeElement = document.getElementById(iframeId);
    const iframeDocument = iframeElement.contentDocument || iframeElement.contentWindow.document;

    if (iframeGlobalListeners[key + eventName]) {
        iframeDocument.removeEventListener(eventName, iframeGlobalListeners[key + eventName]);
        delete iframeGlobalListeners[key + eventName];
    }
}

function applyEditorImageChange(iframeId, width, height) {
    const selectedImgElement = selectionStates[iframeId];

    if (selectedImgElement && selectedImgElement.tagName.toLowerCase() === 'img') {

        selectedImgElement.setAttribute("width", `${width}px`);
        selectedImgElement.setAttribute("height", `${height}px`);
    }
}

function replaceSelectedEditorImage(iframeId, newImageUrl) {
    const selectedImgElement = selectionStates[iframeId];

    if (selectedImgElement && selectedImgElement.tagName.toLowerCase() === 'img' && newImageUrl) {
        selectedImgElement.setAttribute("width", `200px`); // TODO: some default value passed from outside
        selectedImgElement.setAttribute("height", `unset`);
        selectedImgElement.setAttribute("src", newImageUrl);
    }
}

function insertRowAboveTheSelection(iframeId) {
    const selectedElement = selectionStates[iframeId];
    if (!selectedElement) return;

    const tagName = selectedElement.tagName?.toLowerCase();
    if (tagName !== 'td' && tagName !== 'th') return;

    const currentRow = selectedElement.closest('tr');
    if (!currentRow) return;

    const newRow = currentRow.cloneNode(true);

    Array.from(newRow.cells).forEach(cell => {
        cell.innerHTML = '&zwj;';
    });

    currentRow.parentNode.insertBefore(newRow, currentRow);
}

function insertRowBelowTheSelection(iframeId) {
    const selectedElement = selectionStates[iframeId];
    if (!selectedElement) return;

    const tagName = selectedElement.tagName?.toLowerCase();
    if (tagName !== 'td' && tagName !== 'th') return;

    const currentRow = selectedElement.closest('tr');
    if (!currentRow) return;

    const newRow = currentRow.cloneNode(true);

    Array.from(newRow.cells).forEach(cell => {
        cell.innerHTML = '&zwj;';
    });

    if (tagName === 'th') {
        Array.from(newRow.cells).forEach(cell => {
            const td = document.createElement('td');
            td.innerHTML = cell.innerHTML;
            cell.parentNode.replaceChild(td, cell);
        });
    }

    if (currentRow.nextSibling) {
        currentRow.parentNode.insertBefore(newRow, currentRow.nextSibling);
    } else {
        currentRow.parentNode.appendChild(newRow);
    }
}

function removeRowOfTheSelection(iframeId) {
    const selectedElement = selectionStates[iframeId];
    if (!selectedElement) return;

    const tagName = selectedElement.tagName?.toLowerCase();
    if (tagName !== 'td' && tagName !== 'th') return;

    const currentRow = selectedElement.closest('tr');
    if (!currentRow) return;

    const tableSection = currentRow.parentNode;
    tableSection.removeChild(currentRow);
}

function insertColumnLeftOfSelection(iframeId) {
    const selectedElement = selectionStates[iframeId];
    if (!selectedElement) return;

    const tagName = selectedElement.tagName?.toLowerCase();
    if (tagName !== 'td' && tagName !== 'th') return;

    const currentCell = selectedElement;
    const currentRow = currentCell.parentNode;
    const table = currentRow.closest('table');
    if (!table) return;

    const columnIndex = Array.from(currentRow.cells).indexOf(currentCell);

    // Iterate over each row and insert a new cell at the same index
    Array.from(table.rows).forEach(row => {
        const referenceCell = row.cells[columnIndex];

        const newCell = row.rowIndex === 0 && tagName === 'th'
            ? document.createElement('th')
            : document.createElement('td');

        newCell.innerHTML = ''; // Or default content like '&nbsp;'
        if (referenceCell) {
            row.insertBefore(newCell, referenceCell);
        } else {
            // If the row is shorter, just append
            row.appendChild(newCell);
        }
    });
}

function insertColumnRightOfSelection(iframeId) {
    const selectedElement = selectionStates[iframeId];
    if (!selectedElement) return;

    const tagName = selectedElement.tagName?.toLowerCase();
    if (tagName !== 'td' && tagName !== 'th') return;

    const currentCell = selectedElement;
    const currentRow = currentCell.parentNode;
    const table = currentRow.closest('table');
    if (!table) return;

    const columnIndex = Array.from(currentRow.cells).indexOf(currentCell);

    // Iterate over each row and insert a new cell AFTER the selected column
    Array.from(table.rows).forEach(row => {
        const referenceCell = row.cells[columnIndex];
        const nextCell = referenceCell?.nextSibling;

        const newCell = row.rowIndex === 0 && tagName === 'th'
            ? document.createElement('th')
            : document.createElement('td');

        newCell.innerHTML = ''; // Or some default value
        if (nextCell) {
            row.insertBefore(newCell, nextCell);
        } else {
            row.appendChild(newCell);
        }
    });
}

function removeColumnOfTheSelection(iframeId) {
    const selectedElement = selectionStates[iframeId];
    if (!selectedElement) return;

    const tagName = selectedElement.tagName?.toLowerCase();
    if (tagName !== 'td' && tagName !== 'th') return;

    const currentCell = selectedElement;
    const currentRow = currentCell.parentNode;
    const table = currentRow.closest('table');
    if (!table) return;

    const columnIndex = Array.from(currentRow.cells).indexOf(currentCell);

    // Loop through all rows and remove the cell at the selected column index
    Array.from(table.rows).forEach(row => {
        if (row.cells.length > columnIndex) {
            row.deleteCell(columnIndex);
        }
    });
}


/*IFRAME*/


/*Flip Source-Preview*/

function setContentToSourceElement(iframeId, sourceId, contentEditabilityText) {
    const iframeElement = document.getElementById(iframeId);

    const iframeDocument = iframeElement.contentDocument || iframeElement.contentWindow.document;

    const codeMirrorEditor = htmlTextEditors[sourceId];
    let htmlValue = iframeDocument.documentElement.outerHTML;
    htmlValue = htmlValue.replace(contentEditabilityText, '');

    codeMirrorEditor.setValue(htmlValue);

    const from = { ch: 0, line: 0, sticky: null }
    const to = { ch: 0, line: codeMirrorEditor.lineCount(), sticky: null }

    codeMirrorEditor.autoFormatRange(from, to);
    codeMirrorEditor.setCursor(from);
    codeMirrorEditor.save();
    codeMirrorEditor.refresh();
}

function setContentToPreviewElement(iframeId, sourceId, contentEditabilityText) {

    editorsHistory[iframeId] = {
        back: [],
        forward: []
    }

    const iframeElement = document.getElementById(iframeId);
    const codeMirrorEditor = htmlTextEditors[sourceId];

    const iframeDocument = iframeElement.contentDocument || iframeElement.contentWindow.document;
    const content = codeMirrorEditor?.getValue();
    //const formattedTemplate = template.format(content);
    const formattedTemplate = content.replace(/<body\s/i, `<body ${contentEditabilityText}`);

    iframeDocument.open();
    iframeDocument.write(formattedTemplate);
    iframeDocument.close();
}

function getHtmlTextEditorValue(iframeId, sourceId, isPreview) {

    if (isPreview) {
        const iframeElement = document.getElementById(iframeId);
        if (!iframeElement) return null;

        const iframeDocument = iframeElement.contentDocument || iframeElement.contentWindow.document;
        return iframeDocument.documentElement.outerHTML;
    } else {
        const codeMirrorEditor = htmlTextEditors[sourceId];
        if (!codeMirrorEditor) return null;

        return "<div style=\"display: flex; flex-direction: column\">" + codeMirrorEditor?.getValue()?.replaceAll('\n', '') + "</div>";
    }
}

function setHtmlTextEditorValue(iframeId, sourceId, isPreview, value) {

    if (isPreview) {
        const iframeElement = document.getElementById(iframeId);
        if (!iframeElement) return;

        const iframeDocument = iframeElement.contentDocument || iframeElement.contentWindow.document;
        if (!iframeDocument) return;

        const history = editorsHistory[iframeId]; // TODO: UPDATE HISTORY

        history.forward.length = [];
        history.back.push(iframeDocument.documentElement.outerHTML);

        iframeDocument.open();
        iframeDocument.write(value);
        iframeDocument.close();

    } else {
        const codeMirrorEditor = htmlTextEditors[sourceId];
        codeMirrorEditor.setValue(value);

        const from = { ch: 0, line: 0, sticky: null }
        const to = { ch: 0, line: codeMirrorEditor.lineCount(), sticky: null }

        codeMirrorEditor.autoFormatRange(from, to);
        codeMirrorEditor.setCursor(from);
        codeMirrorEditor.save();
        codeMirrorEditor.refresh();
    }
}

/*Flip Source-Preview*/


/*CodeMirror*/

const htmlTextEditors = {};
const editorsHistory = {};
const selectionStates = {}

function initHtmlTextEditorCodeMirror(textAreaId, iframeId, dotnetReference) {

    editorsHistory[iframeId] = {
        back: [],
        forward: []
    }

    const textAreaElement = document.getElementById(textAreaId);

    if (!textAreaElement || htmlTextEditors[textAreaId]) return;

    const htmlEditor = CodeMirror.fromTextArea(textAreaElement, {
        lineNumbers: true,
        autoCloseTags: true,
        autoCloseBrackets: true,
        autoRefresh: true,
        firstLineNumber: 1,
        smartIndent: true,
        lineWrapping: true,
        refresh: true,
        tabSize: 4,
        indentUnit: 4,
        indentWithTabs: true,
        indentAuto: true,
        gutter: true,
        autoFocus: true,
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],

        extraKeys: {
            "Ctrl-Space": "autocomplete",
            "Ctrl-D": function (e) { },
            "Ctrl-U": function (e) { },
            "Ctrl-F": "findPersistent"
        },

        mode: {
            name: "htmlmixed",
            globalVars: true,
        },
        theme: 'default',
        inputStyle: "textarea"
    });

    htmlTextEditors[textAreaId] = htmlEditor;

    //htmlEditor.on('keypress', function (cMirror) {
    //    dotnetReference.invokeMethodAsync('OnKeyPressInCodeMirrorAsync');
    //})

    //htmlEditor.on('change', function (cMirror) {

    //    editorKeyPressed = false;
    //});

    //CodeMirror.commands.autocomplete = function (cm) {
    //    var doc = cm.getDoc();
    //    var POS = doc.getCursor();
    //    var mode = CodeMirror.innerMode(cm.getMode(), cm.getTokenAt(POS).state).mode.name;

    //    if (mode == 'xml') { //html depends on xml
    //        CodeMirror.showHint(cm, CodeMirror.hint.html);
    //    } else if (mode == 'javascript') {
    //        CodeMirror.showHint(cm, CodeMirror.hint.javascript);
    //    } else if (mode == 'css') {
    //        CodeMirror.showHint(cm, CodeMirror.hint.css);
    //    }
    //};

    //// format existing text
    //const from = { ch: 0, line: 0, sticky: null }
    //const to = { ch: 0, line: htmlEditor.lineCount(), sticky: null }

    //htmlEditor.autoFormatRange(from, to);
    //htmlEditor.setCursor(from);
}

function destroyHtmlTextEditorCodeMirror(textAreaId, iframeId) {
    delete htmlTextEditors[textAreaId];
    editorKeyPressed = false;

    delete editorsHistory[iframeId];
}

function getSelectedRangeEditor(htmlEditor) {
    return { from: htmlEditor.getCursor(true), to: htmlEditor.getCursor(false) };
};

function autoFormatSelectionEditor(textAreaId) {
    const htmlEditor = htmlTextEditors[textAreaId];
    if (!htmlEditor.hasFocus()) return;

    const range = getSelectedRangeEditor(htmlEditor);
    htmlEditor.autoFormatRange(range.from, range.to);
};

function commentSelectionEditor(textAreaId, isComment) {
    const htmlEditor = htmlTextEditors[textAreaId];
    if (!htmlEditor.hasFocus()) return;

    const range = getSelectedRangeEditor(htmlEditor);
    htmlEditor.commentRange(isComment, range.from, range.to);
};

/*CodeMirror*/

/*Commands*/

function getUndoRedoButtonsState(history) {
    return {
        undo: history.back.length > 0,
        redo: history.forward.length > 0
    }
}

function onEditorIFrameKeyDown(iframeId, iframeText) {
    const history = editorsHistory[iframeId];

    history.forward.length = [];
    history.back.push(iframeText);
}

function execCommandOnEditorWithSelection(iframeId, command, value = null) {
    const iframeElement = document.getElementById(iframeId);
    if (!iframeElement) return;

    // TODO: take whole doc
    const iframeDocument = iframeElement.contentDocument || iframeElement.contentWindow.document;
    const iframeWindow = iframeElement.contentWindow;
    const iframeBody = iframeDocument.body;

    let selectedElement = getSelectedNode(iframeDocument, iframeWindow);

    const iframeHtml = iframeDocument.documentElement.outerHTML;
    const history = editorsHistory[iframeId];

    switch (command) {
        case 'undo':
        {
            if (!history.back.length) break;

            history.forward.push(iframeHtml);
            const formattedTemplate = history.back.pop();

            iframeDocument.open();
            iframeDocument.write(formattedTemplate);
            iframeDocument.close();
        }
            break;
        case 'redo':
        {
            if (!history.forward.length) break;

            history.back.push(iframeHtml);
            const formattedTemplate = history.forward.pop();

            iframeDocument.open();
            iframeDocument.write(formattedTemplate);
            iframeDocument.close();
        }
            break;
        case 'fontSize': {
            if (!history.back.length || history.back[history.back.length - 1] != iframeHtml) {
                history.forward.length = [];
                history.back.push(iframeHtml);
            }

            if (selectedElement && (selectedElement == iframeBody || iframeDocument.contains(selectedElement))) {
                selectedElement.style.fontSize = value;
            }

            break;
        }
        default:
        {
            if (!history.back.length || history.back[history.back.length - 1] != iframeHtml) {
                history.forward.length = [];
                history.back.push(iframeHtml);
            }

            if (selectedElement && (selectedElement == iframeBody || iframeDocument.contains(selectedElement))) {
                iframeDocument.execCommand(command, false, value);
            } else {
                execCommandOnEditorWithoutSelection(iframeElement, command, value);
            }
        }
            break;
    }

    return getUndoRedoButtonsState(history);
}

function getSelectedNode(doc = null, win = null) {

    if (!doc) {
        doc = document;
        win = window;
    }

    if (doc.selection)
        return doc.selection.createRange().parentElement();
    else {
        var selection = win.getSelection();
        if (selection.rangeCount > 0)
            return selection.getRangeAt(0).startContainer.parentNode;
    }
}

function getSelectedRange(doc = null, win = null) {

    if (!doc) {
        doc = document;
        win = window;
    }

    if (doc.selection)
        return doc.selection.createRange();
    else {
        var selection = win.getSelection();
        if (selection.rangeCount > 0)
            return selection.getRangeAt(0);
    }
}

function getSelectedText(doc = null, win = null) {

    if (!doc) {
        doc = document;
        win = window;
    }

    if (win.getSelection) {
        return win.getSelection().toString();
    } else if (doc.selection && doc.selection.createRange) {
        return doc.selection.createRange().text;
    }
}

function execCommandOnEditorWithoutSelection(iframeElement, command, value = null) {

    const iframeDocument = iframeElement.contentDocument || iframeElement.contentWindow.document;
    const iframeWindow = iframeElement.contentWindow;
    const iframeBody = iframeDocument.body;

    if (typeof iframeWindow.getSelection != "undefined") {
        // Non-IE case
        var sel = iframeWindow.getSelection();

        // Save the current selection
        var savedRanges = [];
        for (var i = 0, len = sel.rangeCount; i < len; ++i) {
            savedRanges[i] = sel.getRangeAt(i).cloneRange();
        }

        // Temporarily enable designMode so that
        // document.execCommand() will work
        iframeDocument.designMode = "on";

        // Select the element's content
        sel = iframeWindow.getSelection();
        var range = iframeDocument.createRange();
        range.selectNodeContents(iframeBody);
        sel.removeAllRanges();
        sel.addRange(range);

        // Execute the command
        iframeDocument.execCommand(command, false, value);

        // Disable designMode
        iframeDocument.designMode = "off";

        // Restore the previous selection
        sel = iframeWindow.getSelection();

    } else if (typeof iframeDocument.body.createTextRange != "undefined") {
        // IE case
        var textRange = iframeDocument.body.createTextRange();
        textRange.moveToElementText(previewElement);
        textRange.execCommand(command, false, value);
    }
}