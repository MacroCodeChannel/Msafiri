using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamata.Helpers
{
    public enum KamataState
    {
        Initial,
        SearchingOrigin,
        SearchingDestination,
        CalculatingRoute,
        ChoosingRide,
        ConfirmingPickUp,
        ShowingXUberPass,
        ShowingHealthyMeasures,
        AssigningDriver,
        WaitingForDriver,
        TripInProgress,
        TripCompleted
    }
}
