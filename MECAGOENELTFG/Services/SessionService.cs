using MECAGOENELTFG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Services
{
    public static class SessionService
    {
        //CLiente
        public static int IdClienteActual { get; set; } = 0;


        //PRofesional
        public static int IdProfesional { get; set; }
        public static string NombreProfesional { get; set; } = string.Empty;

        //Temporales

        public static RegistroMascota? RegistroEdicion { get; set; }
        public static Medicamento? MedicamentoEdicion { get; set; }

        public static Cita? CitaEdicion { get; set; }
    }
}
