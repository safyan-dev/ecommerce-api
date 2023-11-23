namespace api.service.Shared.Models
{
    public record GenerateTokenResult(string AccessToken, DateTime ExpireAt);
}
