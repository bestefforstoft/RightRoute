using RightRoute.ViewModels;

namespace RightRoute.Views;

public partial class RouteEditorPage : ContentPage
{
	public RouteEditorPage()
	{
        BindingContext = new RouteEditorViewModel();
    }
}