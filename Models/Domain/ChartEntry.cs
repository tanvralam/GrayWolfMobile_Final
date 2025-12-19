using MvvmHelpers;
using System;

namespace GrayWolf.Models.Domain
{
    public class ChartEntry : ObservableObject
    {
        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get => _timeStamp;
            set => SetProperty(ref _timeStamp, value);
        }

        private double _value;
        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }
}
