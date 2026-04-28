using MECAGOENELTFG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.ViewModels
{
    public class MedicamentoAgregadoViewModel
    {
        public Medicamento Medicamento { get; }
        public string NomMedica => Medicamento.NomMedica;
        public float Precio => Medicamento.Precio;
        public string StockTexto => $"Stock: {Medicamento.Stock}";

        public MedicamentoAgregadoViewModel(Medicamento medicamento) => Medicamento = medicamento;
    }
    public class MedicamentoPickerItem
    {
        public Medicamento Medicamento { get; }
        // Muestra nombre, gramos y precio — y avisa si no hay stock
        public string ResumenMedicamento => Medicamento.Stock > 0
            ? $"{Medicamento.NomMedica} ({Medicamento.Gramos}g)  —  {Medicamento.Precio:C2}  · Stock: {Medicamento.Stock}"
            : $"{Medicamento.NomMedica}  —  SIN STOCK";

        public MedicamentoPickerItem(Medicamento medicamento) => Medicamento = medicamento;
    }
}
