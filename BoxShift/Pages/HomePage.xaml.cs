namespace BoxShift.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void PlayClicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(LevelSelectPage));
    }

    private async void EditorClicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(EditorPage));
    }

    private async void SettingsClicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(SettingsPage));
    }
}