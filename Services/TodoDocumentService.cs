using GlassNotes.Models;

namespace GlassNotes.Services;

public static class TodoDocumentService
{
    public static List<NoteBlock> Normalize(IEnumerable<NoteBlock> blocks) =>
        blocks.Where(block => block.Kind == "Text")
            .Concat(blocks.Where(block => block.Kind == "Todo" && !block.IsCompleted))
            .Concat(blocks.Where(block => block.Kind == "Todo" && block.IsCompleted))
            .ToList();
}
