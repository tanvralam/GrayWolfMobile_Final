using GrayWolf.Enums;
using MvvmHelpers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace GrayWolf.Models.Domain
{
    public class GraphParameter : ObservableObject
    {
        public string Id => $"{(int)Sensor}:{(int)Unit}";

        private SensorType _sensor;
        public SensorType Sensor
        {
            get => _sensor;
            set => SetProperty(ref _sensor, value);
        }

        private string _sensorName;
        public string SensorName
        {
            get => _sensorName;
            set => SetProperty(ref _sensorName, value);
        }

        private Enums.SensorUnit _unit;
        public Enums.SensorUnit Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }

        private string _unitName;
        public string UnitName
        {
            get => _unitName;
            set => SetProperty(ref _unitName, value);
        }

        private List<GraphParameterInfo> _infoList; // SetId, column index, serial number
        public List<GraphParameterInfo> InfoList
        {
            get => _infoList;
            set => SetProperty(ref _infoList, value);
        }

        public string DisplayName => $"{SensorName} {UnitName}";

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(SensorName):
                case nameof(UnitName):
                    OnPropertyChanged(nameof(DisplayName));
                    break;
            }
        }
    }
}
