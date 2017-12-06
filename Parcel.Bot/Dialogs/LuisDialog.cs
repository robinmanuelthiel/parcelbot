using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Parcel.Bot.Models;

namespace Parcel.Bot.Dialogs
{
    [LuisModel("69309aee-0d7c-483c-880c-acc74eae8208", "75171980197d44a7ac3b6c3380e7bb24", domain: "westeurope.api.cognitive.microsoft.com")]
    [Serializable]
    public class LuisDialog : LuisDialog<ParcelTracking>
    {
        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            var message = context.MakeMessage();
            message.Text = message.Speak = "Hi, I'm the Parcel Bot. How can I help?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}