using MvvmHelpers;
using Newtonsoft.Json;
using System;

namespace GrayWolf.Models.Domain
{
    public class Event : ObservableObject
    {
        private string _dateTime;
        public string DateTime
        {
            get => _dateTime;
            set => SetProperty(ref _dateTime, value);
        }

        private string _label;
        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }
    }
}
