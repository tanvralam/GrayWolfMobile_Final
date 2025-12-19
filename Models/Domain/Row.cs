using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Models.Domain
{
    public class Row : ObservableObject
    {
        private int _a;
        public int A
        {
            get => _a;
            set => SetProperty(ref _a, value);
        }

        private double _b;
        public double B
        {
            get => _b;
            set => SetProperty(ref _b, value);
        }

        private double _c;
        public double C
        {
            get => _c;
            set => SetProperty(ref _c, value);
        }

        private double _d;
        public double D
        {
            get => _d;
            set => SetProperty(ref _d, value);
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }
    }
}
