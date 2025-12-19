using GrayWolf.Droid.Dependencies;
using GrayWolf.Interfaces;

[assembly: Dependency(typeof(ProbeStatus))]
namespace GrayWolf.Droid.Dependencies
{
    public class ProbeStatus : IProbeStatus
    {
        /// <summary>
        /// Todo : To Define Probe Status Method ..
        /// </summary> 
        string IProbeStatus.ProbeStatus()
        {
            //Write your code here..  
            return string.Empty;
        }
    }
}