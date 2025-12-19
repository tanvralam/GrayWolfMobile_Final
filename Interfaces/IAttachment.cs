using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Interfaces
{
    public interface IAttachment
    {
        int Id { get; }

        int LoggerId { get; }

        string Name { get; }

        string Path { get; }

        string Caption { get; }

        string CaptionPath { get; }
    }
}
