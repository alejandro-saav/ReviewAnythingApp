namespace BlazorApp1.Models.Auth;

public class User
{
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? ProfileImage { get; set; }
    public string? Bio { get; set; }
}