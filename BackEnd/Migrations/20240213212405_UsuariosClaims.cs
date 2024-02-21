using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class UsuariosClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosClaims_Roles_RoleId",
                table: "UsuariosClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "UsuariosClaims",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosClaims_RoleId",
                table: "UsuariosClaims",
                newName: "IX_UsuariosClaims_UserId");

            migrationBuilder.CreateTable(
                name: "RolesClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolesClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolesClaims_RoleId",
                table: "RolesClaims",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosClaims_Usuarios_UserId",
                table: "UsuariosClaims",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosClaims_Usuarios_UserId",
                table: "UsuariosClaims");

            migrationBuilder.DropTable(
                name: "RolesClaims");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UsuariosClaims",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuariosClaims_UserId",
                table: "UsuariosClaims",
                newName: "IX_UsuariosClaims_RoleId");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosClaims_Roles_RoleId",
                table: "UsuariosClaims",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
