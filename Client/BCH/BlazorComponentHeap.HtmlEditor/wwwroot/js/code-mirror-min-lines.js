// set minimum number of lines CodeMirror instance is allowed to have
(function (mod) {
    mod(CodeMirror);
})(function (CodeMirror) {
    var fill = function (cm, start, n) {
        while (start < n) {
            let count = cm.lineCount();
            cm.replaceRange("\n", { line: count - 1 }), start++;
            // remove new line change from history (otherwise user could ctrl+z to remove line)
            let history = cm.getHistory();
            history.done.pop(), history.done.pop();
            cm.setHistory(history);
            if (start == n) break;
        }
    };
    var pushLines = function (cm, selection, n) {
        // push lines to last change so that "undo" doesn't add lines back
        var line = cm.lineCount() - 1;
        var history = cm.getHistory();
        history.done[history.done.length - 2].changes.push({
            from: {
                line: line - n,
                ch: cm.getLine(line - n).length,
                sticky: null
            },
            text: [""],
            to: { line: line, ch: 0, sticky: null }
        });
        cm.setHistory(history);
        cm.setCursor({ line: selection.start.line, ch: selection.start.ch });
    };

    var keyMap = {
        Backspace: function (cm) {
            var cursor = cm.getCursor();
            var selection = {
                start: cm.getCursor(true),
                end: cm.getCursor(false)
            };

            // selection
            if (selection.start.line !== selection.end.line) {
                let func = function (e) {
                    var count = cm.lineCount(); // current number of lines
                    var n = cm.options.minLines - count; // lines needed
                    if (e.key == "Backspace" || e.code == "Backspace" || e.which == 8) {
                        fill(cm, 0, n);
                        if (count <= cm.options.minLines) pushLines(cm, selection, n);
                    }
                    cm.display.wrapper.removeEventListener("keydown", func);
                };
                cm.display.wrapper.addEventListener("keydown", func); // fires after CodeMirror.Pass

                return CodeMirror.Pass;
            } else if (selection.start.ch !== selection.end.ch) return CodeMirror.Pass;

            // cursor
            var line = cm.getLine(cursor.line);
            var prev = cm.getLine(cursor.line - 1);
            if (
                cm.lineCount() == cm.options.minLines &&
                prev !== undefined &&
                cursor.ch == 0
            ) {
                if (line.length) {
                    // add a line because this line will be attached to previous line per default behaviour
                    cm.replaceRange("\n", { line: cm.lineCount() - 1 });
                    return CodeMirror.Pass;
                } else cm.setCursor(cursor.line - 1, prev.length); // set cursor at end of previous line
            }
            if (cm.lineCount() > cm.options.minLines || cursor.ch > 0)
                return CodeMirror.Pass;
        },
        Delete: function (cm) {
            var cursor = cm.getCursor();
            var selection = {
                start: cm.getCursor(true),
                end: cm.getCursor(false)
            };

            // selection
            if (selection.start.line !== selection.end.line) {
                let func = function (e) {
                    var count = cm.lineCount(); // current number of lines
                    var n = cm.options.minLines - count; // lines needed
                    if (e.key == "Delete" || e.code == "Delete" || e.which == 46) {
                        fill(cm, 0, n);
                        if (count <= cm.options.minLines) pushLines(cm, selection, n);
                    }
                    cm.display.wrapper.removeEventListener("keydown", func);
                };
                cm.display.wrapper.addEventListener("keydown", func); // fires after CodeMirror.Pass

                return CodeMirror.Pass;
            } else if (selection.start.ch !== selection.end.ch) return CodeMirror.Pass;

            // cursor
            var line = cm.getLine(cursor.line);
            if (cm.lineCount() == cm.options.minLines) {
                if (
                    cursor.ch == 0 &&
                    (line.length !== 0 || cursor.line == cm.lineCount() - 1)
                )
                    return CodeMirror.Pass;
                if (cursor.ch == line.length && cursor.line + 1 < cm.lineCount()) {
                    // add a line because next line will be attached to this line per default behaviour
                    cm.replaceRange("\n", { line: cm.lineCount() - 1 });
                    return CodeMirror.Pass;
                } else if (cursor.ch > 0) return CodeMirror.Pass;
            } else return CodeMirror.Pass;
        }
    };

    var onCut = function (cm) {
        var selection = {
            start: cm.getCursor(true),
            end: cm.getCursor(false)
        };
        setTimeout(function () {
            // wait until after cut is complete
            var count = cm.lineCount(); // current number of lines
            var n = cm.options.minLines - count; // lines needed
            fill(fm, 0, n);
            if (count <= cm.options.minLines) pushLines(cm, selection, n);
        });
    };

    var start = function (cm) {
        // set minimum number of lines on init
        var count = cm.lineCount(); // current number of lines
        cm.setCursor(count); // set the cursor at the end of existing content
        fill(cm, 0, cm.options.minLines - count);

        cm.addKeyMap(keyMap);

        // bind events
        cm.display.wrapper.addEventListener("cut", onCut, true);
    };
    var end = function (cm) {
        cm.removeKeyMap(keyMap);

        // unbind events
        cm.display.wrapper.removeEventListener("cut", onCut, true);
    };

    CodeMirror.defineOption("minLines", undefined, function (cm, val, old) {
        if (val !== undefined && val > 0) start(cm);
        else end(cm);
    });
});