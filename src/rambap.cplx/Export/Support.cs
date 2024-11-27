namespace rambap.cplx.Export;

/// <summary>
/// Support functions for <see cref="cplx.Export"/>
/// </summary>
internal static class Support
{
    // https://en.wikipedia.org/wiki/Filename#Reserved_characters_and_words
    private static string[] InvalidWindowsFilenames
        = ["CON","CONIN$","CONOUT$","PRN","AUX","CLOCK$","NUL",
           "COM0","COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9",
           "LPT0","LPT1","LPT2","LPT3","LPT4","LPT5","LPT6","LPT7","LPT8","LPT9"];

    public static string GetValidFilenameFrom(string unsanitizedFilename)
    {
        var sanitizedFilmename = unsanitizedFilename;
        var invalidChars = System.IO.Path.GetInvalidFileNameChars();
        foreach (var c in invalidChars)
            sanitizedFilmename = sanitizedFilmename.Replace(c, '_');

        if (InvalidWindowsFilenames.Contains(unsanitizedFilename))
            return "_" + sanitizedFilmename;
        else
            return sanitizedFilmename;

    }
}

