namespace Security.Jwt;

public class AccessToken
{
    #region Constructors

    public AccessToken()
    {
        Token = string.Empty;
    }

    public AccessToken(string token, DateTime expiration)
    {
        Token = token;
        Expiration = expiration;
    }

    #endregion

    #region Properties

    public string Token { get; set; }
    public DateTime Expiration { get; set; }

    #endregion
}
