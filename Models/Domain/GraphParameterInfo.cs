using MvvmHelpers;


namespace GrayWolf.Models.Domain
{
    public class GraphParameterInfo : ObservableObject
    {
        private int _columnIndex;
        public int ColumnIndex
        {
            get => _columnIndex;
            set => SetProperty(ref _columnIndex, value);
        }

        private int _setId;
        public int SetId
        {
            get => _setId;
            set => SetProperty(ref _setId, value);
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
    }
}
