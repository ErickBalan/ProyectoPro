using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
namespace ProyectoPro
{
    internal class Gasolinera
    {
        public List<Bomba> Bombas { get; set; }
        public double PrecioPorLitro { get; set; }

        public Gasolinera(double precioPorLitro)
        {
            Bombas = new List<Bomba> { new Bomba(1), new Bomba(2) };
            PrecioPorLitro = precioPorLitro;
        }

      public void GuardarAbastecimiento(string nuevoDato)
      {
    string filePath = "Cuentas.txt";
    List<string> datos = new List<string>();

    if (File.Exists(filePath))
    {
        datos = File.ReadAllLines(filePath).ToList();
    }

    datos.Add(nuevoDato);

    File.WriteAllLines(filePath, datos);
     }


    }

   
}
