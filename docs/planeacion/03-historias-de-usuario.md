# ERP Inteligente para Despachos Contables
## Documento 3: Historias de Usuario

---

## Convenciones

- Formato: **Como** [rol] **quiero** [acción] **para** [beneficio].
- Cada historia referencia su requisito funcional (documento 1) y su prioridad según la fase definida (Fase 1 = MVP).
- Prioridad: **MVP** (Fase 1) · **F2** · **F3** · **F4** · **F5**.
- Estimación en puntos (escala Fibonacci: 1,2,3,5,8,13) — orientativa, para planeación de sprints.

---

## Épica 1: Gestión de Clientes

### US-CLI-01 — Alta de expediente de cliente
**Como** Contador **quiero** registrar el expediente completo de un cliente nuevo **para** tener toda su información fiscal y de contacto centralizada desde el inicio.
- **Criterios de aceptación:**
  - El formulario exige tipo de persona (física/moral) y adapta los campos (RFC/CURP obligatorios según tipo).
  - Valida formato de RFC y CURP contra las reglas del SAT antes de guardar.
  - El cliente queda en estado "Activo" por defecto.
- **RF:** RF-CLI-001 | **Prioridad:** MVP | **Puntos:** 5

### US-CLI-02 — Búsqueda rápida de clientes
**Como** cualquier usuario interno **quiero** buscar un cliente por nombre, RFC, CURP, correo o teléfono **para** encontrar su expediente en segundos.
- **Criterios de aceptación:**
  - Resultados en menos de 1 segundo con hasta 50,000 clientes.
  - Búsqueda parcial (no requiere coincidencia exacta).
  - Resultados muestran nombre, RFC y estado (activo/inactivo).
- **RF:** RF-CLI-002 | **Prioridad:** MVP | **Puntos:** 5

### US-CLI-03 — Vista 360° del cliente
**Como** Contador **quiero** ver toda la información de un cliente (datos, documentos, tareas, calendario fiscal) en una sola pantalla **para** no tener que navegar entre módulos.
- **Criterios de aceptación:**
  - Pestañas o secciones: Datos generales · Documentos · Obligaciones fiscales · Tareas · Historial.
  - Carga completa en menos de 2 segundos.
- **RF:** RF-CLI-003 | **Prioridad:** MVP | **Puntos:** 8

### US-CLI-04 — Desactivar cliente sin perder historial
**Como** Socio **quiero** marcar un cliente como inactivo **para** dejar de darle seguimiento activo sin borrar su expediente histórico.
- **Criterios de aceptación:**
  - Cliente inactivo no aparece en dashboards de pendientes ni genera nuevas alertas de calendario fiscal.
  - Su expediente sigue siendo consultable y auditable.
- **RF:** RF-CLI-004 | **Prioridad:** MVP | **Puntos:** 2

---

## Épica 2: Gestión Documental

### US-DOC-01 — Carga de documentos con versionado
**Como** Auxiliar **quiero** subir un documento al expediente de un cliente **para** mantenerlo digitalizado y respaldado.
- **Criterios de aceptación:**
  - Si ya existe un documento del mismo tipo, se crea una nueva versión (no se sobrescribe).
  - Se calcula un checksum del archivo para garantizar integridad.
  - Tipos permitidos: PDF, JPG, PNG (configurable).
- **RF:** RF-DOC-001 | **Prioridad:** MVP | **Puntos:** 5

### US-DOC-02 — Historial de versiones
**Como** Socio **quiero** ver todas las versiones anteriores de un documento **para** auditar cambios en caso de una revisión fiscal.
- **Criterios de aceptación:**
  - Lista de versiones con fecha, quién la subió y posibilidad de descargar cada una.
- **RF:** RF-DOC-001 | **Prioridad:** MVP | **Puntos:** 3

### US-DOC-03 — Etiquetado y clasificación
**Como** Contador **quiero** etiquetar documentos manualmente **para** facilitar su localización posterior.
- **Criterios de aceptación:**
  - Se pueden agregar múltiples etiquetas por documento.
  - Autocompletado de etiquetas ya existentes en el despacho.
- **RF:** RF-DOC-002 | **Prioridad:** MVP | **Puntos:** 3

### US-DOC-04 — Búsqueda avanzada de documentos
**Como** Contador **quiero** buscar documentos por tipo, cliente, fecha o etiqueta **para** encontrar lo que necesito sin recorrer expedientes uno por uno.
- **Criterios de aceptación:**
  - Filtros combinables (AND).
  - Resultados paginados y ordenables por fecha.
- **RF:** RF-DOC-003 | **Prioridad:** F2 | **Puntos:** 5

---

## Épica 3: OCR Inteligente

