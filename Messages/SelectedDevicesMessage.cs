using GrayWolf.Models.Domain;
using System.Collections.Generic;

namespace GrayWolf.Messages
{
    public class SelectedDevicesMessage
    {
        public List<GrayWolfDevice> Devices { get; set; }
    }
}
