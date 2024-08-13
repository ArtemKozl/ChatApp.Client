using OnionMaui.ViewModel;

namespace OnionMaui.View;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage(RegistrationPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}