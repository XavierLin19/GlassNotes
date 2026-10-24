namespace GlassNotes.Models;

public sealed class NoteBlock
{
    public string Kind { get; set; } = "Text";
    public string Text { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }

    public static NoteBlock FromText(string text) => new() { Kind = "Text", Text = text };

    public static NoteBlock Todo(string text, bool completed = false) => new()
    {
        Kind = "Todo",
        Text = text,
        IsCompleted = completed
    };
}
