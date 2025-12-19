using GrayWolf.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
  using Android.Media;
#elif IOS
  using AudioToolbox;
#endif

namespace GrayWolf.Services
{
    public class AlertSoundServiceNew : IAlertSoundService
    {
        public AlertSoundServiceNew()
        {
        }

        public async void PlaySystemSound(bool isError)
        {
            #if ANDROID
            RingtoneType type = RingtoneType.Notification;
            if (isError)
            {
                type = RingtoneType.Alarm;
            }
            else
            {
                type = RingtoneType.Notification;
            }

            Android.Net.Uri uri = RingtoneManager.GetDefaultUri(type);
            Ringtone rt = RingtoneManager.GetRingtone(Android.App.Application.Context, uri);

            rt.Play();
            await Task.Delay(2500);
            rt.Stop();

           
            #elif IOS
    
            uint soundID = 1005;
            if (isError)
            {
                soundID = 1000;
            }
            else
            {
                soundID = 1005;
            }

            var sound = new SystemSound(soundID);
            sound.PlaySystemSound();
          #endif
        }
    }
}
