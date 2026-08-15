using JobScout.Application.Options;
using JobScout.Application.Validators;
using Microsoft.Extensions.Options;

namespace JobScout.UnitTests;

public class CvUploadValidatorTests
{
    private readonly CvUploadValidator _validator = new(Options.Create(new CvOptions()));

    [Fact]
    public void Accepts_valid_pdf_header()
    {
        var errors = _validator.Validate("cv.pdf", "application/pdf", 1200, "%PDF-1.4"u8.ToArray());
        Assert.Empty(errors);
    }

    [Fact]
    public void Rejects_missing_file()
    {
        var errors = _validator.Validate("", "application/pdf", 0, null);
        Assert.Contains(errors, e => e.Contains("PDF", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Rejects_unsupported_extension()
    {
        var errors = _validator.Validate("cv.docx", "application/pdf", 1200, "%PDF"u8.ToArray());
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Rejects_oversized_file()
    {
        var errors = _validator.Validate("cv.pdf", "application/pdf", 6 * 1024 * 1024, "%PDF"u8.ToArray());
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Rejects_non_pdf_bytes()
    {
        var errors = _validator.Validate("cv.pdf", "application/pdf", 40, "HELLO"u8.ToArray());
        Assert.NotEmpty(errors);
    }
}
