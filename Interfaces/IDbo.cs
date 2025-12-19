using System;
using System.Collections.Generic;
using System.Text;

namespace GrayWolf.Interfaces
{
    public interface IDbo
    {
    }

    public interface IDbo<T> : IDbo
    {
        T Id { get; }
    }
}
