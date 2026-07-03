# ERP Inteligente para Despachos Contables
## Documento 7: Sprint 0 — Estructura del Backend y Setup Local

---

## 1. Prerrequisitos — instala esto primero

| Herramienta | Para qué | Verificación |
|---|---|---|
| .NET 8 SDK | Compilar y correr el backend | `dotnet --version` → debe mostrar 8.x |
| Docker Desktop | Levantar PostgreSQL local sin instalarlo directo en tu máquina | `docker --version` |
| Un cliente de base de datos (opcional pero recomendado) | Ver las tablas visualmente — ej. DBeaver o pgAdmin | — |

Si `dotnet --version` no funciona, descarga el SDK desde https://dotnet.microsoft.com/download — instala la versión **8.0 LTS**, no una preview.

Si `docker --version` no funciona, instala Docker Desktop desde https://www.docker.com/products/docker-desktop — en Windows te va a pedir WSL2, acéptalo (es el subsistema de Linux que Docker necesita).

---

## 2. La estructura de carpetas — Clean Architecture, explicada

La idea central de Clean Architecture: **el código de negocio no debe saber nada de la base de datos ni de la web.** Se organiza en capas, y las capas internas nunca dependen de las externas.

```
erp-contable/
├── src/
│   ├── ERPContable.Domain/          ← Capa 1: el corazón, sin dependencias externas
│   ├── ERPContable.Application/     ← Capa 2: casos de uso, depende solo de Domain
│   ├── ERPContable.Infrastructure/  ← Capa 3: base de datos, storage, depende de Application
│   └── ERPContable.API/             ← Capa 4: controladores web, depende de todas
├── tests/
│   ├── ERPContable.Domain.Tests/
│   ├── ERPContable.Application.Tests/
│   └── ERPContable.API.IntegrationTests/
└── ERPContable.sln
```

**¿Por qué importa esto y no es solo burocracia?** Porque si mañana cambias PostgreSQL por otra base de datos, o agregas la app móvil, **solo tocas `Infrastructure` o agregas otro proyecto de cliente** — la lógica de negocio en `Domain` y `Application` no se entera del cambio. Es la diferencia entre un cambio de una tarde y una reescritura de tres semanas.

### ¿Qué va en cada capa?

- **Domain**: las entidades del modelo de datos (documento 2) como clases C# puras — `Client`, `Document`, `Task`, etc. Sin Entity Framework, sin nada de infraestructura. Es lógica de negocio pura.
- **Application**: los casos de uso — "dar de alta un cliente", "generar obligaciones fiscales". Define *interfaces* de lo que necesita (ej. `IClientRepository`) sin implementarlas.
- **Infrastructure**: implementa esas interfaces usando Entity Framework Core + PostgreSQL, y también el acceso a blob storage, envío de correos, etc.
- **API**: los controladores REST (los endpoints del documento 6), que reciben el HTTP request y llaman a `Application`.

---

## 3. Crear la solución y los proyectos

Todo esto se corre desde la raíz de tu repo (`ERP_CONTABLE/`), en Git Bash.

### 3.1 Crear el archivo de solución

```bash
dotnet new sln -n ERPContable
```

Esto crea `ERPContable.sln` — es el archivo que agrupa todos los proyectos, para poder compilarlos juntos.

### 3.2 Crear cada proyecto

```bash
dotnet new classlib -n ERPContable.Domain -o src/ERPContable.Domain
dotnet new classlib -n ERPContable.Application -o src/ERPContable.Application
dotnet new classlib -n ERPContable.Infrastructure -o src/ERPContable.Infrastructure
dotnet new webapi -n ERPContable.API -o src/ERPContable.API
```

`classlib` = librería de clases (sin punto de entrada propio). `webapi` = proyecto de API con Swagger incluido por defecto.

### 3.3 Crear los proyectos de pruebas

```bash
dotnet new xunit -n ERPContable.Domain.Tests -o tests/ERPContable.Domain.Tests
dotnet new xunit -n ERPContable.Application.Tests -o tests/ERPContable.Application.Tests
```

`xunit` es el framework de pruebas más común en .NET.

### 3.4 Agregar todos los proyectos a la solución

```bash
dotnet sln add src/ERPContable.Domain/ERPContable.Domain.csproj
dotnet sln add src/ERPContable.Application/ERPContable.Application.csproj
dotnet sln add src/ERPContable.Infrastructure/ERPContable.Infrastructure.csproj
dotnet sln add src/ERPContable.API/ERPContable.API.csproj
dotnet sln add tests/ERPContable.Domain.Tests/ERPContable.Domain.Tests.csproj
dotnet sln add tests/ERPContable.Application.Tests/ERPContable.Application.Tests.csproj
```

### 3.5 Conectar las capas entre sí (referencias de proyecto)

Esto es lo que **impone** la regla de Clean Architecture a nivel de compilador — si intentas romper la regla (ej. que `Domain` dependa de `Infrastructure`), el proyecto simplemente no compila.

