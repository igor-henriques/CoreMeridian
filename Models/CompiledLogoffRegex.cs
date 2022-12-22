namespace CoreMeridian.Models;

internal sealed class CompiledLogoffRegex
{
    public readonly static Regex Regex = new Regex(@"rolelogout:userid=([0-9]*)", RegexOptions.Compiled);
}
