using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPContable.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioWithRLS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NombreCompleto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_TenantId_Email",
                table: "usuarios",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            // --- Row-Level Security ---

            migrationBuilder.Sql("ALTER TABLE usuarios ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE usuarios FORCE ROW LEVEL SECURITY;");

            migrationBuilder.Sql(@"
                CREATE POLICY tenant_isolation_policy ON usuarios
                USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP POLICY IF EXISTS tenant_isolation_policy ON usuarios;");
            migrationBuilder.Sql("ALTER TABLE usuarios DISABLE ROW LEVEL SECURITY;");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}