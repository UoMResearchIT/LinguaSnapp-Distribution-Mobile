using LinguaSnapp.Enums;
using LinguaSnapp.Models;
using LinguaSnapp.Services;
using LinguaSnapp.ViewModels.Base;
using LinguaSnapp.ViewModels.ContentViews;
using LinguaSnapp.ViewModels.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels
{
    class ContextPageViewModel : EditorViewModel
    {
        public PickerViewViewModel PositionPickerViewModel { get; }

        public PickerViewViewModel SignTypePickerViewModel { get; }

        public PickerViewViewModel OutletPickerViewModel { get; }

        public EntryEditorWithIconViewModel OtherEntryViewModel { get; }

        public MultiPickerViewViewModel ContentMultiPickViewModel { get; }

        public MultiPickerViewViewModel DesignMultiPickViewModel { get; }

        public EntryEditorWithIconViewModel CommentsViewModel { get; }

        public ContextPageViewModel()
        {
            // Create the picker view models
            PositionPickerViewModel = new PickerViewViewModel()
            {
                Placeholder = (string)Application.Current.Resources["context_position"],
                ItemsSource = ConfigurationService.Instance.GetDescriptors(DescriptorType.Position).Select(i => i.Name).ToList(),
                TooltipIconVisible = false,
                IsReadOnly = IsReadOnly
            };
            SignTypePickerViewModel = new PickerViewViewModel()
            {
                Placeholder = (string)Application.Current.Resources["context_sign_type"],
                ItemsSource = ConfigurationService.Instance.GetDescriptors(DescriptorType.SignType).Select(i => i.Name).ToList(),
                TooltipIconVisible = false,
                IsReadOnly = IsReadOnly
            };
            OutletPickerViewModel = new PickerViewViewModel()
            {
                Placeholder = (string)Application.Current.Resources["context_outlet"],
                ItemsSource = ConfigurationService.Instance.GetDescriptors(DescriptorType.Outlet).Select(i => i.Name).ToList(),
                TooltipIconVisible = false,
                IsReadOnly = IsReadOnly
            };

            // Other entries
            OtherEntryViewModel = new EntryEditorWithIconViewModel()
            {
                Placeholder = (string)Application.Current.Resources["context_outlet_other"],
                IsReadOnly = IsReadOnly
            };
            CommentsViewModel = new EntryEditorWithIconViewModel()
            {
                Placeholder = (string)Application.Current.Resources["context_comments"],
                IsReadOnly = IsReadOnly
            };

            // Create the multi-pick view models
            ContentMultiPickViewModel = new MultiPickerViewViewModel(DescriptorType.Content)
            {
                Placeholder = (string)Application.Current.Resources["context_content"],
                TooltipIconVisible = false,
                IsReadOnly = IsReadOnly
            };
            DesignMultiPickViewModel = new MultiPickerViewViewModel(DescriptorType.Design)
            {
                Placeholder = (string)Application.Current.Resources["context_design"],
                TooltipIconVisible = false,
                IsReadOnly = IsReadOnly
            };

            // Load
            PopulateViewModelFromSubmission();
        }

        // Override load for this VM
        protected override void PopulateViewModelFromSubmission()
        {
            base.PopulateViewModelFromSubmission();

            // Set touched flag
            SubmissionService.Instance.SetTouchedFlag(EditorTabType.Context);

            // Configure properties
            CommentsViewModel.EntryText = SubmissionService.Instance.GetComments();

            // Load in the context components from the active submission and resolve to name via the config service
            var descriptors = SubmissionService.Instance.GetDescriptors();
            if (descriptors.Count() < 1) return;
            PositionPickerViewModel.SelectedItem = ConfigurationService.Instance.GetDescriptorFromCode(descriptors.FirstOrDefault(d => d.DescriptorType == DescriptorType.Position)?.Code)?.Name;
            SignTypePickerViewModel.SelectedItem = ConfigurationService.Instance.GetDescriptorFromCode(descriptors.FirstOrDefault(d => d.DescriptorType == DescriptorType.SignType)?.Code)?.Name;
            var outlet = descriptors.FirstOrDefault(d => d.DescriptorType == DescriptorType.Outlet);
            OutletPickerViewModel.SelectedItem = ConfigurationService.Instance.GetDescriptorFromCode(outlet?.Code)?.Name;
            OtherEntryViewModel.EntryText = outlet?.Value;
            ContentMultiPickViewModel.LoadFromModels(descriptors.Where(d => d.DescriptorType == DescriptorType.Content));
            DesignMultiPickViewModel.LoadFromModels(descriptors.Where(d => d.DescriptorType == DescriptorType.Design));
        }

        // Override update for this VM
        protected override void UpdateSubmissionFromViewModel()
        {
            base.UpdateSubmissionFromViewModel();

            // Check validation flags
            var valid = true;

            // Set flag
            SubmissionService.Instance.SetContextValidityFlag(valid);

            // Write back changes in fields
            if (!string.IsNullOrWhiteSpace(PositionPickerViewModel.SelectedItem))
            {
                SubmissionService.Instance.SetDescriptor(
                new DescriptorModel(
                    DescriptorType.Position,
                    ConfigurationService.Instance.GetDescriptorFromName(PositionPickerViewModel.SelectedItem?.Trim(), DescriptorType.Position)?.Code
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(SignTypePickerViewModel.SelectedItem))
            {
                SubmissionService.Instance.SetDescriptor(
                new DescriptorModel(
                    DescriptorType.SignType,
                    ConfigurationService.Instance.GetDescriptorFromName(SignTypePickerViewModel.SelectedItem?.Trim(), DescriptorType.SignType)?.Code
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(OutletPickerViewModel.SelectedItem))
            {
                SubmissionService.Instance.SetDescriptor(
                new DescriptorModel(
                    DescriptorType.Outlet,
                    ConfigurationService.Instance.GetDescriptorFromName(OutletPickerViewModel.SelectedItem?.Trim(), DescriptorType.Outlet)?.Code,
                    OtherEntryViewModel.EntryText?.Trim()
                    )
                );
            }
            SubmissionService.Instance.SetComments(CommentsViewModel.EntryText?.Trim());

            // Write back changes from the multi-pickers
            SubmissionService.Instance.SetDescriptors(
                DescriptorType.Content,
                ContentMultiPickViewModel.ConvertToModels()
            );
            SubmissionService.Instance.SetDescriptors(
                DescriptorType.Design,
                DesignMultiPickViewModel.ConvertToModels()
            );
        }
    }
}
