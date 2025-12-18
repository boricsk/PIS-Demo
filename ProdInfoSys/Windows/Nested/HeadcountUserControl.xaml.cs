using ProdInfoSys.ViewModels.Nested;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProdInfoSys.Windows.Nested
{
    /// <summary>
    /// Interaction logic for HeadcountUserControl.xaml
    /// </summary>
    public partial class HeadcountUserControl : UserControl
    {
        public HeadcountUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is HeadcountViewModel vm)
            {
                vm.ForceCommit = () =>
                {
                    // 1) Commit a cellára és a sorra
                    DataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                    DataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                    // 2) Aktív vezérlő binding frissítése
                    if (Keyboard.FocusedElement is FrameworkElement fe)
                    {
                        fe.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        fe.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateSource();
                        fe.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
                        fe.GetBindingExpression(ComboBox.SelectedValueProperty)?.UpdateSource();
                        fe.GetBindingExpression(DatePicker.SelectedDateProperty)?.UpdateSource();
                    }

                    // 3) Biztonság kedvéért fókusz le-fel
                    DataGrid.Focus();
                    Keyboard.ClearFocus();

                    // 4) UI frissítés
                    DataGrid.UpdateLayout();
                };
            }
        }

    }
}
