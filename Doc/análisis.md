# Análisis del proyecto: tienda de equipos tecnológicos

## 1. Propósito y alcance del análisis

Este documento analiza [Enunciado.md](./Enunciado.md) y propone una metodología de trabajo, una arquitectura .NET y un plan de desarrollo para la versión 1. No modifica ni sustituye el enunciado, y no implica que la implementación ya haya comenzado.

Las reglas R-01 a R-10 y las consideraciones C-01 a C-08 se consideran requisitos del enunciado. Las decisiones adicionales se identifican como **pendientes de aprobación**; no deben tratarse como requisitos confirmados hasta que el responsable las apruebe.

## 2. Lectura del problema

La necesidad central es mantener un inventario confiable y evitar que las ventas excedan las existencias. Para ello, las operaciones de compra y venta deben registrarse como transacciones completas en el servidor, que es la única autoridad para validar reglas, calcular importes y cambiar el stock.

La aplicación contempla:

- Catálogos de productos, clientes y proveedores, con operaciones de consulta, registro y edición, y desactivación cuando corresponda.
- Registro de compras y ventas con una o más líneas de detalle.
- Consultas de existencias y de compras y ventas con detalle y total.
- Una API web como servidor y una aplicación gráfica en Blazor WebAssembly para el personal de la tienda.
- SQLite y Entity Framework Core inicialmente, con aislamiento de la persistencia para permitir cambiar el motor.

La versión 1 incluye servidor y cliente gráfico. Las mejoras enumeradas como “Para después” quedan fuera del alcance inicial.

## 3. Metodología de trabajo propuesta

Se propone un desarrollo incremental, con entregas pequeñas y verificables. Cada etapa termina con una revisión funcional y técnica antes de avanzar a la siguiente.

1. **Aprobación de análisis.** Confirmar las decisiones pendientes de este documento, el alcance de la versión 1 y los criterios de aceptación.
2. **Preparación de la solución.** Crear solución y proyectos .NET 10, configurar repositorio, compilación, formato, documentación inicial y configuración local sin secretos.
3. **Dominio y casos de uso del servidor.** Implementar reglas y operaciones de negocio independientes de HTTP y de la base de datos.
4. **Persistencia y API.** Integrar EF Core/SQLite, migraciones, transacciones y endpoints HTTP; probar contratos y códigos de respuesta.
5. **Cliente gráfico.** Construir el cliente Blazor WebAssembly consumiendo únicamente la API mediante HTTP/JSON y reutilizando el proyecto de contratos.
6. **Validación integral y entrega.** Ejecutar pruebas, revisar documentación, preparar datos de ejemplo y verificar el recorrido completo de los casos de uso.

En cada incremento se seguirá el ciclo: aclarar criterios → implementar → probar → revisar → documentar. Los cambios de alcance o de reglas se registrarán antes de incorporarse. Las pruebas de dominio se escribirán sin API ni base de datos, conforme a C-04; las pruebas de integración cubrirán persistencia y HTTP.

## 4. Arquitectura propuesta

### 4.1 Principios

- **Servidor como autoridad:** las validaciones, el cálculo de precios y totales, y las modificaciones de stock ocurren en el servidor.
- **Separación de responsabilidades:** dominio, aplicación, contratos, persistencia, API y cliente tienen límites explícitos.
- **Dominio comprobable de forma aislada:** las reglas no dependen de EF Core, SQLite, ASP.NET Core ni Blazor.
- **Persistencia sustituible:** los detalles del proveedor de base de datos y el registro/configuración de EF Core quedan en el proyecto de infraestructura y configuración; los otros proyectos no dependen de SQLite.
- **Contratos compartidos:** los DTO de entrada y salida HTTP viven en un proyecto propio, consumido por servidor y cliente.
- **Errores explícitos:** respuestas coherentes con C-06 para datos inválidos, recursos inexistentes y conflictos de reglas.

### 4.2 Proyectos sugeridos

Los nombres son orientativos y quedan sujetos a aprobación.

