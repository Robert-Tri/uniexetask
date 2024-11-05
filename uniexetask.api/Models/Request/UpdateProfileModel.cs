using System.ComponentModel.DataAnnotations;

public class UpdateProfileModel
{

    [Required]
    [StringLength(100, ErrorMessage = "Full Name can't be longer than 100 characters.")]
    public string FullName { get; set; } = null!;

    [Required]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string? Phone { get; set; }

}