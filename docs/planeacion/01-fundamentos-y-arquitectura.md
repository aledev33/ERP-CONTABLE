# ERP Inteligente para Despachos Contables
## Documento de Fundamentos — Fase 1: Planeación y Arquitectura

---

## 1. Visión del Producto

Sistema integral (ERP vertical) para despachos contables que centraliza la gestión de clientes, expedientes digitales, obligaciones fiscales y tareas internas, con un motor de IA como capa transversal de asistencia y detección de riesgos. Diseñado desde el día uno como **multi-tenant** para evolucionar de herramienta interna a producto SaaS comercial.

**Principio rector:** la IA asiste y alerta; nunca decide ni ejecuta acciones fiscales o legales sin validación humana.

---

## 2. Alcance por Fases

| Fase | Alcance | Usuarios |
|---|---|---|
| Fase 1 (MVP) | Gestión de clientes, documentos, calendario fiscal, tareas | Socios y contadores |
| Fase 2 | OCR inteligente, generación de documentos, dashboard | Todo el equipo interno |
| Fase 3 | Asistente IA, detección de riesgos | Todo el equipo interno |
| Fase 4 | Portal de clientes, apps móviles | Clientes externos |
| Fase 5 | Multi-tenant SaaS, facturación, onboarding self-service | Otros despachos (mercado) |

---

## 3. Requisitos Funcionales (resumen por módulo)

Cada requisito se numerará `RF-MÓDULO-###` para trazabilidad hacia historias de usuario y pruebas.

### RF-CLI (Clientes)
- RF-CLI-001: Registrar expediente completo del cliente (persona física/moral) con los campos definidos.
- RF-CLI-002: Búsqueda por nombre, RFC, CURP, correo o teléfono con resultados en <1s (dataset <50k registros).
- RF-CLI-003: Vista 360° del cliente: datos generales, documentos, tareas, calendario fiscal, historial — en una sola pantalla.
- RF-CLI-004: Marcar cliente como Activo/Inactivo sin eliminar historial.

### RF-DOC (Documental)
- RF-DOC-001: Carga de documentos con versionado automático (no sobrescribe, conserva historial).
- RF-DOC-002: Clasificación y etiquetado, manual y automático.
- RF-DOC-003: Búsqueda avanzada por tipo, cliente, fecha, etiquetas, contenido (OCR indexado).

### RF-OCR (OCR Inteligente)
- RF-OCR-001: Detectar tipo de documento al cargarlo (INE, Constancia Fiscal, Acta, etc.).
- RF-OCR-002: Extraer campos clave y prellenar formularios del expediente.
- RF-OCR-003: Vincular automáticamente el documento al cliente correspondiente (por RFC/CURP detectado).
- RF-OCR-004: Flujo de corrección manual cuando la confianza de extracción sea baja.

### RF-GEN (Generación de Documentos)
- RF-GEN-001: Motor de plantillas dinámicas con variables ligadas al expediente del cliente.
- RF-GEN-002: Exportación a PDF y Word.
- RF-GEN-003: Historial de documentos generados por cliente.

### RF-CAL (Calendario Fiscal)
- RF-CAL-001: Calendario de obligaciones (mensuales/anuales) por régimen fiscal del cliente.
- RF-CAL-002: Semáforo de cumplimiento (verde/amarillo/rojo) por obligación.
- RF-CAL-003: Alertas automáticas configurables (email/push, con antelación configurable).

### RF-TAR (Tareas)
- RF-TAR-001: Asignación de tareas con responsable, prioridad, fecha límite, adjuntos y comentarios.
- RF-TAR-002: Tablero de estado (pendiente/en proceso/en revisión/terminado).
- RF-TAR-003: Notificaciones de vencimiento.

### RF-DSH (Dashboard)
- RF-DSH-001: KPIs en tiempo real: clientes activos, pendientes, documentos vencidos, e.firmas por vencer, productividad por empleado.

