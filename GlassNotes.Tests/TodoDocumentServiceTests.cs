using GlassNotes.Models;
using GlassNotes.Services;
using Xunit;

namespace GlassNotes.Tests;

public sealed class TodoDocumentServiceTests
{
    [Fact]
    public void Normalize_places_completed_todos_after_text_and_open_todos()
    {
        var blocks = new List<NoteBlock>
        {
            NoteBlock.Todo("first", true), NoteBlock.FromText("intro"),
            NoteBlock.Todo("second", false)
        };

        var result = TodoDocumentService.Normalize(blocks);

        Assert.Collection(result,
            block => Assert.Equal("intro", block.Text),
            block => Assert.Equal("second", block.Text),
            block => Assert.Equal("first", block.Text));
        Assert.False(result[1].IsCompleted);
        Assert.True(result[2].IsCompleted);
    }
}