```bash
# Application depende de Domain
dotnet add src/ERPContable.Application reference src/ERPContable.Domain

# Infrastructure depende de Application (y transitivamente de Domain)
dotnet add src/ERPContable.Infrastructure reference src/ERPContable.Application

# API depende de Application e Infrastructure
dotnet add src/ERPContable.API reference src/ERPContable.Application
dotnet add src/ERPContable.API reference src/ERPContable.Infrastructure

# Los proyectos de test referencian lo que están probando
dotnet add tests/ERPContable.Domain.Tests reference src/ERPContable.Domain
dotnet add tests/ERPContable.Application.Tests reference src/ERPContable.Application
```

### 3.6 Verifica que todo compila

```bash
dotnet build
```

Debe terminar con `Build succeeded` y 0 errores. Si algo falla aquí, revisa que las rutas de los comandos anteriores coincidan exactamente con tus carpetas.

---

## 4. Levantar PostgreSQL localmente con Docker

En vez de instalar PostgreSQL directo en tu máquina, usamos Docker — así tu base de datos local es idéntica a la de cualquier otro ambiente, y puedes borrarla y recrearla sin miedo.

### 4.1 Crea un `docker-compose.yml` en la raíz del repo

```yaml
services:
  postgres:
    image: postgres:16
    container_name: erp-contable-db
    environment:
      POSTGRES_USER: erp_dev
      POSTGRES_PASSWORD: dev_password_local
      POSTGRES_DB: erp_contable_dev
    ports:
      - "5432:5432"
    volumes:
      - erp_contable_data:/var/lib/postgresql/data

volumes:
  erp_contable_data:
```

**Nota:** esta contraseña es solo para tu máquina local, nunca para producción — por eso está bien que viva en un archivo versionado. Las credenciales reales de producción van en variables de entorno del servidor, nunca en el repo.

### 4.2 Levanta el contenedor

```bash
docker compose up -d
```

`-d` = "detached", corre en segundo plano. Verifica que quedó arriba:

```bash
docker ps
```

Deberías ver `erp-contable-db` en la lista, con estado `Up`.

### 4.3 Para apagarlo cuando no lo uses

```bash
docker compose down
```

(Tus datos persisten gracias al `volume` — no se borran al apagar el contenedor, solo si borras el volumen explícitamente.)

---

## 5. Conectar el backend a la base de datos

### 5.1 Instala los paquetes de Entity Framework Core en `Infrastructure`

```bash
cd src/ERPContable.Infrastructure
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
cd ../..
```

### 5.2 Configura la cadena de conexión — nunca hardcodeada

En `.NET`, el equivalente al `.env` que ya usas es **User Secrets** para desarrollo local (mantiene el secreto fuera del repo incluso sin depender de `.gitignore`):

```bash
cd src/ERPContable.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=erp_contable_dev;Username=erp_dev;Password=dev_password_local"
cd ../..
```

Esto guarda el secreto en un archivo fuera del repositorio (en tu carpeta de usuario de Windows), no en `appsettings.json`.

### 5.3 Verifica que compila con las nuevas referencias

```bash
dotnet build
```

---

## 6. Primera migración y arranque del proyecto

Este paso ya depende de tener al menos una entidad definida en `Domain` y un `DbContext` en `Infrastructure` — que es justo el primer trabajo real del Sprint 1. Por ahora, el objetivo del Sprint 0 es que:

```bash
dotnet run --project src/ERPContable.API
```

levante el servidor y puedas entrar a `https://localhost:xxxx/swagger` (el puerto exacto lo indica la consola al arrancar) y ver la página de Swagger, aunque todavía no tenga endpoints propios — solo el que trae `webapi` por defecto (`WeatherForecast`, de ejemplo).

**Ese es tu criterio de éxito del Sprint 0:** solución compilando, PostgreSQL corriendo en Docker, backend arrancando y Swagger visible en el navegador.

---

## 7. Actualiza tu `.gitignore` y sube esto al repo

Tu `.gitignore` ya tiene cubierto `bin/`, `obj/` y `appsettings.Development.json` — pero agrega esto si no está:

```
# Docker
*.env.docker

# User secrets nunca se versionan (viven fuera del repo, pero por si acaso)
**/secrets.json
```

Y el flujo de siempre para subir todo esto:

```bash
git add .
git status
git commit -m "chore: scaffolding inicial del backend (.NET, Clean Architecture) + docker-compose para PostgreSQL"
git push
```

---

## 8. Siguiente paso

Con el backend arrancando y la base de datos corriendo, el siguiente trabajo real es empezar el **Sprint 1**: implementar las entidades de `Domain` para Usuario/Rol/Permiso, el `DbContext` en `Infrastructure`, y los primeros endpoints de autenticación del documento 6.

¿Quieres que preparemos ese arranque del Sprint 1 (la primera entidad + el DbContext + la primera migración real), o prefieres primero terminar de correr y confirmar que todo el Sprint 0 te funcionó?
