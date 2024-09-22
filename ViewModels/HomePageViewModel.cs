using Kamata.Models;
using Stateless.Graph;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Kamata.Helpers;
using Controls.UserDialogs.Maui;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Stateless;
namespace Kamata.ViewModels
{
    public class HomePageViewModel :BaseViewModel
    {
        public ObservableCollection<GooglePlaceAutoCompletePrediction> Places { get; private set; }
        public ObservableCollection<GooglePlaceAutoCompletePrediction> RecentPlaces { get; private set; }
        public GooglePlaceAutoCompletePrediction RecentPlace1 { get; private set; }
        public GooglePlaceAutoCompletePrediction RecentPlace2 { get; private set; }
        public ObservableCollection<PriceOption> PriceOptions { get; private set; }
        public PriceOption PriceOptionSelected { get; set; }

        public string PickupLocation { get; set; }

        Location OriginCoordinates { get; set; }
        Location DestinationCoordinates { get; set; }

        string _destinationLocation;
        public string DestinationLocation
        {
            get => _destinationLocation;
            set
            {
                _destinationLocation = value;
                if (!string.IsNullOrEmpty(_destinationLocation))
                {
                    GetPlacesCommand.Execute(_destinationLocation);
                }
            }
        }

        GooglePlaceAutoCompletePrediction _placeSelected;
        public GooglePlaceAutoCompletePrediction PlaceSelected
        {
            get => _placeSelected;
            set
            {
                _placeSelected = value;
                if (_placeSelected != null)
                {
                    GetPlaceDetailCommand.Execute(_placeSelected);
                }

            }
        }

        bool _isOriginFocused;
        public bool IsOriginFocused
        {
            get => _isOriginFocused;
            set
            {
                _isOriginFocused = value;
                if (_isOriginFocused)
                {
                    FireTriggerCommand.Execute(KamataTrigger.ChooseOrigin);
                }
            }
        }

        bool _isDestinationFocused;
        public bool IsDestinationFocused
        {
            get => _isDestinationFocused;
            set
            {
                _isDestinationFocused = value;
                if (_isDestinationFocused)
                {
                    FireTriggerCommand.Execute(KamataTrigger.ChooseDestination);
                }
            }
        }

        public KamataState State { get; private set; }

        public bool IsSearching { get; private set; }

        public ICommand DrawRouteCommand { get; set; }
        public ICommand CenterMapCommand { get; set; }
        public ICommand CleanPolylineCommand { get; set; }
        public ICommand GetPlaceDetailCommand { get; }
        public ICommand FireTriggerCommand { get; }
        public ICommand CameraIdledCommand { get; private set; }
        private ICommand GetPlacesCommand { get; }
        public ICommand GetLocationNameCommand { get; }


