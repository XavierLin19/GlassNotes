using Microsoft.Win32;

namespace GlassNotes.Services;

public interface IRunKey
{
    object? GetValue(string name);
    void SetValue(string name, string value);
    void DeleteValue(string name);
}

public interface IStartupService
{
    bool IsEnabled();
    void Enable();
    void Disable();
}

public sealed class RegistryRunKey : IRunKey
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public object? GetValue(string name)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
        return key?.GetValue(name);
    }

    public void SetValue(string name, string value)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true)
            ?? throw new InvalidOperationException("Windows startup registry key could not be opened.");
        key.SetValue(name, value, RegistryValueKind.String);
    }

    public void DeleteValue(string name)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true)
            ?? throw new InvalidOperationException("Windows startup registry key could not be opened.");
        key.DeleteValue(name, throwOnMissingValue: false);
    }
}

public sealed class StartupService : IStartupService
{
    private readonly string _applicationName;
    private readonly IRunKey _runKey;

    public StartupService(string applicationName, string executablePath, IRunKey? runKey = null)
    {
        _applicationName = applicationName;
        CommandValue = $"\"{executablePath}\"";
        _runKey = runKey ?? new RegistryRunKey();
    }

    public string CommandValue { get; }

    public bool IsEnabled() => string.Equals(
        _runKey.GetValue(_applicationName) as string,
        CommandValue,
        StringComparison.Ordinal);

    public void Enable() => _runKey.SetValue(_applicationName, CommandValue);

    public void Disable() => _runKey.DeleteValue(_applicationName);
}
