using Kamata.Views;

namespace Kamata
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainLandingPage();
        }
    }
}
