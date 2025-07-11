using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string? ConnectionId { get; set; }
    public Chat Chat { get; set; }
    public List<Message> Messages { get; set; }
}