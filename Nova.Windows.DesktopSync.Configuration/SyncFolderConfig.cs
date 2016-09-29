using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Nova.Windows.DesktopSync.Configuration
{
    [Serializable]
    public class SyncFolderConfig : INotifyPropertyChanged
    {
        private string _fixFileExtension;
        private string _folderPath;
        private int _maximumKb;
        private int _minimumKb;
        private string _name;
        private SearchOption _searchOption = SearchOption.AllDirectories;
        private string _searchPattern = "*.jpg;*.png;*.";
        private string _subFolder;

        public string Name
        {
            get { return _name; }
            set
            {
                if (Equals(_name, value))
                    return;

                _name = value;
                OnPropertyChanged();
            }
        }

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                if (Equals(_folderPath, value))
                    return;

                _folderPath = value;
                OnPropertyChanged();
            }
        }

        public string FixFileExtension
        {
            get { return _fixFileExtension; }
            set
            {
                if (Equals(_fixFileExtension, value))
                    return;

                _fixFileExtension = value;
                OnPropertyChanged();
            }
        }

        public int MinimumKb
        {
            get { return _minimumKb; }
            set
            {
                if (Equals(_minimumKb, value))
                    return;

                _minimumKb = value;
                OnPropertyChanged();
            }
        }

        public int MaximumKb
        {
            get { return _maximumKb; }
            set
            {
                if (Equals(_maximumKb, value))
                    return;

                _maximumKb = value;
                OnPropertyChanged();
            }
        }

        public string SubFolder
        {
            get { return _subFolder; }
            set
            {
                if (Equals(_subFolder, value))
                    return;

                _subFolder = value;
                OnPropertyChanged();
            }
        }

        public string SearchPattern
        {
            get { return _searchPattern; }
            set
            {
                if (Equals(_searchPattern, value))
                    return;

                _searchPattern = value;
                OnPropertyChanged();
            }
        }

        public SearchOption SearchOption
        {
            get { return _searchOption; }
            set
            {
                if (Equals(_searchOption, value))
                    return;

                _searchOption = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static string BuildName(string folder)
        {
            string name;
            if (folder.Length > 40)
            {
                var pathParts = folder.Split(Path.PathSeparator);
                var newParts = pathParts.Take(3).Union(new[]
                {
                    "...",
                    pathParts.Last()
                });
                name = string.Join(Path.PathSeparator.ToString(), newParts);
            }
            else
            {
                name = folder;
            }
            return name;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}