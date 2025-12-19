using AVFoundation;
using Foundation;
using GrayWolf.Interfaces;
using GrayWolf.iOS.Dependencies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace GrayWolf.iOS.Dependencies
{
    public class ThumbnailService : IThumbnailService
    {
        TaskCompletionSource<Stream> taskCompletionSource;
        public Task<Stream> GetImageStreamAsync(string filePath)
        {

            CoreMedia.CMTime actualTime;
            NSError outError;
            using (var asset = AVAsset.FromUrl(NSUrl.FromFilename(filePath)))
            using (var imageGen = new AVAssetImageGenerator(asset))
            using (var imageRef = imageGen.CopyCGImageAtTime(new CoreMedia.CMTime(1, 1), out actualTime, out outError))
            {
                if (imageRef == null)
                    return null;
                var image = UIImage.FromImage(imageRef);

                //Stream imagestream = image.AsJPEG(1).AsStream();
                Stream imagestream = image.AsPNG().AsStream();
                taskCompletionSource.SetResult(imagestream);
            }

            return taskCompletionSource.Task;
        }
    }
}