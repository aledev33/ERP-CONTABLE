# ERP Inteligente para Despachos Contables
## Documento 4: Definición Formal del MVP

---

## 1. Objetivo del MVP

Reemplazar el manejo disperso de expedientes de clientes (hojas de cálculo, carpetas, WhatsApp, memoria del equipo) por un sistema único donde los **socios y contadores** puedan: registrar clientes, digitalizar documentos, dar seguimiento a obligaciones fiscales y gestionar tareas internas — con trazabilidad completa.

**Criterio de éxito del MVP:** que el despacho pueda operar su día a día (altas de cliente, seguimiento fiscal, tareas) **sin volver a las hojas de cálculo**, en un plazo de prueba de 4–6 semanas de uso real por el equipo.

---

## 2. Usuarios del MVP

Solo usuarios **internos** del despacho: Socio, Administrador, Contador, Auxiliar.
**El rol Cliente (portal externo) queda fuera del MVP** — es Fase 4.

## 3. Plataforma del MVP

**Solo Windows** (consistente con la prioridad declarada del proyecto). Android, iOS y Web quedan para fases posteriores, aunque la arquitectura API-first ya lo contempla sin retrabajo.

---

## 4. Alcance Funcional — Qué SÍ entra

| Módulo | Historias incluidas | Notas de alcance |
|---|---|---|
| Gestión de Clientes | US-CLI-01 a 04 | Alta, búsqueda, vista 360°, activar/desactivar |
| Gestión Documental | US-DOC-01 a 03 | Carga, versionado, etiquetado manual. **Sin búsqueda avanzada por contenido** (eso requiere OCR) |
| Calendario Fiscal | US-CAL-01 a 03 | Generación automática de obligaciones por régimen fiscal, semáforo, alertas por correo |
| Gestión de Tareas | US-TAR-01 a 03 | Tablero kanban, asignación, comentarios |
| Seguridad y Permisos | US-SEG-01 a 03 | RBAC, MFA, bitácora de auditoría |

**Total: 16 historias, ~79 puntos** (ver documento 3).

---

## 5. Explícitamente FUERA del MVP

| Excluido | Razón | Fase estimada |
|---|---|---|
| OCR inteligente | Requiere modelo entrenado/afinado para documentos mexicanos; alto riesgo técnico para MVP | F2 |
| Generación automática de documentos (plantillas) | Depende de que el expediente ya tenga datos limpios y estables | F2 |
| Dashboard ejecutivo | Requiere datos históricos acumulados para ser útil | F2 |
| Asistente de IA conversacional | Depende de tener datos reales cargados para dar valor | F3 |
| Detección de riesgos | Depende del asistente de IA y de historial de datos | F3 |
| Portal de clientes | Implica exponer el sistema fuera del despacho — requiere hardening de seguridad adicional | F4 |
| Apps Android/iOS | Prioridad explícita es Windows primero | F4 |
| Multi-tenant real / comercialización SaaS | El modelo de datos ya lo soporta, pero no se activa hasta validar el producto con el despacho propio | F5 |

**Importante:** aunque estos módulos no se construyen en el MVP, el modelo de datos (documento 2) y la arquitectura (documento 1) ya están diseñados para no requerir migraciones destructivas cuando se activen.

---

## 6. Fuera de Alcance por Diseño (no es "después", es explícitamente no-objetivo del sistema)

- La IA **nunca** ejecuta acciones fiscales, legales o de negocio por sí sola, en ninguna fase.
- El sistema no sustituye la responsabilidad profesional del contador ante el SAT; es una herramienta de apoyo y trazabilidad.

---

## 7. Definición de "Terminado" (Definition of Done) para el MVP

Una historia de usuario del MVP se considera terminada cuando:
1. Cumple todos sus criterios de aceptación (documento 3).
2. Tiene pruebas automatizadas de la lógica de negocio (no solo UI).
3. Pasa por revisión de código.
4. Queda registrada en la bitácora de auditoría si involucra datos de cliente.
5. Fue probada por al menos un usuario real del despacho (Socio o Contador) antes de marcarse como aceptada.

El MVP completo se considera terminado cuando las 16 historias cumplen lo anterior **y** el equipo del despacho ha usado el sistema en un ciclo fiscal mensual completo sin depender de las hojas de cálculo anteriores.

---

## 8. Riesgos Específicos del MVP

| Riesgo | Impacto | Mitigación |
|---|---|---|
| Migración de datos existentes (Excel/carpetas actuales) mal planeada | Alto — adopción fallida si el equipo pierde información al migrar | Diseñar script de importación y validarlo con una muestra real antes del corte |
| Resistencia al cambio del equipo contable | Medio | Involucrar a 1–2 contadores como usuarios piloto desde el primer sprint, no solo al final |
| Catálogo de obligaciones fiscales (regímenes, periodicidades) incompleto o desactualizado | Alto — el semáforo de cumplimiento pierde credibilidad si falla | Validar el catálogo inicial directamente con un contador senior del despacho antes de codificar |
| Alcance del MVP crece durante desarrollo ("ya que estamos, agreguemos OCR") | Alto | Este documento es el contrato de alcance; cualquier adición pasa a F2+ salvo decisión explícita del Product Owner |

---

## 9. Estimación de Esfuerzo

Con ~79 puntos de historia y asumiendo un equipo pequeño (1–2 desarrolladores full-stack) con velocidad típica de equipos nuevos (~15–20 puntos/sprint de 2 semanas):

**Estimado: 4–5 sprints (8–10 semanas)** para completar el MVP funcional, sin contar el sprint 0 de setup de infraestructura (repositorio, CI/CD, entorno de base de datos, autenticación base).

Esta cifra es orientativa — se debe ajustar en el sprint 0 con el equipo real y su velocidad observada.

---

## 10. Siguiente Paso

Con el MVP formalmente acotado, el siguiente entregable es el **roadmap de desarrollo por sprints**: desglosar estas 16 historias en un plan de 4–5 sprints, con el sprint 0 de infraestructura incluido.

¿Seguimos con el roadmap?
