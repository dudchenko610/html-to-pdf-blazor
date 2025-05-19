namespace BlazorComponentHeap.HtmlEditor;

public static class EditorConstants
{
    public const string EditorTemplate = @"
        <html>
	        <head>
                <meta charset='utf-8' />
                <style>
                    p, h1, h2, h3, h4, h5, h6, pre, code, label {
                        margin: 0;
                    }
                    
                    html {
                        min-height: 100%;
                    }
                    
                    body {
                        position: absolute;
                        min-height: 100%;
                        height: 100%;
                        width: 100%;
                    }
                </style>
            </head>
	        <body ></body>
        </html>
    ";
    
    public static readonly List<string> AlignCommands = new List<string>
    {
        "center",
        "left",
        "right",
        "justify"
    };
    
    public static readonly List<string> Headings = new List<string>
    {
        "Paragraph",
        "Code",
        "Quotation",
        "H1",
        "H2",
        "H3",
        "H4",
        "H5",
        "H6"
    };
    
    public static readonly List<string> FontSizes = new List<string>
    {
        "8pt",
        "10pt",
        "12pt",
        "14pt",
        "16pt",
        "18pt",
        "20pt",
        "22pt",
        "24pt",
        "26pt",
        "28pt",
        "30pt",
        "32pt",
        "34pt"
    };
    
    public static readonly List<string> Colors = new List<string>
    {
        "#ffffff",
        "#000000",
        "#e7e5e6",
        "#44546a",
        "#4472c4",
        "#ed7d31",
        "#a5a5a5",
        "#fec000",
        "#70ad47",
        "#ff0000",

        "#f2f2f2",
        "#808080",
        "#cfcdcd",
        "#d5dce4",
        "#d9e4f2",
        "#fbe4d5",
        "#ededed",
        "#fff1cc",
        "#e2f0d9",
        "#ffcccc",

        "#d9d9d9",
        "#595959",
        "#aeaaaa",
        "#acb9ca",
        "#b4c6e7",
        "#f7caab",
        "#dbdbdb",
        "#ffe59a",
        "#c5e0b3",
        "#ff8080",

        "#bfbfbf",
        "#404040",
        "#747070",
        "#8496b0",
        "#8eaadb",
        "#f4b083",
        "#c9c9c9",
        "#fed966",
        "#a7d08c",
        "#ff3333",

        "#a6a6a6",
        "#262626",
        "#3b3838",
        "#323e4f",
        "#2f5496",
        "#c45911",
        "#7b7b7b",
        "#bf8f00",
        "#538135",
        "#b30000",

        "#7f7f7f",
        "#0d0d0d",
        "#161616",
        "#212934",
        "#1f3763",
        "#823b0b",
        "#525252",
        "#7f5f00",
        "#375623",
        "#660000"
    };
}