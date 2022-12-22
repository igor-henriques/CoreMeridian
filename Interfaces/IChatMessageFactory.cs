namespace CoreMeridian.Interfaces;

internal interface IChatMessageFactory
{
    ChatMessage CreateChatMessageFromLogString(string logMessage, Encoding encoding);
}