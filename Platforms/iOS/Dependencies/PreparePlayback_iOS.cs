using AVFoundation;
using GrayWolf.Interfaces;

namespace GrayWolf.iOS.Dependencies
{
    public class PreparePlayback_iOS : IPreparePlayback
    {
        public void PreparePlayback()
        {
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
        }
    }
}