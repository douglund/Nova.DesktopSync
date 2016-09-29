using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nova.Windows.DesktopSync.Configuration
{
    [Serializable]
    public class SyncConfig : INotifyPropertyChanged
    {
        private bool _overwrite;
        private string _targetFolder;
        private ObservableCollection<SyncFolderConfig> _syncFolders = new ObservableCollection<SyncFolderConfig>();

        public string TargetFolder
        {
            get { return _targetFolder; }
            set
            {
                if (Equals(_targetFolder, value))
                    return;

                _targetFolder = value;
                OnPropertyChanged();
            }
        }

        public bool Overwrite
        {
            get { return _overwrite; }
            set
            {
                if (Equals(_overwrite, value))
                    return;

                _overwrite = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SyncFolderConfig> SyncFolders
        {
            get { return _syncFolders; }
            set
            {
                if (Equals(_syncFolders, value))
                    return;

                _syncFolders = value;
                if (_syncFolders != null)
                {
                    _syncFolders.CollectionChanged += OnCollectionChanged;
                    _syncFolders.CollectionChanged += (o, e) => OnPropertyChanged();

                    foreach (var folderConfig in _syncFolders)
                    {
                        folderConfig.PropertyChanged += (o, e) => OnPropertyChanged();
                    }
                }

                OnPropertyChanged();
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (SyncFolderConfig item in e.OldItems)
                    item.PropertyChanged -= (o, ea) => OnPropertyChanged(nameof(SyncFolders));

            else if (e.Action == NotifyCollectionChangedAction.Add)
                foreach (SyncFolderConfig item in e.NewItems)
                    item.PropertyChanged += (o, ea) => OnPropertyChanged(nameof(SyncFolders));
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}