using GrayWolf.CustomControls;
using GrayWolf.Enums;
using GrayWolf.Models.Domain;
using GrayWolf.ViewModels;
using System.Collections.Generic;


namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectAttachmentFilesPopup
    {
        public SelectAttachmentFilesPopup(List<ArchiveEntry> entries, int logFileId, ExportLogOptions option)
        {
            BindingContext = new SelectAttachmentFilesPopupViewModel(entries, logFileId, option);
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Content.HeightRequest = height * 0.7;
            Content.WidthRequest = width * 0.8;
        }
    }
}