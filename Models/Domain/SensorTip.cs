using MvvmHelpers;

namespace GrayWolf.Models.Domain
{
    public class SensorTip : ObservableObject
    {
        private int _categoryId;
        public int CategoryId
        {
            get => _categoryId;
            set => SetProperty(ref _categoryId, value);
        }

        private string _categoryName;
        public string CategoryName
        {
            get => _categoryName;
            set => SetProperty(ref _categoryName, value);
        }
    }
}
