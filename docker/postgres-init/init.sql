-- Rol de aplicación con privilegio mínimo (usado por la API en tiempo de ejecución)
CREATE ROLE erp_app WITH LOGIN PASSWORD 'app_password_local';

GRANT CONNECT ON DATABASE erp_contable_dev TO erp_app;
GRANT USAGE ON SCHEMA public TO erp_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO erp_app;
GRANT SELECT, USAGE ON ALL SEQUENCES IN SCHEMA public TO erp_app;

-- Para que las tablas creadas por migraciones futuras también tengan estos permisos
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO erp_app;