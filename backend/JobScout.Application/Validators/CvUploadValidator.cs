using JobScout.Application.Options;
using Microsoft.Extensions.Options;

namespace JobScout.Application.Validators;

public sealed class CvUploadValidator
{
    private static readonly byte[] PdfMagic = "%PDF"u8.ToArray();
    private readonly CvOptions _options;

    public CvUploadValidator(IOptions<CvOptions> options)
    {
        _options = options.Value;
    }

    public IReadOnlyList<string> Validate(string fileName, string contentType, long length, byte[]? headerBytes)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(fileName) || length <= 0)
        {
            errors.Add("Please upload a PDF CV under 5 MB.");
            return errors;
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) ||
            !_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add("Please upload a PDF CV under 5 MB.");
            return errors;
        }

        if (length > _options.MaxFileSizeBytes)
        {
            errors.Add("Please upload a PDF CV under 5 MB.");
            return errors;
        }

        var typeOk = string.IsNullOrWhiteSpace(contentType) ||
                     _options.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase) ||
                     contentType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase);

        if (!typeOk)
        {
            errors.Add("Please upload a PDF CV under 5 MB.");
            return errors;
        }

        if (headerBytes is null || headerBytes.Length < 4 || !headerBytes.Take(4).SequenceEqual(PdfMagic))
        {
            errors.Add("Please upload a PDF CV under 5 MB.");
        }

        return errors;
    }
}
