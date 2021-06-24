using ByteSizeLib;
using LinguaSnapp.Enums;
using LinguaSnapp.Interfaces;
using Newtonsoft.Json;
using SkiaSharp;
using SqlBaseLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LinguaSnapp.Models
{
    [SQLite.Table("table_submission")]
    public class SubmissionModel : DatabaseItem
    {
        public string GUID { get; set; }

        public string DateCreated { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Title { get; set; }

        public string NumLanguages { get; set; }

        public string NumAlphabets { get; set; }

        public string Translations { get; set; }

        public SubmissionStatus Status { get; set; }

        public long StatusDateTimestamp { get; set; }

        public string EncodedPhoto { get; set; }

        public string EncodedThumbnail { get; set; }

        public string Size { get; set; }

        public string Descriptors { get; set; }

        public string Comments { get; set; }

        public bool OneLangDominant { get; set; }

        // Validity flags by section
        public bool LanguageValid { get; set; }

        public bool ContextValid { get; set; }

        public bool AnalysisValid { get; set; }

        // Touched flags by section
        public bool TouchedLanguage { get; set; }

        public bool TouchedContext { get; set; }

        public bool TouchedAnalysis { get; set; }

        // Sets encoded photo -- updates size at the same time
        public void SetPhoto(byte[] photoBytes)
        {
            // Downsample and compress
            var maxDimPx = (int)(256 * App.DeviceDensity);
            using (var img = SKImage.FromEncodedData(photoBytes))
            {
                // Compress the raw image to ensure message size stays small
                var imgData = img.Encode(SKEncodedImageFormat.Jpeg, 75);
                EncodedPhoto = Convert.ToBase64String(imgData.ToArray());

                // Resize for thumbnail
                if (maxDimPx < Math.Max(img.Width, img.Height))
                {
                    // Get new output image size
                    var aspect = img.Width / (float)img.Height;
                    int width, height;
                    if (aspect > 1)
                    {
                        width = maxDimPx;
                        height = (int)(width / aspect);
                    }
                    else
                    {
                        height = maxDimPx;
                        width = (int)(height * aspect);
                    }
                    Debug.WriteLine($"Original Image {img.Width} x {img.Height}");
                    Debug.WriteLine($"Thumbnail {width} x {height}");

                    // Create output bitmap
                    var info = new SKImageInfo(width, height, SKColorType.Bgra8888);
                    using (var output = SKImage.Create(info))
                    {
                        // Rescale pixels
                        var res = img.ScalePixels(output.PeekPixels(), SKFilterQuality.Low);

                        // Re-encode thumbnail - use lossless PNG
                        EncodedThumbnail = Convert.ToBase64String(output.Encode().ToArray());
                    }
                }
                else
                {
                    // No need to downsample
                    EncodedThumbnail = EncodedPhoto;
                }
            };

            // Get size of the photo in bytes (approx)
            var approxBytes = (long)(EncodedPhoto.Length * 1.37);
            Size = ByteSize.FromBytes(approxBytes).ToString();
            Debug.WriteLine($"Original = {Size}");
            approxBytes = (long)(EncodedThumbnail.Length * 1.37);
            Debug.WriteLine($"Thumbnail = {ByteSize.FromBytes(approxBytes)}");

            // Set status to draft
            SetStatus(SubmissionStatus.Draft);
        }

        // Set Status -- sets the status date at the same time
        public void SetStatus(SubmissionStatus status, DateTime? timestamp = null)
        {
            Status = status;
            StatusDateTimestamp = timestamp?.Ticks ?? DateTime.Now.Ticks;
        }

        // Check validation flags for this submission
        public bool IsValidSubmission()
        {
            return 
                LanguageValid &&
                (!TouchedContext || ContextValid) && 
                (!TouchedAnalysis || AnalysisValid) &&
                ((Latitude != 999 && Longitude != 999) || !string.IsNullOrWhiteSpace(Comments));
        }

        // Method to generate an ImageSource from the encoded photo
        internal byte[] GetPreviewImageBytes()
        {
            return Convert.FromBase64String(EncodedThumbnail);
        }
    }
}
