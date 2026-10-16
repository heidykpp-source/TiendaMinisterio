# Enunciado: tienda de equipos tecnológicos
 
## El negocio
 
Una tienda de Bogotá vende portátiles, monitores, periféricos y accesorios. Les compra los equipos a varios proveedores y se los vende a personas y empresas. Hoy lleva todo en hojas de cálculo: las compras en una, las ventas en otra y el stock en una tercera que casi nunca cuadra. Ya les pasó vender un portátil que no tenían.
 
Quieren un sistema cliente-servidor: un servidor con las reglas y los datos, y una aplicación gráfica para el personal de la tienda.
 
## Lo que necesita la tienda
 
**Productos.** Registrar, consultar, editar y desactivar productos. De cada producto se guarda el SKU, el nombre, la categoría, el precio de venta, el stock y si está activo.
 
**Clientes.** Registrar, consultar, editar y desactivar clientes. De cada cliente se guarda el documento, el nombre, el correo y el teléfono.
 
**Proveedores.** Registrar, consultar, editar y desactivar proveedores. De cada proveedor se guarda el NIT, la razón social, el nombre del contacto, el teléfono y el correo.
 
**Compras.** Una compra se le hace a un proveedor e incluye uno o varios productos, cada uno con su cantidad y su costo unitario. Al registrar la compra, el stock de cada producto sube.
 
**Ventas.** Una venta se le hace a un cliente e incluye uno o varios productos, cada uno con su cantidad. Al registrar la venta, el stock de cada producto baja.
 
**Consultas.** El stock actual de los productos, y las compras y ventas registradas con su detalle y su total.
 
## Reglas del negocio
 
| ID | Regla |
|---|---|
| R-01 | El SKU de un producto es único. |
| R-02 | El precio de venta de un producto es mayor que cero. |
| R-03 | El stock nunca es negativo y no se edita a mano: solo cambia con compras y ventas. |
| R-04 | Una compra tiene al menos una línea. Cada línea tiene cantidad mayor que cero y costo unitario mayor que cero. |
| R-05 | Una venta tiene al menos una línea. Cada línea tiene cantidad mayor que cero. |
| R-06 | Una venta se registra completa o no se registra: si alguna línea no tiene stock suficiente, la venta no se guarda y el stock de ningún producto cambia. |
| R-07 | El precio de cada línea de venta lo pone el servidor, con el precio de venta del producto en ese momento. Si el cliente envía un precio, se ignora. |
| R-08 | El total de una compra o de una venta lo calcula el servidor. |
| R-09 | Un producto que ya tiene compras o ventas no se borra: se desactiva. Un producto inactivo no se puede vender. |
| R-10 | El documento de un cliente y el NIT de un proveedor son únicos. |
 
## Consideraciones técnicas
 
| ID | Consideración |
|---|---|
| C-01 | .NET 10 y C# en todas las partes: una API web en el servidor y, en una etapa posterior, un cliente gráfico en Blazor WebAssembly. |
| C-02 | Cliente-servidor: el cliente solo habla con la API por HTTP y JSON. No accede a la base de datos ni tiene reglas del negocio. |
| C-03 | Base de datos SQLite con Entity Framework Core. Debe poder cambiarse a otro motor (por ejemplo SQL Server o PostgreSQL) tocando solo el proyecto de datos y la configuración. |
| C-04 | Las reglas del negocio se pueden probar sin base de datos y sin la API. Las pruebas se escriben en el Módulo 3. |
| C-05 | Los datos que viajan entre el cliente y el servidor están en su propio proyecto, para que el cliente los use sin copiarlos. |
| C-06 | El servidor valida todo lo que recibe y nunca confía en el cliente. Responde con códigos HTTP claros: 400 si los datos no son válidos, 404 si algo no existe y 409 si se viola una regla del negocio. |
| C-07 | No hay secretos en el repositorio. La base de datos de cada computador no se sube. |
| C-08 | El proyecto está documentado: README, comentarios XML en todo lo público y la bitácora de IA. |
 
## Alcance
 
**Versión 1.** Todo lo anterior: el servidor completo y el cliente gráfico.
 
**Para después.** Mejoras que el equipo puede proponer cuando la versión 1 esté lista: alerta de stock mínimo, anular una venta (devuelve el stock), reporte de ventas por mes, descuentos, número de serie y garantía, búsqueda y paginación, usuarios y permisos.
 
## Datos de ejemplo
 
Para que todos los equipos prueben con los mismos datos.
 
| Tipo | Datos |
|---|---|
| Producto | `PORT-001` · Portátil 14" Core i5 16 GB · Portátiles · $3.200.000 |
| Producto | `MON-024` · Monitor 24" Full HD · Monitores · $650.000 |
| Producto | `MOU-010` · Mouse inalámbrico · Periféricos · $85.000 |
| Proveedor | NIT `900123456-1` · Distribuidora Andina SAS · Laura Gómez · 601 555 0101 · ventas@andina.example |
| Cliente | Documento `1020304050` · Carlos Pérez · carlos@correo.example · 300 555 0102 |
 
 