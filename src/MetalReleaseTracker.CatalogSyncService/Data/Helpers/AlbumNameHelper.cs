using System.Text.RegularExpressions;

namespace MetalReleaseTracker.CatalogSyncService.Data.Helpers;

public static class AlbumNameHelper
{
    public static string StripMediaSuffix(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        name = Regex.Replace(name, @"(\s+-\s+|/).*\b(CD|LP|Vinyl|Tape|Cassette|Musiccassette|Digipak|Digisleeve|Digipack)\b.*$", string.Empty);
        name = Regex.Replace(name, @"\s+(Digipak|Digisleeve|Digipack|Musiccassette)\b.*$", string.Empty);
        name = Regex.Replace(name, @"\s+[A-Z][A-Za-z\s,+-]*\b(CD|LP|Vinyl|Tape|Cassette|Musiccassette)\b.*$", string.Empty);
        name = Regex.Replace(name, @"\s+CD$", string.Empty);
        name = Regex.Replace(name, @"\s+EP$", string.Empty);

        return name.Trim();
    }
}
