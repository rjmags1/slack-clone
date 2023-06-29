using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PersistenceService.Migrations
{
    /// <inheritdoc />
    public partial class InitialEntityCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
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
                    OnlineStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OnlineStatusUntil = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ThemeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Timezone = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    UserName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SecurityStamp = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "timestamp", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelInvites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelInviteStatus = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelInvites_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelInvites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Admin = table.Column<bool>(type: "boolean", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnableNotifications = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "true"),
                    LastViewedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Starred = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessageLaterFlags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
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
                    table.ForeignKey(
                        name: "FK_ChannelMessageLaterFlags_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessageMentions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ChannelMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    MentionedId = table.Column<Guid>(type: "uuid", nullable: false),
                    MentionerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMessageMentions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelMessageMentions_AspNetUsers_MentionedId",
                        column: x => x.MentionedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelMessageMentions_AspNetUsers_MentionerId",
                        column: x => x.MentionerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessageNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ChannelMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelMessageNotificationType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Seen = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMessageNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelMessageNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessageReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ChannelMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Emoji = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMessageReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelMessageReactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessageReplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ChannelMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageRepliedToId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepliedToId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelMessageReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelMessageReplies_AspNetUsers_RepliedToId",
                        column: x => x.RepliedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelMessageReplies_AspNetUsers_ReplierId",
                        column: x => x.ReplierId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(2500)", maxLength: 2500, nullable: false),
                    ConcurrencyStamp = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
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
                    table.ForeignKey(
                        name: "FK_ChannelMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    AllowThreads = table.Column<bool>(type: "boolean", nullable: false),
                    AvatarId = table.Column<Guid>(type: "uuid", nullable: true),
                    AllowedChannelPostersMask = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Description = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    NumMembers = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    Private = table.Column<bool>(type: "boolean", nullable: false),
                    Topic = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageGroupMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    DirectMessageGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastViewedGroupMessagesAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageGroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectMessageGroupMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ConcurrencyStamp = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ConcurrencyStamp = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
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
                        name: "FK_DirectMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "DirectMessageMentions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    MentionedId = table.Column<Guid>(type: "uuid", nullable: false),
                    MentionerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageMentions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectMessageMentions_AspNetUsers_MentionedId",
                        column: x => x.MentionedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessageMentions_AspNetUsers_MentionerId",
                        column: x => x.MentionerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessageMentions_DirectMessages_DirectMessageId",
                        column: x => x.DirectMessageId,
                        principalTable: "DirectMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectMessageNotificationType = table.Column<int>(type: "integer", nullable: false),
                    Seen = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectMessageNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessageNotifications_DirectMessages_DirectMessageId",
                        column: x => x.DirectMessageId,
                        principalTable: "DirectMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Emoji = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectMessageReactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessageReactions_DirectMessages_DirectMessageId",
                        column: x => x.DirectMessageId,
                        principalTable: "DirectMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectMessageReplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    DirectMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepliedToId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageRepliedToId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessageReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectMessageReplies_AspNetUsers_RepliedToId",
                        column: x => x.RepliedToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessageReplies_AspNetUsers_ReplierId",
                        column: x => x.ReplierId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessageReplies_DirectMessages_DirectMessageId",
                        column: x => x.DirectMessageId,
                        principalTable: "DirectMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessageReplies_DirectMessages_MessageRepliedToId",
                        column: x => x.MessageRepliedToId,
                        principalTable: "DirectMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
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
                name: "Workspaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    AvatarId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConcurrencyStamp = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Description = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    NumMembers = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1")
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
                name: "DirectMessageLaterFlags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
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
                        name: "FK_DirectMessageLaterFlags_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessageLaterFlags_DirectMessageGroups_DirectMessageGr~",
                        column: x => x.DirectMessageGroupId,
                        principalTable: "DirectMessageGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessageLaterFlags_DirectMessages_DirectMessageId",
                        column: x => x.DirectMessageId,
                        principalTable: "DirectMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectMessageLaterFlags_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Threads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    FirstMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumMessages = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "2"),
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
                    ConcurrencyStamp = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    WorkspaceAdminPermissionsMask = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceAdminPermissions", x => new { x.AdminId, x.WorkspaceId });
                    table.ForeignKey(
                        name: "FK_WorkspaceAdminPermissions_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
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
                        name: "FK_WorkspaceInvites_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceInvites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
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
                        name: "FK_WorkspaceMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "now()"),
                    Query = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceSearches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceSearches_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
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
                        name: "FK_ThreadWatches_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadWatches_Threads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "Threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AvatarId",
                table: "AspNetUsers",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Deleted",
                table: "AspNetUsers",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ThemeId",
                table: "AspNetUsers",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

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
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Files_AvatarId",
                table: "AspNetUsers",
                column: "AvatarId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelInvites_Channels_ChannelId",
                table: "ChannelInvites",
                column: "ChannelId",
                principalTable: "Channels",
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
                name: "FK_ChannelMessageNotifications_ChannelMessages_ChannelMessageId",
                table: "ChannelMessageNotifications",
                column: "ChannelMessageId",
                principalTable: "ChannelMessages",
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
                name: "FK_Channels_Files_AvatarId",
                table: "Channels",
                column: "AvatarId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

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
                name: "FK_DirectMessageGroups_Workspaces_WorkspaceId",
                table: "DirectMessageGroups",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMessages_AspNetUsers_UserId",
                table: "ChannelMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Channels_AspNetUsers_CreatedById",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectMessages_AspNetUsers_UserId",
                table: "DirectMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Files_AvatarId",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Files_AvatarId",
                table: "Workspaces");

            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMessages_Channels_ChannelId",
                table: "ChannelMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Channels_ChannelId",
                table: "Threads");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_Workspaces_WorkspaceId",
                table: "Threads");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_ChannelMessages_FirstMessageId",
                table: "Threads");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

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
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Themes");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "DirectMessages");

            migrationBuilder.DropTable(
                name: "DirectMessageGroups");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "Workspaces");

            migrationBuilder.DropTable(
                name: "ChannelMessages");

            migrationBuilder.DropTable(
                name: "Threads");
        }
    }
}
