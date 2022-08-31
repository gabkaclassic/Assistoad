using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssisToad.Entities;

[Table("users")]
public class User
{
    
    [Key]
    public long ChatId { get; set; }
    
    public List<Task> Tasks { get; set; }

    public List<Category> Categories { get; set; }

    [Column("settings")]
    public Settings Settings { get; set; }

    public User()
    {
        Settings = new Settings();
    }
}