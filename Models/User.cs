using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
  public class User
  {
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
  }
}

