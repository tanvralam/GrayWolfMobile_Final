using GrayWolf.Interfaces;
using MvvmHelpers;

namespace GrayWolf.Models.Domain
{
    public class Attachment : ObservableObject, IAttachment
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private int _loggerId;
        public int LoggerId
        {
            get => _loggerId;
            set => SetProperty(ref _loggerId, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _path;
        public string Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        private string _caption;
        public string Caption
        {
            get => _caption;
            set => SetProperty(ref _caption, value);
        }

        private string _captionPath;
        public string CaptionPath
        {
            get => _captionPath;
            set => SetProperty(ref _captionPath, value);
        }

        private byte[] _binaryContent;
        public byte[] BinaryContent
        {
            get => _binaryContent;
            set => SetProperty(ref _binaryContent, value);
        }
    }
}
