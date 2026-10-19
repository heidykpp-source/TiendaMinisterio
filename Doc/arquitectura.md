# Propuesta de arquitectura modular .NET para TiendaMinisterio

## Opción 1: Clean Architecture modular (centrada en casos de uso)

### Proyectos en `src/`

- `Tienda.Contracts`: DTO de entrada/salida HTTP y contratos compartidos cliente-servidor.
- `Tienda.Domain`: entidades y reglas del negocio (R-01 a R-10) sin dependencias tecnológicas.
- `Tienda.Application`: casos de uso, validaciones de negocio y puertos (interfaces) para persistencia.
- `Tienda.Infrastructure.Data`: EF Core, `DbContext`, mapeos, repositorios y configuración del proveedor (SQLite inicial).
- `Tienda.Api`: endpoints `/api/v1`, validación de entrada, mapeo a casos de uso y respuestas HTTP.
- `Tienda.Client`: Blazor WebAssembly que consume la API vía HTTP/JSON.

### Proyecto de pruebas en `tests/`

- `tests/Tienda.Pruebas` (xUnit): valida reglas de negocio y casos de uso sin base de datos; referencia `Tienda.Domain` y `Tienda.Application`.

### Dependencias entre proyectos

```text
Tienda.Client --> Tienda.Contracts
Tienda.Api --> Tienda.Contracts
Tienda.Api --> Tienda.Application
Tienda.Api --> Tienda.Infrastructure.Data
Tienda.Application --> Tienda.Domain
Tienda.Infrastructure.Data --> Tienda.Application
Tienda.Infrastructure.Data --> Tienda.Domain
```

### Cumplimiento de C-02 a C-06

| Consideración | Cómo la cumple la opción 1 |
|---|---|
| C-02 | `Tienda.Client` solo usa `HttpClient` + `Tienda.Contracts`; no referencia infraestructura ni base de datos. |
| C-03 | El proveedor EF Core y detalles de SQLite quedan encapsulados en `Tienda.Infrastructure.Data`; para cambiar a SQL Server/PostgreSQL se ajusta ese proyecto y configuración. |
| C-04 | Las reglas están en `Tienda.Domain` y `Tienda.Application`; `tests/Tienda.Pruebas` las prueba sin API ni DB. |
| C-05 | `Tienda.Contracts` centraliza DTOs para reutilizarlos en API y cliente sin duplicación. |
| C-06 | `Tienda.Api` valida todo y traduce errores a `400/404/409`; `Application/Domain` ignoran precios/totales del cliente y recalculan en servidor. |

---

## Opción 2: Vertical slices modulares por funcionalidad

### Proyectos en `src/`

- `Tienda.Contracts`: DTOs y contratos HTTP compartidos por cliente y servidor.
- `Tienda.Core`: reglas de dominio comunes, tipos base e interfaces transversales.
- `Tienda.Modulos.Catalogos`: casos de uso y lógica de productos/clientes/proveedores.
- `Tienda.Modulos.Operaciones`: casos de uso y lógica de compras/ventas/inventario.
- `Tienda.Infrastructure.Data`: EF Core, `DbContext`, mapeos y repositorios concretos para los módulos.
- `Tienda.Api`: composición de módulos, endpoints por slice (`catalogos`, `operaciones`) y manejo de errores HTTP.
- `Tienda.Client`: Blazor WebAssembly consumidor de la API.

### Proyecto de pruebas en `tests/`

- `tests/Tienda.Pruebas` (xUnit): prueba reglas y casos de uso por módulo sin DB; referencia `Tienda.Core`, `Tienda.Modulos.Catalogos` y `Tienda.Modulos.Operaciones`.

### Dependencias entre proyectos

```text
Tienda.Client --> Tienda.Contracts
Tienda.Api --> Tienda.Contracts
Tienda.Api --> Tienda.Modulos.Catalogos
Tienda.Api --> Tienda.Modulos.Operaciones
Tienda.Api --> Tienda.Infrastructure.Data
Tienda.Modulos.Catalogos --> Tienda.Core
Tienda.Modulos.Operaciones --> Tienda.Core
Tienda.Infrastructure.Data --> Tienda.Core
Tienda.Infrastructure.Data --> Tienda.Modulos.Catalogos
Tienda.Infrastructure.Data --> Tienda.Modulos.Operaciones
```

### Cumplimiento de C-02 a C-06

| Consideración | Cómo la cumple la opción 2 |
|---|---|
| C-02 | `Tienda.Client` consume únicamente endpoints HTTP/JSON y contratos compartidos. |
| C-03 | La selección de motor queda en `Tienda.Infrastructure.Data`; módulos y cliente permanecen agnósticos al proveedor. |
| C-04 | `Tienda.Core` y módulos contienen la lógica verificable sin API ni persistencia; xUnit prueba por slice. |
| C-05 | `Tienda.Contracts` evita duplicación de modelos de transporte entre cliente y servidor. |
| C-06 | `Tienda.Api` centraliza validación y códigos HTTP; los módulos aplican reglas y conflictos de negocio (`409`) con lógica de servidor. |

---

## Comparación de las dos opciones

| Criterio | Opción 1: Clean Architecture | Opción 2: Vertical slices |
|---|---|---|
| Escribir pruebas unitarias sin base de datos | Muy directa: dominio y casos de uso claramente aislados. | Directa: pruebas por módulo, con más piezas a aislar. |
| Cambiar de motor de base de datos | Bajo impacto: concentrado en `Infrastructure.Data` y DI. | Bajo impacto también, pero con más adaptadores por módulo. |
| Agregar el cliente | Simple: `Contracts` ya estabiliza el intercambio API-cliente. | Simple también, con endpoints naturalmente alineados por funcionalidad. |
| Cantidad de proyectos | Menor (6 en `src/` + 1 en `tests/`). | Mayor (7 en `src/` + 1 en `tests/`). |
| Esfuerzo inicial | Medio: estructura conocida y rápida de levantar. | Medio-alto: más diseño de fronteras entre módulos desde el inicio. |

## Recomendación

Recomiendo la **Opción 1** para la versión 1: minimiza complejidad inicial y facilita cumplir C-04/C-06 rápidamente.
Si el dominio crece mucho en funcionalidades independientes, se puede evolucionar luego hacia la Opción 2 por módulos verticales.

## Decisión del equipo
