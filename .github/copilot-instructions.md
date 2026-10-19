# Estándares de desarrollo .NET para TiendaMinisterio

## Arquitectura y diseño

1. Usa .NET 10 y C# en los proyectos del servidor y del cliente.
2. Separa contratos, dominio, aplicación, infraestructura, API y cliente en los límites de proyecto acordados.
3. Mantén cada clase en un archivo con una responsabilidad principal.
4. Mantén el dominio independiente de ASP.NET Core, EF Core, SQLite y Blazor.
5. El cliente Blazor consume la API por HTTP/JSON; no referencia infraestructura ni accede a la base de datos.
6. Mantén dependencias dirigidas a abstracciones donde corresponda.

## Código y documentación

7. Usa nombres explícitos, `PascalCase` para tipos y miembros públicos y `camelCase` para variables locales y parámetros.
8. Devuelve errores explícitos para fallos esperados y controla las excepciones en el límite apropiado, sin ocultarlas con valores nulos ni exponer detalles internos.
9. Documenta los tipos y miembros públicos con comentarios XML, incluidos `<summary>` y, cuando aplique, `<param>` y `<returns>`.
10. Mantén el README actualizado con configuración, migraciones, ejecución de API y cliente, y comandos de prueba.

## API

11. Versiona las rutas bajo `/api/v1` y usa recursos para `/productos`, `/clientes`, `/proveedores`, `/compras`, `/ventas` e `/inventario`.
12. Usa `GET` y `POST` en colecciones, `GET` y `PUT` en recursos individuales; registra compras y ventas mediante `POST` y consulta inventario mediante `GET`.
13. Desactiva registros mediante una operación explícita; no borres físicamente productos con historial.
14. Responde `400` por formato o datos inválidos, `404` por recursos inexistentes y `409` por conflictos de reglas; usa `201` al crear y `200` o `204` según el contenido de éxito.
15. Devuelve todos los errores en formato uniforme `ProblemDetails`; para validación, incluye los errores por campo en `errors`.
16. Documenta las rutas y sus códigos de respuesta en el contrato o README; no implementes filtros o paginación sin aprobación del alcance.

## Datos y persistencia

17. Usa Entity Framework Core para persistencia y consultas; no escribas SQL manual ni ejecutes SQL crudo.
18. Mantén `DbContext`, configuraciones, repositorios, migraciones y selección del proveedor en infraestructura.
19. Accede a persistencia mediante repositorios o las abstracciones acordadas; no uses `DbContext` desde dominio, cliente ni endpoints.
20. Mantén el cambio de motor limitado a infraestructura y configuración, sin dependencias de SQLite en otros proyectos.
21. Persiste cada compra y venta junto con sus líneas e importes históricos.
22. Ejecuta cada operación de compra o venta en una transacción; una venta fallida no persiste líneas ni modifica stock.

## Seguridad y validación

23. Valida en el servidor todos los datos recibidos, incluso si el cliente ya los validó.
24. Trata toda entrada del cliente como no confiable; nunca aceptes del cliente precios de venta, totales ni cambios directos de stock.
25. Calcula en el servidor los precios de venta y los totales de compras y ventas usando datos vigentes.
26. Comprueba las reglas del dominio antes de persistir: unicidad, importes y cantidades positivos, stock suficiente y estado activo del producto vendido.
27. Refuerza en persistencia las restricciones que correspondan, incluidas las claves únicas y relaciones requeridas.
28. No guardes secretos ni bases de datos locales en el repositorio; configura los datos sensibles fuera del control de versiones.

## Pruebas

29. Escribe las pruebas en `tests/TiendaMHCP.Pruebas` con xUnit.
30. Cubre cada regla de negocio del núcleo con al menos una prueba basada en los criterios de `Doc/análisis.md`.
31. Escribe las pruebas con el patrón Arrange-Act-Assert y nombres `Metodo_Escenario_Resultado`.
32. Las pruebas unitarias del dominio no usan base de datos, API ni la fecha real.
33. Prueba separadamente los casos inválidos, límites y regresiones relevantes de cada cambio.
34. Las pruebas de integración verifican HTTP, persistencia, transacciones y errores conforme al contrato, incluida la atomicidad de ventas y el rechazo de precios o totales manipulados.
35. Nunca modifiques una prueba para hacer que pase; corrige la implementación o reporta el criterio que debe aclararse.

## Forma de trabajar y verificación

36. Antes de editar, identifica la causa raíz y el punto exacto que requiere cambio; no conviertas decisiones pendientes de `Doc/análisis.md` en requisitos sin aprobación.
37. Mantén los cambios pequeños, pertinentes y verificables; no hagas refactorizaciones ajenas al objetivo.
38. Verifica cada etapa con `dotnet build` y `dotnet test`; ambos deben terminar sin errores ni advertencias.

## Bitácora de cambios con IA

Después de cada cambio que hagas en el proyecto, agrega una entrada al final de docs/bitacora-ia.md con este formato:

## AAAA-MM-DD HH:mm · Etapa X.Y · Título corto
- Cambio: qué se hizo, en una o dos líneas.
- Motivo: el hallazgo del diagnóstico o la etapa del plan que lo pide.
- Archivos: los archivos creados o modificados.
- Verificación: cómo se comprobó.
- Decisión del equipo: pendiente

Para la fecha y la hora, ejecuta en la terminal: Get-Date -Format "yyyy-MM-dd HH:mm"
No borres ni cambies entradas anteriores. Si no hiciste cambios, no agregues entrada.
## Verificación

- No declares una etapa terminada si `dotnet build` o `dotnet test` produce errores o advertencias.