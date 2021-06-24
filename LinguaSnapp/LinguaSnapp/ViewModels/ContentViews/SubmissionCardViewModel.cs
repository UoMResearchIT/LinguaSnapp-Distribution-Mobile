using LinguaSnapp.Models;
using DotNetExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using LinguaSnapp.Interfaces;
using LinguaSnapp.Enums;
using LinguaSnapp.ViewModels.Base;
using System.Threading.Tasks;
using System.Diagnostics;
using LinguaSnapp.Services;
using System.IO;

namespace LinguaSnapp.ViewModels.ContentViews
{
    class SubmissionCardViewModel : BaseViewModel, IComparable<SubmissionCardViewModel>
    {
        public string Title { get; }

        public ImageSource ThumbnailImageSource { get; private set; }

        public string Date { get; }

        public string Size { get; }

        public string Status { get; }

        public Color StatusColour { get; }

        public IconLabelButtonViewModel EditIconViewModel { get; }

        // Store ID of the model so we can retrieve it later if necessary
        public int SubId { get; }

        // Fields
        private const string dateFormat = "ddd d MMM yyyy, HH:mm";
        private readonly DateTime statusTimestamp;

        public SubmissionCardViewModel(SubmissionModel model)
        {
            SubId = model.ID;
            EditIconViewModel = new IconLabelButtonViewModel
            {
                LabelText = model.Status == SubmissionStatus.Draft ? (string)Application.Current.Resources["uploads_edit"] : (string)Application.Current.Resources["uploads_view"],
                ImageSource = model.Status == SubmissionStatus.Draft ? "ic_edit_primary" : "ic_show_primary",
                TappedCommand = new Command(async () => await (Application.Current as IShell)?.LoadEditorShellAsync(SubId)),
                IconSize = 56,
                LabelColour = (Color)Application.Current.Resources["PrimaryLightBackground"]
            };

            // Convert model to view model
            Status = model.Status.GetDescription();
            Title = model.Title;
            statusTimestamp = new DateTime(model.StatusDateTimestamp);
            if (model.Status == SubmissionStatus.Draft)
            {
                Date = $"Saved: ";
            }
            else if (model.Status == SubmissionStatus.Outbox)
            {
                Date = $"Completed: ";
            }
            else
            {
                Date = $"Uploaded: ";
            }
            Date += statusTimestamp.ToString(dateFormat);
            Size = model.Size;
            StatusColour = (Color)Application.Current.Resources["PrimaryLightBackground"];

            // Set preview image
            Task.Run(() => GeneratePreviewImage(model));
        }

        private void GeneratePreviewImage(SubmissionModel model)
        {
            ThumbnailImageSource = ImageSource.FromStream(() => new MemoryStream(model.GetPreviewImageBytes()));
        }

        public int CompareTo(SubmissionCardViewModel other)
        {
            return statusTimestamp.CompareTo(other.statusTimestamp);
        }
    }
}
