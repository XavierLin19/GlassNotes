using System.Windows;
using System.Windows.Controls;
using GlassNotes.Helpers;
using GlassNotes.Models;
using Xunit;

namespace GlassNotes.Tests;

public sealed class TodoListControllerTests
{
    [Fact]
    public void Refresh_uses_the_application_default_font_size_for_todo_text()
    {
        var fontSize = OnStaThread(() =>
        {
            var note = new Note { Blocks = [NoteBlock.Todo("Task")] };
            var list = new StackPanel();
            var controller = new TodoListController(list, () => note, _ => { });

            controller.Refresh();

            var row = Assert.IsType<Grid>(Assert.Single(list.Children));
            var input = Assert.IsType<TextBox>(row.Children[1]);
            return input.FontSize;
        });

        Assert.Equal(new AppSettings().FontSize, fontSize);
    }

    [Fact]
    public void RemoveEmptyTodo_deletes_the_last_empty_row_without_recreating_it()
    {
        var rowCount = OnStaThread(() =>
        {
            var note = new Note { Blocks = [NoteBlock.Todo(string.Empty)] };
            var list = new StackPanel();
            var controller = new TodoListController(list, () => note, _ => { });

            Assert.True(controller.RemoveEmptyTodoAt(0));
            controller.Refresh();
            return list.Children.Count;
        });

        Assert.Equal(0, rowCount);
    }

    private static T OnStaThread<T>(Func<T> action)
    {
        T? result = default;
        Exception? error = null;
        var thread = new Thread(() =>
        {
            try { result = action(); }
            catch (Exception exception) { error = exception; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (error is not null) throw error;
        return result!;
    }
}
