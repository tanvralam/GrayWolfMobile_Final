using Android.Widget;
using GrayWolf.Droid.Dependencies;
using GrayWolf.Interfaces;
using GrayWolf.Models.DTO;
using GrayWolf.Services;
using Java.IO;
using Environment = Android.OS.Environment;


[assembly:Dependency(typeof(GetLocalDeviceDataAndroid))]
namespace GrayWolf.Droid.Dependencies
{
    public class GetLocalDeviceDataAndroid : IGetLocalDeviceData
    {
        private String _mSdcardDirectory = "";
        private String _mDir = "";
        private List<String> _mSubdirs = null;
        private ArrayAdapter<String> _mListAdapter = null;
        private bool _mGoToUpper = true;

        public GetDeviceDataReturn GetLocalDeviceData()
        {
            //TODO: implement code

            return null;
        }

        public string GetExternalPath()
        {
            return Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDocuments).AbsolutePath;
        }

        public IEnumerable<string> GetDirectories(string dir)
        {
            if (dir == null)
            {
                dir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDocuments).AbsolutePath;
            }

            List<String> dirs = new List<String>();
            try
            {
                Java.IO.File dirFile = new Java.IO.File(dir);

                // if directory is not the base sd card directory add ".." for going up one directory
                if ((_mGoToUpper || !_mDir.Equals(_mSdcardDirectory)) && !"/".Equals(_mDir))
                {
                    dirs.Add("..");
                }
                System.Diagnostics.Debug.WriteLine("~~~~", "m_dir=" + _mDir);
                if (!dirFile.Exists() || !dirFile.IsDirectory)
                {
                    return dirs;
                }

                foreach (Java.IO.File file in dirFile.ListFiles())
                {
                    if (file.IsDirectory)
                    {
                        // Add "/" to directory names to identify them in the list
                        dirs.Add(file.Name + "/");
                    }
                    //else if (_selectType == _fileSave || _selectType == _fileOpen)
                    //{
                    //    // Add file names to the list if we are doing a file save or file open operation
                    //    dirs.Add(file.Name);
                    //}
                }
            }
            catch (Exception e) { }

            dirs.Sort();
            return dirs;
        }
    }


}