using System.Collections.ObjectModel;

namespace GrayWolf.Models.Domain
{
    public class EventAttachment : Attachment
    {
        private bool _isCreated;
        public bool IsCreated
        {
            get => _isCreated;
            set => SetProperty(ref _isCreated, value);
        }

        private ObservableCollection<Event> _events;
        public ObservableCollection<Event> Events
        {
            get => _events;
            set => SetProperty(ref _events, value);
        }
    }
}
