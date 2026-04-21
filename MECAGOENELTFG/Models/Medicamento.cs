using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.Models
{
    public partial class Medicamento : ObservableObject
    {

        [ObservableProperty]
        public int idMedica;

        [ObservableProperty]
        public string nomMedica;

        [ObservableProperty]
        public float gramos;

        [ObservableProperty]
        public float precio;

        [ObservableProperty]
        public int stock;

        [ObservableProperty]
        public bool estado;

    }
}
