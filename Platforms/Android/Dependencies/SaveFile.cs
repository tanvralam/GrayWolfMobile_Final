using Android.Content;
using CommunityToolkit.Mvvm.DependencyInjection;
using GalaSoft.MvvmLight.Ioc;
using GrayWolf.Droid.Dependencies;
using GrayWolf.Interfaces;
using Java.IO;
using Plugin.CurrentActivity;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;



namespace GrayWolf.Droid.Dependencies
{
    public class SaveFileAndroid : ISaveFile
    {
        public async Task SaveFileAsync(string localFilePath, string type, IEnumerable<string> attachments)
        {
            var fileName = Path.GetFileName(localFilePath);
            var intent = new Intent(Intent.ActionCreateDocument)
                .AddCategory(Intent.CategoryOpenable)
                .SetType(type)
                .PutExtra(Intent.ExtraTitle, fileName);

            if (!(CrossCurrentActivity.Current.Activity is MainActivity activity))
                throw new System.NullReferenceException();
            var result = await activity.CreateFileAsync(intent);
            var fileSystem = Ioc.Default.GetService<Interfaces.IFileSystem>();
            var bytes = await fileSystem.ReadAllBytesAsync(localFilePath);
            var context = activity.ApplicationContext;
            var pfd = context.ContentResolver.OpenFileDescriptor(result, "w");
            var fos = new FileOutputStream(pfd.FileDescriptor);
            await fos.WriteAsync(bytes);
            fos.Close();
            pfd.Close();
        }
    }
}

