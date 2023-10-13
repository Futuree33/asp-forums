using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Database;

namespace WebApplication1.Data.Attributes;

public class Unique : ValidationAttribute
{
    private readonly string _column;
    private readonly string _table;

    public Unique(string table, string column)
    {
        _column = column;
        _table = table;
    }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var context = (DatabaseContext)validationContext.GetService(typeof(DatabaseContext))!;
        
        var results = context.Database
            .SqlQueryRaw<string>("SELECT * FROM " + _table + " WHERE " + _column + " = {0}", value.ToString())
            .ToList();

        return results.Count == 0
            ? ValidationResult.Success
            : new ValidationResult( _column + " already exists");
    }
}