| Proyecto | Responsabilidad | Dependencias principales |
|---|---|---|
| `Tienda.Contracts` | DTO de solicitudes y respuestas, y tipos compartidos necesarios para el protocolo HTTP. | Sin dependencia de EF Core ni de infraestructura. |
| `Tienda.Domain` | Entidades, invariantes y reglas del negocio; operaciones de cálculo y validación puras cuando aplique. | Ninguna dependencia de tecnología de persistencia o transporte. |
| `Tienda.Application` | Casos de uso, coordinación de operaciones y abstracciones necesarias para acceder a datos. | `Tienda.Domain` y, si se decide, `Tienda.Contracts`. |
| `Tienda.Infrastructure` | `DbContext`, configuraciones EF Core, repositorios/adaptadores, migraciones y selección del proveedor. | `Tienda.Domain`, `Tienda.Application` y EF Core. |
| `Tienda.Api` | Composición de dependencias, endpoints HTTP, autenticación si se aprobara posteriormente, validación de solicitudes y conversión de errores a HTTP. | `Tienda.Application`, `Tienda.Contracts` e `Tienda.Infrastructure`. |
| `Tienda.Client` | Interfaz Blazor WebAssembly para el personal; consume exclusivamente la API por HTTP/JSON. | `Tienda.Contracts`; no referencia `Tienda.Infrastructure` ni accede a la base de datos. |
| `Tienda.Domain.Tests` | Pruebas unitarias del dominio y sus reglas sin API ni base de datos. | `Tienda.Domain` y framework de pruebas. |
| `Tienda.Api.Tests` | Pruebas de integración del protocolo, persistencia y transacciones. | API y utilidades de pruebas, según configuración acordada. |

La solución puede organizarse en `src/` y `tests/`. Esta estructura y los nombres definitivos deben confirmarse antes de crear proyectos.

### 4.3 Flujo de una operación

1. El cliente envía un DTO HTTP; no envía un total ni un precio de venta que el servidor deba aceptar como autoridad.
2. La API comprueba formato y campos requeridos y ejecuta el caso de uso.
3. El dominio valida invariantes y calcula importes usando los datos vigentes del servidor.
4. La infraestructura persiste los cambios en una transacción. En una venta, todas las líneas y actualizaciones de stock se confirman juntas o ninguna se confirma.
5. La API responde con DTO y código HTTP apropiados. El cliente presenta el resultado y los errores recibidos, sin reproducir las reglas como fuente de verdad.

### 4.4 Modelo de datos conceptual

- **Producto:** identificador, SKU único, nombre, categoría, precio de venta, stock y estado activo.
- **Cliente:** identificador, documento único, nombre, correo y teléfono.
- **Proveedor:** identificador, NIT único, razón social, contacto, teléfono y correo.
- **Compra:** identificador, proveedor, fecha, líneas y total calculado.
- **Línea de compra:** producto, cantidad y costo unitario; permite conservar el costo que se registró en esa compra.
- **Venta:** identificador, cliente, fecha, líneas y total calculado.
- **Línea de venta:** producto, cantidad y precio unitario aplicado al momento de la venta; permite que cambios posteriores en el catálogo no alteren el historial.

Los tipos, restricciones, índices, obligatoriedad de campos y precisión monetaria deben concretarse durante el diseño detallado, respetando las decisiones aprobadas.

### 4.5 API inicial sugerida

Como punto de partida, se propone una API REST versionada, por ejemplo bajo `/api/v1`, con recursos para productos, clientes, proveedores, compras, ventas y consultas de inventario. Las operaciones de registrar compras y ventas serían comandos que reciben líneas y referencias, y el servidor devolvería el registro persistido con sus precios y totales definitivos.

Rutas, filtros, paginación, formato de errores y política de versionado deben definirse en el diseño de API. La búsqueda y paginación se mencionan como mejora posterior y no se consideran requisito de la primera versión salvo aprobación expresa.

## 5. Reglas de negocio del enunciado

Estas reglas se consideran confirmadas por el enunciado y deben implementarse y probarse:

| ID | Regla confirmada |
|---|---|
| R-01 | El SKU de un producto es único. |
| R-02 | El precio de venta de un producto es mayor que cero. |
| R-03 | El stock nunca es negativo y no se edita manualmente; solo cambia mediante compras y ventas. |
| R-04 | Una compra tiene al menos una línea; cada línea tiene cantidad mayor que cero y costo unitario mayor que cero. |
| R-05 | Una venta tiene al menos una línea; cada línea tiene cantidad mayor que cero. |
| R-06 | Una venta es atómica: si alguna línea no tiene stock suficiente, no se guarda la venta y no cambia el stock de ningún producto. |
| R-07 | El precio de cada línea de venta lo determina el servidor con el precio vigente del producto; cualquier precio enviado por el cliente se ignora. |
| R-08 | El servidor calcula el total de cada compra y venta. |
| R-09 | Un producto con compras o ventas no se borra, sino que se desactiva; un producto inactivo no se puede vender. |
| R-10 | El documento del cliente y el NIT del proveedor son únicos. |

