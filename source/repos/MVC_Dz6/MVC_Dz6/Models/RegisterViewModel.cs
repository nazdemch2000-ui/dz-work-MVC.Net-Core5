using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MVC_Dz6.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Username cannot be longer than 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$",
            ErrorMessage = "Username can only contain letters, digits and underscore ( _ ).")]
        [Remote(action: "IsUserNameAvailable", controller: "Account",
            ErrorMessage = "This username is already taken.")]
        [Display(Name = "Username")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [StringLength(100, ErrorMessage = "Confirm password cannot be longer than 100 characters.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]
        [Display(Name = "Age")]
        public int? Age { get; set; }

        [CreditCard(ErrorMessage = "Invalid credit card number.")]
        [Display(Name = "Credit Card Number")]
        public string? CreditCardNumber { get; set; }

        [Url(ErrorMessage = "Invalid URL format (example: https://example.com).")]
        [Display(Name = "Website")]
        public string? Website { get; set; }

        [ValidateNever]
        [Display(Name = "I accept the Terms of Service")]
        public bool? TermsOfService { get; set; }
    }
}
