using GrayWolf.Extensions;
using GrayWolf.Interfaces;
using GrayWolf.Models.DBO;
using GrayWolf.Models.Domain;
using GrayWolf.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace GrayWolf.Converters
{
    public static class AttachmentConverters
    {
        public static AttachmentDBO ToAttachmentDBO(this IAttachment attachment)
        {
            var dbo = new AttachmentDBO
            {
                Id = attachment.Id,
                Name = attachment.Name,
                Caption = attachment.Caption,
                IsAudio = attachment is AudioAttachment,
                IsDrawing = attachment is DrawableAttachment,
                IsVideo = attachment is VideoAttachment,
                IsMedia = attachment is PhotoAttachment,
                IsEvent = attachment is EventAttachment,
                LoggerId = attachment.LoggerId,
            };
            if (attachment is TextAttachment textAttachment)
            {
                dbo.IsText = true;
                dbo.TextContent = textAttachment.TextContent;
            }
            return dbo;
        }

        public static IAttachment ToAttachment(this AttachmentDBO dbo, string folderPath)
        {
            if (dbo.IsAudio)
            {
                return dbo.ToAudioAttachment(folderPath);
            }
            else if (dbo.IsDrawing)
            {
                return dbo.ToDrawableAttachment(folderPath);
            }
            else if (dbo.IsVideo)
            {
                return dbo.ToVideoAttachment(folderPath);
            }
            else if (dbo.IsMedia)
            {
                return dbo.ToPhotoAttachment(folderPath);
            }
            else if (dbo.IsText)
            {
                return dbo.ToTextAttachment(folderPath);
            }
            else if (dbo.IsEvent)
            {
                return dbo.ToEventAttachment(true, folderPath);
            }
            throw new ArgumentException();
        }

        private static TAttachment ToAttachmentBase<TAttachment>(this AttachmentDBO dbo, string folderPath, string extension, bool setCap)
            where TAttachment : Attachment, new()
        {
            var captionName = "";
            var logName = Path.GetDirectoryName(folderPath);
            if (dbo.IsMedia)
            {
                captionName = $"{dbo.Name.Replace("photo", "caption")}.cap";
            }
            else if (dbo.IsVideo)
            {
                captionName = $"{dbo.Name.Replace("video", "caption")}.vid";
            }
            return new TAttachment()
            {
                BinaryContent = new byte[] { },
                Caption = dbo.Caption,
                Name = dbo.Name,
                Id = dbo.Id,
                LoggerId = dbo.LoggerId,
                CaptionPath = setCap ? Path.Combine(folderPath, captionName) : "",
                Path = Path.Combine(folderPath, $"{dbo.Name}{extension}")
            };
        }

        public static TextAttachment ToTextAttachment(this AttachmentDBO dbo, string folderPath)
        {
            var attachment = dbo.ToAttachmentBase<TextAttachment>(folderPath, ".txt", false);
            attachment.TextContent = dbo.TextContent;
            return attachment;
        }

        public static AudioAttachment ToAudioAttachment(this AttachmentDBO dbo, string folderPath) =>
            dbo.ToAttachmentBase<AudioAttachment>(folderPath, ".mp3", false);

        public static VideoAttachment ToVideoAttachment(this AttachmentDBO dbo, string folderPath) =>
            dbo.ToAttachmentBase<VideoAttachment>(folderPath, LogService.VideoExtension, true);

        public static DrawableAttachment ToDrawableAttachment(this AttachmentDBO dbo, string folderPath) =>
            dbo.ToAttachmentBase<DrawableAttachment>(folderPath, ".bmp", false);

        public static PhotoAttachment ToPhotoAttachment(this AttachmentDBO dbo, string folderPath) =>
            dbo.ToAttachmentBase<PhotoAttachment>(folderPath, LogService.PhotoExtension, true);

        public static EventAttachment ToEventAttachment(this AttachmentDBO dbo, bool isCreated, string folderPath, List<Event> events = null)
        {
            var attachment = dbo.ToAttachmentBase<EventAttachment>(folderPath, ".evt", false);
            attachment.Events = events?.ToObservableCollection();
            attachment.IsCreated = isCreated;
            return attachment;
        }
    }
}
