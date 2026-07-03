# ERP Inteligente para Despachos Contables
## Documento 6: Diseño de API

---

## 1. Principios de diseño

- **REST sobre HTTP/JSON.** Recursos como sustantivos (`/clients`, no `/getClients`), verbos HTTP para la acción (GET/POST/PUT/PATCH/DELETE).
- **API-first:** el backend no asume ningún cliente específico (MAUI, Blazor, futura app móvil) — cualquier cliente consume el mismo contrato.
- **Versionado explícito en la URL:** `/api/v1/...`. Cuando haya un cambio incompatible, se crea `/api/v2/...` en paralelo — nunca se rompe un contrato ya publicado sin versión nueva.
- **El tenant nunca va en la URL.** Se deriva del token de autenticación (claim `tenant_id`), no de un parámetro que el cliente pueda manipular — esto evita que un bug o un usuario malicioso pida datos de otro despacho cambiando un ID en la URL.

---

## 2. Autenticación y autorización

### 2.1 Flujo de login

```
POST /api/v1/auth/login
Body: { "email": "...", "password": "..." }

Respuesta si MFA está habilitado (RF-SEG-002):
200 OK
{ "mfaRequired": true, "mfaToken": "token-temporal-corto" }

POST /api/v1/auth/mfa/verify
Body: { "mfaToken": "...", "code": "123456" }

Respuesta final (con o sin MFA):
200 OK
{
  "accessToken": "jwt...",
  "refreshToken": "jwt...",
  "expiresIn": 3600,
  "user": { "id": "...", "nombre": "...", "role": "Contador" }
}
```

### 2.2 Autorización en cada request

Header estándar en todas las peticiones autenticadas:
```
Authorization: Bearer <accessToken>
```

El JWT contiene: `user_id`, `tenant_id`, `role`. El backend valida permisos por rol en cada endpoint según la matriz RBAC (documento 1, RF-SEG-001) — nunca confía en lo que el cliente dice que puede hacer, siempre revalida server-side.

### 2.3 Refresh de token

```
POST /api/v1/auth/refresh
Body: { "refreshToken": "..." }
```
Access token de vida corta (1h), refresh token de vida más larga (7–14 días), revocable desde el backend si se detecta actividad sospechosa (se apoya en `AuditLog`).

---

## 3. Convenciones de respuesta

### 3.1 Envelope estándar

Todas las respuestas exitosas siguen este formato:

```json
{
  "data": { },
  "meta": { }
}
```

Para listas, `meta` incluye paginación:

```json
{
  "data": [ ],
  "meta": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 143,
    "totalPages": 8
  }
}
```

### 3.2 Paginación — parámetros de query estándar

```
GET /api/v1/clients?page=1&pageSize=20&search=perez
```

### 3.3 Formato de error estándar

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "El RFC proporcionado no tiene un formato válido",
    "details": [
      { "field": "rfc", "issue": "formato_invalido" }
    ]
  }
}
```

| Código HTTP | Uso |
|---|---|
| 400 | Error de validación de datos enviados |
| 401 | No autenticado / token inválido o expirado |
| 403 | Autenticado pero sin permiso para esta acción (RBAC) |
| 404 | Recurso no encontrado (o existe pero pertenece a otro tenant — mismo código, no se revela la diferencia) |
| 409 | Conflicto (ej. RFC duplicado en el mismo tenant) |
| 422 | Entidad válida en formato pero inválida en reglas de negocio |
| 500 | Error interno — nunca debe exponer detalles internos/stack trace al cliente |

**Nota de seguridad:** un 404 se devuelve tanto si el recurso no existe como si existe pero pertenece a otro tenant. Esto evita que alguien pueda "adivinar" IDs válidos de otros despachos observando la diferencia entre 403 y 404.

---

## 4. Endpoints por módulo (alcance del MVP)

### 4.1 Usuarios y Seguridad (RF-SEG)

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/v1/users` | Lista usuarios del tenant (requiere rol Administrador/Socio) |
| POST | `/api/v1/users` | Crea usuario |
| GET | `/api/v1/users/{id}` | Detalle de usuario |
| PUT | `/api/v1/users/{id}` | Editar usuario |
| PATCH | `/api/v1/users/{id}/status` | Activar/desactivar usuario |
| GET | `/api/v1/roles` | Lista roles disponibles y sus permisos |
| GET | `/api/v1/audit-logs?entityType=&userId=&from=&to=` | Consulta de bitácora (RF-SEG-003), filtrable |

