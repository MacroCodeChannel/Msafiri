using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Reflection;
using System.Windows.Input;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace Kamata.Controls
{
    public class ExtendedMap : Map, IDisposable
    {
        public static readonly BindableProperty CalculateCommandProperty =
          BindableProperty.Create(nameof(CalculateCommand), typeof(ICommand), typeof(ExtendedMap), null, BindingMode.TwoWay);

        public ICommand CalculateCommand
        {
            get { return (ICommand)GetValue(CalculateCommandProperty); }
            set { SetValue(CalculateCommandProperty, value); }
        }

        public static readonly BindableProperty UpdateCommandProperty =
          BindableProperty.Create(nameof(UpdateCommand), typeof(ICommand), typeof(ExtendedMap), null, BindingMode.TwoWay);

        public ICommand UpdateCommand
        {
            get { return (ICommand)GetValue(UpdateCommandProperty); }
            set { SetValue(UpdateCommandProperty, value); }
        }

        public static readonly BindableProperty GetActualLocationCommandProperty =
          BindableProperty.Create(nameof(GetActualLocationCommand), typeof(ICommand), typeof(ExtendedMap), null, BindingMode.TwoWay);

        public ICommand GetActualLocationCommand
        {
            get { return (ICommand)GetValue(GetActualLocationCommandProperty); }
            set { SetValue(GetActualLocationCommandProperty, value); }
        }

        public static readonly BindableProperty ActivateMapClickedProperty =
          BindableProperty.Create(nameof(ActivateMapClicked), typeof(bool), typeof(ExtendedMap), false, BindingMode.TwoWay);

        public static readonly BindableProperty CenterMapCommandProperty =
          BindableProperty.Create(nameof(CenterMapCommand), typeof(ICommand), typeof(ExtendedMap), null, BindingMode.TwoWay);

        public ICommand CenterMapCommand
        {
            get { return (ICommand)GetValue(CenterMapCommandProperty); }
            set { SetValue(CenterMapCommandProperty, value); }
        }

        public static readonly BindableProperty DrawRouteCommandProperty =
            BindableProperty.Create(nameof(DrawRouteCommand), typeof(ICommand), typeof(ExtendedMap), null, BindingMode.TwoWay);

        public ICommand DrawRouteCommand
        {
            get { return (ICommand)GetValue(DrawRouteCommandProperty); }
            set { SetValue(DrawRouteCommandProperty, value); }
        }

        public static readonly BindableProperty CleanPolylineCommandProperty =
          BindableProperty.Create(nameof(CleanPolylineCommand), typeof(ICommand), typeof(ExtendedMap), null, BindingMode.TwoWay);

        public ICommand CleanPolylineCommand
        {
            get { return (ICommand)GetValue(CleanPolylineCommandProperty); }
            set { SetValue(CleanPolylineCommandProperty, value); }
        }

        public static readonly BindableProperty CameraIdledCommandProperty =
         BindableProperty.Create(nameof(CameraIdledCommand), typeof(ICommand), typeof(ExtendedMap), null, BindingMode.TwoWay);

        public ICommand CameraIdledCommand
        {
            get { return (ICommand)GetValue(CameraIdledCommandProperty); }
            set { SetValue(CameraIdledCommandProperty, value); }
        }

        public bool ActivateMapClicked
        {
            get { return (bool)GetValue(ActivateMapClickedProperty); }
            set { SetValue(ActivateMapClickedProperty, value); }
        }

        public event EventHandler OnCalculate = delegate { };
        public event EventHandler OnLocationError = delegate { };

        public ExtendedMap()
        {
            AddMapStyle();
        }

        private void ExtendedMap_MapCameraIdled(object sender, EventArgs e)
        {
            // Fetch the current visible region of the map
            var visibleRegion = this.VisibleRegion;

            if (visibleRegion != null)
            {
                // Create a target position from the center of the visible region
                var target = new Location(visibleRegion.Center.Latitude,visibleRegion.Center.Longitude);

                // Execute the command if it's set
                if (CameraIdledCommand != null && CameraIdledCommand.CanExecute(target))
                {
                    CameraIdledCommand.Execute(target);
                }
            }
        }

        private void ExtendedMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            // Handle map click event if needed
        }


        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                CalculateCommand = new Command<List<Location>>(Calculate);
                UpdateCommand = new Command<Location>(Update);
                GetActualLocationCommand = new Command(async () => await GetActualLocation());
                DrawRouteCommand = new Command<List<Location>>(DrawRoute);
                CenterMapCommand = new Command<Location>(OnCenterMap);
                CleanPolylineCommand = new Command(CleanPolyline);
            }
        }

        void AddMapStyle()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream($"Kamata.MapStyle.json");
            string styleFile;
            using (var reader = new System.IO.StreamReader(stream))
            {
                styleFile = reader.ReadToEnd();
            }
        }


        async void Update(Location position)
        {
        }

        void Calculate(List<Location> list)
        {



        }

        async Task GetActualLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    MoveToRegion(MapSpan.FromCenterAndRadius(
                        new Location(location.Latitude, location.Longitude), Distance.FromMiles(0.3)));

                }
            }
            catch (Exception ex)
            {
                OnLocationError?.Invoke(this, default(EventArgs));
            }
        }


        void CleanPolyline()
        {

        }

        void DrawRoute(List<Location> list)
        {

        }


        void OnCenterMap(Location location)
        {
            MoveToRegion(MapSpan.FromCenterAndRadius(
                new Location(location.Latitude, location.Longitude), Distance.FromMiles(2)));

            LoadNearCars(location);
        }

        void LoadNearCars(Location location)
        {

        }

        public void Dispose()
        {

        }
    }
}