La implementación debe reforzar estas reglas tanto en la lógica de dominio como en la persistencia cuando sea pertinente. La atomicidad y la ausencia de sobreventa también deben comprobarse bajo operaciones concurrentes, no solo en el flujo secuencial de una prueba.

## 6. Decisiones de negocio pendientes de aprobación

Las siguientes propuestas cubren ambigüedades del enunciado. Se recomienda aprobarlas, modificarlas o rechazarlas antes de implementarlas:

| ID | Pregunta o ambigüedad | Propuesta para aprobación |
|---|---|---|
| P-01 | ¿Con qué stock comienza un producto y cómo se corrige un error de inventario, dado que el stock no se edita a mano? | Crear productos con stock inicial cero. En la versión 1 no permitir ajustes manuales; las correcciones requerirían un proceso formal futuro de movimientos de inventario. |
| P-02 | ¿Se permite repetir el mismo producto en varias líneas de una compra o venta? | Rechazar líneas duplicadas del mismo producto dentro de una operación, para evitar ambigüedad en validación y presentación. |
| P-03 | ¿Se pueden comprar productos inactivos? ¿Y vender a clientes o comprar a proveedores inactivos? | Impedir nuevas operaciones con productos inactivos; impedir compras/ventas con proveedores/clientes inactivos. El enunciado solo prohíbe explícitamente vender productos inactivos y no especifica el estado de clientes/proveedores; por tanto, esta propuesta necesita confirmación. |
| P-04 | ¿Qué campos y comportamientos tienen clientes y proveedores activos/desactivados? | Añadir estado activo a ambos catálogos, permitir desactivarlos sin borrar su historial y definir explícitamente si la desactivación bloquea nuevas operaciones, según P-03. |
| P-05 | ¿Cómo se representa el dinero y cómo se redondean importes? ¿Se incluyen impuestos? | Manejar importes como valores decimales en COP, con precisión y redondeo definidos de forma consistente; no agregar IVA ni otros impuestos porque el enunciado no los solicita. Acordar precisión y redondeo antes de persistir o exponer importes. |
| P-06 | ¿Qué validaciones tienen correo, teléfono, documento y NIT, más allá de la unicidad indicada? | Validar campos requeridos y formato básico razonable, sin imponer reglas de longitud o formato fiscal no especificadas hasta su aprobación. |
| P-07 | ¿La unicidad de SKU, documento y NIT distingue mayúsculas, minúsculas, espacios o signos de puntuación? | Recortar espacios externos y comparar de manera insensible a mayúsculas/minúsculas; conservar el valor de presentación. No normalizar documentos o NIT eliminando signos hasta acordarlo. |
| P-08 | ¿Se pueden editar o desactivar clientes, proveedores y productos referenciados por operaciones históricas? | No borrar registros que tengan historial. Permitir editar datos de catálogo sin modificar los datos históricos copiados a compras/ventas; desactivar en vez de borrar. Confirmar si se permiten cambios de SKU, documento y NIT una vez usados. |
| P-09 | ¿Qué debe suceder si una compra contiene un producto inactivo, o si el precio del producto cambia después de una compra/venta? | Rechazar productos inactivos en compras (sujeto a P-03); conservar en cada línea los valores unitarios de esa operación, para que el historial y sus totales no cambien retrospectivamente. |
| P-10 | ¿Qué datos deben incluir las consultas y qué filtros son indispensables para la versión 1? | Incluir fecha, tercero, detalle, cantidades, precios/costos unitarios y total; consulta de existencias con SKU, nombre, categoría, stock y estado. Dejar búsqueda avanzada, filtros y paginación fuera de la primera versión salvo necesidad expresa. |
| P-11 | ¿Qué respuesta recibe una solicitud inválida y cómo se distinguen errores de formato y conflictos de reglas? | Usar `400` para datos/formato inválidos, `404` para recursos inexistentes y `409` para conflictos de negocio, como stock insuficiente, SKU duplicado o intento de vender producto inactivo. Mantener un cuerpo de error uniforme y documentado. |
| P-12 | ¿Qué categorías y datos de ejemplo deben cargarse automáticamente? | Tratar las categorías como texto inicialmente y proporcionar los datos del enunciado mediante una carga de desarrollo o instrucciones reproducibles, no crear registros de producción silenciosamente. Confirmar si se desea un mecanismo de seed. |

