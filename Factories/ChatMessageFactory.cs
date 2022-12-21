namespace CoreMeridian.Factories;

internal sealed class ChatMessageFactory : IChatMessageFactory
{
    public ChatMessage CreateChatMessageFromLogString(string logMessage)
    {
        if (!logMessage.Contains("src=") &
            logMessage.Contains("src=-1") &
            logMessage.Contains("whisper"))
        {
            return default;
        }

        if (!int.TryParse(Regex.Match(logMessage, @"chl=([0-9]*)").Value.Replace("chl=", ""), out int messageChannel))
            return default;

        if (!int.TryParse(Regex.Match(logMessage, @"src=([0-9]*)").Value.Replace("src=", ""), out int roleId))
            return default;

        string text = Encoding.Unicode.GetString(Convert.FromBase64String(Regex.Match(logMessage, @"msg=([\s\S]*)").Value.Replace("msg=", "")));

        var chatMessage = new ChatMessage
        {
            Channel = (BroadcastChannel)messageChannel,
            RoleID = roleId,
            Text = text
        };

        return chatMessage;
    }
}
