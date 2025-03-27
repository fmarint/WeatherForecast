using WeatherForecast.MVVM.ViewModels;

namespace WeatherForecast.MVVM.Views;

public partial class WeatherView : ContentPage
{
	public WeatherView()
	{
		InitializeComponent();
		BindingContext = new WeatherViewModel();
	}


}