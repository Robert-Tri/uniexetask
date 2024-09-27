using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uniexetask.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CAMPUS",
                columns: table => new
                {
                    campus_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    campus_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    campus_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CAMPUS__01989FD1D20DE96D", x => x.campus_id);
                });

            migrationBuilder.CreateTable(
                name: "EVENT",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    location = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    reg_url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EVENT__2370F7279EC27DDB", x => x.event_id);
                });

            migrationBuilder.CreateTable(
                name: "LABEL",
                columns: table => new
                {
                    label_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    label_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LABEL__E44FFA584979913C", x => x.label_id);
                });

            migrationBuilder.CreateTable(
                name: "PERMISSION",
                columns: table => new
                {
                    permission_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PERMISSI__E5331AFAA7F0158F", x => x.permission_id);
                });

            migrationBuilder.CreateTable(
                name: "ROLE",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ROLE__760965CC64BDA0EE", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "SUBJECT",
                columns: table => new
                {
                    subject_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subject_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    subject_name = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    description = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SUBJECT__5004F6608A050065", x => x.subject_id);
                });

            migrationBuilder.CreateTable(
                name: "ROLE_PERMISSION",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false),
                    permission_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ROLE_PER__C85A546393D53628", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK__ROLE_PERM__permi__440B1D61",
                        column: x => x.permission_id,
                        principalTable: "PERMISSION",
                        principalColumn: "permission_id");
                    table.ForeignKey(
                        name: "FK__ROLE_PERM__role___4316F928",
                        column: x => x.role_id,
                        principalTable: "ROLE",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    campus_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__USER__B9BE370FA12F3B22", x => x.user_id);
                    table.ForeignKey(
                        name: "FK__USER__campus_id__3F466844",
                        column: x => x.campus_id,
                        principalTable: "CAMPUS",
                        principalColumn: "campus_id");
                    table.ForeignKey(
                        name: "FK__USER__role_id__403A8C7D",
                        column: x => x.role_id,
                        principalTable: "ROLE",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "PROJECT",
                columns: table => new
                {
                    project_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    topic_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    topic_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    subject_id = table.Column<int>(type: "int", nullable: false),
                    score = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PROJECT__BC799E1FC9EB9923", x => x.project_id);
                    table.ForeignKey(
                        name: "FK__PROJECT__subject__5629CD9C",
                        column: x => x.subject_id,
                        principalTable: "SUBJECT",
                        principalColumn: "subject_id");
                });

            migrationBuilder.CreateTable(
                name: "CHAT_GROUP",
                columns: table => new
                {
                    chat_group_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chatbox_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    owner_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CHAT_GRO__F18D35793DC5074C", x => x.chat_group_id);
                    table.ForeignKey(
                        name: "FK__CHAT_GROU__creat__48CFD27E",
                        column: x => x.created_by,
                        principalTable: "USER",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__CHAT_GROU__owner__49C3F6B7",
                        column: x => x.owner_id,
                        principalTable: "USER",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "MENTOR",
                columns: table => new
                {
                    mentor_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    specialty = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MENTOR__E5D27EF3F0FC653A", x => x.mentor_id);
                    table.ForeignKey(
                        name: "FK__MENTOR__user_id__73BA3083",
                        column: x => x.user_id,
                        principalTable: "USER",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "NOFITICATION",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sender_id = table.Column<int>(type: "int", nullable: false),
                    receiver_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NOFITICA__E059842FB5BB2374", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK__NOFITICAT__recei__0F624AF8",
                        column: x => x.receiver_id,
                        principalTable: "USER",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__NOFITICAT__sende__0E6E26BF",
                        column: x => x.sender_id,
                        principalTable: "USER",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "SPONSOR",
                columns: table => new
                {
                    sponsor_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    investment_field = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SPONSOR__BE37D454EF7E9B7D", x => x.sponsor_id);
                    table.ForeignKey(
                        name: "FK__SPONSOR__user_id__6C190EBB",
                        column: x => x.user_id,
                        principalTable: "USER",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "STUDENT",
                columns: table => new
                {
                    student_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    student_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    major = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__STUDENT__2A33069AD32336FB", x => x.student_id);
                    table.ForeignKey(
                        name: "FK__STUDENT__user_id__04E4BC85",
                        column: x => x.user_id,
                        principalTable: "USER",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "GROUP",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    project_id = table.Column<int>(type: "int", nullable: false),
                    group_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GROUP__D57795A0961D119E", x => x.group_id);
                    table.ForeignKey(
                        name: "FK__GROUP__project_i__7B5B524B",
                        column: x => x.project_id,
                        principalTable: "PROJECT",
                        principalColumn: "project_id");
                });

            migrationBuilder.CreateTable(
                name: "PROJECT_LABEL",
                columns: table => new
                {
                    project_id = table.Column<int>(type: "int", nullable: false),
                    label_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PROJECT___223D61BAFF76C671", x => new { x.project_id, x.label_id });
                    table.ForeignKey(
                        name: "FK__PROJECT_L__label__5FB337D6",
                        column: x => x.label_id,
                        principalTable: "LABEL",
                        principalColumn: "label_id");
                    table.ForeignKey(
                        name: "FK__PROJECT_L__proje__5EBF139D",
                        column: x => x.project_id,
                        principalTable: "PROJECT",
                        principalColumn: "project_id");
                });

            migrationBuilder.CreateTable(
                name: "REQUIREMENT",
                columns: table => new
                {
                    requirement_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    project_id = table.Column<int>(type: "int", nullable: false),
                    requestor_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__REQUIREM__2A73C1AD5181B1DB", x => x.requirement_id);
                    table.ForeignKey(
                        name: "FK__REQUIREME__proje__6383C8BA",
                        column: x => x.project_id,
                        principalTable: "PROJECT",
                        principalColumn: "project_id");
                });

            migrationBuilder.CreateTable(
                name: "RESOURCE",
                columns: table => new
                {
                    resource_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    project_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    upload_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RESOURCE__4985FC733E56EC39", x => x.resource_id);
                    table.ForeignKey(
                        name: "FK__RESOURCE__projec__6754599E",
                        column: x => x.project_id,
                        principalTable: "PROJECT",
                        principalColumn: "project_id");
                });

            migrationBuilder.CreateTable(
                name: "TASK",
                columns: table => new
                {
                    task_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    project_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    due_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TASK__0492148D926959F6", x => x.task_id);
                    table.ForeignKey(
                        name: "FK__TASK__project_id__59FA5E80",
                        column: x => x.project_id,
                        principalTable: "PROJECT",
                        principalColumn: "project_id");
                });

            migrationBuilder.CreateTable(
                name: "CHAT_MESSAGE",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chat_group_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    message_content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    send_datetime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CHAT_MES__0BBF6EE6642C1C94", x => x.message_id);
                    table.ForeignKey(
                        name: "FK__CHAT_MESS__chat___4D94879B",
                        column: x => x.chat_group_id,
                        principalTable: "CHAT_GROUP",
                        principalColumn: "chat_group_id");
                    table.ForeignKey(
                        name: "FK__CHAT_MESS__user___4E88ABD4",
                        column: x => x.user_id,
                        principalTable: "USER",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "PROJECT_MENTOR",
                columns: table => new
                {
                    project_id = table.Column<int>(type: "int", nullable: false),
                    mentor_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PROJECT___9224B9F0FC6D694F", x => new { x.project_id, x.mentor_id });
                    table.ForeignKey(
                        name: "FK__PROJECT_M__mento__778AC167",
                        column: x => x.mentor_id,
                        principalTable: "MENTOR",
                        principalColumn: "mentor_id");
                    table.ForeignKey(
                        name: "FK__PROJECT_M__proje__76969D2E",
                        column: x => x.project_id,
                        principalTable: "PROJECT",
                        principalColumn: "project_id");
                });

            migrationBuilder.CreateTable(
                name: "PROJECT_SPONSOR",
                columns: table => new
                {
                    project_id = table.Column<int>(type: "int", nullable: false),
                    sponsor_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PROJECT___E79AE35A7A565ABE", x => new { x.project_id, x.sponsor_id });
                    table.ForeignKey(
                        name: "FK__PROJECT_S__proje__6EF57B66",
                        column: x => x.project_id,
                        principalTable: "PROJECT",
                        principalColumn: "project_id");
                    table.ForeignKey(
                        name: "FK__PROJECT_S__spons__6FE99F9F",
                        column: x => x.sponsor_id,
                        principalTable: "SPONSOR",
                        principalColumn: "sponsor_id");
                });

            migrationBuilder.CreateTable(
                name: "GROUP_INVITE",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false),
                    notification_id = table.Column<int>(type: "int", nullable: false),
                    inviter_id = table.Column<int>(type: "int", nullable: false),
                    invitee_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GROUP_IN__3B720DE21B8DF914", x => new { x.group_id, x.notification_id });
                    table.ForeignKey(
                        name: "FK__GROUP_INV__group__14270015",
                        column: x => x.group_id,
                        principalTable: "GROUP",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "FK__GROUP_INV__notif__151B244E",
                        column: x => x.notification_id,
                        principalTable: "NOFITICATION",
                        principalColumn: "notification_id");
                });

            migrationBuilder.CreateTable(
                name: "GROUP_MEMBER",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GROUP_ME__67D4A5C909236B5D", x => new { x.group_id, x.student_id });
                    table.ForeignKey(
                        name: "FK__GROUP_MEM__group__08B54D69",
                        column: x => x.group_id,
                        principalTable: "GROUP",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "FK__GROUP_MEM__stude__09A971A2",
                        column: x => x.student_id,
                        principalTable: "STUDENT",
                        principalColumn: "student_id");
                });

            migrationBuilder.CreateTable(
                name: "MEETING_SCHEDULE",
                columns: table => new
                {
                    schedule_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<int>(type: "int", nullable: false),
                    mentor_id = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<int>(type: "int", nullable: false),
                    meeting_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    content = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MEETING___C46A8A6FE5A602ED", x => x.schedule_id);
                    table.ForeignKey(
                        name: "FK__MEETING_S__group__00200768",
                        column: x => x.group_id,
                        principalTable: "GROUP",
                        principalColumn: "group_id");
                    table.ForeignKey(
                        name: "FK__MEETING_S__mento__01142BA1",
                        column: x => x.mentor_id,
                        principalTable: "MENTOR",
                        principalColumn: "mentor_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHAT_GROUP_created_by",
                table: "CHAT_GROUP",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_CHAT_GROUP_owner_id",
                table: "CHAT_GROUP",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_CHAT_MESSAGE_chat_group_id",
                table: "CHAT_MESSAGE",
                column: "chat_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_CHAT_MESSAGE_user_id",
                table: "CHAT_MESSAGE",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_GROUP_project_id",
                table: "GROUP",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_GROUP_INVITE_notification_id",
                table: "GROUP_INVITE",
                column: "notification_id");

            migrationBuilder.CreateIndex(
                name: "IX_GROUP_MEMBER_student_id",
                table: "GROUP_MEMBER",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_MEETING_SCHEDULE_group_id",
                table: "MEETING_SCHEDULE",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_MEETING_SCHEDULE_mentor_id",
                table: "MEETING_SCHEDULE",
                column: "mentor_id");

            migrationBuilder.CreateIndex(
                name: "IX_MENTOR_user_id",
                table: "MENTOR",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_NOFITICATION_receiver_id",
                table: "NOFITICATION",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_NOFITICATION_sender_id",
                table: "NOFITICATION",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_subject_id",
                table: "PROJECT",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_LABEL_label_id",
                table: "PROJECT_LABEL",
                column: "label_id");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_MENTOR_mentor_id",
                table: "PROJECT_MENTOR",
                column: "mentor_id");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_SPONSOR_sponsor_id",
                table: "PROJECT_SPONSOR",
                column: "sponsor_id");

            migrationBuilder.CreateIndex(
                name: "IX_REQUIREMENT_project_id",
                table: "REQUIREMENT",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_RESOURCE_project_id",
                table: "RESOURCE",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_ROLE_PERMISSION_permission_id",
                table: "ROLE_PERMISSION",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_SPONSOR_user_id",
                table: "SPONSOR",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_STUDENT_user_id",
                table: "STUDENT",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_TASK_project_id",
                table: "TASK",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_USER_campus_id",
                table: "USER",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_USER_role_id",
                table: "USER",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UQ__USER__AB6E6164B010E360",
                table: "USER",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHAT_MESSAGE");

            migrationBuilder.DropTable(
                name: "EVENT");

            migrationBuilder.DropTable(
                name: "GROUP_INVITE");

            migrationBuilder.DropTable(
                name: "GROUP_MEMBER");

            migrationBuilder.DropTable(
                name: "MEETING_SCHEDULE");

            migrationBuilder.DropTable(
                name: "PROJECT_LABEL");

            migrationBuilder.DropTable(
                name: "PROJECT_MENTOR");

            migrationBuilder.DropTable(
                name: "PROJECT_SPONSOR");

            migrationBuilder.DropTable(
                name: "REQUIREMENT");

            migrationBuilder.DropTable(
                name: "RESOURCE");

            migrationBuilder.DropTable(
                name: "ROLE_PERMISSION");

            migrationBuilder.DropTable(
                name: "TASK");

            migrationBuilder.DropTable(
                name: "CHAT_GROUP");

            migrationBuilder.DropTable(
                name: "NOFITICATION");

            migrationBuilder.DropTable(
                name: "STUDENT");

            migrationBuilder.DropTable(
                name: "GROUP");

            migrationBuilder.DropTable(
                name: "LABEL");

            migrationBuilder.DropTable(
                name: "MENTOR");

            migrationBuilder.DropTable(
                name: "SPONSOR");

            migrationBuilder.DropTable(
                name: "PERMISSION");

            migrationBuilder.DropTable(
                name: "PROJECT");

            migrationBuilder.DropTable(
                name: "USER");

            migrationBuilder.DropTable(
                name: "SUBJECT");

            migrationBuilder.DropTable(
                name: "CAMPUS");

            migrationBuilder.DropTable(
                name: "ROLE");
        }
    }
}
