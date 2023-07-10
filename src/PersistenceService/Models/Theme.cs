using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[Index(nameof(Name), IsUnique = true)]
public class Theme
{
    public Guid Id { get; set; }

    [MaxLength(40)]
#pragma warning disable CS8618
    public string Name { get; set; }
#pragma warning restore CS8618
}
