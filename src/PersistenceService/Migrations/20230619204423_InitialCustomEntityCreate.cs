using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersistenceService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCustomEntityCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelInvites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelInviteStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelInvites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Admin = table.Column<bool>(type: "boolean", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnableNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    LastViewedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Starred = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessageLaterFlags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelLaterFlagStatus = table.Column<int>(type: "integer", nullable: false),
                    ChannelMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMessageLaterFlags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessageMentions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    MentionedId = table.Column<Guid>(type: "uuid", nullable: false),
                    MentionerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMessageMentions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessageNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelMessageNotificationType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Seen = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMessageNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessageReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Emoji = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMessageReactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessageReplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageRepliedToId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepliedToId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMessageReplies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(2500)", maxLength: 2500, nullable: false),
                    ConcurrencyStamp = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Draft = table.Column<bool>(type: "boolean", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SentAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowThreads = table.Column<bool>(type: "boolean", nullable: false),
                    AvatarId = table.Column<Guid>(type: "uuid", nullable: true),
                    AllowedChannelPostersMask = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<byte[]>(type: "bytea", nullable: false),
                    Description = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    NumMembers = table.Column<int>(type: "integer", nullable: false),
                    Private = table.Column<bool>(type: "boolean", nullable: false),
                    Topic = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageGroupMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectMessageGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastViewedGroupMessagesAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageGroupMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageLaterFlags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    DirectMessageLaterFlagStatus = table.Column<int>(type: "integer", nullable: false),
                    DirectMessageGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageLaterFlags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectMessageLaterFlags_DirectMessageGroups_DirectMessageGr~",
                        column: x => x.DirectMessageGroupId,
                        principalTable: "DirectMessageGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageMentions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    MentionedId = table.Column<Guid>(type: "uuid", nullable: false),
                    MentionerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageMentions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectMessageNotificationType = table.Column<int>(type: "integer", nullable: false),
                    Seen = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Emoji = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageReactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageReplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepliedToId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageRepliedToId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageReplies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<byte[]>(type: "bytea", nullable: false),
                    Content = table.Column<string>(type: "character varying(2500)", maxLength: 2500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DirectMessageGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Draft = table.Column<bool>(type: "boolean", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ReplyToId = table.Column<Guid>(type: "uuid", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectMessages_DirectMessageGroups_DirectMessageGroupId",
                        column: x => x.DirectMessageGroupId,
                        principalTable: "DirectMessageGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessages_DirectMessages_ReplyToId",
                        column: x => x.ReplyToId,
                        principalTable: "DirectMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChannelMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    DirectMessageGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    StoreKey = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_ChannelMessages_ChannelMessageId",
                        column: x => x.ChannelMessageId,
                        principalTable: "ChannelMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Files_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Files_DirectMessageGroups_DirectMessageGroupId",
                        column: x => x.DirectMessageGroupId,
                        principalTable: "DirectMessageGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Files_DirectMessages_DirectMessageId",
                        column: x => x.DirectMessageId,
                        principalTable: "DirectMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AvatarId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserNotificationsPreferencesMask = table.Column<int>(type: "integer", nullable: false),
                    NotificationsAllowStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    NotificationsAllowEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    NotificationsPauseUntil = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    NotificationSound = table.Column<int>(type: "integer", nullable: false),
                    OnlineStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OnlineStatusUntil = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ThemeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Timezone = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    UserName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ConcurrencyStamp = table.Column<byte[]>(type: "bytea", nullable: false),
                    SecurityStamp = table.Column<byte[]>(type: "bytea", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Files_AvatarId",
                        column: x => x.AvatarId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Users_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Workspaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AvatarId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConcurrencyStamp = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Description = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    NumMembers = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspaces_Files_AvatarId",
                        column: x => x.AvatarId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Threads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<byte[]>(type: "bytea", nullable: false),
                    FirstMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumMessages = table.Column<int>(type: "integer", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Threads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Threads_ChannelMessages_FirstMessageId",
                        column: x => x.FirstMessageId,
                        principalTable: "ChannelMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Threads_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Threads_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceAdminPermissions",
                columns: table => new
                {
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<byte[]>(type: "bytea", nullable: false),
                    WorkspaceAdminPermissionsMask = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceAdminPermissions", x => new { x.AdminId, x.WorkspaceId });
                    table.ForeignKey(
                        name: "FK_WorkspaceAdminPermissions_Users_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceAdminPermissions_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceInvites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceInviteStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceInvites_Users_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceInvites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceInvites_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Admin = table.Column<bool>(type: "boolean", nullable: false),
                    AvatarId = table.Column<Guid>(type: "uuid", nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    NotificationsAllowTimeStart = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    NotificationsAllTimeEnd = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    NotificationSound = table.Column<int>(type: "integer", nullable: false),
                    OnlineStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OnlineStatusUntil = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Owner = table.Column<bool>(type: "boolean", nullable: false),
                    ThemeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceMembers_Files_AvatarId",
                        column: x => x.AvatarId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkspaceMembers_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkspaceMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceMembers_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceSearches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Query = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceSearches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceSearches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceSearches_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThreadWatches",
                columns: table => new
                {
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadWatches", x => new { x.UserId, x.ThreadId });
                    table.ForeignKey(
                        name: "FK_ThreadWatches_Threads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "Threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadWatches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelInvites_AdminId",
                table: "ChannelInvites",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelInvites_ChannelId",
                table: "ChannelInvites",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelInvites_ChannelInviteStatus",
                table: "ChannelInvites",
                column: "ChannelInviteStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelInvites_CreatedAt",
                table: "ChannelInvites",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelInvites_UserId_WorkspaceId",
                table: "ChannelInvites",
                columns: new[] { "UserId", "WorkspaceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelInvites_WorkspaceId",
                table: "ChannelInvites",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMembers_ChannelId_UserId",
                table: "ChannelMembers",
                columns: new[] { "ChannelId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMembers_UserId",
                table: "ChannelMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageLaterFlags_ChannelId",
                table: "ChannelMessageLaterFlags",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageLaterFlags_ChannelMessageId_UserId",
                table: "ChannelMessageLaterFlags",
                columns: new[] { "ChannelMessageId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageLaterFlags_UserId",
                table: "ChannelMessageLaterFlags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageLaterFlags_WorkspaceId_UserId",
                table: "ChannelMessageLaterFlags",
                columns: new[] { "WorkspaceId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageMentions_ChannelMessageId",
                table: "ChannelMessageMentions",
                column: "ChannelMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageMentions_CreatedAt",
                table: "ChannelMessageMentions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageMentions_MentionedId_ChannelMessageId_Mention~",
                table: "ChannelMessageMentions",
                columns: new[] { "MentionedId", "ChannelMessageId", "MentionerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageMentions_MentionerId",
                table: "ChannelMessageMentions",
                column: "MentionerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageNotifications_ChannelMessageId",
                table: "ChannelMessageNotifications",
                column: "ChannelMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageNotifications_CreatedAt",
                table: "ChannelMessageNotifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageNotifications_UserId_ChannelMessageId",
                table: "ChannelMessageNotifications",
                columns: new[] { "UserId", "ChannelMessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageReactions_ChannelMessageId_UserId",
                table: "ChannelMessageReactions",
                columns: new[] { "ChannelMessageId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageReactions_CreatedAt",
                table: "ChannelMessageReactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageReactions_UserId",
                table: "ChannelMessageReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageReplies_ChannelMessageId",
                table: "ChannelMessageReplies",
                column: "ChannelMessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageReplies_MessageRepliedToId",
                table: "ChannelMessageReplies",
                column: "MessageRepliedToId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageReplies_RepliedToId",
                table: "ChannelMessageReplies",
                column: "RepliedToId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageReplies_ReplierId",
                table: "ChannelMessageReplies",
                column: "ReplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessageReplies_ThreadId",
                table: "ChannelMessageReplies",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessages_ChannelId",
                table: "ChannelMessages",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessages_Deleted",
                table: "ChannelMessages",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessages_Draft",
                table: "ChannelMessages",
                column: "Draft");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessages_SentAt",
                table: "ChannelMessages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessages_ThreadId",
                table: "ChannelMessages",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMessages_UserId",
                table: "ChannelMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_AvatarId",
                table: "Channels",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_CreatedById",
                table: "Channels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Private",
                table: "Channels",
                column: "Private");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_WorkspaceId_Name",
                table: "Channels",
                columns: new[] { "WorkspaceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageGroupMembers_DirectMessageGroupId",
                table: "DirectMessageGroupMembers",
                column: "DirectMessageGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageGroupMembers_UserId_DirectMessageGroupId",
                table: "DirectMessageGroupMembers",
                columns: new[] { "UserId", "DirectMessageGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageGroups_WorkspaceId",
                table: "DirectMessageGroups",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageLaterFlags_DirectMessageGroupId",
                table: "DirectMessageLaterFlags",
                column: "DirectMessageGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageLaterFlags_DirectMessageId_UserId",
                table: "DirectMessageLaterFlags",
                columns: new[] { "DirectMessageId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageLaterFlags_UserId",
                table: "DirectMessageLaterFlags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageLaterFlags_WorkspaceId_UserId",
                table: "DirectMessageLaterFlags",
                columns: new[] { "WorkspaceId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageMentions_CreatedAt",
                table: "DirectMessageMentions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageMentions_DirectMessageId",
                table: "DirectMessageMentions",
                column: "DirectMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageMentions_MentionedId_DirectMessageId_Mentioner~",
                table: "DirectMessageMentions",
                columns: new[] { "MentionedId", "DirectMessageId", "MentionerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageMentions_MentionerId",
                table: "DirectMessageMentions",
                column: "MentionerId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageNotifications_CreatedAt",
                table: "DirectMessageNotifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageNotifications_DirectMessageId",
                table: "DirectMessageNotifications",
                column: "DirectMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageNotifications_UserId_DirectMessageId",
                table: "DirectMessageNotifications",
                columns: new[] { "UserId", "DirectMessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageReactions_CreatedAt",
                table: "DirectMessageReactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageReactions_DirectMessageId_UserId",
                table: "DirectMessageReactions",
                columns: new[] { "DirectMessageId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageReactions_UserId",
                table: "DirectMessageReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageReplies_DirectMessageId",
                table: "DirectMessageReplies",
                column: "DirectMessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageReplies_MessageRepliedToId",
                table: "DirectMessageReplies",
                column: "MessageRepliedToId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageReplies_RepliedToId",
                table: "DirectMessageReplies",
                column: "RepliedToId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessageReplies_ReplierId",
                table: "DirectMessageReplies",
                column: "ReplierId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_Deleted",
                table: "DirectMessages",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_DirectMessageGroupId",
                table: "DirectMessages",
                column: "DirectMessageGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_Draft",
                table: "DirectMessages",
                column: "Draft");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_ReplyToId",
                table: "DirectMessages",
                column: "ReplyToId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_SentAt",
                table: "DirectMessages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_UserId",
                table: "DirectMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ChannelId",
                table: "Files",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ChannelMessageId",
                table: "Files",
                column: "ChannelMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_DirectMessageGroupId",
                table: "Files",
                column: "DirectMessageGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_DirectMessageId",
                table: "Files",
                column: "DirectMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_UploadedAt",
                table: "Files",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_Name",
                table: "Themes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Threads_ChannelId",
                table: "Threads",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Threads_FirstMessageId",
                table: "Threads",
                column: "FirstMessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Threads_WorkspaceId",
                table: "Threads",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadWatches_ThreadId",
                table: "ThreadWatches",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AvatarId",
                table: "Users",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Deleted",
                table: "Users",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedUserName",
                table: "Users",
                column: "NormalizedUserName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ThemeId",
                table: "Users",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceAdminPermissions_WorkspaceId",
                table: "WorkspaceAdminPermissions",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceInvites_AdminId",
                table: "WorkspaceInvites",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceInvites_CreatedAt",
                table: "WorkspaceInvites",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceInvites_UserId",
                table: "WorkspaceInvites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceInvites_WorkspaceId",
                table: "WorkspaceInvites",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceInvites_WorkspaceInviteStatus",
                table: "WorkspaceInvites",
                column: "WorkspaceInviteStatus");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_AvatarId",
                table: "WorkspaceMembers",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_JoinedAt",
                table: "WorkspaceMembers",
                column: "JoinedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_ThemeId",
                table: "WorkspaceMembers",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_UserId_WorkspaceId",
                table: "WorkspaceMembers",
                columns: new[] { "UserId", "WorkspaceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_WorkspaceId_UserId",
                table: "WorkspaceMembers",
                columns: new[] { "WorkspaceId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_AvatarId",
                table: "Workspaces",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceSearches_CreatedAt",
                table: "WorkspaceSearches",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceSearches_UserId",
                table: "WorkspaceSearches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceSearches_WorkspaceId_UserId",
                table: "WorkspaceSearches",
                columns: new[] { "WorkspaceId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelInvites_Channels_ChannelId",
                table: "ChannelInvites",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelInvites_Users_AdminId",
                table: "ChannelInvites",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelInvites_Users_UserId",
                table: "ChannelInvites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelInvites_Workspaces_WorkspaceId",
                table: "ChannelInvites",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMembers_Channels_ChannelId",
                table: "ChannelMembers",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMembers_Users_UserId",
                table: "ChannelMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageLaterFlags_ChannelMessages_ChannelMessageId",
                table: "ChannelMessageLaterFlags",
                column: "ChannelMessageId",
                principalTable: "ChannelMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageLaterFlags_Channels_ChannelId",
                table: "ChannelMessageLaterFlags",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageLaterFlags_Users_UserId",
                table: "ChannelMessageLaterFlags",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageLaterFlags_Workspaces_WorkspaceId",
                table: "ChannelMessageLaterFlags",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageMentions_ChannelMessages_ChannelMessageId",
                table: "ChannelMessageMentions",
                column: "ChannelMessageId",
                principalTable: "ChannelMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageMentions_Users_MentionedId",
                table: "ChannelMessageMentions",
                column: "MentionedId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageMentions_Users_MentionerId",
                table: "ChannelMessageMentions",
                column: "MentionerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageNotifications_ChannelMessages_ChannelMessageId",
                table: "ChannelMessageNotifications",
                column: "ChannelMessageId",
                principalTable: "ChannelMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageNotifications_Users_UserId",
                table: "ChannelMessageNotifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageReactions_ChannelMessages_ChannelMessageId",
                table: "ChannelMessageReactions",
                column: "ChannelMessageId",
                principalTable: "ChannelMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageReactions_Users_UserId",
                table: "ChannelMessageReactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageReplies_ChannelMessages_ChannelMessageId",
                table: "ChannelMessageReplies",
                column: "ChannelMessageId",
                principalTable: "ChannelMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageReplies_ChannelMessages_MessageRepliedToId",
                table: "ChannelMessageReplies",
                column: "MessageRepliedToId",
                principalTable: "ChannelMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageReplies_Threads_ThreadId",
                table: "ChannelMessageReplies",
                column: "ThreadId",
                principalTable: "Threads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageReplies_Users_RepliedToId",
                table: "ChannelMessageReplies",
                column: "RepliedToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessageReplies_Users_ReplierId",
                table: "ChannelMessageReplies",
                column: "ReplierId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessages_Channels_ChannelId",
                table: "ChannelMessages",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessages_Threads_ThreadId",
                table: "ChannelMessages",
                column: "ThreadId",
                principalTable: "Threads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMessages_Users_UserId",
                table: "ChannelMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Files_AvatarId",
                table: "Channels",
                column: "AvatarId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Users_CreatedById",
                table: "Channels",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Workspaces_WorkspaceId",
                table: "Channels",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageGroupMembers_DirectMessageGroups_DirectMessage~",
                table: "DirectMessageGroupMembers",
                column: "DirectMessageGroupId",
                principalTable: "DirectMessageGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageGroupMembers_Users_UserId",
                table: "DirectMessageGroupMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageGroups_Workspaces_WorkspaceId",
                table: "DirectMessageGroups",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageLaterFlags_DirectMessages_DirectMessageId",
                table: "DirectMessageLaterFlags",
                column: "DirectMessageId",
                principalTable: "DirectMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageLaterFlags_Users_UserId",
                table: "DirectMessageLaterFlags",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageLaterFlags_Workspaces_WorkspaceId",
                table: "DirectMessageLaterFlags",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageMentions_DirectMessages_DirectMessageId",
                table: "DirectMessageMentions",
                column: "DirectMessageId",
                principalTable: "DirectMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageMentions_Users_MentionedId",
                table: "DirectMessageMentions",
                column: "MentionedId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageMentions_Users_MentionerId",
                table: "DirectMessageMentions",
                column: "MentionerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageNotifications_DirectMessages_DirectMessageId",
                table: "DirectMessageNotifications",
                column: "DirectMessageId",
                principalTable: "DirectMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageNotifications_Users_UserId",
                table: "DirectMessageNotifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageReactions_DirectMessages_DirectMessageId",
                table: "DirectMessageReactions",
                column: "DirectMessageId",
                principalTable: "DirectMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageReactions_Users_UserId",
                table: "DirectMessageReactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageReplies_DirectMessages_DirectMessageId",
                table: "DirectMessageReplies",
                column: "DirectMessageId",
                principalTable: "DirectMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageReplies_DirectMessages_MessageRepliedToId",
                table: "DirectMessageReplies",
                column: "MessageRepliedToId",
                principalTable: "DirectMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageReplies_Users_RepliedToId",
                table: "DirectMessageReplies",
                column: "RepliedToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessageReplies_Users_ReplierId",
                table: "DirectMessageReplies",
                column: "ReplierId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessages_Users_UserId",
                table: "DirectMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMessages_Channels_ChannelId",
                table: "ChannelMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Channels_ChannelId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Channels_ChannelId",
                table: "Threads");

            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMessages_Users_UserId",
                table: "ChannelMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectMessages_Users_UserId",
                table: "DirectMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectMessageGroups_Workspaces_WorkspaceId",
                table: "DirectMessageGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Workspaces_WorkspaceId",
                table: "Threads");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_ChannelMessages_FirstMessageId",
                table: "Threads");

            migrationBuilder.DropTable(
                name: "ChannelInvites");

            migrationBuilder.DropTable(
                name: "ChannelMembers");

            migrationBuilder.DropTable(
                name: "ChannelMessageLaterFlags");

            migrationBuilder.DropTable(
                name: "ChannelMessageMentions");

            migrationBuilder.DropTable(
                name: "ChannelMessageNotifications");

            migrationBuilder.DropTable(
                name: "ChannelMessageReactions");

            migrationBuilder.DropTable(
                name: "ChannelMessageReplies");

            migrationBuilder.DropTable(
                name: "DirectMessageGroupMembers");

            migrationBuilder.DropTable(
                name: "DirectMessageLaterFlags");

            migrationBuilder.DropTable(
                name: "DirectMessageMentions");

            migrationBuilder.DropTable(
                name: "DirectMessageNotifications");

            migrationBuilder.DropTable(
                name: "DirectMessageReactions");

            migrationBuilder.DropTable(
                name: "DirectMessageReplies");

            migrationBuilder.DropTable(
                name: "ThreadWatches");

            migrationBuilder.DropTable(
                name: "WorkspaceAdminPermissions");

            migrationBuilder.DropTable(
                name: "WorkspaceInvites");

            migrationBuilder.DropTable(
                name: "WorkspaceMembers");

            migrationBuilder.DropTable(
                name: "WorkspaceSearches");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Themes");

            migrationBuilder.DropTable(
                name: "Workspaces");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "DirectMessages");

            migrationBuilder.DropTable(
                name: "DirectMessageGroups");

            migrationBuilder.DropTable(
                name: "ChannelMessages");

            migrationBuilder.DropTable(
                name: "Threads");
        }
    }
}
