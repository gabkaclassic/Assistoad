using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace AssisToad.Entities;

[Table("tasks")]
public class Task
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column]
    public User Owner { get; set; }

    // private DateTime deadline;
    //
    // [Column]
    // public string Deadline
    // {
    //     set
    //     {
    //         if(Owner != null)
    //             deadline = DateTime.Parse(value, Owner.Settings.GetDateFormat().FormatProvider); 
    //         
    //     }
    //     
    //     get => deadline.ToString(CultureInfo.InvariantCulture);
    // }

    [Column(TypeName = "date")] 
    public DateTime Deadline { get; set; }

    // public DateTime GetDeadline()
    // {
    //     return deadline;
    // }

    [Column] 
    public string Content { get; set; }

    [Column] 
    public string Title { get; set; }

    [Column] 
    public Category Category { set; get; }

    [Column]
    public Level Importance { set; get; }

    [Column] 
    public Status Status { set; get; }

    public Task()
    {
        Deadline = DateTime.Now;
        Content = "Homework";
        Category = Category.Unknown;
        Importance = Level.LOW;
        Status = Status.NONE;
    }

    public Task(DateTime date, string content, Category category, Level importance, Status status)
    {
        Deadline = date;
        Content = content;
        Category = category;
        Importance = importance;
        Status = status;
    }

    private string ImportanceToString(Level level)
    {
        return level switch
        {
            Level.LOW => "Low",
            Level.MEDIUM => "Medium",
            _ => "High"
        };
    }
    
    private string StatusToString(Status status)
    {
        return status switch
        {
            Status.DONE => "Done",
            Status.PAUSED => "Paused",
            Status.WORK => "At work",
            Status.TODO => "To do",
            _ => "-"
        };
    }

    public override string ToString()
    {
        return Owner.Settings.TaskViewPattern
            .Replace("{deadline}", Deadline.ToString(Owner.Settings.GetDateFormat().FormatProvider))
            .Replace("{importance}", ImportanceToString(Importance))
            .Replace("{status}", StatusToString(Status))
            .Replace("{category}", Category.ToString())
            .Replace("{title}", Title)
            .Replace("{task}", Content)
            .Replace("{nl}", "\n");
    }
}