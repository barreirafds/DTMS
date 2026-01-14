namespace DTMS.ViewModels
{
    public class UserVM
    {
        public int? Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}