### US-OCR-01 — Detección automática del tipo de documento
**Como** Auxiliar **quiero** que el sistema detecte qué tipo de documento acabo de subir **para** no tener que clasificarlo manualmente.
- **Criterios de aceptación:**
  - Precisión mínima aceptada 90% en documentos mexicanos estándar (INE, Constancia Fiscal, Actas).
  - Si la confianza es baja, se marca para clasificación manual.
- **RF:** RF-OCR-001 | **Prioridad:** F2 | **Puntos:** 13

### US-OCR-02 — Extracción y prellenado de formulario
**Como** Auxiliar **quiero** que los datos de un documento cargado (ej. RFC de una Constancia Fiscal) prellenen el expediente del cliente **para** reducir captura manual.
- **Criterios de aceptación:**
  - Campos prellenados quedan marcados como "sugeridos por OCR" hasta confirmación humana.
  - El usuario puede corregir antes de guardar.
- **RF:** RF-OCR-002 | **Prioridad:** F2 | **Puntos:** 8

### US-OCR-03 — Vinculación automática al cliente
**Como** Auxiliar **quiero** que un documento subido se asocie automáticamente al cliente correcto por su RFC/CURP detectado **para** no tener que buscarlo y adjuntarlo manualmente.
- **Criterios de aceptación:**
  - Si el RFC/CURP detectado no coincide con ningún cliente existente, se solicita confirmación manual.
- **RF:** RF-OCR-003 | **Prioridad:** F2 | **Puntos:** 5

---

## Épica 4: Generación de Documentos

### US-GEN-01 — Generar documento desde plantilla
**Como** Contador **quiero** generar un contrato o carta poder usando los datos ya guardados del cliente **para** no volver a capturarlos manualmente.
- **Criterios de aceptación:**
  - El sistema detecta variables faltantes en el expediente antes de generar y las solicita.
  - Exportación en PDF y Word.
- **RF:** RF-GEN-001, RF-GEN-002 | **Prioridad:** F2 | **Puntos:** 8

### US-GEN-02 — Administrar plantillas
**Como** Administrador **quiero** crear y editar plantillas de documentos **para** adaptar el sistema a los formatos que usa el despacho.
- **Criterios de aceptación:**
  - Editor de plantillas con variables tipo `{{cliente.nombre}}`.
  - Vista previa antes de publicar la plantilla.
- **RF:** RF-GEN-001 | **Prioridad:** F2 | **Puntos:** 8

---

## Épica 5: Calendario Fiscal

### US-CAL-01 — Calendario de obligaciones por cliente
**Como** Contador **quiero** ver las obligaciones fiscales próximas de cada cliente **para** no perder ninguna fecha límite.
- **Criterios de aceptación:**
  - Las obligaciones se generan automáticamente según el régimen fiscal del cliente.
  - Vista de calendario y vista de lista.
- **RF:** RF-CAL-001 | **Prioridad:** MVP | **Puntos:** 8

### US-CAL-02 — Semáforo de cumplimiento
**Como** Socio **quiero** ver de un vistazo qué clientes están en riesgo de incumplimiento **para** priorizar la atención del equipo.
- **Criterios de aceptación:**
  - Verde: a tiempo · Amarillo: próximo a vencer (configurable, ej. 5 días) · Rojo: vencido.
- **RF:** RF-CAL-002 | **Prioridad:** MVP | **Puntos:** 5

### US-CAL-03 — Alertas de vencimiento
**Como** Contador **quiero** recibir una notificación antes de que venza una obligación fiscal **para** actuar a tiempo.
- **Criterios de aceptación:**
  - Antelación configurable por tipo de obligación.
  - Notificación por correo y dentro del sistema.
- **RF:** RF-CAL-003 | **Prioridad:** MVP | **Puntos:** 5

---

## Épica 6: Gestión de Tareas

### US-TAR-01 — Asignar tarea
**Como** Socio **quiero** asignar una tarea a un miembro del equipo con fecha límite y prioridad **para** distribuir el trabajo del despacho.
- **Criterios de aceptación:**
  - Campos: responsable, cliente (opcional), prioridad, fecha límite, descripción, adjuntos.
- **RF:** RF-TAR-001 | **Prioridad:** MVP | **Puntos:** 5

### US-TAR-02 — Tablero de tareas
**Como** empleado **quiero** ver mis tareas organizadas por estado **para** saber en qué debo trabajar hoy.
- **Criterios de aceptación:**
  - Vista tipo kanban: Pendiente / En proceso / En revisión / Terminada.
  - Arrastrar y soltar para cambiar de estado.
- **RF:** RF-TAR-002 | **Prioridad:** MVP | **Puntos:** 5

### US-TAR-03 — Comentarios en tareas
**Como** empleado **quiero** comentar en una tarea **para** dejar contexto o dudas visibles para el equipo.
- **RF:** RF-TAR-001 | **Prioridad:** MVP | **Puntos:** 2

---

