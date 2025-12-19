using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Media;
using GrayWolf.Droid.Dependencies;
using GrayWolf.Interfaces;

namespace GrayWolf.Droid.Dependencies
{
    public class AlertSoundService : IAlertSoundService
    {
        public AlertSoundService()
        {
        }

        public async void PlaySystemSound(bool isError)
        {
            RingtoneType type = RingtoneType.Notification;
            if(isError)
            {
                type = RingtoneType.Alarm;
            }
            else
            {
                type = RingtoneType.Notification;
            }

            Android.Net.Uri uri = RingtoneManager.GetDefaultUri(type);
             var context = Android.App.Application.Context;
            Ringtone rt = RingtoneManager.GetRingtone(context, uri);
            
            rt.Play();
            await Task.Delay(2500);
            rt.Stop();
        }
    }
}
