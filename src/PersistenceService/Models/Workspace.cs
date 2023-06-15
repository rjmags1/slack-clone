using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

public class Workspace
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public File? Avatar { get; set; }

    [ForeignKey(nameof(Avatar))]
    public Guid? AvatarId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(TypeName = "timestamp")]
    public DateTime CreatedAt { get; set; }

#pragma warning disable CS8618
    [MaxLength(120)]
    public string Description { get; set; }

    [MaxLength(80)]
    public string Name { get; set; }
#pragma warning restore CS8618

    [DefaultValue(1)]
    public int NumMembers { get; set; }
}
