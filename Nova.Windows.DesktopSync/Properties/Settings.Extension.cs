using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using Nova.Windows.DesktopSync.Configuration;

namespace Nova.Windows.DesktopSync.Properties
{
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class Settings
    {
        private const string ContentFolder = "Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets";

        public Settings()
        {
            SettingChanging += SettingChangingEventHandler;
            PropertyChanged += PropertyChangedEventHandler;
            SettingsLoaded += SettingsLoadedEventHandler;
            SettingsSaving += SettingsSavingEventHandler;

            if (SyncConfig != null) SyncConfig.PropertyChanged += PropertyChangedEventHandler;
        }

        private static SyncFolderConfig[] DefaultSyncFolders { get; } = {
            GetContentSyncFolder(Environment.SpecialFolder.ApplicationData),
            GetContentSyncFolder(Environment.SpecialFolder.LocalApplicationData)
        };

        public bool HasChanges { get; private set; }

        private void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            Console.Out.WriteProperty($"Property Changed", e.PropertyName);
            HasChanges = true;
        }

        private static SyncFolderConfig GetContentSyncFolder(Environment.SpecialFolder specialFolder)
        {
            return new SyncFolderConfig()
            {
                FolderPath = Path.Combine(Environment.GetFolderPath(specialFolder), ContentFolder),
                Name = specialFolder.ToString(),
                MinimumKb = 150,
                FixFileExtension = "jpg"
            };
        }

        private void SettingsLoadedEventHandler(object sender, SettingsLoadedEventArgs e)
        {
            ConsoleColor.TextStatus.WriteLine("Settings Loaded.");
            HasChanges = false;
        }

        private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
        {
            Console.Out.WriteProperty($"Setting Changing ({e.SettingName})", e.NewValue);
            HasChanges = true;
        }

        private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
        {
            ConsoleColor.TextStatus.WriteLine("Saving Settings...");
        }

        public static SyncConfig CreateDefaultSyncConfig()
        {
            var target = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); 
            if (!Directory.Exists(target))
                target = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);

            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);
            return new SyncConfig
            {
                TargetFolder = Path.Combine(target, "DesktopSync"),
                SyncFolders = new ObservableCollection<SyncFolderConfig>(DefaultSyncFolders)
            };
        }
    }
}