using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Imob.Services.Pdf;
using Microsoft.Extensions.Configuration;
using PdfSharp.Fonts;

namespace Imob
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        public App()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            if (GlobalFontSettings.FontResolver is null)
            {
                GlobalFontSettings.FontResolver = new WindowsFontResolver();
            }
        }
    }
}
