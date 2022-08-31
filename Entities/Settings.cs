using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Microsoft.VisualBasic;

namespace AssisToad.Entities;

[Table("settings")]
public class Settings
{
    private static readonly string DEFAULT_PATTERN = "| {title} | {deadline} |" +
                                                     "\n{task}";
    
    private static readonly string DEFAULT_FORMAT = "g";

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("date_format")]
    public string DateTimeFormat
    {
        get => dateFormat.FormatString;
        set => dateFormat = new DateTimeFormat(value);
    }

    private DateTimeFormat dateFormat;

    public bool Notify { get; set; }

    public string TaskViewPattern { get; set; }

    public Settings()
    {
        TaskViewPattern = DEFAULT_PATTERN;
        DateTimeFormat = DEFAULT_FORMAT;
        Notify = false;
    }

    public DateTimeFormat GetDateFormat() => dateFormat;

    public void SetDateFormat(string pattern)
    {
        DateTimeFormat =  pattern;
    }
}