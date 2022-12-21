namespace CoreMeridian.Utilities;

internal sealed class EncodingConvert
{
    public static string GB2312ToUtf8(byte[] gb2312bytes)
    {
        Encoding encoding = Encoding.GetEncoding("GB2312");
        Encoding uTF = Encoding.UTF8;

        return Convert(gb2312bytes, encoding, uTF);
    }

    private static string Convert(byte[] fromBytes, Encoding fromEncoding, Encoding toEncoding)
    {
        byte[] bytes = Encoding.Convert(fromEncoding, toEncoding, fromBytes);
        return toEncoding.GetString(bytes);
    }
}
