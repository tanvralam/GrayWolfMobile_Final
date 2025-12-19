using MvvmHelpers;

namespace GrayWolf.Models.Domain
{
    public class VOCItem : ObservableObject
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _altNames;
        public string AltNames
        {
            get => _altNames;
            set => SetProperty(ref _altNames, value);
        }

        private string _chemicalSymbol;
        public string ChemicalSymbol
        {
            get => _chemicalSymbol;
            set => SetProperty(ref _chemicalSymbol, value);
        }

        private double? _correctionFactor;
        public double? CorrectionFactor
        {
            get => _correctionFactor;
            set => SetProperty(ref _correctionFactor, value);
        }

        private double? _oshapel;
        public double? OSHApel
        {
            get => _oshapel;
            set => SetProperty(ref _oshapel, value);
        }

        private string _unitName = "ppm";
        public string UnitName
        {
            get => _unitName;
            set => SetProperty(ref _unitName, value);
        }

        public string OSHApelFormatted
        {
            get
            {
                if (OSHApel == null)
                    return "";
                return $"{OSHApel} {UnitName}";
            }
        }

        private string _sources;
        public string Sources
        {
            get => _sources;
            set => SetProperty(ref _sources, value);
        }
    }
}