        private readonly StateMachine<KamataState, KamataTrigger> _stateMachine;
        public HomePageViewModel()
        {
            RecentPlaces = new ObservableCollection<GooglePlaceAutoCompletePrediction>()
            {
                {new GooglePlaceAutoCompletePrediction(){ PlaceId="ChIJq0wAE_CJr44RtWSsTkp4ZEM", StructuredFormatting=new StructuredFormatting(){ MainText="Random Place", SecondaryText="Paseo de los locutores #32" } } },
                {new GooglePlaceAutoCompletePrediction(){ PlaceId="ChIJq0wAE_CJr44RtWSsTkp4ZEM", StructuredFormatting=new StructuredFormatting(){ MainText="Green Tower", SecondaryText="Ensanche Naco #4343, Green 232" } } },
                {new GooglePlaceAutoCompletePrediction(){ PlaceId="ChIJm02ImNyJr44RNs73uor8pFU", StructuredFormatting=new StructuredFormatting(){ MainText="Tienda Aurora", SecondaryText="Rafael Augusto Sanchez" } } },
            };

            RecentPlace1 = RecentPlaces[0];
            RecentPlace2 = RecentPlaces[1];

            PriceOptions = new ObservableCollection<PriceOption>()
            {
                {new PriceOption(){ Tag="KamataX", Category="Economy", CategoryDescription="Affortable, everyday rides", PriceDetails=new System.Collections.Generic.List<PriceDetail>(){
                    { new PriceDetail(){ Type="Kamata X", Price=332, ArrivalETA="12:00pm", Icon="https://carcody.com/wp-content/uploads/2019/11/Webp.net-resizeimage.jpg" } },
                  { new PriceDetail(){ Type="Kamata Black", Price=150, ArrivalETA="12:40pm", Icon="https://i0.wp.com/uponarriving.com/wp-content/uploads/2019/08/uberxl.jpg" } }}
                 } },
                {new PriceOption(){Tag="kAMATAXL", Category="Extra Seats", CategoryDescription="Affortable rides for group up to 6" ,  PriceDetails=new System.Collections.Generic.List<PriceDetail>(){
                    { new PriceDetail(){ Type="kAMATA XL", Price=332, ArrivalETA="12:00pm", Icon="https://i0.wp.com/uponarriving.com/wp-content/uploads/2019/08/uberxl.jpg" } }
                  } } }
            };

            var _stateMachine = new StateMachine<KamataState, KamataTrigger>(KamataState.Initial);


            _stateMachine.Configure(KamataState.Initial)
                .OnEntry(Initialize)
                .OnExit(() => { Places = new ObservableCollection<GooglePlaceAutoCompletePrediction>(RecentPlaces); })
                .OnActivateAsync(GetActualUserLocation)
                .Permit(KamataTrigger.ChooseDestination, KamataState.SearchingDestination)
                .Permit(KamataTrigger.CalculateRoute, KamataState.CalculatingRoute);

            _stateMachine
                .Configure(KamataState.SearchingOrigin)
                .Permit(KamataTrigger.Cancel, KamataState.Initial)
                .Permit(KamataTrigger.ChooseDestination, KamataState.SearchingDestination);

            _stateMachine
                .Configure(KamataState.SearchingDestination)
                .Permit(KamataTrigger.Cancel, KamataState.Initial)
                .Permit(KamataTrigger.ChooseOrigin, KamataState.SearchingOrigin)
                .PermitIf(KamataTrigger.CalculateRoute, KamataState.CalculatingRoute, () => OriginCoordinates != null);

      

            _stateMachine
               .Configure(KamataState.ChoosingRide)
               .Permit(KamataTrigger.Cancel, KamataState.Initial)
               .Permit(KamataTrigger.ChooseDestination, KamataState.SearchingDestination)
               .Permit(KamataTrigger.ConfirmPickUp, KamataState.ConfirmingPickUp);

            _stateMachine
              .Configure(KamataState.ConfirmingPickUp)
              .Permit(KamataTrigger.ChooseRide, KamataState.ChoosingRide)
              .Permit(KamataTrigger.ShowXUberPass, KamataState.ShowingXUberPass);

            _stateMachine
              .Configure(KamataState.ShowingXUberPass)
              .Permit(KamataTrigger.ConfirmPickUp, KamataState.ConfirmingPickUp)
              .Permit(KamataTrigger.WaitForDriver, KamataState.WaitingForDriver);

            _stateMachine
             .Configure(KamataState.WaitingForDriver)
             .Permit(KamataTrigger.CancelTrip, KamataState.Initial)
             .Permit(KamataTrigger.StartTrip, KamataState.TripInProgress);

         
            GetPlacesCommand = new Command<string>(async (param) => await GetPlacesByName(param));
            GetLocationNameCommand = new Command<Location>(async (param) => await GetLocationName(param));

            FireTriggerCommand = new Command<KamataTrigger>((trigger) =>
            {
                if (_stateMachine.CanFire(trigger))
                {
                    _stateMachine.Fire(trigger);
                    State = _stateMachine.State;
                }

                IsSearching = (State == KamataState.SearchingOrigin || State == KamataState.SearchingDestination);
            });

            PriceOptionSelected = PriceOptions.First();

            State = _stateMachine.State;

            _stateMachine.ActivateAsync();
        }

        private void Initialize()
        {
            CleanPolylineCommand.Execute(null);
            DestinationLocation = string.Empty;
        }

        private async Task GetActualUserLocation()
        {
            try
            {
                await Task.Yield();
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(5000));
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    OriginCoordinates = location;
                    CenterMapCommand.Execute(location);
                    GetLocationNameCommand.Execute(new Location(location.Latitude, location.Longitude));
                }
            }
            catch (Exception)
            {
                await UserDialogs.Instance.AlertAsync("Error", "Unable to get actual location", "Ok");
            }
        }
        private async Task GetLocationName(Location position)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(position.Latitude, position.Longitude);
                PickupLocation = placemarks?.FirstOrDefault()?.FeatureName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private async Task GetPlacesByName(string placeText)
        {
           
        }

        private async Task GetPlacesDetail(GooglePlaceAutoCompletePrediction placeA)
        {

          
        }

        private async Task<bool> LoadRoute()
        {
            var retVal = false;

           

            return retVal;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

