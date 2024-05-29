using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPro
{
    internal class Bomba
    {
        public int Id { get; set; }
        public bool EnFuncionamiento { get; set; }
        public List<Abastecimiento> HistorialAbastecimientos { get; set; }

        public Bomba(int id)
        {
            Id = id;
            EnFuncionamiento = false;
            HistorialAbastecimientos = new List<Abastecimiento>();
        }

       
    }
}
