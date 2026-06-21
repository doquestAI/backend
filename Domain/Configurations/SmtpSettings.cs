namespace Domain.Configurations;

public class SmtpSettings
{
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string User { get; set; } = string.Empty;
    public string Pass { get; set; } = string.Empty;
}