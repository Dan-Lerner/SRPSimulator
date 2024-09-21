using SRPConfig;
using SRPSimulator.MathModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;

namespace SRPSimulator.PropertiesControl
{
    // Code behind class for config panel (Window forms)

    public partial class PropertiesPanel : UserControl
    {
        public PropertiesControl ParentControl;

        public PropertiesPanel()
        {
            // Avoidng errors while desingtime
            if (!DesignModeStatus.IsInDesignMode)
                InitializeComponent();
        }

        private void PropertiesPanel_Load(object sender, EventArgs e)
        {
            TabChanged(tabsParametersGroups.SelectedIndex);

            // Bindings to WPF adapter
            buttonDelete.DataBindings.Add(new Binding("Enabled", ParentControl, "CanDelete"));
            buttonSave.DataBindings.Add(new Binding("Enabled", ParentControl, "CanSave"));
            labelConnection.DataBindings.Add(new Binding("Text", ParentControl, "ServerStatus"));
            //comboConfigSelect.DataBindings.Add(new Binding("Text", ParentControl, "ConfigName",
            //    false, DataSourceUpdateMode.OnPropertyChanged));
        }

        // Control's commands handlers

        private void comboConfigSelect_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                OnSaveConfig();
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            ParentControl.CommandReset.Execute(Name);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            OnSaveConfig();
        }

        private void OnSaveConfig()
        {
            string Name = comboConfigSelect.Text;
            if (comboConfigSelect.SelectedIndex != -1) {
                string message = "Are you sure you want to overwrite \"" + Name + "\" configuration?";
                const string caption = "Configuration changing";
                var result = MessageBox.Show(message, caption,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;
            }
            ParentControl.CommandSave.Execute(Name);
            CommandManager.InvalidateRequerySuggested();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            string Name = comboConfigSelect.Text;
            string message = "Are you sure you want to delete \"" + Name + "\" configuration?";
            const string caption = "Configuration changing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Warning);
            if (result == DialogResult.No)
                return;
            ParentControl.CommandDelete.Execute(Name);
            CommandManager.InvalidateRequerySuggested();
        }

        // Control's events handlers

        private void tabsParametersGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabChanged(tabsParametersGroups.SelectedIndex);
        }

        private void comboConfigSelect_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ParentControl.SelectedConfig = comboConfigSelect.SelectedIndex;
        }

        private void comboConfigSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboConfigSelect.SelectAll();
        }

        private void comboConfigSelect_TextUpdate(object sender, EventArgs e)
        {
            ParentControl.ConfigName = comboConfigSelect.Text;
        }

        private void SRPProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // Validation of config object after parameter changing

            var context = new ValidationContext(SRPProperties.SelectedObject);
            var results = new List<ValidationResult>();

            if (Validator.TryValidateObject(SRPProperties.SelectedObject, context, results, true))
                ParentControl.CommandSaveState.Execute(null);
            else {
                System.Text.StringBuilder messageBoxCS1 = new System.Text.StringBuilder();

                foreach (var error in results) {
                    messageBoxCS1.AppendFormat("{0}", error.ErrorMessage);
                    messageBoxCS1.AppendLine();
                }

                MessageBox.Show(messageBoxCS1.ToString(), "Incorrect Value", 
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Object obj = e.ChangedItem.PropertyDescriptor.GetEditor(
                    typeof(System.Drawing.Design.UITypeEditor));

                e.ChangedItem.PropertyDescriptor.SetValue(SRPProperties.SelectedObject, 
                    e.OldValue);
                e.ChangedItem.Select();
            }

            SRPProperties.Refresh();
            SRPProperties.ExpandAllGridItems();

            CommandManager.InvalidateRequerySuggested();
        }

        private void SRPProperties_SelectedGridItemChanged(object sender, 
            SelectedGridItemChangedEventArgs e)
        {
            if (SRPProperties.SelectedObject.GetType() == typeof(PUnitConfigBrowsable))
                Scheme.Image = GetSchemeImage(e.NewSelection.Label);
        }

        //  Additional processing functions

        private void TabChanged(int Index)
        {
            if (ParentControl.SelectedObjectIndex != Index)
                ParentControl.SelectedObjectIndex = Index;

            SetCurrentConfigGroup(Index);
        }

        private void SetCurrentConfigGroup(int index)
        {
            Scheme.Visible = (index == 0);
            CommandManager.InvalidateRequerySuggested();
        }

        private Image GetSchemeImage(String label) => label switch
        {
            MathModel.PUnitConfigBrowsable.labelSizeA => Properties.Resource.UnitGeometryA as Bitmap,
            MathModel.PUnitConfigBrowsable.labelSizeC => Properties.Resource.UnitGeometryC as Bitmap,
            MathModel.PUnitConfigBrowsable.labelSizeG => Properties.Resource.UnitGeometryG as Bitmap,
            MathModel.PUnitConfigBrowsable.labelSizeH => Properties.Resource.UnitGeometryH as Bitmap,
            MathModel.PUnitConfigBrowsable.labelSizeI => Properties.Resource.UnitGeometryI as Bitmap,
            MathModel.PUnitConfigBrowsable.labelSizeK => Properties.Resource.UnitGeometryK as Bitmap,
            MathModel.PUnitConfigBrowsable.labelSizeP => Properties.Resource.UnitGeometryP as Bitmap,
            MathModel.PUnitConfigBrowsable.labelSizeR => Properties.Resource.UnitGeometryR as Bitmap,
            _ => Properties.Resource.UnitGeometry as Bitmap
        };

        // Interface functions for adapter class

        internal void UpdateTabs(List<ViewModel.ConfigObjectProperties> items)
        {
            tabsParametersGroups.TabPages.Clear();
            foreach (var item in items) {
                TabPage newTabPage = new TabPage();
                newTabPage.Text = item.Name;
                tabsParametersGroups.TabPages.Add(newTabPage);
            }
        }

        public void SetObject(object Object)
        {
            SRPProperties.SelectedObject = Object;
            SRPProperties.ExpandAllGridItems();
            CommandManager.InvalidateRequerySuggested();
        }

        internal void BindConfigsNames(List<ConfigIdentity> configItems, 
            string Id, string DisplayedName)
        {
            comboConfigSelect.ValueMember = Id;
            comboConfigSelect.DisplayMember = DisplayedName;
            comboConfigSelect.DataSource = configItems;
            CommandManager.InvalidateRequerySuggested();
        }

        public void SetSelectedConfig(int selectedConfig)
        {
            comboConfigSelect.SelectedIndex = selectedConfig;
            CommandManager.InvalidateRequerySuggested();
        }

        public void SetConfigName(string Name)
        {
            comboConfigSelect.Text = Name;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
