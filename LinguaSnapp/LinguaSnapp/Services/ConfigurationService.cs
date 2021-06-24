using LinguaSnapp.Enums;
using LinguaSnapp.Models;
using Microsoft.AppCenter.Crashes;
using SqlBaseLibrary;
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

namespace LinguaSnapp.Services
{
    sealed class ConfigurationService
    {
        private static readonly Lazy<ConfigurationService> lazy = new Lazy<ConfigurationService>(() => new ConfigurationService());

        public static ConfigurationService Instance { get { return lazy.Value; } }

        public bool IsReady { get; private set; }

        // Hold private copies of the database values for speed and use DB for persistence
        private List<ToolTipConfigModel> toolTipValues;
        private List<DescriptorConfigModel> descriptorValues;
        private List<ServerEndpointConfigModel> serverEndpointValues;
        private LocalAppDatabase<ToolTipConfigModel> tooltipDatabase;
        private LocalAppDatabase<DescriptorConfigModel> descriptorDatabase;
        private LocalAppDatabase<ServerEndpointConfigModel> serverEndpointDatabase;

        private ConfigurationService()
        {
        }

        // Method to initialise the local configuration databases if the version number has changed since the last time it was initialised
        internal async Task InitialiseConfigurationDatabaseAsync()
        {
            // Set flag to false
            IsReady = false;

            // Create the databases if they don't exist
            if (tooltipDatabase == null)
            {
                tooltipDatabase = new LocalAppDatabase<ToolTipConfigModel>(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "tooltips.db3")
                    );
            }
            if (descriptorDatabase == null)
            {
                descriptorDatabase = new LocalAppDatabase<DescriptorConfigModel>(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "descriptors.db3")
                    );
            }
            if (serverEndpointDatabase == null)
            {
                serverEndpointDatabase = new LocalAppDatabase<ServerEndpointConfigModel>(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "serverEndpoints.db3")
                    );
            }

            // Load database content into memory for best performance
            toolTipValues = await tooltipDatabase.GetItemsAsync();
            descriptorValues = await descriptorDatabase.GetItemsAsync();
            serverEndpointValues = await serverEndpointDatabase.GetItemsAsync();

            // Check if we need to (re-)initialise or not
            string lastInitialisedVersion = Preferences.Get((string)Application.Current.Resources["key_config_database_version"], null);
            if (toolTipValues.Count == 0 || descriptorValues.Count == 0 || serverEndpointValues.Count == 0 
                || lastInitialisedVersion == null || VersionTracking.CurrentVersion.Trim() != lastInitialisedVersion.Trim())
            {
                // Clear existing lists
                toolTipValues = new List<ToolTipConfigModel>();
                descriptorValues = new List<DescriptorConfigModel>();
                serverEndpointValues = new List<ServerEndpointConfigModel>();

                // Flush the databases
                try
                {
                    await tooltipDatabase.DeleteAllAsync();
                    await descriptorDatabase.DeleteAllAsync();
                    await serverEndpointDatabase.DeleteAllAsync();
                }
                catch (Exception e)
                {
                    var msg = $"Issue flushing the databases! {e.Message}";
                    Debug.WriteLine(msg);
                    Crashes.TrackError(new Exception(msg, e));
                }

                // Load tooltip file
                var assembly = Assembly.GetAssembly(typeof(App));
                Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.tooltips.txt");
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        while (sr.Peek() >= 0)
                        {
                            // Get line chunks (tab-separated)
                            var line = sr.ReadLine().Split('\t');

                            // Only parse valid lines
                            if (line.Length == 3 && int.TryParse(line[0], out var temp))
                            {
                                try
                                {
                                    toolTipValues.Add(new ToolTipConfigModel(int.Parse(line[0].Trim()), line[1].Trim(), line[2].Trim()));
                                }
                                catch (Exception e)
                                {
                                    var msg = $"Issue parsing line from the tooltip config file! {e.Message}";
                                    Debug.WriteLine(msg);
                                    Crashes.TrackError(new Exception(msg, e));
                                }
                            }
                        }
                    }

                    // Write new list to database
                    await tooltipDatabase.AddMultipleItemsAsync(toolTipValues);
                }

                // Load picker source file
                stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.descriptorValues.txt");
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        while (sr.Peek() >= 0)
                        {
                            // Get line chunks (tab-separated)
                            var line = sr.ReadLine().Split('\t');

                            // Only parse valid lines
                            if (line.Length == 3 && int.TryParse(line[0], out var temp))
                            {
                                try
                                {
                                    descriptorValues.Add(new DescriptorConfigModel(int.Parse(line[0].Trim()), line[1].Trim(), line[2].Trim()));
                                }
                                catch (Exception e)
                                {
                                    var msg = $"Issue parsing line from the descriptor value config file! {e.Message}";
                                    Debug.WriteLine(msg);
                                    Crashes.TrackError(new Exception(msg, e));
                                }
                            }
                        }
                    }

                    // Write new list to database
                    await descriptorDatabase.AddMultipleItemsAsync(descriptorValues);
                }

                // Load endpoints file
                stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.endpoints.txt");
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        while (sr.Peek() >= 0)
                        {
                            // Get line chunks (tab-separated)
                            var line = sr.ReadLine().Split('\t');

                            // Only parse valid lines
                            if (line.Length == 2 && int.TryParse(line[0], out var temp))
                            {
                                try
                                {
                                    var type = (ServerEndpointType)Enum.Parse(typeof(ServerEndpointType), line[0].Trim());
                                    if (!Enum.IsDefined(typeof(ServerEndpointType), type)) throw new Exception("Failure parsing enum!");
                                    serverEndpointValues.Add(new ServerEndpointConfigModel(type, line[1].Trim()));
                                }
                                catch (Exception e)
                                {
                                    var msg = $"Issue parsing line from the endpoint config file! {e.Message}";
                                    Debug.WriteLine(msg);
                                    Crashes.TrackError(new Exception(msg, e));
                                }
                            }
                        }
                    }

                    // Write new list to database
                    await serverEndpointDatabase.AddMultipleItemsAsync(serverEndpointValues);
                }

                // Set version after initialisation
                Preferences.Set((string)Application.Current.Resources["key_config_database_version"], VersionTracking.CurrentVersion);
            }

            // Set ready flag
            IsReady = true;
        }

        // Gets the tooltip body from the type
        internal FormattedString GetTooltipBody(TooltipType type)
        {
            // Check if ready
            if (!IsReady) throw new Exception("Config Service accessed before it was ready!");

            // Get the list of items
            var items = toolTipValues.Where(v => v.DescriptorId == (int)type);

            // Format items
            var fString = new FormattedString();
            foreach (var item in items)
            {
                fString.Spans.Add(new Span
                {
                    Text = $"{item.Keyword}\n",
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Italic,
                    FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label))
                });
                fString.Spans.Add(new Span
                {
                    Text = $"{item.Text}\n",
                    TextColor = (Color)Application.Current.Resources["PrimaryLightBackground"],
                    FontSize = Device.GetNamedSize(NamedSize.Body, typeof(Label))
                });
            }
            return fString;
        }

        // Gets all descriptors matching the type
        internal IEnumerable<DescriptorConfigModel> GetDescriptors(DescriptorType type)
        {
            // Check if ready
            if (!IsReady) throw new Exception("Config Service accessed before it was ready!");
            return descriptorValues.Where(v => v.DescriptorType == (int)type);
        }

        // Gets a descriptor matching the name & type since name is not unique
        internal DescriptorConfigModel GetDescriptorFromName(string name, DescriptorType type)
        {
            // Check if ready
            if (!IsReady) throw new Exception("Config Service accessed before it was ready!");
            return descriptorValues.FirstOrDefault(v => v.Name.Trim() == name?.Trim() && v.DescriptorType == (int)type);
        }

        // Gets a descriptor matching the code
        internal DescriptorConfigModel GetDescriptorFromCode(string code)
        {
            // Check if ready
            if (!IsReady) throw new Exception("Config Service accessed before it was ready!");
            return descriptorValues.FirstOrDefault(v => v.Code.Trim() == code?.Trim());
        }

        // Gets an endpoint URL
        internal string GetURL(ServerEndpointType type)
        {
            // Check if ready
            if (!IsReady) throw new Exception("Config Service accessed before it was ready!");
            return serverEndpointValues.First(v => v.Type == type)?.URL;
        }

        // Gets the descriptor type code from the value code
        internal string GetDescriptorTypeCode(string code)
        {
            // Descriptor type code is always the first two characters of the code itself
            return code.Substring(0, 2);
        }
    }
}
