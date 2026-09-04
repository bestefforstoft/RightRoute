using RouteLoadoutApp.ViewModels;

namespace RouteLoadoutApp.Views;

public partial class RouteEditorPage : ContentPage
{
    public RouteEditorPage()
    {
        InitializeComponent();
        
        // Connect UI data paths directly to the functional VM engine logic blocks
        BindingContext = new RouteEditorViewModel();
    }
}
