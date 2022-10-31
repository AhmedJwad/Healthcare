using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCare.API.Migrations
{
    public partial class userpationt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userPatientId",
                table: "patients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserPatients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPatients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPatients_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_patients_userPatientId",
                table: "patients",
                column: "userPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPatients_UserId",
                table: "UserPatients",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_patients_UserPatients_userPatientId",
                table: "patients",
                column: "userPatientId",
                principalTable: "UserPatients",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_patients_UserPatients_userPatientId",
                table: "patients");

            migrationBuilder.DropTable(
                name: "UserPatients");

            migrationBuilder.DropIndex(
                name: "IX_patients_userPatientId",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "userPatientId",
                table: "patients");
        }
    }
}
