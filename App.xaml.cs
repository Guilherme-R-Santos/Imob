using System.Configuration;
using System.Data;
using System.Windows;
using Imob.Services.Pdf;
using PdfSharp.Fonts;

namespace Imob
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            if (GlobalFontSettings.FontResolver is null)
            {
                GlobalFontSettings.FontResolver = new WindowsFontResolver();
            }
        }
    }
}
