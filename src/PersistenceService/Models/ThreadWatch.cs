using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[PrimaryKey(nameof(UserId), nameof(ThreadId))]
[Index(nameof(ThreadId))]
public class ThreadWatch
{
#pragma warning disable CS8618
    public Thread Thread { get; set; }
#pragma warning restore CS8618

    public Guid ThreadId { get; set; }

#pragma warning disable CS8618
    public User User { get; set; }
#pragma warning restore CS8618

    public Guid UserId { get; set; }
}
