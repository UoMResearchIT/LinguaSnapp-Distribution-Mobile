using LinguaSnapp.Enums;
using LinguaSnapp.Models;
using LinguaSnapp.Services;
using LinguaSnapp.ViewModels.Base;
using LinguaSnapp.ViewModels.Popups;
using LinguaSnapp.Views.Popups;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LinguaSnapp.ViewModels.ContentViews
{
    class MultiPickerViewViewModel : ControlWithToolTipViewModel
    {
        private IEnumerable<MultiPickItemViewModel> selections = new List<MultiPickItemViewModel>();
        public IEnumerable<MultiPickItemViewModel> Selections { get => selections; set => SetProperty(ref selections, value); }

        private string text;
        public string Text
        {
            get => text;
            set
            {
                SetProperty(ref text, value);

                // Change state of clear button
                ClearButtonViewModel.IconVisible = !string.IsNullOrWhiteSpace(Text) && !IsReadOnly;
            }
        }

        public string Placeholder { get; set; }

        public ICommand OkTapped { get; }

        public DescriptorType DescriptorType { get; }

        public MultiPickerViewViewModel(DescriptorType type)
        {
            // Stash type
            DescriptorType = type;

            // Populate the available selections from the config service
            var list = new List<MultiPickItemViewModel>();
            foreach (var i in ConfigurationService.Instance.GetDescriptors(type)) list.Add(new MultiPickItemViewModel(i.Code, i.Name));
            Selections = list;

            // Standard command for all
            OkTapped = new Command(async () =>
            {
                // Update the picker text
                UpdateControlText();

                // Dismiss the popup
                await Shell.Current.Navigation.PopAllPopupAsync();
            });
        }

        private void UpdateControlText()
        {
            var selectedItems = Selections.Where(i => i.ItemChecked);
            Text = selectedItems.Count() > 0 ? string.Join("\n", selectedItems.Select(i => i.ItemLabel)) : null;
        }

        internal void LoadFromModels(IEnumerable<DescriptorModel> models)
        {
            // Load selections based on provided models
            foreach (var m in models)
            {
                var match = Selections.FirstOrDefault(i => i.Code == m.Code);
                if (match == null)
                {
                    Debug.WriteLine($"Could not find a match for {m.Code} in the available list!");
                }
                else
                {
                    match.ItemChecked = true;
                }
            }

            // Update the text on the control
            UpdateControlText();
        }

        internal IEnumerable<DescriptorModel> ConvertToModels()
        {
            // Convert the selections back to a list of models
            return Selections.Where(i => i.ItemChecked)
                .Select(i => new DescriptorModel(DescriptorType, i.Code));
        }

        internal async Task ShowMultiPickPopupAsync()
        {
            await Shell.Current.Navigation.PushPopupAsync(new MultiPickPopup(this));
        }

        protected override void ClearTapped()
        {
            foreach (var sel in Selections) sel.ItemChecked = false;
            Text = null;
        }
    }
}
