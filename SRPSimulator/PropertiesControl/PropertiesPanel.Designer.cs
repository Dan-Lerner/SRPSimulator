namespace SRPSimulator.PropertiesControl
{
    partial class PropertiesPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabsParametersGroups = new System.Windows.Forms.TabControl();
            one = new System.Windows.Forms.TabPage();
            two = new System.Windows.Forms.TabPage();
            panel1 = new System.Windows.Forms.Panel();
            comboConfigSelect = new System.Windows.Forms.ComboBox();
            buttonDefault = new System.Windows.Forms.Button();
            buttonSave = new System.Windows.Forms.Button();
            buttonDelete = new System.Windows.Forms.Button();
            SRPProperties = new System.Windows.Forms.PropertyGrid();
            Scheme = new System.Windows.Forms.PictureBox();
            panel2 = new System.Windows.Forms.Panel();
            labelConnection = new System.Windows.Forms.Label();
            tabsParametersGroups.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Scheme).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // tabsParametersGroups
            // 
            tabsParametersGroups.Controls.Add(one);
            tabsParametersGroups.Controls.Add(two);
            tabsParametersGroups.Dock = System.Windows.Forms.DockStyle.Top;
            tabsParametersGroups.ItemSize = new System.Drawing.Size(40, 20);
            tabsParametersGroups.Location = new System.Drawing.Point(0, 0);
            tabsParametersGroups.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tabsParametersGroups.Name = "tabsParametersGroups";
            tabsParametersGroups.SelectedIndex = 0;
            tabsParametersGroups.Size = new System.Drawing.Size(354, 39);
            tabsParametersGroups.TabIndex = 1;
            tabsParametersGroups.SelectedIndexChanged += tabsParametersGroups_SelectedIndexChanged;
            // 
            // one
            // 
            one.Font = new System.Drawing.Font("Segoe UI", 9F);
            one.Location = new System.Drawing.Point(4, 24);
            one.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            one.Name = "one";
            one.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            one.Size = new System.Drawing.Size(346, 11);
            one.TabIndex = 1;
            one.Text = "one";
            one.UseVisualStyleBackColor = true;
            // 
            // two
            // 
            two.Location = new System.Drawing.Point(4, 24);
            two.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            two.Name = "two";
            two.Size = new System.Drawing.Size(346, 11);
            two.TabIndex = 2;
            two.Text = "two";
            two.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(comboConfigSelect);
            panel1.Controls.Add(buttonDefault);
            panel1.Controls.Add(buttonSave);
            panel1.Controls.Add(buttonDelete);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 39);
            panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(354, 32);
            panel1.TabIndex = 0;
            // 
            // comboConfigSelect
            // 
            comboConfigSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            comboConfigSelect.FormattingEnabled = true;
            comboConfigSelect.Items.AddRange(new object[] { "Pumping Unit", "Sucker Rod Pump", "Drive", "Well fluid", "Tubing", "Rod" });
            comboConfigSelect.Location = new System.Drawing.Point(0, 0);
            comboConfigSelect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            comboConfigSelect.Name = "comboConfigSelect";
            comboConfigSelect.Size = new System.Drawing.Size(195, 28);
            comboConfigSelect.TabIndex = 2;
            comboConfigSelect.SelectedIndexChanged += comboConfigSelect_SelectedIndexChanged;
            comboConfigSelect.SelectionChangeCommitted += comboConfigSelect_SelectionChangeCommitted;
            comboConfigSelect.TextUpdate += comboConfigSelect_TextUpdate;
            comboConfigSelect.KeyPress += comboConfigSelect_KeyPress;
            // 
            // buttonDefault
            // 
            buttonDefault.Dock = System.Windows.Forms.DockStyle.Right;
            buttonDefault.FlatAppearance.BorderSize = 0;
            buttonDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonDefault.Location = new System.Drawing.Point(195, 0);
            buttonDefault.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            buttonDefault.Name = "buttonDefault";
            buttonDefault.Size = new System.Drawing.Size(53, 32);
            buttonDefault.TabIndex = 5;
            buttonDefault.Text = "Def";
            buttonDefault.UseVisualStyleBackColor = true;
            buttonDefault.Click += buttonDefault_Click;
            // 
            // buttonSave
            // 
            buttonSave.Dock = System.Windows.Forms.DockStyle.Right;
            buttonSave.FlatAppearance.BorderSize = 0;
            buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonSave.Location = new System.Drawing.Point(248, 0);
            buttonSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(53, 32);
            buttonSave.TabIndex = 3;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonDelete
            // 
            buttonDelete.Dock = System.Windows.Forms.DockStyle.Right;
            buttonDelete.FlatAppearance.BorderSize = 0;
            buttonDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonDelete.Location = new System.Drawing.Point(301, 0);
            buttonDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new System.Drawing.Size(53, 32);
            buttonDelete.TabIndex = 4;
            buttonDelete.Text = "Del";
            buttonDelete.UseVisualStyleBackColor = true;
            buttonDelete.Click += buttonDelete_Click;
            // 
            // SRPProperties
            // 
            SRPProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            SRPProperties.Location = new System.Drawing.Point(0, 71);
            SRPProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            SRPProperties.Name = "SRPProperties";
            SRPProperties.Size = new System.Drawing.Size(354, 482);
            SRPProperties.TabIndex = 5;
            SRPProperties.PropertyValueChanged += SRPProperties_PropertyValueChanged;
            SRPProperties.SelectedGridItemChanged += SRPProperties_SelectedGridItemChanged;
            // 
            // Scheme
            // 
            Scheme.BackColor = System.Drawing.SystemColors.ControlLightLight;
            Scheme.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Scheme.Dock = System.Windows.Forms.DockStyle.Bottom;
            Scheme.Image = Properties.Resource.UnitGeometry;
            Scheme.Location = new System.Drawing.Point(0, 553);
            Scheme.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Scheme.Name = "Scheme";
            Scheme.Size = new System.Drawing.Size(354, 287);
            Scheme.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            Scheme.TabIndex = 7;
            Scheme.TabStop = false;
            // 
            // panel2
            // 
            panel2.Controls.Add(labelConnection);
            panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            panel2.Location = new System.Drawing.Point(0, 840);
            panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(354, 93);
            panel2.TabIndex = 8;
            // 
            // labelConnection
            // 
            labelConnection.AllowDrop = true;
            labelConnection.Location = new System.Drawing.Point(5, 4);
            labelConnection.Name = "labelConnection";
            labelConnection.Size = new System.Drawing.Size(346, 85);
            labelConnection.TabIndex = 0;
            labelConnection.Text = "Disconnected";
            // 
            // PropertiesPanel
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(SRPProperties);
            Controls.Add(Scheme);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(tabsParametersGroups);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "PropertiesPanel";
            Size = new System.Drawing.Size(354, 933);
            Load += PropertiesPanel_Load;
            tabsParametersGroups.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Scheme).EndInit();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabsParametersGroups;
        private System.Windows.Forms.TabPage two;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboConfigSelect;
        private System.Windows.Forms.PropertyGrid SRPProperties;
        private System.Windows.Forms.PictureBox Scheme;
        private System.Windows.Forms.TabPage one;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelConnection;
        private System.Windows.Forms.Button buttonDefault;
    }
}
