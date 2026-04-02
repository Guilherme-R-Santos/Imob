using System;
using System.Collections.Generic;
using System.IO;
using PdfSharp.Fonts;

namespace Imob.Services.Pdf
{
    public sealed class WindowsFontResolver : IFontResolver
    {
        private static readonly IReadOnlyDictionary<string, string> FontFiles =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Times New Roman#Regular"] = "times.ttf",
                ["Times New Roman#Bold"] = "timesbd.ttf",
                ["Times New Roman#Italic"] = "timesi.ttf",
                ["Times New Roman#BoldItalic"] = "timesbi.ttf",

                ["Arial#Regular"] = "arial.ttf",
                ["Arial#Bold"] = "arialbd.ttf",
                ["Arial#Italic"] = "ariali.ttf",
                ["Arial#BoldItalic"] = "arialbi.ttf"
            };

        public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (string.IsNullOrWhiteSpace(familyName))
                return null;

            if (familyName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase))
                return new FontResolverInfo(BuildFaceName("Times New Roman", isBold, isItalic));

            if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
                return new FontResolverInfo(BuildFaceName("Arial", isBold, isItalic));

            return new FontResolverInfo(BuildFaceName("Arial", isBold, isItalic));
        }

        public byte[]? GetFont(string faceName)
        {
            if (!FontFiles.TryGetValue(faceName, out var fileName))
                return null;

            var windowsFontsPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var fullPath = Path.Combine(windowsFontsPath, fileName);

            return File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : null;
        }

        private static string BuildFaceName(string familyName, bool isBold, bool isItalic)
        {
            if (isBold && isItalic) return $"{familyName}#BoldItalic";
            if (isBold) return $"{familyName}#Bold";
            if (isItalic) return $"{familyName}#Italic";
            return $"{familyName}#Regular";
        }
    }
}
