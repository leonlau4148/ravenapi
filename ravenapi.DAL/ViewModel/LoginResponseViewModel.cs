namespace BiondEocAPI.DAL.ViewModels;

public class LoginResponseViewModel
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}