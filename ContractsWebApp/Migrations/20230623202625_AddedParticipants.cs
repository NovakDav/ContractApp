using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractsWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Users_ManagerId",
                table: "Contracts");

            migrationBuilder.DropTable(
                name: "ContractUser");

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    ParticipantID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContractID = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.ParticipantID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Users_ManagerId",
                table: "Contracts",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Users_ManagerId",
                table: "Contracts");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.CreateTable(
                name: "ContractUser",
                columns: table => new
                {
                    ContractsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParticipantsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractUser", x => new { x.ContractsId, x.ParticipantsId });
                    table.ForeignKey(
                        name: "FK_ContractUser_Contracts_ContractsId",
                        column: x => x.ContractsId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractUser_Users_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractUser_ParticipantsId",
                table: "ContractUser",
                column: "ParticipantsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Users_ManagerId",
                table: "Contracts",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
