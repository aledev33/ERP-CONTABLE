# ERP Inteligente para Despachos Contables
## Documento 5: Roadmap de Desarrollo — MVP por Sprints

---

## 1. Supuestos de planeación

- Sprints de 2 semanas.
- Equipo pequeño (1–2 desarrolladores full-stack).
- Velocidad estimada inicial: 20–25 puntos/sprint (se ajusta con datos reales después del Sprint 1).
- El orden de las épicas sigue dependencias técnicas: **Seguridad primero** (todo lo demás depende de usuarios/roles), **Clientes segundo** (entidad central de la que cuelga todo lo demás).

---

## 2. Sprint 0 — Infraestructura (sin historias de negocio)

**Objetivo:** dejar el terreno listo para que el Sprint 1 sea puro desarrollo de funcionalidad, no configuración.

| Tarea | Entregable |
|---|---|
| Scaffolding del proyecto backend | Solución/proyecto base corriendo localmente ("Hello World" de la API) |
| Conexión a base de datos | PostgreSQL local + primera migración vacía funcionando |
| CI real en GitHub Actions | Reemplazar el placeholder actual por build + test automático en cada PR |
| Esqueleto de autenticación | Login básico funcionando (sin roles todavía, solo email/password) |
| Ambiente de staging | Un ambiente desplegado, aunque sea mínimo, para no descubrir problemas de deploy hasta el final |
| Convención de ramas activa | Protección de `main` configurada en GitHub (ver `docs/workflow-git.md`) |

**Criterio de salida del Sprint 0:** puedes hacer login con un usuario de prueba y ver una pantalla en blanco autenticada, desplegada en staging, con CI corriendo en verde.

---

## 3. Sprint 1 (semanas 1–2) — Seguridad + arranque de Clientes

| Historia | Puntos |
|---|---|
| US-SEG-01 Control de acceso por rol | 8 |
| US-SEG-02 Autenticación multifactor | 5 |
| US-SEG-03 Bitácora de auditoría | 5 |
| US-CLI-01 Alta de expediente de cliente | 5 |
| **Total** | **23** |

**Por qué este orden:** el RBAC y la auditoría son transversales — si los dejas para el final, terminas reescribiendo permisos sobre funcionalidad ya construida. Mejor pagar ese costo una sola vez, al inicio.

**Entregable del sprint:** un usuario Socio puede crear otro usuario con rol Contador, ese Contador puede iniciar sesión con MFA, y dar de alta un cliente — quedando registrado en la bitácora.

---

## 4. Sprint 2 (semanas 3–4) — Clientes completo + arranque Documental

| Historia | Puntos |
|---|---|
| US-CLI-02 Búsqueda rápida de clientes | 5 |
| US-CLI-03 Vista 360° del cliente | 8 |
| US-CLI-04 Desactivar cliente | 2 |
| US-DOC-01 Carga de documentos con versionado | 5 |
| **Total** | **20** |

**Entregable del sprint:** el módulo de Clientes queda funcionalmente completo para el MVP, y ya se puede subir el primer documento a un expediente.

---

## 5. Sprint 3 (semanas 5–6) — Documental completo + Calendario Fiscal

| Historia | Puntos |
|---|---|
| US-DOC-02 Historial de versiones | 3 |
| US-DOC-03 Etiquetado y clasificación | 3 |
| US-CAL-01 Calendario de obligaciones | 8 |
| US-CAL-02 Semáforo de cumplimiento | 5 |
| US-CAL-03 Alertas de vencimiento | 5 |
| **Total** | **24** |

**Nota de riesgo:** US-CAL-01 depende de tener bien cargado el catálogo de obligaciones fiscales por régimen — esto se valida con un contador senior **antes** de este sprint, no durante (ver riesgos en el documento 4).

**Entregable del sprint:** un cliente con régimen fiscal asignado ya genera automáticamente sus obligaciones, con semáforo y alertas por correo.

---

## 6. Sprint 4 (semanas 7–8) — Tareas + piloto interno

| Historia | Puntos |
|---|---|
| US-TAR-01 Asignar tarea | 5 |
| US-TAR-02 Tablero de tareas | 5 |
| US-TAR-03 Comentarios en tareas | 2 |
| **Total historias** | **12** |
| Migración de datos reales de prueba (Excel actual → sistema) | — |
| Inicio de piloto con 1–2 contadores reales | — |

**Entregable del sprint:** el sistema completo del MVP funcionando, con 1–2 usuarios reales del despacho usándolo en paralelo a su flujo actual (no reemplazándolo todavía — validando).

---

## 7. Sprint 5 (semanas 9–10) — Estabilización y cierre de MVP

Sin historias nuevas — este sprint es **buffer intencional**, no relleno. Su trabajo es:

- Corregir bugs reportados por el piloto interno.
- Completar la migración real de datos desde Excel/carpetas.
- Cerrar huecos de UX que solo se ven con uso real (casi siempre los hay).
- Ejecutar el checklist de producción del documento de respaldos (`docs/respaldos-y-continuidad.md`): probar restauración de base de datos, confirmar redundancia de documentos, revisar que `main` esté protegida.
- Tag de versión `v1.0.0` cuando el checklist esté completo y el equipo del despacho use el sistema sin depender de las hojas de cálculo anteriores (criterio de éxito del documento 4).

---

## 8. Prácticas de seguimiento (recomendado, no obligatorio para un equipo de 1–2 personas, pero ayuda)

- **Planning al inicio de cada sprint:** revisar qué se completó, qué no, y por qué — antes de comprometerte al siguiente lote de historias.
- **Actualizar velocidad real:** después del Sprint 1, compara puntos planeados vs. puntos completados. Ajusta la carga del Sprint 2 con ese dato real, no con la estimación inicial.
- **Un tag de git por sprint cerrado:** `git tag sprint-1-cierre` — te da puntos de referencia claros en el historial para volver si algo se rompe.

---

## 9. Resumen visual del roadmap

| Sprint | Semanas | Foco principal | Puntos |
|---|---|---|---|
| 0 | — | Infraestructura | — |
| 1 | 1–2 | Seguridad + Clientes (inicio) | 23 |
| 2 | 3–4 | Clientes (cierre) + Documental (inicio) | 20 |
| 3 | 5–6 | Documental (cierre) + Calendario Fiscal | 24 |
| 4 | 7–8 | Tareas + piloto interno | 12 |
| 5 | 9–10 | Estabilización y cierre v1.0.0 | — |

Coincide con el estimado de 8–10 semanas del documento de definición del MVP.

---

## 10. Siguiente paso

Con el roadmap definido, el siguiente entregable técnico natural es el **diseño de API** (contratos REST, autenticación, versionado) — antes de escribir la primera línea de código del Sprint 0, para que el backend tenga un contrato claro desde el inicio.

¿Seguimos con eso, o prefieres primero definir con más detalle el Sprint 0 (qué stack exacto, cómo se ve el scaffolding)?
