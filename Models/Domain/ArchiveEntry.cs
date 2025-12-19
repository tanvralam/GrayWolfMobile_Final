using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace GrayWolf.Models.Domain
{
    public class ArchiveEntry : ObservableObject
    {
        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        private string _captionPath;
        public string CaptionPath
        {
            get => _captionPath;
            set => SetProperty(ref _captionPath, value);
        }

        public string FileName => Path.GetFileName(FilePath);

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            if(propertyName == nameof(FilePath))
            {
                OnPropertyChanged(FileName);
            }
        }
    }
}
