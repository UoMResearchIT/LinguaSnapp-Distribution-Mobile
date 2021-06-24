using LinguaSnapp.Enums;
using LinguaSnapp.Models;
using LinguaSnapp.Services;
using LinguaSnapp.ViewModels.Base;
using LinguaSnapp.ViewModels.ContentViews;
using LinguaSnapp.ViewModels.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels
{
    class AnalysisPageViewModel : EditorViewModel
    {
        public PickerViewViewModel SectorPickerViewModel { get; }

        public PickerViewViewModel AudiencePickerViewModel { get; }

        public MultiPickerViewViewModel PurposePickerViewModel { get; }

        public MultiPickerViewViewModel ArrangementPickerViewModel { get; }

        public MultiPickerViewViewModel FunctionPickerViewModel { get; }

        public MultiPickerViewViewModel DominancePickerViewModel { get; }

        private bool oneLanguageDominant;
        public bool OneLanguageDominant { get => oneLanguageDominant; set => SetProperty(ref oneLanguageDominant, value); }

        public ICommand DominanceLabelTapped { get; }

        public AnalysisPageViewModel()
        {
            // Create VMs, populating sources from the config service
            SectorPickerViewModel = new PickerViewViewModel()
            {
                Placeholder = (string)Application.Current.Resources["analysis_sector"],
                TooltipPopupTitle = (string)Application.Current.Resources["analysis_sector"],
                TooltipPopupBody = ConfigurationService.Instance.GetTooltipBody(TooltipType.Sector),
                ItemsSource = ConfigurationService.Instance.GetDescriptors(DescriptorType.Sector).Select(i => i.Name).ToList(),
                IsReadOnly = IsReadOnly
            };
            AudiencePickerViewModel = new PickerViewViewModel()
            {
                Placeholder = (string)Application.Current.Resources["analysis_audience"],
                TooltipPopupTitle = (string)Application.Current.Resources["analysis_audience"],
                TooltipPopupBody = ConfigurationService.Instance.GetTooltipBody(TooltipType.Audience),
                ItemsSource = ConfigurationService.Instance.GetDescriptors(DescriptorType.Audience).Select(i => i.Name).ToList(),
                IsReadOnly = IsReadOnly
            };
            PurposePickerViewModel = new MultiPickerViewViewModel(DescriptorType.Purpose)
            {
                Placeholder = (string)Application.Current.Resources["analysis_purpose"],
                TooltipPopupTitle = (string)Application.Current.Resources["analysis_purpose"],
                TooltipPopupBody = ConfigurationService.Instance.GetTooltipBody(TooltipType.Purpose),
                IsReadOnly = IsReadOnly
            };
            ArrangementPickerViewModel = new MultiPickerViewViewModel(DescriptorType.Arrangement)
            {
                Placeholder = (string)Application.Current.Resources["analysis_arrangement"],
                TooltipPopupTitle = (string)Application.Current.Resources["analysis_arrangement"],
                TooltipPopupBody = ConfigurationService.Instance.GetTooltipBody(TooltipType.Arrangement),
                IsReadOnly = IsReadOnly
            };
            FunctionPickerViewModel = new MultiPickerViewViewModel(DescriptorType.Function)
            {
                Placeholder = (string)Application.Current.Resources["analysis_function"],
                TooltipPopupTitle = (string)Application.Current.Resources["analysis_function"],
                TooltipPopupBody = ConfigurationService.Instance.GetTooltipBody(TooltipType.Function),
                IsReadOnly = IsReadOnly
            };
            DominancePickerViewModel = new MultiPickerViewViewModel(DescriptorType.Dominance)
            {
                Placeholder = (string)Application.Current.Resources["analysis_dominance"],
                TooltipIconVisible = false,
                IsReadOnly = IsReadOnly
            };
            DominanceLabelTapped = new Command(() => OneLanguageDominant = !OneLanguageDominant);

            // Load
            PopulateViewModelFromSubmission();
        }

        protected override void PopulateViewModelFromSubmission()
        {
            base.PopulateViewModelFromSubmission();

            // Set touched flag
            SubmissionService.Instance.SetTouchedFlag(EditorTabType.Analysis);

            // Configure properties
            OneLanguageDominant = SubmissionService.Instance.GetOneLanguageDominant();

            // Configure descriptors
            var descriptors = SubmissionService.Instance.GetDescriptors();
            if (descriptors.Count() < 1) return;
            SectorPickerViewModel.SelectedItem = ConfigurationService.Instance.GetDescriptorFromCode(descriptors.FirstOrDefault(d => d.DescriptorType == DescriptorType.Sector)?.Code)?.Name;
            AudiencePickerViewModel.SelectedItem = ConfigurationService.Instance.GetDescriptorFromCode(descriptors.FirstOrDefault(d => d.DescriptorType == DescriptorType.Audience)?.Code)?.Name;
            PurposePickerViewModel.LoadFromModels(descriptors.Where(d => d.DescriptorType == DescriptorType.Purpose));
            ArrangementPickerViewModel.LoadFromModels(descriptors.Where(d => d.DescriptorType == DescriptorType.Arrangement));
            FunctionPickerViewModel.LoadFromModels(descriptors.Where(d => d.DescriptorType == DescriptorType.Function));
            DominancePickerViewModel.LoadFromModels(descriptors.Where(d => d.DescriptorType == DescriptorType.Dominance));
        }

        protected override void UpdateSubmissionFromViewModel()
        {
            base.UpdateSubmissionFromViewModel();

            // Check validation flags
            var valid = true;

            // Set flag
            SubmissionService.Instance.SetAnalysisValidityFlag(valid);

            // Write back changes in fields
            if (!string.IsNullOrWhiteSpace(SectorPickerViewModel.SelectedItem))
            {
                SubmissionService.Instance.SetDescriptor(
                new DescriptorModel(
                    DescriptorType.Sector,
                    ConfigurationService.Instance.GetDescriptorFromName(SectorPickerViewModel.SelectedItem?.Trim(), DescriptorType.Sector)?.Code
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(AudiencePickerViewModel.SelectedItem))
            {
                SubmissionService.Instance.SetDescriptor(
                new DescriptorModel(
                    DescriptorType.Audience,
                    ConfigurationService.Instance.GetDescriptorFromName(AudiencePickerViewModel.SelectedItem?.Trim(), DescriptorType.Audience)?.Code
                    )
                );
            }
            SubmissionService.Instance.SetOneLanguageDominant(OneLanguageDominant);

            // Write back changes from the multi-pickers
            SubmissionService.Instance.SetDescriptors(
                DescriptorType.Purpose,
                PurposePickerViewModel.ConvertToModels()
            );
            SubmissionService.Instance.SetDescriptors(
                DescriptorType.Arrangement,
                ArrangementPickerViewModel.ConvertToModels()
            );
            SubmissionService.Instance.SetDescriptors(
                DescriptorType.Function,
                FunctionPickerViewModel.ConvertToModels()
            );
            SubmissionService.Instance.SetDescriptors(
                DescriptorType.Dominance,
                DominancePickerViewModel.ConvertToModels()
            );
        }
    }
}
