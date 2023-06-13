using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersistenceService.Models;

[PrimaryKey(nameof(AdminId), nameof(WorkspaceId))]
public class WorkspaceAdminPermissions
{
#pragma warning disable CS8618
    public User Admin { get; set; }
#pragma warning restore CS8618

    public Guid AdminId { get; set; }

    [DefaultValue(1)]
    public int WorkspaceAdminPermissionsMask { get; set; }

#pragma warning disable CS8618
    public Workspace Workspace { get; set; }
#pragma warning restore CS8618

    public Guid WorkspaceId { get; set; }
}
