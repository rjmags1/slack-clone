using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersistenceService.Models;

namespace PersistenceService.Data.ApplicationDb;

#pragma warning disable CS8618
public class ApplicationDbContext : DbContext
{
    DbSet<Channel> Channels { get; set; }
    DbSet<ChannelInvite> ChannelInvites { get; set; }
    DbSet<ChannelMember> ChannelMembers { get; set; }
    DbSet<ChannelMessage> ChannelMessages { get; set; }
    DbSet<ChannelMessageLaterFlag> ChannelMessageLaterFlags { get; set; }
    DbSet<ChannelMessageMention> ChannelMessageMentions { get; set; }
    DbSet<ChannelMessageNotification> ChannelMessageNotifications { get; set; }
    DbSet<ChannelMessageReaction> ChannelMessageReactions { get; set; }
    DbSet<ChannelMessageReply> ChannelMessageReplies { get; set; }
    DbSet<DirectMessage> DirectMessages { get; set; }
    DbSet<DirectMessageGroup> DirectMessageGroups { get; set; }
    DbSet<DirectMessageGroupMember> DirectMessageGroupMembers { get; set; }
    DbSet<DirectMessageLaterFlag> DirectMessageLaterFlags { get; set; }
    DbSet<DirectMessageMention> DirectMessageMentions { get; set; }
    DbSet<DirectMessageNotification> DirectMessageNotifications { get; set; }
    DbSet<DirectMessageReaction> DirectMessageReactions { get; set; }
    DbSet<DirectMessageReply> DirectMessageReplies { get; set; }
    DbSet<Models.File> Files { get; set; }
    DbSet<Theme> Themes { get; set; }
    DbSet<Models.Thread> Threads { get; set; }
    DbSet<ThreadWatch> ThreadWatches { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<Workspace> Workspaces { get; set; }
    DbSet<WorkspaceAdminPermissions> WorkspaceAdminPermissions { get; set; }
    DbSet<WorkspaceInvite> WorkspaceInvites { get; set; }
    DbSet<WorkspaceMember> WorkspaceMembers { get; set; }
    DbSet<WorkspaceSearch> WorkspaceSearches { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<ChannelMessage>()
            .HasMany(e => e.Replies)
            .WithOne(e => e.ChannelMessage)
            .HasForeignKey(e => e.ChannelMessageId)
            .IsRequired();

        modelBuilder
            .Entity<DirectMessage>()
            .HasMany(e => e.Replies)
            .WithOne(e => e.DirectMessage)
            .HasForeignKey(e => e.DirectMessageId)
            .IsRequired();

        modelBuilder
            .Entity<ChannelMessage>()
            .HasOne(e => e.Thread)
            .WithMany(e => e.Messages)
            .HasForeignKey(e => e.ThreadId);
    }
}

public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.Load();
        var optionsBuilder =
            new DbContextOptionsBuilder<ApplicationDbContext>();
        string connectionString =
            Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";
        optionsBuilder.UseNpgsql(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
