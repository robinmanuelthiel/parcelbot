using System;
using Microsoft.Bot.Builder.FormFlow;

namespace Parcel.Bot.Models
{
    [Serializable]
    public class ParcelTracking
    {
        /// <summary>
        /// Tracking number, when user entered invalid entries
        /// </summary>
        public static string NoTrackingNumberCode = "-1";

        [Prompt("What is your {&}?")]
        [Describe("Package Tracking Number")]
        public string TrackingNumber;

        public static IForm<ParcelTracking> BuildForm()
        {
            return new FormBuilder<ParcelTracking>()
                .Message("Let's see where we can find your package!")
                .Field(nameof(TrackingNumber), validate: async (state, value) =>
                {
                    // Check if entered tracking number is valid
                    // TODO: Ask LUIS for cancellation words
                    if (((string)value).Contains("Ahnung"))
                    {
                        // User does not know his tracking number
                        return new ValidateResult { IsValid = true, Value = NoTrackingNumberCode };
                    }

                    // TODO: Add pattern check if valid number has been entered
                    // Tracking number is valid
                    return new ValidateResult { IsValid = true, Value = value };
                })
                //.AddRemainingFields() // Would automatically add all remaining fields of this class
                //.Confirm() // Would add a confirmation dialog at the end of all entries
                .Build();
        }
    }
}