### RF-SEG (Seguridad y Permisos)
- RF-SEG-001: RBAC con roles Socio/Administrador/Contador/Auxiliar/Cliente, granular por módulo.
- RF-SEG-002: Autenticación multifactor obligatoria para roles internos.
- RF-SEG-003: Bitácora de auditoría inmutable (quién, qué, cuándo, desde dónde).
- RF-SEG-004: Cifrado en reposo y en tránsito de datos sensibles (RFC, CURP, documentos).

### RF-IA (Inteligencia Artificial)
- RF-IA-001: Asistente conversacional con acceso de solo lectura a los datos del despacho (vía RAG sobre la base de datos/documentos).
- RF-IA-002: Resumen ejecutivo automático de expediente.
- RF-IA-003: Generación de documentos a partir de instrucciones en lenguaje natural (usa el motor de plantillas RF-GEN).
- RF-IA-004: Motor de detección de riesgos: inconsistencias fiscales, documentación faltante, patrones inusuales — **solo genera alertas, nunca ejecuta acciones**.

---

## 4. Requisitos No Funcionales

| Categoría | Requisito |
|---|---|
| Disponibilidad | 99.5% uptime para versión SaaS (Fase 5); backups diarios desde MVP |
| Rendimiento | Búsquedas <1s, carga de expediente <2s con dataset típico |
| Escalabilidad | Arquitectura multi-tenant desde el diseño de base de datos, aunque el despliegue inicial sea single-tenant |
| Seguridad | Cumplimiento con manejo de datos personales (LFPDPPP en México), cifrado AES-256 en reposo, TLS 1.3 en tránsito |
| Auditoría | Todo cambio a datos fiscales/documentos debe quedar registrado de forma inmutable |
| Portabilidad | Backend desacoplado del cliente (API-first) para soportar Windows, Android, iOS y Web sin reescribir lógica de negocio |
| Mantenibilidad | Arquitectura por capas/Clean Architecture, cobertura de pruebas automatizadas >70% en lógica de negocio |
| Usabilidad | Curva de aprendizaje baja para contadores no técnicos; UI consistente entre plataformas |

---

## 5. Recomendación de Stack Tecnológico

Contexto clave para la decisión: **Windows es prioridad**, seguido de Android/iOS, con Web y **multi-tenant SaaS** como visión a mediano plazo. Esto favorece un ecosistema con soporte nativo fuerte en Windows y que no obligue a reescribir la app para cada plataforma.

### Opción recomendada — Ecosistema .NET

| Capa | Tecnología | Por qué |
|---|---|---|
| Backend / API | ASP.NET Core (.NET 8), Clean Architecture + DDD | API-first real, tipado fuerte, rendimiento alto, gran soporte empresarial, facilita multi-tenant |
| Base de datos | PostgreSQL | Open source, robusto, excelente soporte multi-tenant (schema-per-tenant o row-level), evita costos de licencia de SQL Server a futuro |
| Frontend Windows/Android/iOS | .NET MAUI | Un solo código base C# para las 3 plataformas prioritarias, look-and-feel nativo en Windows |
| Frontend Web (futuro) | Blazor (WASM o Server) | Reutiliza componentes y lógica C# del resto del stack |
| Almacenamiento de documentos | Azure Blob Storage (o S3-compatible) | Versionado nativo, escalable, integra bien con OCR |
| OCR | Azure AI Document Intelligence | Modelos preentrenados para documentos tipo INE/actas + capacidad de entrenar modelos custom para documentos fiscales mexicanos |
| Búsqueda avanzada | Azure AI Search / Elasticsearch | Búsqueda full-text sobre contenido OCR |
| IA conversacional / generación | API de Claude (Anthropic) vía llamadas HTTP desde el backend | Ya evaluado por el usuario; buen soporte para RAG y generación de documentos |
| Autenticación | ASP.NET Identity + MFA (o Azure AD B2C si se busca SaaS multi-tenant más adelante) | RBAC nativo, extensible a multi-tenant |
| Infraestructura | Azure (App Service / Container Apps) | Coherente con el resto del stack, buen soporte de compliance |

