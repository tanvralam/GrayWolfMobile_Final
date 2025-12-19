using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrayWolf.Models.Domain
{
        // Custom EventArgs class for passing data
        public class EventArg<T> : EventArgs
        {
            public T Data { get; }

            public EventArg(T data)
            {
                 Data = data;
            }
        }
    
}
