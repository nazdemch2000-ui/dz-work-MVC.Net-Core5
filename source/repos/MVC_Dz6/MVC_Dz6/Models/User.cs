namespace MVC_Dz6.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string? CreditCardNumber { get; set; }
        public string? Website { get; set; }
        public bool TermsOfService { get; set; }
    }
}
