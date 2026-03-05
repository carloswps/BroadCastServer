using System.Text.Json;
using System.Text.Json.Nodes;
using BroadcastServer.Utils.UI;

namespace BroadcastServer.Utils;

public static class JwtParser
{
    public static int GetUserIdFromToken(string jwtToken)
    {
        try
        {
            var parts = jwtToken.Split('.');
            if (parts.Length != 3) throw new ArgumentException("Invalid JWT token format");

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var json = JsonSerializer.Deserialize<JsonNode>(jsonBytes);

            if (json != null && json["nameid"] != null)
                if (int.TryParse(json["nameid"]!.ToString(), out var userId))
                    return userId;

            throw new Exception("Could not find 'nameid' claim in JWT token.");
        }
        catch (Exception ex)
        {
            Logger.Error("Error decoding JWT token: ", ex);
            throw;
        }
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}