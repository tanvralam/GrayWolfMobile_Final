using AVFoundation;
using Foundation;
using GrayWolf.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace GrayWolf.iOS.Dependencies
{
    public class PrepareRecording_iOS : IPrepareRecording
    {
        public void PrepareRecording()
        {
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Record);
        }
    }
}