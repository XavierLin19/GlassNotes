using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GlassNotes.Models;
using GlassNotes.Services;

namespace GlassNotes.Helpers;

public sealed class TodoListController
{
    private readonly StackPanel _list;
    private readonly Func<Note?> _getNote;
    private readonly Action<Note> _save;
    private readonly Func<AppSettings> _getSettings;
    private bool _rendering;

    public TodoListController(
        StackPanel list,
        Func<Note?> getNote,
        Action<Note> save,
        Func<AppSettings>? getSettings = null)
    {
        _list = list;
        _getNote = getNote;
        _save = save;
        _getSettings = getSettings ?? (() => new AppSettings());
    }

    public void Refresh()
    {
        _rendering = true;
        _list.Children.Clear();
        var note = _getNote();
        if (note is null) return;
        TodoDocumentService.EnsureBlocks(note);
        foreach (var block in note.Blocks) _list.Children.Add(CreateRow(note, block));
        _rendering = false;
    }

    public void AddTodo() { var note = _getNote(); if (note is null) return; note.Blocks.Add(NoteBlock.Todo(string.Empty)); Commit(note); Refresh(); FocusLast(); }

    public bool RemoveEmptyTodoAt(int index)
    {
        var note = _getNote();
        if (note is null || !TodoDocumentService.RemoveEmptyTodo(note.Blocks, index)) return false;
        Commit(note);
        Refresh();
        Focus(Math.Max(0, index - 1));
        return true;
    }

    private Grid CreateRow(Note note, NoteBlock block)
    {
        var row = new Grid { Margin = new Thickness(0, 2, 0, 2), MinHeight = 40, Background = Brushes.Transparent };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32) }); row.ColumnDefinitions.Add(new ColumnDefinition()); row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var bullet = Circle(block.IsCompleted ? "✓" : "", block.IsCompleted ? Color.FromRgb(82, 200, 107) : Colors.Transparent, block.IsCompleted ? Colors.Transparent : Color.FromRgb(51, 76, 67));
        bullet.MouseLeftButtonDown += (_, _) => { block.Kind = "Todo"; block.IsCompleted = !block.IsCompleted; Commit(note); Refresh(); };
        Grid.SetColumn(bullet, 0); row.Children.Add(bullet);
        var settings = _getSettings();
        var input = new TextBox
        {
            Text = block.Text,
            BorderThickness = new Thickness(0),
            Background = Brushes.Transparent,
            FontSize = settings.FontSize,
            FontFamily = new FontFamily(settings.FontFamily),
            VerticalContentAlignment = VerticalAlignment.Center,
            TextDecorations = block.IsCompleted ? TextDecorations.Strikethrough : null
        };
        input.TextChanged += (_, _) => { if (!_rendering) { block.Text = input.Text; Commit(note); } };
        input.KeyDown += (_, e) => { var index = note.Blocks.IndexOf(block); if (e.Key == Key.Enter) { e.Handled = true; note.Blocks.Insert(index + 1, NoteBlock.Todo(string.Empty)); Commit(note); Refresh(); Focus(index + 1); } else if (e.Key == Key.Back && string.IsNullOrEmpty(input.Text)) { e.Handled = RemoveEmptyTodoAt(index); } };
        Grid.SetColumn(input, 1); row.Children.Add(input);
        var actions = new StackPanel { Orientation = Orientation.Horizontal, Visibility = Visibility.Collapsed, Margin = new Thickness(8, 0, 0, 0) };
        actions.Children.Add(Action("✓", Color.FromRgb(82,200,107), () => { block.Kind = "Todo"; block.IsCompleted = !block.IsCompleted; Commit(note); Refresh(); }));
        actions.Children.Add(Action("↑", Color.FromRgb(255,148,13), () => { block.Kind = "Todo"; block.IsPinned = !block.IsPinned; Commit(note); Refresh(); }));
        actions.Children.Add(Action("×", Color.FromRgb(255,91,98), () => { note.Blocks.Remove(block); Commit(note); Refresh(); }));
        Grid.SetColumn(actions, 2); row.Children.Add(actions); row.MouseEnter += (_, _) => actions.Visibility = Visibility.Visible; row.MouseLeave += (_, _) => actions.Visibility = Visibility.Collapsed; return row;
    }
    private static Border Circle(string text, Color fill, Color stroke) => new() { Width=22, Height=22, CornerRadius=new CornerRadius(11), Background=new SolidColorBrush(fill), BorderBrush=new SolidColorBrush(stroke), BorderThickness=new Thickness(2), Child=new TextBlock { Text=text, Foreground=Brushes.White, HorizontalAlignment=HorizontalAlignment.Center, VerticalAlignment=VerticalAlignment.Center } };
    private static Border Action(string text, Color color, Action click) { var b=Circle(text,color,Colors.Transparent); b.Margin=new Thickness(3,0,0,0); b.Cursor=Cursors.Hand; b.MouseLeftButtonDown+=(_,_)=>click(); return b; }
    private void Commit(Note note) { note.Blocks = TodoDocumentService.Normalize(note.Blocks); note.SetContentFromBlocks(TodoDocumentService.ToPlainText(note.Blocks)); _save(note); }
    private void Focus(int index) { if (index >= 0 && index < _list.Children.Count && ((Grid)_list.Children[index]).Children[1] is TextBox input) { input.Focus(); input.CaretIndex=input.Text.Length; } }
    private void FocusLast() => Focus(_list.Children.Count - 1);
}
