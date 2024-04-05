using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Profisys_Zadanie
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void DatePickerButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.TemplatedParent is DatePicker datePicker)
            {
                datePicker.IsDropDownOpen = !datePicker.IsDropDownOpen;
            }
        }
    }
}
