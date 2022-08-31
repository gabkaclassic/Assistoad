using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssisToad.Entities;

[Table("categories")]
public class Category
{
    public static Category Unknown { get; private set; }

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column] 
    public string Title { get; set; }

    [Column] 
    public User Owner { get; set; }

    static Category()
    {
        Unknown = new Category("Unknown")
        {
            Id = 0,
        };
    }

    public Category()
    {
        
    }

    public Category(string title) : this()
    {
        Title = title;
    }

    public override bool Equals(object? obj)
    {
        Category other;
        try
        {
            other = (Category)obj!;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return Title.Equals(other.Title);
    }

    public override string ToString()
    {
        return Title;
    }
}