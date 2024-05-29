using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPro
{
    internal class Abastecimiento
    {
         public DateTime FechaHora { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string IdCliente { get; set; }
    public double CantidadAbastecida { get; set; }
    public string TipoAbastecimiento { get; set; }

    public Abastecimiento(string nombreCliente, string apellidoCliente, string idCliente, double cantidadAbastecida, string tipoAbastecimiento)
    {
        FechaHora = DateTime.Now;
        NombreCliente = nombreCliente;
        ApellidoCliente = apellidoCliente;
        IdCliente = idCliente;
        CantidadAbastecida = cantidadAbastecida;
        TipoAbastecimiento = tipoAbastecimiento;
    }
    }
}
