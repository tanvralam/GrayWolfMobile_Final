using System;
using AudioToolbox;
using GrayWolf.Interfaces;
using GrayWolf.iOS.Dependencies;


namespace GrayWolf.iOS.Dependencies
{
    public class AlertSoundService : IAlertSoundService
    {
        public void PlaySystemSound(bool isError)
        {
            uint soundID = 1005;
            if(isError)
            {
                soundID = 1000;
            }
            else
            {
                soundID = 1005;
            }

            var sound = new SystemSound(soundID);
            sound.PlaySystemSound();
        }
    }
}
