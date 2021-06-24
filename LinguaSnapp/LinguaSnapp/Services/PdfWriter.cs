using DotNetExtensions;
using LinguaSnapp.Enums;
using LinguaSnapp.Models;
using Microsoft.AppCenter.Crashes;
using PdfSharp.Drawing.Layout;
using PdfSharp.Xamarin.Forms.Contracts;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.Services
{
    class PdfWriter
    {
        // Document
        PdfDocument pdf;

        // Layout helper
        LayoutHelper helper;

        public PdfWriter()
        {
            try
            {
                // Set our custom font resolver
                GlobalFontSettings.FontResolver = new LinguaSnappFontProvider(null);
            }
            catch (Exception)
            {
                Debug.WriteLine("Could not set FontResolver -- possibly already used?");
            }

            // Create new document
            pdf = new PdfDocument();

            // Uses A4, page height is 29.7 cm, page width is 21.0cm. We use margins of 2.5 cm.
            helper = new LayoutHelper(pdf, XUnit.FromCentimeter(2.5), XUnit.FromCentimeter(2.5), XUnit.FromCentimeter(21.0 - 2.5), XUnit.FromCentimeter(29.7 - 2.5));
        }

        // Main writing method
        internal void WriteSubmission(
            SubmissionStatus submissionStatus,
            SubmissionMetaModel meta,
            IEnumerable<DescriptorModel> descriptors,
            string comments,
            IEnumerable<TranslationModel> translations,
            bool oneLang
        )
        {
            // Start a new page
            helper.CreatePage();

            // Draw Title
            var text = meta.Title;
            if (submissionStatus == SubmissionStatus.Draft) text += $" ({submissionStatus.GetDescription()})";
            helper.DrawString(text, type: TextType.Title);
            helper.AddSkip();

            // Draw Photo
            var img = XImage.FromStream(() =>
            {
                return new MemoryStream(Convert.FromBase64String(meta.EncodedThumbnail));
            });
            var reqHeight = 200d;
            var reqWidth = (img.PointWidth / img.PointHeight) * reqHeight;
            helper.DrawImage(img, reqWidth, reqHeight);
            helper.AddSkip();

            // Draw Meta
            helper.DrawString($"{(string)Application.Current.Resources["lang_num_lang"]} = {meta.NumLang}");
            helper.DrawString($"{(string)Application.Current.Resources["lang_num_alpha"]} = {meta.NumAlpha}");
            helper.DrawString($"{(string)Application.Current.Resources["pdf_latitude"]} = {meta.Latitude}");
            helper.DrawString($"{(string)Application.Current.Resources["pdf_longitude"]} = {meta.Longitude}");
            helper.DrawString($"{(string)Application.Current.Resources["analysis_dominance_check"]} = {(oneLang ? "True" : "False")}");
            helper.AddSkip();

            // Draw Translations
            helper.DrawString($"{(string)Application.Current.Resources["pdf_translations"]}", type: TextType.Heading);
            foreach (var t in translations)
            {
                var lang = t.LanguageOtherValue != null ? t.LanguageOtherValue : ConfigurationService.Instance.GetDescriptorFromCode(t.LanguageCode)?.Name;
                var alpha = t.AlphabetOtherValue != null ? t.AlphabetOtherValue : ConfigurationService.Instance.GetDescriptorFromCode(t.AlphabetCode)?.Name;
                helper.DrawString($"{(string)Application.Current.Resources["lang_card_lang"]} = {lang}");
                helper.DrawString($"{(string)Application.Current.Resources["lang_card_alpha"]} = {alpha}");
                helper.DrawString($"{(string)Application.Current.Resources["lang_card_trans"]}");
                helper.DrawString($"{t.Translation}", 5);
                helper.AddSkip();
            }
            helper.AddSkip();

            // Draw Descriptors
            helper.DrawString($"{(string)Application.Current.Resources["pdf_descriptors"]}", type: TextType.Heading);
            foreach (var g in descriptors.GroupBy(d => d.DescriptorType))
            {
                helper.DrawString($"{g.Key}:");
                foreach (var d in g)
                {
                    string other = !string.IsNullOrWhiteSpace(d.Value) ? $": {d.Value}" : "";
                    helper.DrawString($"{ConfigurationService.Instance.GetDescriptorFromCode(d.Code).Name}{other}", 5);
                }
                helper.AddSkip();
            }
            helper.AddSkip();

            // Draw Comments
            helper.DrawString($"{(string)Application.Current.Resources["context_comments"]}:");
            helper.DrawString($"{comments}", 5);
        }

        internal void SaveAndClose(string file)
        {
            pdf.Save(file);
            pdf.Close();
        }
    }

    // Helper to manage layout
    public class LayoutHelper
    {
        private readonly PdfDocument _document;
        private readonly XUnit _topPosition;
        private readonly XUnit _leftPosition;
        private readonly XUnit _rightPosition;
        private readonly XUnit _bottomPosition;
        private XUnit _currentPosition;

        // Fonts
        XFont fontTitle = new XFont(GlobalFontSettings.FontResolver.DefaultFontName, 24, XFontStyle.Bold);
        XFont fontHeading = new XFont(GlobalFontSettings.FontResolver.DefaultFontName, 16, XFontStyle.Underline);
        XFont fontBody = new XFont(GlobalFontSettings.FontResolver.DefaultFontName, 16);
        XFont fontFooter = new XFont(GlobalFontSettings.FontResolver.DefaultFontName, 8, XFontStyle.Italic);

        public LayoutHelper(PdfDocument document, XUnit leftPosition, XUnit topPosition, XUnit rightPosition, XUnit bottomPosition)
        {
            _document = document;
            _leftPosition = leftPosition;
            _topPosition = topPosition;
            _rightPosition = rightPosition;
            _bottomPosition = bottomPosition;
        }

        public XGraphics Gfx { get; private set; }
        public PdfPage Page { get; private set; }

        public void CreatePage()
        {
            Page = _document.AddPage();
            Page.Size = PageSize.A4;
            Gfx = XGraphics.FromPdfPage(Page);
            _currentPosition = _topPosition;

            // Add footer
            XStringFormat format = new XStringFormat
            {
                Alignment = XStringAlignment.Center,
                LineAlignment = XLineAlignment.Near
            };
            var footerRect = new XRect(_leftPosition, _bottomPosition, _rightPosition - _leftPosition, XUnit.FromCentimeter(2.5));
            var tf = new XTextFormatterEx(Gfx)
            {
                Alignment = XParagraphAlignment.Center
            };
            tf.DrawString($"{(string)Application.Current.Resources["pdf_footer_1"]}\n{(string)Application.Current.Resources["pdf_footer_2"]}", fontFooter, XBrushes.Black, footerRect, format);
        }

        public void DrawString(string text, double indent = 0, TextType type = TextType.Body)
        {
            // Don't do anything if string is empty
            if (string.IsNullOrWhiteSpace(text)) return;

            // Set font
            var font = fontBody;
            if (type == TextType.Heading) font = fontHeading;
            else if (type == TextType.Title) font = fontTitle;

            // Create page if we have ran out of room
            if (_bottomPosition <= _currentPosition) CreatePage();

            // Create text formatter
            double neededHeight;
            var tf = new XTextFormatterEx(Gfx)
            {
                Alignment = type == TextType.Title ? XParagraphAlignment.Center : XParagraphAlignment.Left
            };

            // Measure based on available space left on page
            var bounds = new XRect(_leftPosition + indent, _currentPosition, _rightPosition - _leftPosition - indent, _bottomPosition - _currentPosition);
            tf.PrepareDrawString(text, font, bounds, out _, out neededHeight);

            // Check whether we need a new page and recreate bounds and text formatter if we do
            if (neededHeight > bounds.Height)
            {
                CreatePage();
                bounds = new XRect(_leftPosition + indent, _currentPosition, _rightPosition - _leftPosition - indent, _bottomPosition - _currentPosition);
                tf = new XTextFormatterEx(Gfx)
                {
                    Alignment = type == TextType.Title ? XParagraphAlignment.Center : XParagraphAlignment.Left
                };
            }

            // Draw string
            tf.DrawString(text, font, XBrushes.Black, bounds);

            // Increment the current position tracker
            _currentPosition += neededHeight;
        }

        public void AddSkip()
        {
            _currentPosition += fontBody.Size;
        }

        internal void DrawImage(XImage img, double reqWidth, double reqHeight)
        {
            var imgRect = new XRect(_leftPosition + (_rightPosition - _leftPosition - reqWidth) / 2d, _currentPosition, reqWidth, reqHeight);
            Gfx.DrawImage(img, imgRect);
            _currentPosition += imgRect.Height;
        }
    }

    public enum TextType
    {
        Body,
        Title,
        Heading
    }

    internal class LinguaSnappFontProvider : IFontResolver
    {
        public string DefaultFontName
        {
            get { return "OpenSans"; }
        }

        private string[] DefaultFontFiles =
        {
            "OpenSans_Regular.ttf",
            "OpenSans_SemiBold.ttf"
        };

        public ICustomFontProvider _fontProvider;

        public LinguaSnappFontProvider(ICustomFontProvider fontProvider)
        {
            _fontProvider = fontProvider;
        }

        #region IFontResolver implementation
        public byte[] GetFont(string faceName)
        {
            if (DefaultFontFiles.Contains(faceName) || _fontProvider == null)
            {
                var assembly = typeof(LinguaSnappFontProvider).GetTypeInfo().Assembly;
                var resName = $"LinguaSnapp.Resources.{faceName}";
                Stream stream = assembly.GetManifestResourceStream(resName);
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var bytes = default(byte[]);
                        using (var memstream = new MemoryStream())
                        {
                            reader.BaseStream.CopyTo(memstream);
                            bytes = memstream.ToArray();
                        }
                        return bytes;
                    }
                }
                else
                {
                    Debug.WriteLine($"Cannot find: {resName}");
                    Crashes.TrackError(new Exception($"Cannot find font {resName}"));
                }
                return default(byte[]);
            }
            else
            {
                return _fontProvider.GetFont(faceName);
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string fontName;
            if (familyName == DefaultFontName || _fontProvider == null)
                fontName = DefaultFontFiles[!isBold ? 0 : 1];
            else
                fontName = _fontProvider.ProvideFont(familyName, isBold, isItalic);

            return new FontResolverInfo(fontName);
        }
        #endregion
    }
}
