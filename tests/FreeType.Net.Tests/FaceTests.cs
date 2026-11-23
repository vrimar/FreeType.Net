using System.Runtime.InteropServices;

namespace FreeType.Net.Tests;

// Checked against the font's own tables, so a struct field laid out wrong on a platform fails there.
public sealed class FaceTests
{
    static readonly SfntTables Tables = new(OpenFace.Font);

    [Test]
    public async Task AFace_ReadsItsMetricsAsTheFontsTablesHoldThem()
    {
        using var open = new OpenFace();
        FT_FaceRec face;
        FT_CharMapRec firstCharmap,
            charmap;
        string? family,
            style;
        bool scalable,
            pointsBack;
        unsafe
        {
            face = *open.Face;
            firstCharmap = *face.charmaps[0];
            charmap = *face.charmap;
            family = Marshal.PtrToStringUTF8((nint)face.family_name);
            style = Marshal.PtrToStringUTF8((nint)face.style_name);
            scalable = FT.FT_IS_SCALABLE(open.Face);
            pointsBack =
                firstCharmap.face == open.Face
                && face.glyph->face == open.Face
                && face.glyph->library == open.Library
                && face.size->face == open.Face;
        }
        var flags = (FT_FACE_FLAG)face.face_flags.Value;
        var box = Tables.HeadBox;
        var height = Tables.Ascender - Tables.Descender + Tables.LineGap;

        await Assert.That(face.num_faces.Value).IsEqualTo(1);
        await Assert.That(face.face_index.Value).IsEqualTo(0);
        await Assert.That(face.num_glyphs.Value).IsEqualTo(Tables.GlyphCount);
        await Assert.That(scalable).IsTrue();
        await Assert
            .That(flags & (FT_FACE_FLAG.FT_FACE_FLAG_SFNT | FT_FACE_FLAG.FT_FACE_FLAG_HORIZONTAL))
            .IsEqualTo(FT_FACE_FLAG.FT_FACE_FLAG_SFNT | FT_FACE_FLAG.FT_FACE_FLAG_HORIZONTAL);
        await Assert.That(face.style_flags.Value).IsEqualTo(0);
        await Assert.That(family).IsEqualTo("Noto Sans");
        await Assert.That(style).IsEqualTo("Regular");
        await Assert.That(face.num_fixed_sizes).IsEqualTo(0);
        await Assert.That(face.num_charmaps).IsEqualTo(Tables.CharmapCount);
        await Assert.That(charmap.encoding).IsEqualTo(FT_Encoding.FT_ENCODING_UNICODE);
        await Assert
            .That(
                (
                    face.bbox.xMin.Value,
                    face.bbox.yMin.Value,
                    face.bbox.xMax.Value,
                    face.bbox.yMax.Value
                )
            )
            .IsEqualTo(((nint)box.XMin, (nint)box.YMin, (nint)box.XMax, (nint)box.YMax));
        await Assert.That((int)face.units_per_EM).IsEqualTo(Tables.UnitsPerEm);
        await Assert.That((int)face.ascender).IsEqualTo(Tables.Ascender);
        await Assert.That((int)face.descender).IsEqualTo(Tables.Descender);
        await Assert.That((int)face.height).IsEqualTo(height);
        await Assert.That((int)face.max_advance_width).IsEqualTo(Tables.AdvanceWidthMax);
        await Assert.That((int)face.max_advance_height).IsEqualTo(height);
        await Assert
            .That((int)face.underline_position)
            .IsEqualTo(Tables.UnderlinePosition - Tables.UnderlineThickness / 2);
        await Assert.That((int)face.underline_thickness).IsEqualTo(Tables.UnderlineThickness);
        await Assert.That(pointsBack).IsTrue();
    }

