using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Models.Domain
{
    public class TextAttachment : Attachment
    {
        private string _textContent;
        public string TextContent
        {
            get => _textContent;
            set => SetProperty(ref _textContent, value);
        }
    }
}
