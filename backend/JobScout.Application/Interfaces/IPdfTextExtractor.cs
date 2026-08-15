namespace JobScout.Application.Interfaces;

public interface IPdfTextExtractor
{
    string ExtractText(byte[] pdfBytes);
}
