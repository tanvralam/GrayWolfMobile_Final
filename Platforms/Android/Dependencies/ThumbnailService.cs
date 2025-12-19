using Android.Graphics;
using Android.Media;
using GrayWolf.Droid.Dependencies;
using GrayWolf.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace GrayWolf.Droid.Dependencies
{
    public class ThumbnailService : IThumbnailService
    {
        public Task<System.IO.Stream> GetImageStreamAsync(string filePath)
        {
            MediaMetadataRetriever retriever = new MediaMetadataRetriever();
            retriever.SetDataSource(filePath);
            Bitmap bitmap = retriever.GetFrameAtTime(0);

            //Convert bitmap to a 'Stream' and then to an 'ImageSource' 
            if (bitmap != null)
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                //byte[] bitmapData = stream.ToArray();
                return Task.FromResult((System.IO.Stream)stream);
            }
            return null;
        }
    }
}