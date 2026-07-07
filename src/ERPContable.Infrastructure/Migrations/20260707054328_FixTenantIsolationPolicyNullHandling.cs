using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPContable.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTenantIsolationPolicyNullHandling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP POLICY IF EXISTS tenant_isolation_policy ON usuarios;");
            migrationBuilder.Sql(@"
                CREATE POLICY tenant_isolation_policy ON usuarios
                USING (""TenantId"" = NULLIF(current_setting('app.current_tenant_id', true), '')::uuid);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP POLICY IF EXISTS tenant_isolation_policy ON usuarios;");
            migrationBuilder.Sql(@"
                CREATE POLICY tenant_isolation_policy ON usuarios
                USING (""TenantId"" = current_setting('app.current_tenant_id', true)::uuid);
            ");
        }
    }
}