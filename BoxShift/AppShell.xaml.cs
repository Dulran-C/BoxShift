using BoxShift.Pages;
using BoxShift.Services;

namespace BoxShift;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        SettingsService settingsService =
            new SettingsService();

        settingsService.ApplyTheme();

        Routing.RegisterRoute(
            nameof(LevelSelectPage),
            typeof(LevelSelectPage));

        Routing.RegisterRoute(
            nameof(GamePage),
            typeof(GamePage));

        Routing.RegisterRoute(
            nameof(EditorPage),
            typeof(EditorPage));

        Routing.RegisterRoute(
            nameof(SettingsPage),
            typeof(SettingsPage));
    }
}