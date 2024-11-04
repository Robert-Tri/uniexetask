using System.ComponentModel.DataAnnotations;

public class UserUpdateModel
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Full Name can't be longer than 100 characters.")]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string? Phone { get; set; }

    [Required]
    public int CampusId { get; set; }

    [Required]
    public int RoleId { get; set; }
}