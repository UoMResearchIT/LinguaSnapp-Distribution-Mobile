using HtmlAgilityPack;
using MDS.Essentials.Shared.Pages;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LinguaSnapp.Views.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoContent : ContentView
    {
        public static BindableProperty ShowInfoButtonBarProperty = BindableProperty.Create(
            nameof(ShowInfoButtonBar),
            typeof(bool),
            typeof(InfoContent),
            defaultValue: false,
            propertyChanged: ShowHideInfoBar);

        public bool ShowInfoButtonBar
        {
            get => (bool)GetValue(ShowInfoButtonBarProperty);
            set => SetValue(ShowInfoButtonBarProperty, value);
        }

        private static void ShowHideInfoBar(BindableObject bindable, object oldValue, object newValue)
        {
            ((InfoContent)bindable).InfoBar.IsVisible = (bool)newValue;
        }


        public InfoContent(string docString)
        {
            InitializeComponent();
            ReadInSource(docString);
            InfoBar.IsVisible = ShowInfoButtonBar;
        }

        private void ReadInSource(string resource, Assembly assembly = null)
        {
            // Snippet from docs allow reading of an embedded resource
            if (assembly == null) assembly = Assembly.GetCallingAssembly();
            var resName = $"{assembly.GetName().Name}.{resource}";
            Debug.WriteLine($"Loading {resName}");
            Stream stream = assembly.GetManifestResourceStream(resName);

            if (stream != null)
            {
                // Read the html into memory
                string source;
                using (var reader = new StreamReader(stream))
                {
                    source = reader.ReadToEnd();
                }

                // Generate an HTML doc
                var doc = new HtmlDocument();
                doc.LoadHtml(source);

                // Pass HTML document to the label generator
                var stack = new HtmlLabelGenerator().ProcessDocument(doc, new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    PreferredControlColor = Color.White,
                    PreferredToolbarColor = (Color)Application.Current.Resources["Primary"]
                });

                // Assign stack to content
                InfoStack.Children.Insert(1, stack);
            }
            else
            {
                var msg = $"Failed to load {resource} from assembly {assembly}";
                InfoStack.Children.Add(new Label { Text = msg, HorizontalTextAlignment = TextAlignment.Center });
                Crashes.TrackError(new Exception(msg));
            }
        }
    }
}