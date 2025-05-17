using Microsoft.AspNetCore.Components;

namespace BlazorComponentHeap.HtmlEditor.Components.TableOptions;

public partial class TableOptionsComponent
{
    [Parameter] public EventCallback OnInsertRowAbove { get; set; }
    [Parameter] public EventCallback OnInsertRowBelow { get; set; }
    [Parameter] public EventCallback OnRemoveSelectedRow { get; set; }
    [Parameter] public EventCallback OnInsertColumnToTheLeft { get; set; }
    [Parameter] public EventCallback OnInsertColumnToTheRight { get; set; }
    [Parameter] public EventCallback OnRemoveSelectedColumn { get; set; }
}