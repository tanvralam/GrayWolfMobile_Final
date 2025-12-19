using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Models.Domain
{
    public class AudioAttachment : Attachment
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
