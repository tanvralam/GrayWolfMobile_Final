using MvvmHelpers;
using System.Collections.Generic;

namespace GrayWolf.Models.Domain
{
    public class DataGridSet : ObservableObject
    {
        private int _setId;
        public int SetId
        {
            get => _setId;
            set => SetProperty(ref _setId, value);
        }

        private List<string> _columns;
        public List<string> Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        private List<string> _lines;
        public List<string> Lines
        {
            get => _lines;
            set => SetProperty(ref _lines, value);
        }
    }
}
