using GrayWolf.ViewModels;
using Syncfusion.Maui.ImageEditor;
using System.IO;



namespace GrayWolf.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrawingNotePopup
    {
        private byte[] _bytes = null;

        public DrawingNotePopup()
        {
            InitializeComponent();
            BindingContext = new ImageNotePopupViewModel();
        }

        protected override async void OnAppearing()
        {
            if (BindingContext is ImageNotePopupViewModel vm)
            {
                await vm.Load();
            }
        }

        private void imgEditor_ImageSaved(object sender, ImageSavedEventArgs args)
        {
           // if(BindingContext is ImageNotePopupViewModel vm)
            {
            //    vm.OnImageSaved(_bytes);
            }
        }

        private void ToolbarSettings_ToolbarItemSelected(object sender, ToolbarItemSelectedEventArgs e)
        {
            if (e.ToolbarItem.Name == "Save")
            {
                var info = DeviceDisplay.MainDisplayInfo;
              //  var rendered =  imgEditor.ActualImageRenderedBounds;
               // var density = info.Density;
               // imgEditor.Save("bmp", new Size(rendered.Width * 2 * density, rendered.Height * 2 * density));
                e.Cancel = true;
            }
        }

        private void imgEditor_ImageSaving(object sender, ImageSavingEventArgs args)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                args.ImageStream.CopyTo(ms);
                _bytes = ms.ToArray();
            }
        }
    }
}