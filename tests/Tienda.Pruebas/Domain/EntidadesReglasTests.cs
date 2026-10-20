using Tienda.Domain.Entities;
using Tienda.Domain.Exceptions;
using Tienda.Domain.ValueObjects;

namespace Tienda.Pruebas.Domain;

public class EntidadesReglasTests
{
    // R-02
    [Theory]
    [InlineData(0.01)]
    [InlineData(100.50)]
    public void Producto_ActualizarPrecio_ConPrecioPositivo_ActualizaPrecio(decimal precio)
    {
        // Arrange
        var producto = new Producto(Guid.NewGuid(), "SKU-001", "Teclado", 25m);

        // Act
        producto.ActualizarPrecio(precio);

        // Assert
        Assert.Equal(precio, producto.PrecioUnitario);
    }

    // R-02
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Producto_ActualizarPrecio_ConPrecioNoPositivo_LanzaExcepcion(decimal precio)
    {
        // Arrange
        var producto = new Producto(Guid.NewGuid(), "SKU-001", "Teclado", 25m);

        // Act & Assert
        var exception = Assert.Throws<DomainRuleException>(() => producto.ActualizarPrecio(precio));
        Assert.Contains("precio", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // R-03
    [Fact]
    public void Producto_RegistrarVenta_ConStockExacto_DescuentaStock()
    {
        // Arrange
        var producto = new Producto(Guid.NewGuid(), "SKU-002", "Mouse", 20m, stock: 3, activo: true);

        // Act
        producto.RegistrarVenta(3);

        // Assert
        Assert.Equal(0, producto.Stock);
    }

    // R-03
    [Fact]
    public void Producto_RegistrarVenta_ConCantidadMayorAlStock_LanzaExcepcion()
    {
        // Arrange
        var producto = new Producto(Guid.NewGuid(), "SKU-003", "Monitor", 250m, stock: 2, activo: true);

        // Act & Assert
        var exception = Assert.Throws<DomainRuleException>(() => producto.RegistrarVenta(3));
        Assert.Contains("stock", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // R-03
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Producto_RegistrarVenta_ConCantidadNoPositiva_LanzaExcepcion(int cantidad)
    {
        // Arrange
        var producto = new Producto(Guid.NewGuid(), "SKU-004", "Laptop", 1000m, stock: 5, activo: true);

        // Act & Assert
        var exception = Assert.Throws<DomainRuleException>(() => producto.RegistrarVenta(cantidad));
        Assert.Contains("cantidad", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // R-04
    [Fact]
    public void Compra_ConLineaValida_CreaCompraConLineas()
    {
        // Arrange
        var lineas = new[]
        {
            new CompraLinea(Guid.NewGuid(), 2, 15.50m),
            new CompraLinea(Guid.NewGuid(), 1, 20m)
        };

        // Act
        var compra = new Compra(Guid.NewGuid(), Guid.NewGuid(), lineas, DateTime.UtcNow);

        // Assert
        Assert.Equal(2, compra.Lineas.Count);
        Assert.All(compra.Lineas, linea => Assert.True(linea.Cantidad > 0));
    }

    // R-04
    [Fact]
    public void Compra_SinLineas_LanzaExcepcion()
    {
        // Arrange
        var lineas = Array.Empty<CompraLinea>();

        // Act & Assert
        var exception = Assert.Throws<DomainRuleException>(() => new Compra(Guid.NewGuid(), Guid.NewGuid(), lineas));
        Assert.Contains("línea", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // R-04
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CompraLinea_ConCantidadNoPositiva_LanzaExcepcion(int cantidad)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainRuleException>(() => new CompraLinea(Guid.NewGuid(), cantidad, 10m));
        Assert.Contains("cantidad", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // R-04
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CompraLinea_ConCostoUnitarioNoPositivo_LanzaExcepcion(decimal costoUnitario)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainRuleException>(() => new CompraLinea(Guid.NewGuid(), 1, costoUnitario));
        Assert.Contains("costo", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // R-05
    [Fact]
    public void Venta_ConLineaValida_CreaVentaConLineas()
    {
        // Arrange
        var lineas = new[]
        {
            new VentaLinea(Guid.NewGuid(), 2, 25m),
            new VentaLinea(Guid.NewGuid(), 1, 30m)
        };

        // Act
        var venta = new Venta(Guid.NewGuid(), Guid.NewGuid(), lineas, DateTime.UtcNow);

        // Assert
        Assert.Equal(2, venta.Lineas.Count);
        Assert.All(venta.Lineas, linea => Assert.True(linea.Cantidad > 0));
    }

    // R-05
    [Fact]
    public void Venta_SinLineas_LanzaExcepcion()
    {
        // Arrange
        var lineas = Array.Empty<VentaLinea>();

        // Act & Assert
        var exception = Assert.Throws<DomainRuleException>(() => new Venta(Guid.NewGuid(), Guid.NewGuid(), lineas));
        Assert.Contains("línea", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // R-05
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void VentaLinea_ConCantidadNoPositiva_LanzaExcepcion(int cantidad)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainRuleException>(() => new VentaLinea(Guid.NewGuid(), cantidad, 25m));
        Assert.Contains("cantidad", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // R-05
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void VentaLinea_ConPrecioUnitarioNoPositivo_LanzaExcepcion(decimal precioUnitario)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainRuleException>(() => new VentaLinea(Guid.NewGuid(), 1, precioUnitario));
        Assert.Contains("precio", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // R-09
    [Fact]
    public void Producto_Desactivar_ImpideCrearVenta()
    {
        // Arrange
        var producto = new Producto(Guid.NewGuid(), "SKU-010", "Cargador", 40m, stock: 5, activo: true);

        // Act
        producto.Desactivar();

        // Assert
        var exception = Assert.Throws<DomainRuleException>(() => producto.RegistrarVenta(1));
        Assert.Contains("inactivo", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    // R-10
    [Theory]
    [InlineData("12345678", "12345678")]
    [InlineData(" 12345678 ", "12345678")]
    [InlineData("abc123", "ABC123")]
    public void DocumentoIdentidad_ConValoresEquivalentes_SeConsideranIguales(string valor1, string valor2)
    {
        // Arrange
        var documento1 = new DocumentoIdentidad(valor1);
        var documento2 = new DocumentoIdentidad(valor2);

        // Act & Assert
        Assert.Equal(documento1, documento2);
    }

    // R-10
    [Theory]
    [InlineData("1234567-8", "1234567-8")]
    [InlineData(" 1234567-8 ", "1234567-8")]
    [InlineData("abc-123", "ABC-123")]
    public void Nit_ConValoresEquivalentes_SeConsideranIguales(string valor1, string valor2)
    {
        // Arrange
        var nit1 = new Nit(valor1);
        var nit2 = new Nit(valor2);

        // Act & Assert
        Assert.Equal(nit1, nit2);
    }
}
