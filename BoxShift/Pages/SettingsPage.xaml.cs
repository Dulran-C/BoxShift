using BoxShift.Services;

namespace BoxShift.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsService _settingsService;

    public SettingsPage()
    {
        InitializeComponent();

        _settingsService =
            new SettingsService();

        GridThemePicker.ItemsSource =
            new List<string>
            {
                "Classic",
                "Ocean",
                "Forest"
            };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        DarkModeSwitch.IsToggled =
            _settingsService.IsDarkMode;

        AnimationsSwitch.IsToggled =
            _settingsService.AnimationsEnabled;

        GridThemePicker.SelectedItem =
            _settingsService.GridTheme;
    }

    private void DarkModeToggled(
        object sender,
        ToggledEventArgs e)
    {
        _settingsService.IsDarkMode =
            e.Value;

        _settingsService.ApplyTheme();

        SettingsStatus.Text =
            "Theme saved.";
    }

    private void GridThemeChanged(
        object sender,
        EventArgs e)
    {
        if (GridThemePicker.SelectedItem
            is not string selectedTheme)
        {
            return;
        }

        _settingsService.GridTheme =
            selectedTheme;

        SettingsStatus.Text =
            $"Grid theme: {selectedTheme}";
    }

    private void AnimationsToggled(
        object sender,
        ToggledEventArgs e)
    {
        _settingsService.AnimationsEnabled =
            e.Value;

        SettingsStatus.Text =
            "Animation setting saved.";
    }
}