### Alternativa (open source / menor costo inicial)

| Capa | Tecnología |
|---|---|
| Backend | Node.js + NestJS (TypeScript) |
| Frontend multiplataforma | Flutter (Windows, Android, iOS, Web con un solo código) |
| Base de datos | PostgreSQL |
| Almacenamiento | AWS S3 |
| OCR | AWS Textract o Google Document AI |
| Infraestructura | AWS o self-hosted |

**Recomendación:** iniciar con el stack .NET si el equipo tiene o puede adquirir experiencia en C#, dado que da la mejor experiencia nativa en Windows (prioridad explícita) sin sacrificar el camino a móvil/web. Si el equipo de desarrollo ya domina JavaScript/TypeScript, la alternativa con Flutter + NestJS es igualmente viable y algo más económica en infraestructura.

---

## 6. Arquitectura de Alto Nivel

Arquitectura por capas (Clean Architecture), API-first, para que cualquier cliente (Windows, Android, iOS, Web) consuma los mismos servicios:

```
┌─────────────────────────────────────────────────────┐
│  Clientes: MAUI (Windows/Android/iOS)  |  Blazor Web │
└───────────────────────┬───────────────────────────────┘
                         │ HTTPS / REST + WebSockets (notificaciones)
┌───────────────────────▼───────────────────────────────┐
│                    API Gateway                        │
│      (Auth, rate limiting, ruteo por tenant)           │
└───────────────────────┬───────────────────────────────┘
        ┌────────────────┼────────────────┬───────────────┐
        ▼                ▼                ▼               ▼
  ┌───────────┐   ┌─────────────┐  ┌─────────────┐  ┌───────────┐
  │ Servicio  │   │  Servicio   │  │  Servicio   │  │ Servicio  │
  │ Clientes/ │   │ Documental  │  │  Fiscal /   │  │    IA     │
  │ Expedien. │   │  + OCR      │  │  Tareas     │  │ (asistente│
  │           │   │             │  │             │  │ /riesgos) │
  └─────┬─────┘   └──────┬──────┘  └──────┬──────┘  └─────┬─────┘
        └────────────────┴────────────────┴───────────────┘
                         │
        ┌────────────────┼────────────────┐
        ▼                ▼                ▼
  ┌───────────┐   ┌─────────────┐  ┌─────────────┐
  │ PostgreSQL│   │ Blob Storage│  │  Búsqueda   │
  │ (datos)   │   │ (documentos)│  │  (índices)  │
  └───────────┘   └─────────────┘  └─────────────┘

  Transversal: Auditoría/Logging · RBAC · Cifrado · Cola de eventos (para OCR async, alertas, IA)
```

**Decisiones clave de diseño:**
- **Multi-tenant desde el modelo de datos** (aunque el MVP corra en modo single-tenant): cada tabla relevante lleva `tenant_id`, evitando una migración dolorosa cuando se venda a otros despachos.
- **Procesamiento asíncrono** para OCR e IA vía cola de mensajes (evita bloquear la API cuando se procesan documentos grandes).
- **Módulo de IA desacoplado**: consume datos vía capa de servicios, nunca accede directo a la base de datos — así se controla exactamente qué puede leer y se garantiza que solo emite alertas/sugerencias, nunca escribe datos fiscales.

---

## 7. Próximos Documentos

1. **Modelo entidad-relación** detallado (clientes, documentos, obligaciones fiscales, tareas, auditoría).
2. **Historias de usuario** por módulo, priorizadas para el MVP.
3. **Definición formal del MVP** (qué entra y qué no entra en la primera versión funcional).
4. **Roadmap de desarrollo** por sprints/fases.
5. **Diseño de API** (contratos REST, autenticación, versionado).

¿Seguimos con el modelo entidad-relación o prefieres primero cerrar la definición del MVP para tener claro qué construir primero?
