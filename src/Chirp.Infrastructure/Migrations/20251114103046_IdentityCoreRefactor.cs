using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IdentityCoreRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cheeps_Authors_AuthorId",
                table: "Cheeps");

            migrationBuilder.DropIndex(
                name: "IX_Cheeps_AuthorId",
                table: "Cheeps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Authors",
                newName: "UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Authors_Name",
                table: "Authors",
                newName: "IX_Authors_UserName");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId1",
                table: "Cheeps",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Authors",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Authors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Authors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Authors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Authors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Authors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Authors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Authors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Authors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Authors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Authors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Authors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Authors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Authors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Cheeps_AuthorId1",
                table: "Cheeps",
                column: "AuthorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheeps_Authors_AuthorId1",
                table: "Cheeps",
                column: "AuthorId1",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cheeps_Authors_AuthorId1",
                table: "Cheeps");

            migrationBuilder.DropIndex(
                name: "IX_Cheeps_AuthorId1",
                table: "Cheeps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "AuthorId1",
                table: "Cheeps");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "Firstname",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Authors",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Authors_UserName",
                table: "Authors",
                newName: "IX_Authors_Name");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Authors",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cheeps_AuthorId",
                table: "Cheeps",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheeps_Authors_AuthorId",
                table: "Cheeps",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
