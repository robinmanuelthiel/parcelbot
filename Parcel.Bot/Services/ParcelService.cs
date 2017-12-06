using System;
using System.Threading;
using System.Threading.Tasks;

namespace Parcel.Bot.Services
{
    [Serializable]
    public class ParcelService
    {
        public async Task<string> CheckTrackingNumberAsync(string trackingNumber)
        {
            Thread.Sleep(3000);
            return "Your package is currently at the parcel center in Nuremberg and will be delivered tommorrow at 3:00 P.M.";
        }

        public async Task SendComplaintAsync(string message)
        {
            Thread.Sleep(3000);
        }
    }
}