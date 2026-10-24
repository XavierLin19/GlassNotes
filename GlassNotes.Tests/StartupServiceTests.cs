using GlassNotes.Services;
using GlassNotes.Models;
using Xunit;

namespace GlassNotes.Tests;

public sealed class StartupServiceTests
{
    [Fact]
    public void CommandValue_quotes_the_executable_path()
    {
        var service = new StartupService("GlassNotes", @"D:\Glass Note\GlassNotes.exe", new FakeRunKey());

        Assert.Equal("\"D:\\Glass Note\\GlassNotes.exe\"", service.CommandValue);
    }

    [Fact]
    public void Enable_writes_the_quoted_command_to_the_run_key()
    {
        var key = new FakeRunKey();
        var service = new StartupService("GlassNotes", @"D:\Glass Note\GlassNotes.exe", key);

        service.Enable();

        Assert.Equal(service.CommandValue, key.GetValue("GlassNotes"));
    }

    [Fact]
    public void Disable_removes_the_application_value_from_the_run_key()
    {
        var key = new FakeRunKey();
        var service = new StartupService("GlassNotes", @"D:\Glass Note\GlassNotes.exe", key);
        service.Enable();

        service.Disable();

        Assert.Null(key.GetValue("GlassNotes"));
    }

    [Fact]
    public void IsEnabled_requires_the_exact_expected_command()
    {
        var key = new FakeRunKey();
        var service = new StartupService("GlassNotes", @"D:\Glass Note\GlassNotes.exe", key);

        key.SetValue("GlassNotes", "D:\\Other\\GlassNotes.exe");

        Assert.False(service.IsEnabled());
    }

    [Fact]
    public void IsEnabled_returns_true_when_the_run_key_matches_the_expected_command()
    {
        var key = new FakeRunKey();
        var service = new StartupService("GlassNotes", @"D:\Glass Note\GlassNotes.exe", key);
        key.SetValue("GlassNotes", service.CommandValue);

        Assert.True(service.IsEnabled());
    }

    [Fact]
    public void Enable_propagates_a_run_key_write_failure()
    {
        var service = new StartupService("GlassNotes", @"D:\Glass Note\GlassNotes.exe", new ThrowingRunKey());

        var exception = Assert.Throws<InvalidOperationException>(() => service.Enable());

        Assert.Equal("Run key unavailable", exception.Message);
    }

    [Fact]
    public void IsEnabled_propagates_a_run_key_read_failure()
    {
        var service = new StartupService("GlassNotes", @"D:\Glass Note\GlassNotes.exe", new ThrowingRunKey());

        var exception = Assert.Throws<InvalidOperationException>(() => service.IsEnabled());

        Assert.Equal("Run key unavailable", exception.Message);
    }

    [Fact]
    public void NewSettings_do_not_request_launch_at_startup()
    {
        Assert.False(new AppSettings().LaunchAtStartup);
    }

    [Fact]
    public void Enable_failure_keeps_the_previously_disabled_setting()
    {
        var settings = new AppSettings { LaunchAtStartup = false };
        var controller = new StartupSettingController(new ThrowingStartupService(), settings);

        var result = controller.SetLaunchAtStartup(true);

        Assert.False(result.Succeeded);
        Assert.False(result.LaunchAtStartup);
        Assert.False(settings.LaunchAtStartup);
    }

    [Fact]
    public void Disable_failure_keeps_the_previously_enabled_setting()
    {
        var settings = new AppSettings { LaunchAtStartup = true };
        var controller = new StartupSettingController(new ThrowingStartupService(), settings);

        var result = controller.SetLaunchAtStartup(false);

        Assert.False(result.Succeeded);
        Assert.True(result.LaunchAtStartup);
        Assert.True(settings.LaunchAtStartup);
    }

    private sealed class FakeRunKey : IRunKey
    {
        private readonly Dictionary<string, object?> _values = new();

        public object? GetValue(string name) => _values.GetValueOrDefault(name);

        public void SetValue(string name, string value) => _values[name] = value;

        public void DeleteValue(string name) => _values.Remove(name);
    }

    private sealed class ThrowingRunKey : IRunKey
    {
        public object? GetValue(string name) => throw new InvalidOperationException("Run key unavailable");

        public void SetValue(string name, string value) => throw new InvalidOperationException("Run key unavailable");

        public void DeleteValue(string name) => throw new InvalidOperationException("Run key unavailable");
    }

    private sealed class ThrowingStartupService : IStartupService
    {
        public bool IsEnabled() => throw new InvalidOperationException("Run key unavailable");

        public void Enable() => throw new InvalidOperationException("Run key unavailable");

        public void Disable() => throw new InvalidOperationException("Run key unavailable");
    }
}
