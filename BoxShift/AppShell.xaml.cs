using BoxShift.Pages;

namespace BoxShift;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            nameof(GamePage),
            typeof(GamePage)
        );

        Routing.RegisterRoute(
            nameof(EditorPage),
            typeof(EditorPage)
        );
    }
}
