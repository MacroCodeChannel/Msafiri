using Kamata.Views;

namespace Kamata
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SearchHeaderView), typeof(SearchHeaderView));
            Routing.RegisterRoute(nameof(RecentPlaceView), typeof(RecentPlaceView));
            Routing.RegisterRoute(nameof(PriceView), typeof(PriceView));
        }
    }
}