    [Test]
    public async Task AnUnscaledGlyph_HoldsItsOutlineAndMetricsInFontUnits()
    {
        using var open = new OpenFace();
        uint index;
        FT_Error loaded,
            advanced;
        FT_GlyphSlotRec slot;
        nint advance;
        short[] ends;
        (nint XMin, nint YMin, nint XMax, nint YMax) points = (
            nint.MaxValue,
            nint.MaxValue,
            nint.MinValue,
            nint.MinValue
        );
        unsafe
        {
            index = FT.FT_Get_Char_Index(open.Face, new CULong('H'));
            loaded = FT.FT_Load_Glyph(
                open.Face,
                index,
                FT_LOAD.FT_LOAD_NO_SCALE | FT_LOAD.FT_LOAD_LINEAR_DESIGN
            );
            slot = *open.Face->glyph;
            ends = new ReadOnlySpan<short>(
                slot.outline.contours,
                slot.outline.n_contours
            ).ToArray();
            for (var i = 0; i < slot.outline.n_points; i++)
            {
                var point = slot.outline.points[i];
                points = (
                    Math.Min(points.XMin, point.x.Value),
                    Math.Min(points.YMin, point.y.Value),
                    Math.Max(points.XMax, point.x.Value),
                    Math.Max(points.YMax, point.y.Value)
                );
            }
            (advanced, advance) = AdvanceOf(open.Face, index);
        }
        var glyph = Tables.Glyph((int)index);
        var width = Tables.Advance((int)index);

        await Assert.That(index).IsNotEqualTo(0u);
        await Assert.That(loaded).IsEqualTo(FT_Error.FT_Err_Ok);
        await Assert.That(slot.glyph_index).IsEqualTo(index);
        await Assert.That(slot.format).IsEqualTo(FT_Glyph_Format.FT_GLYPH_FORMAT_OUTLINE);
        await Assert.That(ends.Select(end => (int)end)).IsEquivalentTo(glyph.ContourEnds);
        await Assert.That((int)slot.outline.n_points).IsEqualTo(glyph.ContourEnds[^1] + 1);
        await Assert
            .That(points)
            .IsEqualTo(((nint)glyph.XMin, (nint)glyph.YMin, (nint)glyph.XMax, (nint)glyph.YMax));
        await Assert.That(slot.metrics.horiBearingX.Value).IsEqualTo(glyph.XMin);
        await Assert.That(slot.metrics.horiBearingY.Value).IsEqualTo(glyph.YMax);
        await Assert.That(slot.metrics.width.Value).IsEqualTo(glyph.XMax - glyph.XMin);
        await Assert.That(slot.metrics.height.Value).IsEqualTo(glyph.YMax - glyph.YMin);
        await Assert.That(slot.metrics.horiAdvance.Value).IsEqualTo(width);
        await Assert.That(slot.advance.x.Value).IsEqualTo(width);
        await Assert.That(slot.linearHoriAdvance.Value).IsEqualTo(width);
        await Assert.That(advanced).IsEqualTo(FT_Error.FT_Err_Ok);
        await Assert.That(advance).IsEqualTo(width);

        static unsafe (FT_Error, nint) AdvanceOf(FT_FaceRec* face, uint glyph)
        {
            CLong advance;
            var error = FT.FT_Get_Advance(face, glyph, FT_LOAD.FT_LOAD_NO_SCALE, &advance);
            return (error, advance.Value);
        }
    }

    [Test]
    public async Task ASecondSize_ScalesTheFaceUntilTheFirstIsActivatedAgain()
    {
        using var open = new OpenFace();
        FT_Size_Metrics first,
            second,
            firstAgain;
        bool switched,
            switchedBack;
        FT_Error done;
        unsafe
        {
            OpenFace.Check(
                FT.FT_Set_Char_Size(open.Face, new CLong(0), new CLong(16 * 64), 72, 72)
            );
            var initial = open.Face->size;
            first = initial->metrics;
            var other = NewSize(open.Face);
            OpenFace.Check(FT.FT_Activate_Size(other));
            OpenFace.Check(
                FT.FT_Set_Char_Size(open.Face, new CLong(0), new CLong(32 * 64), 72, 72)
            );
            switched = open.Face->size == other;
            second = open.Face->size->metrics;
            OpenFace.Check(FT.FT_Activate_Size(initial));
            switchedBack = open.Face->size == initial;
            firstAgain = open.Face->size->metrics;
            done = FT.FT_Done_Size(other);
        }
        var scale = (16 * 64 * 65536L + Tables.UnitsPerEm / 2) / Tables.UnitsPerEm;
        var height = Tables.Ascender - Tables.Descender + Tables.LineGap;
        nint Scaled(int units) => (nint)(units * scale / 65536);

        await Assert.That((first.x_ppem, first.y_ppem)).IsEqualTo(((ushort)16, (ushort)16));
        await Assert.That((long)first.x_scale.Value).IsEqualTo(scale);
        await Assert.That((long)first.y_scale.Value).IsEqualTo(scale);
        await Assert
            .That(first.ascender.Value)
            .IsBetween(Scaled(Tables.Ascender) - 64, Scaled(Tables.Ascender) + 64);
        await Assert
            .That(first.descender.Value)
            .IsBetween(Scaled(Tables.Descender) - 64, Scaled(Tables.Descender) + 64);
        await Assert.That(first.height.Value).IsBetween(Scaled(height) - 64, Scaled(height) + 64);
        await Assert
            .That(first.max_advance.Value)
            .IsBetween(Scaled(Tables.AdvanceWidthMax) - 64, Scaled(Tables.AdvanceWidthMax) + 64);
        await Assert.That(switched).IsTrue();
        await Assert.That((second.x_ppem, second.y_ppem)).IsEqualTo(((ushort)32, (ushort)32));
        await Assert.That(switchedBack).IsTrue();
        await Assert.That(firstAgain.x_ppem).IsEqualTo((ushort)16);
        await Assert.That(done).IsEqualTo(FT_Error.FT_Err_Ok);

        static unsafe FT_SizeRec* NewSize(FT_FaceRec* face)
        {
            FT_SizeRec* size;
            OpenFace.Check(FT.FT_New_Size(face, &size));
            return size;
        }
    }

