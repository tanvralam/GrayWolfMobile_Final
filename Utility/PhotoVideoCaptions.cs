using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace GrayWolf.Utility
{
    public class PhotoVideoCaptionData
    {
        public PhotoVideoCaptionData()
        {
            Label = "";
            Timestamp = DateTime.MinValue;
        }

        public PhotoVideoCaptionData(string s)
        {
            Decode(s);
        }

        public DateTime Timestamp { get; set; }
        public string Label { get; set; }

        public string Encode()
        {
            string enc = Label;
            if (Timestamp != DateTime.MinValue)
            {
                enc += '\t';
                enc += Timestamp.ToString("o", CultureInfo.InvariantCulture);
            }
            return enc;
        }

        public void Decode(string s)
        {
            string[] parts = s.Split(new char[1] { '\t' });

            if (parts.Length==1)
            {
                // we only have a single item.
                // Check to see if its a Date
                try
                {
                    Label = "";
                    Timestamp = DateTime.Parse(parts[0]);
                    return;
                }
                catch
                {
                    Timestamp = DateTime.MinValue;
                    Label = parts[0];
                    return;
                }
            }

            // Otherwise, treat it as a normal thing

            try
            {
                Label = parts[0];
            }
            catch
            {
                Label = "";
            }

            if (parts.Length > 1)
            {
                try
                {
                    Timestamp = DateTime.Parse(parts[1]);
                }
                catch
                {
                    Timestamp = DateTime.MinValue;
                }
            }
        }
    }
}
