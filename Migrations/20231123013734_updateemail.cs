using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ESPRESSO.Migrations
{
    /// <inheritdoc />
    public partial class updateemail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pagecounter",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    COUNT = table.Column<int>(type: "integer", nullable: false),
                    STREAMNAME = table.Column<string>(name: "STREAM_NAME", type: "text", nullable: true),
                    emailId = table.Column<string>(type: "text", nullable: true),
                    LASTUPDATEDDATEANDTIME = table.Column<DateTime>(name: "LAST_UPDATED_DATE_AND_TIME", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagecounter", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pagecounter");
        }
    }
}
