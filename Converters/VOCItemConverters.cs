using GrayWolf.Models.Domain;
using GrayWolf.Models.DTO;

namespace GrayWolf.Converters
{
    public static class VOCItemsConverters
    {
        public static VOCItem ToDomain(this VOCItemDTO dto)
        {
            return new VOCItem
            {
                Name = dto.Name,
                AltNames = dto.AltNames,
                ChemicalSymbol = dto.ChemicalSymbol,
                CorrectionFactor = double.IsNaN(dto.CorrectionFactor) ? null : dto.CorrectionFactor,
                OSHApel = double.IsNaN(dto.OSHApel) ? null : dto.OSHApel,
                Sources = dto.Sources
            };
        }
    }
}
