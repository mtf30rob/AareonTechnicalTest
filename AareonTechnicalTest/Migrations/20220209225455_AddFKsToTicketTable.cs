using Microsoft.EntityFrameworkCore.Migrations;

namespace AareonTechnicalTest.Migrations
{
    public partial class AddFKsToTicketTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PersonId",
                table: "Tickets",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Persons_PersonId",
                table: "Tickets",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Persons_PersonId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_PersonId",
                table: "Tickets");
        }
    }
}
