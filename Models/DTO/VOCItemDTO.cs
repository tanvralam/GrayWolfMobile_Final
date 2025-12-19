using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Models.DTO
{
    public class VOCItemDTO
    {
        public string Name { get; set; }

        public string AltNames { get; set; }

        public string ChemicalSymbol { get; set; }

        public double CorrectionFactor { get; set; }

        public double OSHApel { get; set; }

        public string Sources { get; set; }
    }
}
