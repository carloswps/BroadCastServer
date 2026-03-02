namespace BroadcastServer.Models;

public record LoginResponseDto(string Token, string Message);

public record LoginRequestDto(string Email, string Password);