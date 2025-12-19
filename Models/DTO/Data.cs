using GrayWolf.Models.Domain;
using GrayWolf.Services;
using System.Collections.Generic;

namespace GrayWolf.Models.DTO
{
    public class Data
    {
        public bool UserAuthenticated { get; set; }
        public string ErrorMessage { get; set; }
        public List<GrayWolfDevice> Devices { get; set; }
    }
}
