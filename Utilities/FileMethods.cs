namespace CoreMeridian.Utilities;

internal static class FileMethods
{
    public static long GetFileSize(string fileName)
    {
        return new FileInfo(fileName).Length;
    }
}
