using System.ComponentModel.DataAnnotations;

namespace BlazorApiClient.Models;

public class AuthenticationModel
{
    [Required(ErrorMessage ="User name is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage ="Password is required")]
    public string? Password { get; set; }
}
