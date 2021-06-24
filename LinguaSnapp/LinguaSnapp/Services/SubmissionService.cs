using LinguaSnapp.Enums;
using LinguaSnapp.Models;
using LinguaSnapp.Models.Base;
using LinguaSnapp.ViewModels.ContentViews;
using SqlBaseLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LinguaSnapp.Services
{
    sealed class SubmissionService
    {
        private static readonly Lazy<SubmissionService> lazy = new Lazy<SubmissionService>(() => new SubmissionService());

        internal static SubmissionService Instance { get { return lazy.Value; } }

        internal ImageSource PreviewImage { get; private set; }

        // Fields controlled by the service instance
        private SubmissionModel activeSubmission;
        private LocalAppDatabase<SubmissionModel> submissionDatabase;
        private byte[] imgBytes;

        private SubmissionService()
        {
            // Create the local database on construction if necessary
            if (submissionDatabase == null)
            {
                submissionDatabase = new LocalAppDatabase<SubmissionModel>(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "submissions.db3")
                    );
            }
        }

        // Method to convert a list of models to a grouped list of view models
        internal async Task<List<GroupOfSubmissionCardViewModels>> GetSubmissionsAsGroupsOfViewModelsAsync()
        {
            // Get list of models
            var ungroupedModels = await submissionDatabase.GetItemsAsync();
            var groupedModels = ungroupedModels.GroupBy(s => s.Status);

            // Assembled groups
            var groupsOfViewModels = new List<GroupOfSubmissionCardViewModels>();
            foreach (var grp in groupedModels)
            {
                var group = new GroupOfSubmissionCardViewModels(grp.First().Status);
                foreach (var sub in grp) group.Add(new SubmissionCardViewModel(sub));
                group.Sort();
                group.Reverse();
                groupsOfViewModels.Add(group);
            }
            groupsOfViewModels.Sort();
            return groupsOfViewModels;
        }

        // Create a submission model and add it straight to the DB
        internal async Task<int> CreateNewSubmissionAsync(byte[] photoBytes, double latitude, double longitude)
        {
            // Create submission
            var sub = new SubmissionModel()
            {
                GUID = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow.ToString("O"),
                Latitude = latitude,
                Longitude = longitude
            };
            sub.SetPhoto(photoBytes);

            // Add to DB
            await submissionDatabase.AddItemAsync(sub);

            // Return the submission model PK
            return sub.ID;
        }

        // Method to starting editing a certain submission
        internal async Task<bool> StartEditingAsync(int? pk, bool streamImage = true)
        {
            // Cannot load null
            if (pk == null) return false;

            // Get the submission
            activeSubmission = await submissionDatabase.GetItemAsync(pk ?? 0);

            // Set the preview image
            if (streamImage && activeSubmission.EncodedThumbnail != null)
            {
                await Task.Run(() =>
                {
                    imgBytes = activeSubmission?.GetPreviewImageBytes();
                    PreviewImage = ImageSource.FromStream(() => new MemoryStream(imgBytes));
                });
            }

            // Check that we started editing properly
            return activeSubmission != null;
        }

        // Stop editing and optionally update the DB
        internal async Task StopEditingAsync(SubmissionStatus? newStatus = null, bool delete = false)
        {
            // Delete submission
            if (delete)
            {
                Debug.WriteLine($"Deleting submission {activeSubmission.ID}");
                await submissionDatabase.DeleteItemAsync(activeSubmission);
            }

            // Set status
            else if (newStatus != null)
            {
                // Update the status
                activeSubmission.SetStatus(newStatus ?? SubmissionStatus.Draft);

                // Update the database
                Debug.WriteLine($"Saving submission {activeSubmission.ID}");
                await submissionDatabase.UpdateItemAsync(activeSubmission);
            }

            // Reset the active submission
            activeSubmission = null;
        }

        // Sets whether the user has viewed the editor tab at any point
        internal void SetTouchedFlag(EditorTabType tab)
        {
            switch (tab)
            {
                case EditorTabType.Language:
                    activeSubmission.TouchedLanguage = true;
                    break;

                case EditorTabType.Context:
                    activeSubmission.TouchedContext = true;
                    break;

                case EditorTabType.Analysis:
                    activeSubmission.TouchedAnalysis = true;
                    break;
            }
        }

        // Gets whether a submission has had all sections touched
        internal bool IsTouchedSubmission()
        {
            return activeSubmission.TouchedLanguage && activeSubmission.TouchedContext && activeSubmission.TouchedAnalysis;
        }

        internal void SetSubmissionStatus(SubmissionStatus status)
        {
            activeSubmission.SetStatus(status);
        }

        internal bool IsValidSubmission()
        {
            return activeSubmission?.IsValidSubmission() ?? false;
        }

        internal SubmissionStatus GetSubmissionStatus()
        {
            return activeSubmission?.Status ?? SubmissionStatus.Unknown;
        }

        // Method to encode any encodable string type
        private string Encode(IEnumerable<StringEncodableModel> models)
        {
            return models.Count() == 0 ? null : string.Join("~", models.Select(m => m.Encode()));
        }

        // Method to decide any encodable string type
        private IEnumerable<string> Decode(string encodedModels)
        {
            return encodedModels?.Split('~') ?? new string[0];
        }

        // Getter for active submission ID
        internal int? GetSubmissionId()
        {
            return activeSubmission?.ID;
        }

        // Getter for meta components
        internal SubmissionMetaModel GetSubmissionMeta()
        {
            return new SubmissionMetaModel(
                activeSubmission.GUID,
                activeSubmission.DateCreated,
                activeSubmission.EncodedPhoto,
                activeSubmission.EncodedThumbnail,
                activeSubmission.Latitude,
                activeSubmission.Longitude,
                activeSubmission.Title,
                activeSubmission.NumLanguages,
                activeSubmission.NumAlphabets
                );
        }

        // Setter for single descriptor
        internal void SetDescriptor(DescriptorModel model)
        {
            SetDescriptors(model.DescriptorType, new List<DescriptorModel> { model });
        }

        // Setter for descriptors
        internal void SetDescriptors(DescriptorType type, IEnumerable<DescriptorModel> models)
        {
            // Get all descriptors then swap models matching type and re-encode
            var allModels = GetDescriptors().ToList();
            allModels.RemoveAll(m => m.DescriptorType == type);
            allModels.AddRange(models);
            activeSubmission.Descriptors = Encode(allModels);
        }

        // Getter for descriptors based on type if not null
        internal IEnumerable<DescriptorModel> GetDescriptors(DescriptorType? type = null)
        {
            // Convert the descriptors to models and filter based on descriptor type
            var models = Decode(activeSubmission.Descriptors)
                .Select(m =>
                {
                    var model = new DescriptorModel();
                    model.Decode(m);
                    return model;
                });
            if (type != null) models = models.Where(m => m.DescriptorType == type);
            return models;
        }

        // Setter for language components
        internal void SetLanguageMetaData(string title, string numLang, string numAlpha)
        {
            activeSubmission.Title = title;
            activeSubmission.NumLanguages = numLang;
            activeSubmission.NumAlphabets = numAlpha;
        }

        // Add a single translation
        internal void AddTranslation(TranslationModel model)
        {
            AddTranslations(new List<TranslationModel> { model });
        }

        // Add translations
        internal void AddTranslations(IEnumerable<TranslationModel> translationModels)
        {
            // Get all descriptors, remove matching translations, add new ones
            var allModels = GetTranslations().ToList();
            var matches = allModels.Intersect(translationModels);
            foreach (var m in matches) allModels.Remove(m);
            allModels.AddRange(translationModels);
            activeSubmission.Translations = Encode(allModels);
        }

        // Get translation models
        internal IEnumerable<TranslationModel> GetTranslations()
        {
            return Decode(activeSubmission.Translations).Select(m =>
            {
                var model = new TranslationModel();
                model.Decode(m);
                return model;
            });
        }

        // Remove a translation by ID
        internal void RemoveTranslation(string id)
        {
            var allModels = GetTranslations().ToList();
            allModels.RemoveAll(m => m.TranslationId == id);
            activeSubmission.Translations = Encode(allModels);
        }

        // Setter for comments
        internal void SetComments(string comments)
        {
            activeSubmission.Comments = comments;
        }

        // Get comments
        internal string GetComments()
        {
            return activeSubmission.Comments;
        }

        // Setter for dominance flag
        internal void SetOneLanguageDominant(bool value)
        {
            activeSubmission.OneLangDominant = value;
        }

        // Get comments
        internal bool GetOneLanguageDominant()
        {
            return activeSubmission.OneLangDominant;
        }

        // Setter validity flag for language section
        internal void SetLanguageValidityFlag(bool value)
        {
            activeSubmission.LanguageValid = value;
        }

        // Setter validity flag for context section
        internal void SetContextValidityFlag(bool value)
        {
            activeSubmission.ContextValid = value;
        }

        // Setter validity flag for analysis section
        internal void SetAnalysisValidityFlag(bool value)
        {
            activeSubmission.AnalysisValid = value;
        }
    }
}
