using Controls.UserDialogs.Maui;
using Kamata.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Kamata.Views;

public partial class HomePage : ContentPage
{
	public HomePageViewModel vm;
    public HomePage()
    {
		InitializeComponent();
        BindingContext = vm = new HomePageViewModel();
    }
	protected override async void OnAppearing()
	{
		base.OnAppearing();
        var geolocationRequest = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
        var location = await Geolocation.GetLocationAsync(geolocationRequest);
         map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromMiles(1)));

        var pin = new Pin 
        { 
            Address="Address",
            Location =location,
            Type = PinType.Place,
            Label ="Your Location"
        };
        pin.MarkerClicked +=  Pin_MakerClicked;
         map.Pins.Add(pin);

    }

    private async void Pin_MakerClicked(object sender, PinClickedEventArgs e)
    {
        var pintinfor = (Pin)sender;
        await DisplayAlert("Hi Kamata", pintinfor.Address, "OK");
    }
    async Task<PermissionStatus> CheckAndRequestLocationPermission()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status == PermissionStatus.Granted)
            return status;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            return status;
        }

        if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
        {
            // Prompt the user with additional information as to why the permission is needed
        }

        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        return status;
    }
    void OnMapTypePickerSelectedIndexChanged(object sender, EventArgs e)
    {
        Picker picker = (Picker)sender;
       
        switch (picker.SelectedItem.ToString())
        {
            default:
            case "Street":
                map.MapType = MapType.Street;
                break;
            case "Satellite":
                map.MapType = MapType.Satellite;
                break;
            case "Hybrid":
                map.MapType = MapType.Hybrid;
                break;
        }
       
    }

    void OnContentPageUnloaded(object sender, EventArgs e)
    {
        map.Handler?.DisconnectHandler();
    }
}