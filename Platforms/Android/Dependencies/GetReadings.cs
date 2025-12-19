using GrayWolf.Droid.Dependencies;
using GrayWolf.Interfaces;

[assembly: Dependency(typeof(GetReadings))]
namespace GrayWolf.Droid.Dependencies
{
    public class GetReadings : IGetReadings
    {
        /// <summary>
        /// Todo : To Define Get Readings Method ..
        /// </summary> 
        string IGetReadings.GetReadings()
        {
            //Write your code here..  
            return string.Empty;
        }
    }
}