using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Archie.Api.Database.Migrations
{
    public partial class AddDueDateToWorkOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "WorkOrders");
        }
    }
}
