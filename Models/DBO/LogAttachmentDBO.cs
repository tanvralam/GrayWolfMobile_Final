using GrayWolf.Interfaces;
using SQLite;


namespace GrayWolf.Models.DBO
{
    public class AttachmentDBO : IDbo<int>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int LoggerId { get; set; }
        public string Name { get; set; }
        public bool IsAudio { get; set; }
        public bool IsMedia { get; set; }
        public bool IsText { get; set; }
        public bool IsVideo { get; set; }
        public bool IsDrawing { get; set; }
        public bool IsEvent { get; set; }
        [Ignore]
        public string TextContent { get; set; }
        public string Caption { get; set; }
        public string ThumbnailSource { get; set; }
        public string Path { get; set; }
        [Ignore]
        public string Icon { get; set; }
        public string CaptionPath { get; set; }
    }
}
