# Plan por etapas · Versión 1 (Opción 1 de arquitectura)

Este plan sigue la opción 1 de [arquitectura.md](./arquitectura.md): `Contracts`, `Domain`, `Application`, `Infrastructure.Data`, `Api`, `Client` y pruebas con xUnit en `tests/`.

## Tabla de etapas (3.1 en adelante)

| Etapa | Objetivo (<= 15 min) | Proyectos y archivos que toca | Reglas y criterios de aceptación que resuelve | Cómo se verifica |
|---|---|---|---|---|
| 3.1 | Crear la solución, los proyectos vacíos y el proyecto de pruebas con referencias correctas. | `Tienda.sln`; `src/Tienda.Contracts/`; `src/Tienda.Domain/`; `src/Tienda.Application/`; `src/Tienda.Infrastructure.Data/`; `src/Tienda.Api/`; `src/Tienda.Client/`; `tests/Tienda.Pruebas/`; actualización de referencias `.csproj`. | Base estructural para C-01, C-04 y C-05. Criterio: la solución compila en vacío y las dependencias respetan la arquitectura elegida. | `dotnet restore`; `dotnet build`; revisión de referencias entre proyectos. |
| 3.2 | Crear entidades del núcleo con reglas internas de validez (sin DB ni API). | `src/Tienda.Domain/Entities/*`; `src/Tienda.Domain/ValueObjects/*`; `src/Tienda.Domain/Exceptions/*`; `src/Tienda.Domain/Abstractions/*` (si aplica). | R-02, R-03, R-04, R-05, R-09, R-10 (invariantes de entidad). Criterio: entidades impiden estados inválidos desde su construcción/actualización. | Revisión de compilación y walkthrough de invariantes en dominio; `dotnet build`. |
| 3.3 | Crear pruebas unitarias de entidades a partir de criterios de aceptación del análisis. | `tests/Tienda.Pruebas/Domain/*EntityTests.cs`; datos de prueba mínimos en `tests/Tienda.Pruebas/TestData/*` (si aplica). | Cubre criterios de aceptación de R-02, R-03, R-04, R-05, R-09 y R-10 en nivel entidad. C-04 explícito. | `dotnet test --filter "FullyQualifiedName~Domain"`; revisión AAA y nombres `Metodo_Escenario_Resultado`. |
| 3.4 | Definir puertos/repositorios y casos de uso de catálogos en aplicación (sin EF ni HTTP). | `src/Tienda.Application/Abstractions/*`; `src/Tienda.Application/Catalogos/*`; `src/Tienda.Application/Common/*`. | R-01 (SKU único), R-10 (documento/NIT únicos), R-09 (desactivación en lugar de borrado). Criterio: contratos de aplicación contemplan validación de unicidad y estado activo. | `dotnet build`; revisión de firmas de casos de uso y puertos. |
| 3.5 | Crear pruebas unitarias de aplicación para catálogos con dobles de prueba (sin DB). | `tests/Tienda.Pruebas/Application/Catalogos/*Tests.cs`; `tests/Tienda.Pruebas/Doubles/*`. | Verifica R-01, R-09 y R-10 en flujo de casos de uso; C-04 (sin base de datos). | `dotnet test --filter "FullyQualifiedName~Catalogos"`. |
| 3.6 | Implementar casos de uso de compras y ventas en aplicación, incluyendo cálculo y atomicidad lógica. | `src/Tienda.Application/Compras/*`; `src/Tienda.Application/Ventas/*`; `src/Tienda.Application/Transactions/*`. | R-03, R-04, R-05, R-06, R-07, R-08, R-09. Criterio: venta falla completa si no hay stock; precios/totales los define servidor. | `dotnet build`; revisión de flujo de orquestación y resultados de negocio. |
| 3.7 | Crear pruebas unitarias de aplicación para compras/ventas y reglas transaccionales (sin DB). | `tests/Tienda.Pruebas/Application/Compras/*Tests.cs`; `tests/Tienda.Pruebas/Application/Ventas/*Tests.cs`; dobles de puertos/repositorios. | Cubre R-06 (atomicidad), R-07 (ignorar precio cliente), R-08 (total servidor), además de R-03, R-04 y R-05. | `dotnet test --filter "FullyQualifiedName~Compras|FullyQualifiedName~Ventas"`. |
| 3.8 | Implementar infraestructura de datos con EF Core SQLite y mapeos (sin exponer en API aún). | `src/Tienda.Infrastructure.Data/TiendaDbContext.cs`; `Configurations/*`; `Repositories/*`; `Migrations/*`; `DependencyInjection.cs`. | C-03 (cambio de motor acotado a datos/config), respaldo de R-01 y R-10 con índices únicos, relaciones para R-04/R-05. | `dotnet build`; generación/aplicación de migración local; validación de esquema EF. |
| 3.9 | Crear pruebas de infraestructura para persistencia y restricciones en SQLite. | `tests/Tienda.Pruebas/Infrastructure/*Tests.cs`; fixtures de base temporal. | Verifica persistencia de compras/ventas con detalle e históricos; respalda R-01, R-10 y consistencia de R-03. | `dotnet test --filter "FullyQualifiedName~Infrastructure"`. |
| 3.10 | Exponer API de catálogos e inventario con contratos compartidos y validación de entrada. | `src/Tienda.Contracts/Catalogos/*`; `src/Tienda.Contracts/Inventario/*`; `src/Tienda.Api/Endpoints/Catalogos/*`; `src/Tienda.Api/Endpoints/Inventario/*`; `Program.cs`. | C-02, C-05, C-06. Reglas cubiertas: R-01, R-09, R-10 en capa HTTP + aplicación. Criterio: códigos 400/404/409 según caso. | `dotnet build`; pruebas de integración HTTP de catálogos/inventario. |
| 3.11 | Crear pruebas de integración API para catálogos/inventario y `ProblemDetails`. | `tests/Tienda.Pruebas/Api/Catalogos/*Tests.cs`; `tests/Tienda.Pruebas/Api/Inventario/*Tests.cs`. | C-06 completo (errores claros 400/404/409), C-02 (solo contrato HTTP). | `dotnet test --filter "FullyQualifiedName~Api.Catalogos|FullyQualifiedName~Api.Inventario"`. |
| 3.12 | Exponer API de compras y ventas con transacciones y cálculo en servidor. | `src/Tienda.Contracts/Compras/*`; `src/Tienda.Contracts/Ventas/*`; `src/Tienda.Api/Endpoints/Compras/*`; `src/Tienda.Api/Endpoints/Ventas/*`; manejo transaccional en composición. | R-03, R-04, R-05, R-06, R-07, R-08, R-09; C-06. Criterio: venta atómica, precios/totales de servidor y conflictos de negocio en 409. | `dotnet build`; prueba manual API con casos válidos e inválidos. |
| 3.13 | Crear pruebas de integración API para compras/ventas e invariantes de negocio extremo a extremo. | `tests/Tienda.Pruebas/Api/Compras/*Tests.cs`; `tests/Tienda.Pruebas/Api/Ventas/*Tests.cs`. | Cubre R-03 a R-09 en HTTP + persistencia, con foco en R-06/R-07/R-08 y códigos C-06. | `dotnet test --filter "FullyQualifiedName~Api.Compras|FullyQualifiedName~Api.Ventas"`. |
| 3.14 | Preparar cliente Blazor: estructura, navegación y servicios HTTP usando `Tienda.Contracts`. | `src/Tienda.Client/Pages/*`; `src/Tienda.Client/Services/*`; `src/Tienda.Client/Shared/*`; configuración `Program.cs` del cliente. | C-02 y C-05. Criterio: cliente sin reglas de negocio ni acceso a DB; consume endpoints existentes. | `dotnet build`; ejecución del cliente contra API local y prueba de navegación. |
| 3.15 | Implementar flujos UI de catálogos e inventario con manejo de errores del servidor. | `src/Tienda.Client/Pages/Catalogos/*`; `src/Tienda.Client/Pages/Inventario/*`; componentes de formulario/listado. | Refuerza C-02 y C-06 desde UX (mostrar 400/404/409). Reglas de catálogos reflejadas vía respuestas API (R-01, R-09, R-10). | Pruebas manuales de UI sobre API real; checklist de escenarios válidos/inválidos. |
| 3.16 | Implementar flujos UI de compras y ventas y validación final de versión 1. | `src/Tienda.Client/Pages/Compras/*`; `src/Tienda.Client/Pages/Ventas/*`; README y documentación operativa. | Cubre operación completa de R-03 a R-09 a través de API; cierre de C-02 a C-06 y C-08 (documentación). | `dotnet build`; `dotnet test`; prueba integral extremo a extremo cliente-servidor. |

