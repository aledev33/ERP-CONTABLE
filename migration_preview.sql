START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260707034845_AddUsuarioWithRLS') THEN
    CREATE TABLE usuarios (
        "Id" uuid NOT NULL,
        "NombreCompleto" character varying(255) NOT NULL,
        "Email" character varying(255) NOT NULL,
        "PasswordHash" text NOT NULL,
        "Activo" boolean NOT NULL,
        "TenantId" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone,
        CONSTRAINT "PK_usuarios" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260707034845_AddUsuarioWithRLS') THEN
    CREATE UNIQUE INDEX "IX_usuarios_TenantId_Email" ON usuarios ("TenantId", "Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260707034845_AddUsuarioWithRLS') THEN
    ALTER TABLE usuarios ENABLE ROW LEVEL SECURITY;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260707034845_AddUsuarioWithRLS') THEN
    ALTER TABLE usuarios FORCE ROW LEVEL SECURITY;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260707034845_AddUsuarioWithRLS') THEN

                    CREATE POLICY tenant_isolation_policy ON usuarios
                    USING ("TenantId" = current_setting('app.current_tenant_id', true)::uuid);
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260707034845_AddUsuarioWithRLS') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260707034845_AddUsuarioWithRLS', '8.0.11');
    END IF;
END $EF$;
COMMIT;

