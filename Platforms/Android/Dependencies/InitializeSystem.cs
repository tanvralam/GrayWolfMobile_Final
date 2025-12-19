using GrayWolf.Droid.Dependencies;
using GrayWolf.Interfaces;

[assembly: Dependency(typeof(InitializeSystem))]
namespace GrayWolf.Droid.Dependencies
{
    public class InitializeSystem : IInititalizeSystem
    {
        /// <summary>
        /// Todo : To Define Sytem Initialized Method ..
        /// </summary>
        /// <param name="BTLE"></param>
        /// <param name="USB"></param>
        void IInititalizeSystem.InitializeSystem(bool BTLE, bool USB)
        {
            //Write your code here..
        }
    }
}