## Cobertura de reglas R-01 a R-10 por etapa

| Regla | Etapas donde se implementa/valida |
|---|---|
| R-01 (SKU único) | 3.4, 3.5, 3.8, 3.10, 3.11 |
| R-02 (precio > 0) | 3.2, 3.3 |
| R-03 (stock no negativo, solo compras/ventas) | 3.2, 3.3, 3.6, 3.7, 3.12, 3.13 |
| R-04 (compra con líneas válidas) | 3.2, 3.3, 3.6, 3.7, 3.12, 3.13 |
| R-05 (venta con líneas válidas) | 3.2, 3.3, 3.6, 3.7, 3.12, 3.13 |
| R-06 (venta atómica) | 3.6, 3.7, 3.12, 3.13 |
| R-07 (precio de venta lo define servidor) | 3.6, 3.7, 3.12, 3.13 |
| R-08 (total lo calcula servidor) | 3.6, 3.7, 3.12, 3.13 |
| R-09 (desactivar, no borrar; inactivo no se vende) | 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.10, 3.11, 3.12, 3.13 |
| R-10 (documento/NIT únicos) | 3.2, 3.3, 3.4, 3.5, 3.8, 3.10, 3.11 |

## Nota de ejecución

- Cada etapa está pensada para una sesión corta de hasta 15 minutos con alcance acotado y verificable.
- Si una etapa excede el tiempo, se divide en subetapas consecutivas manteniendo el patrón: núcleo → pruebas. 
