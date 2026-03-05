namespace BroadcastServer.Models;

public record PrivateMessageDto(int SenderId, string Content, int Timestamp);