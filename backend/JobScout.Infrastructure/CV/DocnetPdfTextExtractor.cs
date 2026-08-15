using System.Text;
using Docnet.Core;
using Docnet.Core.Models;
using JobScout.Application.Interfaces;

namespace JobScout.Infrastructure.CV;

public sealed class DocnetPdfTextExtractor : IPdfTextExtractor
{
    public string ExtractText(byte[] pdfBytes)
    {
        if (pdfBytes is null || pdfBytes.Length == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        using var docReader = DocLib.Instance.GetDocReader(pdfBytes, new PageDimensions(1080, 1920));
        var pages = docReader.GetPageCount();

        for (var i = 0; i < pages; i++)
        {
            using var pageReader = docReader.GetPageReader(i);
            var pageText = pageReader.GetText();
            if (!string.IsNullOrWhiteSpace(pageText))
            {
                builder.AppendLine(pageText);
            }
        }

        return builder.ToString();
    }
}