### 4.2 Clientes (RF-CLI)

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/v1/clients?search=&status=&page=` | Búsqueda/listado (RF-CLI-002) |
| POST | `/api/v1/clients` | Alta de cliente (RF-CLI-001) |
| GET | `/api/v1/clients/{id}` | Detalle básico |
| PUT | `/api/v1/clients/{id}` | Editar datos generales |
| PATCH | `/api/v1/clients/{id}/status` | Activar/desactivar (RF-CLI-004) |
| GET | `/api/v1/clients/{id}/summary` | Vista 360° — agrega documentos, obligaciones, tareas en una sola llamada (RF-CLI-003) |

**Ejemplo — alta de cliente:**
```json
POST /api/v1/clients
{
  "tipoPersona": "fisica",
  "nombreCompleto": "Juan Pérez López",
  "rfc": "PELJ800101ABC",
  "curp": "PELJ800101HDFRZN01",
  "regimenFiscal": "605",
  "actividadEconomica": "Servicios profesionales",
  "contactos": [
    { "tipo": "email", "valor": "juan@example.com", "esPrincipal": true },
    { "tipo": "telefono", "valor": "3312345678", "esPrincipal": true }
  ],
  "domicilio": {
    "calle": "Av. Vallarta", "numero": "123", "colonia": "Americana",
    "municipio": "Guadalajara", "estado": "Jalisco", "cp": "44160"
  }
}
```

### 4.3 Documentos (RF-DOC)

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/v1/clients/{clientId}/documents` | Documentos de un cliente |
| POST | `/api/v1/clients/{clientId}/documents` | Subir documento (multipart/form-data) — crea `Document` + primera `DocumentVersion` |
| GET | `/api/v1/documents/{id}` | Detalle del documento (cabecera) |
| POST | `/api/v1/documents/{id}/versions` | Subir nueva versión (RF-DOC-001) |
| GET | `/api/v1/documents/{id}/versions` | Historial de versiones (RF-DOC-001) |
| GET | `/api/v1/documents/{id}/versions/{versionId}/download` | Descargar versión específica |
| POST | `/api/v1/documents/{id}/tags` | Agregar etiquetas (RF-DOC-002) |
| GET | `/api/v1/documents/search?type=&clientId=&tag=&from=&to=` | Búsqueda avanzada (RF-DOC-003 — Fase 2, se deja el contrato listo desde ahora) |

### 4.4 Calendario Fiscal (RF-CAL)

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/v1/clients/{clientId}/obligations` | Obligaciones del cliente con su semáforo |
| POST | `/api/v1/clients/{clientId}/obligations/generate` | Genera obligaciones según régimen fiscal (RF-CAL-001) |
| PATCH | `/api/v1/obligations/{id}/complete` | Marca obligación como cumplida |
| GET | `/api/v1/obligation-types` | Catálogo de tipos (IVA, ISR, DIOT, etc.) |

**Ejemplo — respuesta de obligaciones con semáforo:**
```json
{
  "data": [
    {
      "id": "uuid",
      "tipo": "IVA",
      "fechaLimite": "2026-07-17",
      "status": "pendiente",
      "semaforo": "amarillo"
    }
  ]
}
```

### 4.5 Tareas (RF-TAR)

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/v1/tasks?assignedTo=&status=&clientId=` | Lista de tareas, filtrable (para el tablero kanban) |
| POST | `/api/v1/tasks` | Crear tarea |
| GET | `/api/v1/tasks/{id}` | Detalle |
| PATCH | `/api/v1/tasks/{id}/status` | Cambiar estado (drag-and-drop del kanban) |
| POST | `/api/v1/tasks/{id}/comments` | Agregar comentario |
| POST | `/api/v1/tasks/{id}/attachments` | Adjuntar archivo o vincular documento existente |

### 4.6 Fuera del MVP — contratos reservados (no se implementan aún, pero se documentan para no romper la numeración de versión después)

- `/api/v1/ocr/*` — Fase 2
- `/api/v1/templates/*`, `/api/v1/generated-documents/*` — Fase 2
- `/api/v1/dashboard/*` — Fase 2
- `/api/v1/ai/assistant`, `/api/v1/ai/risk-alerts` — Fase 3

---

## 5. Multi-tenancy en la práctica

Cada request autenticado lleva el `tenant_id` en el JWT. El backend aplica esto en **dos capas** (defensa en profundidad, no solo una):

1. **Capa de aplicación:** cada query a la base de datos filtra explícitamente por `tenant_id` del token.
2. **Capa de base de datos:** políticas Row-Level Security en PostgreSQL como respaldo — si por error humano una query en capa 1 olvida el filtro, la base de datos igual bloquea el acceso cruzado.

Esto es intencionalmente redundante: un solo punto de falla en el aislamiento de tenants es inaceptable para un sistema que maneja datos fiscales de terceros.

---

## 6. Seguridad adicional a nivel API

- **Rate limiting** por usuario/IP en endpoints de login (previene fuerza bruta sobre contraseñas).
- **Validación de entrada estricta** en cada endpoint — nunca confiar en que el frontend ya validó (el frontend valida por UX, el backend valida por seguridad).
- **CORS restringido** a los orígenes conocidos de los clientes oficiales (Blazor Web, futura versión).
- **Logs de acceso a datos sensibles:** cualquier GET a `/clients/{id}` con datos completos de RFC/CURP queda registrado en `AuditLog` con acción `ver_sensible`, no solo las escrituras.

---

## 7. Siguiente paso

Con el contrato de API definido, el Sprint 0 (scaffolding) ya tiene un objetivo claro: levantar el backend con estos endpoints devolviendo datos de ejemplo (mocks), antes de conectar la lógica de negocio real — así el frontend puede empezar a integrarse en paralelo sin esperar a que cada regla de negocio esté terminada.

¿Quieres que detallemos el **Sprint 0** con ese nivel de concreción (estructura de carpetas del backend, cómo levantar el proyecto localmente paso a paso), o prefieres que sigamos con otro documento?
