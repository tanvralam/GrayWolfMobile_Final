using GrayWolf.Droid.Dependencies;
using GrayWolf.Interfaces;

[assembly: Dependency(typeof(ShutDown))]
namespace GrayWolf.Droid.Dependencies
{
    public class ShutDown : IShutDown
    {
        /// <summary>
        /// Todo : To Define ShutDown Method ..
        /// </summary>  
        void IShutDown.ShutDown()
        {
            //Write your code here..  
        }
    }
}