## Épica 7: Dashboard Ejecutivo

### US-DSH-01 — Panel de indicadores del despacho
**Como** Socio **quiero** ver clientes activos, pendientes, documentos vencidos y productividad del equipo en un solo panel **para** tomar decisiones rápidas.
- **Criterios de aceptación:**
  - KPIs se actualizan en tiempo real o casi real (<5 min de rezago).
  - Cada KPI es clickeable y lleva al listado detallado correspondiente.
- **RF:** RF-DSH-001 | **Prioridad:** F2 | **Puntos:** 8

---

## Épica 8: Seguridad y Permisos

### US-SEG-01 — Control de acceso por rol
**Como** Administrador **quiero** que cada usuario solo vea y edite lo que su rol permite **para** proteger información sensible de clientes.
- **Criterios de aceptación:**
  - Roles predefinidos (Socio, Administrador, Contador, Auxiliar, Cliente) con permisos configurables por módulo.
- **RF:** RF-SEG-001 | **Prioridad:** MVP | **Puntos:** 8

### US-SEG-02 — Autenticación multifactor
**Como** usuario interno **quiero** confirmar mi identidad con un segundo factor al iniciar sesión **para** proteger el acceso a información fiscal sensible.
- **RF:** RF-SEG-002 | **Prioridad:** MVP | **Puntos:** 5

### US-SEG-03 — Bitácora de auditoría
**Como** Socio **quiero** consultar quién hizo qué cambio y cuándo **para** tener trazabilidad completa ante una auditoría externa.
- **Criterios de aceptación:**
  - Registro inmutable (no editable ni borrable) de acciones sobre clientes, documentos y obligaciones fiscales.
- **RF:** RF-SEG-003 | **Prioridad:** MVP | **Puntos:** 5

---

## Épica 9: Inteligencia Artificial

### US-IA-01 — Asistente conversacional
**Como** Contador **quiero** preguntarle al sistema en lenguaje natural sobre vencimientos o pendientes **para** obtener respuestas sin navegar reportes manualmente.
- **Criterios de aceptación:**
  - El asistente solo tiene acceso de lectura a los datos del despacho.
  - Cada respuesta registra qué datos consultó (para auditoría).
- **RF:** RF-IA-001 | **Prioridad:** F3 | **Puntos:** 13

### US-IA-02 — Resumen ejecutivo de expediente
**Como** Socio **quiero** un resumen automático del expediente de un cliente **para** ponerme al día rápidamente antes de una reunión.
- **RF:** RF-IA-002 | **Prioridad:** F3 | **Puntos:** 8

### US-IA-03 — Generar documento por instrucción en lenguaje natural
**Como** Contador **quiero** pedirle al asistente que redacte un escrito libre a partir de una instrucción **para** ahorrar tiempo de redacción.
- **Criterios de aceptación:**
  - El documento generado siempre requiere revisión y aprobación humana antes de enviarse.
- **RF:** RF-IA-003 | **Prioridad:** F3 | **Puntos:** 8

---

## Épica 10: Detección de Riesgos

### US-RIE-01 — Alertas de inconsistencias fiscales
**Como** Socio **quiero** que el sistema me alerte sobre posibles inconsistencias fiscales de un cliente **para** revisarlas antes de que se conviertan en un problema.
- **Criterios de aceptación:**
  - La alerta nunca ejecuta ninguna acción automática; solo se muestra para revisión humana.
  - Cada alerta tiene severidad y debe marcarse como revisada/descartada/confirmada por un usuario.
- **RF:** RF-IA-004 | **Prioridad:** F3 | **Puntos:** 13

### US-RIE-02 — Alerta de documentación faltante
**Como** Contador **quiero** ver qué documentos obligatorios le faltan a un cliente **para** solicitarlos a tiempo.
- **RF:** RF-IA-004 | **Prioridad:** F3 | **Puntos:** 5

---

## Resumen de Priorización (Backlog del MVP)

| Épica | Historias en MVP | Puntos MVP |
|---|---|---|
| Clientes | US-CLI-01 a 04 | 20 |
| Documental | US-DOC-01 a 03 | 11 |
| Calendario Fiscal | US-CAL-01 a 03 | 18 |
| Tareas | US-TAR-01 a 03 | 12 |
| Seguridad | US-SEG-01 a 03 | 18 |
| **Total MVP** | **16 historias** | **~79 puntos** |

Con velocidad estimada de equipo (a definir con el equipo real), esto da una referencia directa para planear sprints.

---

## Siguiente Paso

Con requisitos, modelo de datos e historias de usuario ya definidos, el siguiente entregable natural es cerrar el **alcance formal del MVP** (qué queda dentro/fuera de la v1) y after eso, el **roadmap de desarrollo por sprints**.

¿Seguimos con la definición del MVP o prefieres pasar directo al roadmap?
