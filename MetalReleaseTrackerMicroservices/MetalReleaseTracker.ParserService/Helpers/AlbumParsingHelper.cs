using System.Globalization;
using MetalReleaseTracker.ParserService.Parsers.Dtos;
using MetalReleaseTracker.ParserService.Parsers.Models;

namespace MetalReleaseTracker.ParserService.Helpers;

public static class AlbumParsingHelper
{
    public static AlbumStatus? ParseAlbumStatus(string status)
    {
        return status switch
        {
            "New" => AlbumStatus.New,
            "Restock" => AlbumStatus.Restock,
            "Preorder" => AlbumStatus.PreOrder,
            _ => null
        };
    }

    public static AlbumMediaType? ParseMediaType(string mediaType)
    {
        return mediaType switch
        {
            "CD" => AlbumMediaType.CD,
            "LP" => AlbumMediaType.LP,
            "Tape" => AlbumMediaType.Tape,
            _ => null
        };
    }

    public static DateTime ParseYear(string year)
    {
        if (DateTime.TryParseExact(year?.Trim(), "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        return DateTime.MinValue;
    }

    public static float ParsePrice(string priceText)
    {
        if (!string.IsNullOrEmpty(priceText))
        {
            if (float.TryParse(priceText, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedPrice))
            {
                return parsedPrice;
            }
        }

        return 0.0f;
    }
}