## 7. Plan de desarrollo propuesto

El orden siguiente respeta la dependencia entre componentes y permite validar reglas antes de construir toda la interfaz:

| Etapa | Entregable | Criterio de salida |
|---|---|---|
| 0. Aprobación | Decisiones de P-01 a P-12 resueltas; alcance y criterios de aceptación acordados. | No quedan reglas de negocio ambiguas que bloqueen el diseño del modelo o las operaciones. |
| 1. Base de solución | Solución .NET 10, proyectos y referencias, configuración local, README inicial y estructura de pruebas. | La solución restaura y compila; no se versionan secretos ni archivos de base de datos local. |
| 2. Contratos y dominio | DTO iniciales, entidades e invariantes; pruebas unitarias del dominio. | Las reglas R-01 a R-10 aplicables tienen pruebas, sin depender de API ni base de datos. |
| 3. Persistencia | EF Core, esquema SQLite, migraciones, índices y transacciones. | Se puede crear la base desde cero; relaciones y restricciones respaldan las reglas confirmadas. |
| 4. API y catálogos | API para productos, clientes y proveedores; validación y errores HTTP uniformes. | Operaciones de catálogo verificables por HTTP y códigos `400`, `404` y `409` según contrato. |
| 5. Compras y consultas | Registro transaccional de compras; consulta de existencias e historial con detalle y total. | Una compra válida incrementa el stock una sola vez y conserva sus precios/costos históricos. |
| 6. Ventas | Registro atómico de ventas, precio de servidor y validación de stock/estado. | Sin stock suficiente no persiste venta ni cambia stock; una venta válida registra precios vigentes y descuenta existencias. |
| 7. Cliente Blazor | Pantallas de catálogos, compras, ventas y consultas, consumiendo los contratos y la API. | Recorridos principales funcionan de extremo a extremo; errores del servidor se muestran claramente. |
| 8. Cierre de versión 1 | Pruebas de integración y aceptación, documentación, datos de ejemplo e instrucciones de ejecución. | Criterios de aceptación aprobados, compilación y pruebas satisfactorias, y puesta en marcha reproducible. |

## 8. Estrategia de pruebas y aceptación

- **Pruebas unitarias de dominio:** unicidad, importes positivos, líneas no vacías, stock, producto inactivo, cálculo de totales y cualquier decisión aprobada.
- **Pruebas de aplicación:** resultado de casos de uso y comportamiento ante recursos inexistentes o conflictos.
- **Pruebas de infraestructura/API:** persistencia real con SQLite de prueba, restricciones, migraciones, transacciones y respuestas HTTP.
- **Pruebas de atomicidad:** una venta con varias líneas y una línea sin stock no debe guardar cabecera ni detalle ni modificar stock de ninguna línea.
- **Pruebas de confianza del servidor:** enviar precios/totales manipulados desde el cliente no debe alterar el valor persistido.
- **Pruebas de interfaz:** validar los flujos principales de catálogo, compra, venta y consulta, incluyendo la presentación de errores.
- **Pruebas de sustitución de motor:** revisar las dependencias para confirmar que el cambio de proveedor se limita a infraestructura/configuración; realizar una prueba con otro motor solo si se aprueba y está disponible en el entorno.

## 9. Documentación y operación

La entrega deberá incluir README con requisitos, configuración, migraciones, ejecución de API y cliente, y ejecución de pruebas; comentarios XML en miembros públicos según C-08; y bitácora de uso de IA conforme al formato que acuerde el equipo. La configuración sensible se suministrará fuera del repositorio y la base de datos local se excluirá del control de versiones.

## 10. Aprobación requerida antes de implementar

Antes de comenzar el desarrollo, se solicita aprobar:

1. La arquitectura por proyectos y el uso de API REST versionada.
2. Las propuestas de negocio P-01 a P-12, indicando cuáles se aceptan o qué cambios requieren.
3. El orden y los criterios de salida del plan de desarrollo.
4. Las decisiones pendientes de precisión monetaria, validación de datos, operaciones con entidades inactivas y carga de datos de ejemplo.

Una vez recibida la aprobación, las decisiones aceptadas se convertirán en criterios de aceptación y pruebas; las rechazadas o modificadas se actualizarán aquí antes de iniciar su implementación.
