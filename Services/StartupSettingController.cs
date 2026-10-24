using GlassNotes.Models;

namespace GlassNotes.Services;

public sealed record StartupSettingResult(bool Succeeded, bool LaunchAtStartup, Exception? Error = null);

public sealed class StartupSettingController
{
    private readonly IStartupService _startupService;
    private readonly AppSettings _settings;

    public StartupSettingController(IStartupService startupService, AppSettings settings)
    {
        _startupService = startupService;
        _settings = settings;
    }

    public StartupSettingResult ReadLaunchAtStartup()
    {
        try
        {
            var isEnabled = _startupService.IsEnabled();
            _settings.LaunchAtStartup = isEnabled;
            return new StartupSettingResult(true, isEnabled);
        }
        catch (Exception ex)
        {
            return new StartupSettingResult(false, _settings.LaunchAtStartup, ex);
        }
    }

    public StartupSettingResult SetLaunchAtStartup(bool enabled)
    {
        try
        {
            if (enabled)
                _startupService.Enable();
            else
                _startupService.Disable();

            _settings.LaunchAtStartup = enabled;
            return new StartupSettingResult(true, enabled);
        }
        catch (Exception ex)
        {
            return new StartupSettingResult(false, _settings.LaunchAtStartup, ex);
        }
    }
}
