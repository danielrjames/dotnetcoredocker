namespace core.Domain.Entities.Auth
{
    public class TokenConfig
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
