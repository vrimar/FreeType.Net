using System.Buffers.Binary;

namespace FreeType.Net.Tests;

/// <summary>The few sfnt table fields the tests check FreeType's face against, read straight from the font file.</summary>
sealed class SfntTables(byte[] font)
{
    public int UnitsPerEm => U16("head", 18);

    public (int XMin, int YMin, int XMax, int YMax) HeadBox =>
        (S16("head", 36), S16("head", 38), S16("head", 40), S16("head", 42));

    public int Ascender => S16("hhea", 4);

    public int Descender => S16("hhea", 6);

    public int LineGap => S16("hhea", 8);

    public int AdvanceWidthMax => U16("hhea", 10);

    public int GlyphCount => U16("maxp", 4);

    public int UnderlinePosition => S16("post", 8);

    public int UnderlineThickness => S16("post", 10);

    public int CharmapCount => U16("cmap", 2);

    public int Advance(int glyph)
    {
        var metrics = U16("hhea", 34);
        return U16("hmtx", Math.Min(glyph, metrics - 1) * 4);
    }

    /// <summary>A simple glyph's header: its box and the index of each contour's last point.</summary>
    public (int XMin, int YMin, int XMax, int YMax, int[] ContourEnds) Glyph(int glyph)
    {
        var longOffsets = S16("head", 50) == 1;
        var start = longOffsets ? (int)U32("loca", glyph * 4) : U16("loca", glyph * 2) * 2;
        var glyf = Table("glyf")[start..];
        var contours = BinaryPrimitives.ReadInt16BigEndian(glyf);
        var ends = new int[contours];
        for (var i = 0; i < contours; i++)
            ends[i] = BinaryPrimitives.ReadUInt16BigEndian(glyf[(10 + i * 2)..]);
        return (
            BinaryPrimitives.ReadInt16BigEndian(glyf[2..]),
            BinaryPrimitives.ReadInt16BigEndian(glyf[4..]),
            BinaryPrimitives.ReadInt16BigEndian(glyf[6..]),
            BinaryPrimitives.ReadInt16BigEndian(glyf[8..]),
            ends
        );
    }

    int U16(string tag, int offset) => BinaryPrimitives.ReadUInt16BigEndian(Table(tag)[offset..]);

    int S16(string tag, int offset) => BinaryPrimitives.ReadInt16BigEndian(Table(tag)[offset..]);

    uint U32(string tag, int offset) => BinaryPrimitives.ReadUInt32BigEndian(Table(tag)[offset..]);

    ReadOnlySpan<byte> Table(string tag)
    {
        var wanted = BinaryPrimitives.ReadUInt32BigEndian(System.Text.Encoding.ASCII.GetBytes(tag));
        var count = BinaryPrimitives.ReadUInt16BigEndian(font.AsSpan(4));
        for (var i = 0; i < count; i++)
        {
            var record = font.AsSpan(12 + i * 16);
            if (BinaryPrimitives.ReadUInt32BigEndian(record) != wanted)
                continue;
            var offset = (int)BinaryPrimitives.ReadUInt32BigEndian(record[8..]);
            var length = (int)BinaryPrimitives.ReadUInt32BigEndian(record[12..]);
            return font.AsSpan(offset, length);
        }
        throw new InvalidOperationException($"The font has no '{tag}' table.");
    }
}
