

using Kamata.ViewModels;

namespace Kamata.Views
{
    public partial class MainLandingPage : ContentPage
    {
        public MainLandingPageViewModel vm;
        public MainLandingPage()
        {
            InitializeComponent();
            BindingContext = vm = new MainLandingPageViewModel();
        }
    }
}