namespace CoreMeridian.Utilities;

internal static class FileMethods
{
    public static void EnsureFileExists(string filePath)
    {
        if (!File.Exists(filePath))
            _ = File.Create(filePath);
    }

    public static long GetFileSize(string fileName)
    {
        return new FileInfo(fileName).Length;
    }

    public static List<string> ReadTail(string filename, long offset)
    {
        using FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        fileStream.Seek(offset * -1, SeekOrigin.End);

        var array = new byte[offset];
        fileStream.Read(array, 0, (int)offset);

        return EncodingConvert.GB2312ToUtf8(array)
            .Split('\n')
            .Where(line => !string.IsNullOrEmpty(line.Trim()))
            .ToList();
    }
}
