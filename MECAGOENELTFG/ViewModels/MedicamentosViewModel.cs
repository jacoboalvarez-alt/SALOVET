using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MECAGOENELTFG.ViewModels
{
    partial class MedicamentosViewModel : ObservableObject
    {
        [RelayCommand]
        public async Task IrADashboard()
        {
            await Shell.Current.GoToAsync("ProfDashBoard");
        }
    }
}
