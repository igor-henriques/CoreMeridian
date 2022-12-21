namespace CoreMeridian.Models;

internal class CompiledLogoffRegex
{
    public readonly static Regex Regex = new Regex(@"写入用户([0-9]*)", RegexOptions.Compiled);
}
