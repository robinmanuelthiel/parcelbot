using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Parcel.Bot.Models;
using Parcel.Bot.Services;

namespace Parcel.Bot.Dialogs
{
    [LuisModel("69309aee-0d7c-483c-880c-acc74eae8208", "75171980197d44a7ac3b6c3380e7bb24", domain: "westeurope.api.cognitive.microsoft.com")]
    [Serializable]
    public class LuisDialog : LuisDialog<ParcelTracking>
    {
        private readonly BuildFormDelegate<ParcelTracking> parcelTrackingForm;
        private readonly ParcelService parcelService;

        public LuisDialog(BuildFormDelegate<ParcelTracking> parcelTrackingForm, ParcelService parcelService)
        {
            this.parcelTrackingForm = parcelTrackingForm;
            this.parcelService = parcelService;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I didn't get that.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            var message = context.MakeMessage();
            message.Text = message.Speak = "Hi, I'm the Parcel Bot. How can I help?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("ParcelTracking")]
        public async Task ParcelTracking(IDialogContext context, LuisResult result)
        {
            // Create a new dialog for ParcelTracking
            var formDialog = new FormDialog<ParcelTracking>(new ParcelTracking(), parcelTrackingForm, FormOptions.PromptInStart);

            // Kick off dialog
            context.Call(formDialog, ParcelTrackingCallback);
        }

        private async Task ParcelTrackingCallback(IDialogContext context, IAwaitable<object> result)
        {
            var parcelTracking = await result as ParcelTracking;

            // Check if tracking number has been entered successfully
            if (parcelTracking.TrackingNumber.Equals(Models.ParcelTracking.NoTrackingNumberCode))
            {
                // Tracking number not found
                await context.PostAsync($"Oh, you don't know your tracking number? Please take a look at your E-Mails!");
            }
            else
            {
                // Valid Tracking number found
                await context.PostAsync($"Alright, let's see what I can find for {parcelTracking.TrackingNumber}...");

                // Show typing as loading indicator
                await TypeAsync(context);

                // Check status at Parcel Tracking Backend
                var status = await parcelService.CheckTrackingNumberAsync(parcelTracking.TrackingNumber);

                // Create card as response
                var heroCard = new HeroCard(
                    "Package Status", // Title
                    parcelTracking.TrackingNumber, // Subtitle
                    status, // Text
                    new List<CardImage> { new CardImage("https://github.com/robinmanuelthiel/parcelbot/raw/master/assets/demomap.png") }, // Image
                    new List<CardAction> { new CardAction("openUrl", "Details", null, $"http://www.paketnavigator.de/redirect.aspx?action=1&parcelno={parcelTracking.TrackingNumber}&locale=de_DE") },
                    null);
                var response = context.MakeMessage();
                response.Attachments.Add(heroCard.ToAttachment());

                await context.PostAsync(response);
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Complaint")]
        public async Task Complaint(IDialogContext context, LuisResult result)
        {
            // Add Prompt Dialog to ask for details
            PromptDialog.Text(context, ComplaintCallback, "Oh, sorry to hear that. What happened?");
        }

        private async Task ComplaintCallback(IDialogContext context, IAwaitable<string> result)
        {
            // Show typing as loading indicator
            await TypeAsync(context);

            // Process complaint
            await parcelService.SendComplaintAsync(await result);
            await context.PostAsync("Sorry for that, I forwarded your compaint internally. Thanks for reporting!");

            context.Wait(MessageReceived);
        }

        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I am happy to help. Can I do anything else for you?");
            context.Wait(MessageReceived);
        }

        private async Task TypeAsync(IDialogContext context)
        {
            var typingMessage = context.MakeMessage();
            typingMessage.Type = ActivityTypes.Typing;
            await context.PostAsync(typingMessage);
        }
    }
}