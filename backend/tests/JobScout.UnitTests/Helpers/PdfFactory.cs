using System.Text;

namespace JobScout.UnitTests.Helpers;

internal static class PdfFactory
{
    public static byte[] WithText(string text)
    {
        var escaped = text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        return Build($"BT /F1 12 Tf 50 720 Td ({escaped}) Tj ET");
    }

    public static byte[] Empty()
    {
        return Build("BT /F1 12 Tf 50 720 Td () Tj ET");
    }

    private static byte[] Build(string content)
    {
        var objects = new[]
        {
            "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj\n",
            "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj\n",
            "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >> endobj\n",
            $"4 0 obj << /Length {Encoding.ASCII.GetByteCount(content)} >> stream\n{content}\nendstream\nendobj\n",
            "5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj\n"
        };

        var header = "%PDF-1.4\n";
        var builder = new StringBuilder(header);
        var offsets = new List<int> { 0 };
        var ascii = Encoding.ASCII;

        foreach (var obj in objects)
        {
            offsets.Add(ascii.GetByteCount(builder.ToString()));
            builder.Append(obj);
        }

        var xrefPos = ascii.GetByteCount(builder.ToString());
        builder.Append($"xref\n0 {offsets.Count}\n");
        builder.Append("0000000000 65535 f \n");
        for (var i = 1; i < offsets.Count; i++)
        {
            builder.Append($"{offsets[i]:D10} 00000 n \n");
        }

        builder.Append($"trailer << /Size {offsets.Count} /Root 1 0 R >>\nstartxref\n{xrefPos}\n%%EOF");
        return ascii.GetBytes(builder.ToString());
    }
}
