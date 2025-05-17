namespace BlazorComponentHeap.HtmlEditor.Attributes;

public class DocumentCommandAttribute : Attribute
{
    public string Command { get; } = string.Empty;
    public int BitShift { get; }

    public DocumentCommandAttribute(string command, int bitShift = 0)
    {
        Command = command;
        BitShift = bitShift;
    }
}