    [Test]
    public async Task AHintedGlyph_RendersOneBitWhereItsMetricsPlaceIt()
    {
        using var open = new OpenFace();
        FT_Error property,
            rendered;
        FT_GlyphSlotRec slot;
        (nint XMin, nint YMin, nint XMax, nint YMax) hinted = (
            nint.MaxValue,
            nint.MaxValue,
            nint.MinValue,
            nint.MinValue
        );
        var inked = 0;
        unsafe
        {
            property = UseInterpreter35(open.Library);
            OpenFace.Check(
                FT.FT_Set_Char_Size(open.Face, new CLong(0), new CLong(16 * 64), 72, 72)
            );
            var index = FT.FT_Get_Char_Index(open.Face, new CULong('H'));
            OpenFace.Check(FT.FT_Load_Glyph(open.Face, index, FT_LOAD.FT_LOAD_DEFAULT));
            var outline = open.Face->glyph->outline;
            for (var i = 0; i < outline.n_points; i++)
            {
                var point = outline.points[i];
                hinted = (
                    Math.Min(hinted.XMin, point.x.Value),
                    Math.Min(hinted.YMin, point.y.Value),
                    Math.Max(hinted.XMax, point.x.Value),
                    Math.Max(hinted.YMax, point.y.Value)
                );
            }
            rendered = FT.FT_Render_Glyph(open.Face->glyph, FT_Render_Mode.FT_RENDER_MODE_MONO);
            slot = *open.Face->glyph;
            for (var row = 0; row < slot.bitmap.rows; row++)
            {
                foreach (
                    var bits in new ReadOnlySpan<byte>(
                        slot.bitmap.buffer + row * slot.bitmap.pitch,
                        slot.bitmap.pitch
                    )
                )
                    inked += System.Numerics.BitOperations.PopCount(bits);
            }
        }

        await Assert.That(property).IsEqualTo(FT_Error.FT_Err_Ok);
        await Assert.That(rendered).IsEqualTo(FT_Error.FT_Err_Ok);
        await Assert.That(slot.format).IsEqualTo(FT_Glyph_Format.FT_GLYPH_FORMAT_BITMAP);
        await Assert.That(slot.bitmap.pixel_mode).IsEqualTo(FT_Pixel_Mode.FT_PIXEL_MODE_MONO);
        // FreeType rounds a one-bit glyph's 26.6 box in to the pixels whose centres it covers.
        var (left, top) = ((hinted.XMin + 31) >> 6, (hinted.YMax + 32) >> 6);
        await Assert.That((nint)slot.bitmap_left).IsEqualTo(left);
        await Assert.That((nint)slot.bitmap_top).IsEqualTo(top);
        await Assert.That((nint)slot.bitmap.width).IsEqualTo(((hinted.XMax + 32) >> 6) - left);
        await Assert.That((nint)slot.bitmap.rows).IsEqualTo(top - ((hinted.YMin + 31) >> 6));
        await Assert.That(slot.bitmap.pitch).IsEqualTo((int)(slot.bitmap.width + 15) / 16 * 2);
        await Assert
            .That(inked)
            .IsGreaterThan(0)
            .And.IsLessThan((int)(slot.bitmap.width * slot.bitmap.rows));
        await Assert.That(slot.advance.x.Value % 64).IsEqualTo(0);

        static unsafe FT_Error UseInterpreter35(FT_LibraryRec* library)
        {
            var version = 35u;
            fixed (byte* module = "truetype\0"u8)
            fixed (byte* name = "interpreter-version\0"u8)
                return FT.FT_Property_Set(library, module, name, &version);
        }
    }
}
