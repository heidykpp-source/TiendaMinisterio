# Bitácora de cambios con IA

Registro de cada cambio hecho con ayuda de la IA: qué cambió, por qué, cómo se verificó y qué decidió el equipo.

## 2026-10-16 10:48 · Etapa 0 · Análisis del problema
- Cambio: se creó docs/analisis.md con las historias de usuario, los criterios de aceptación y las preguntas abiertas.
- Motivo: etapa 0 del método; el punto de partida es docs/enunciado.md.
- Archivos: docs/analisis.md (nuevo).
- Verificación: revisamos que cada regla R-01 a R-10 tuviera al menos un criterio.
- Decisión del equipo: aceptado. Entrada escrita a mano el 2026-10-19, porque la regla de la bitácora todavía no estaba en los estándares.

## 2026-10-19 16:55 · Etapa 0.1 · Adaptación de instrucciones del proyecto
- Cambio: se adaptaron las instrucciones de Copilot a la arquitectura y los requisitos de TiendaMinisterio, incluidas API, datos, seguridad, validación y pruebas.
- Motivo: retirar las reglas específicas de Triqui y reflejar los requisitos de Doc/Enunciado.md y Doc/análisis.md.
- Archivos: .github/copilot-instructions.md.
- Verificación: revisión del contenido y conteo de 38 reglas; no aplica ejecutar build ni test por ser un cambio de instrucciones.
- Decisión del equipo: pendiente

## 2026-10-19 17:12 · Etapa 0.0 · Propuesta de arquitectura modular
- Cambio: se creó Doc/arquitectura.md con dos opciones de arquitectura modular .NET para servidor y cliente, con proyectos de src/, prueba xUnit en tests/, diagrama de dependencias, cumplimiento C-02 a C-06, comparación y recomendación.
- Motivo: solicitud de definir alternativas de arquitectura alineadas con el enunciado, el análisis y los estándares del proyecto.
- Archivos: Doc/arquitectura.md.
- Verificación: revisión del archivo generado para confirmar que incluye todas las secciones y tablas pedidas.
- Decisión del equipo: pendiente

## 2026-10-19 17:44 · Etapa 3.1 · Estructura inicial de solución y proyectos
- Cambio: se creó la solución `Tienda.slnx` y los proyectos base en `src/` y `tests/` según la arquitectura elegida (Contracts, Domain, Application, Infrastructure.Data, Api, Client y Tienda.Pruebas). Se configuraron las referencias entre proyectos conforme a `Doc/plan.md` y `Doc/arquitectura.md`. También se eliminaron clases de plantilla (`Class1.cs` y `UnitTest1.cs`) para dejar la estructura limpia.
- Motivo: ejecutar la etapa 3.1 y dejar lista la base para las siguientes etapas sin lógica de negocio aún.
- Archivos: Tienda.slnx; src/Tienda.Contracts/Tienda.Contracts.csproj; src/Tienda.Domain/Tienda.Domain.csproj; src/Tienda.Application/Tienda.Application.csproj; src/Tienda.Infrastructure.Data/Tienda.Infrastructure.Data.csproj; src/Tienda.Api/Tienda.Api.csproj; src/Tienda.Client/Tienda.Client.csproj; tests/Tienda.Pruebas/Tienda.Pruebas.csproj; eliminación de src/Tienda.Contracts/Class1.cs, src/Tienda.Domain/Class1.cs, src/Tienda.Application/Class1.cs, src/Tienda.Infrastructure.Data/Class1.cs y tests/Tienda.Pruebas/UnitTest1.cs.
- Verificación: `dotnet build Tienda.slnx -m:1` (ok); `dotnet test Tienda.slnx` (ok, sin pruebas detectadas porque se eliminaron las plantillas).
- Decisión del equipo: pendiente

## 2026-10-19 17:59 · Etapa 3.2 · Entidades de dominio e invariantes
- Cambio: se implementó el núcleo de dominio en `Tienda.Domain` con entidades, value objects, abstracción base y excepción de regla de negocio para cubrir invariantes de R-02, R-03, R-04, R-05, R-09 y R-10. Se añadieron validaciones de precio > 0, stock no negativo, líneas obligatorias en compra/venta, cantidades/costos/precios positivos, desactivación lógica y restricción de venta para productos inactivos.
- Motivo: ejecutar la etapa 3.2 de `Doc/plan.md` manteniendo la lógica de negocio en dominio, sin dependencias de DB ni API.
- Archivos: src/Tienda.Domain/Abstractions/Entity.cs; src/Tienda.Domain/Exceptions/DomainRuleException.cs; src/Tienda.Domain/ValueObjects/DocumentoIdentidad.cs; src/Tienda.Domain/ValueObjects/Nit.cs; src/Tienda.Domain/Entities/Producto.cs; src/Tienda.Domain/Entities/Cliente.cs; src/Tienda.Domain/Entities/Proveedor.cs; src/Tienda.Domain/Entities/Compra.cs; src/Tienda.Domain/Entities/CompraLinea.cs; src/Tienda.Domain/Entities/Venta.cs; src/Tienda.Domain/Entities/VentaLinea.cs.
- Verificación: `dotnet build D:\S5\Tienda\TiendaMinisterio` (ok). Resultado: compilación exitosa con advertencias preexistentes NU1903 en `Microsoft.OpenApi` del proyecto `Tienda.Api`.
- Decisión del equipo: pendiente