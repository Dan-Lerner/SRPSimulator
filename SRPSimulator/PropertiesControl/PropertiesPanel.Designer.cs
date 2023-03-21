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
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabsParametersGroups = new System.Windows.Forms.TabControl();
            this.one = new System.Windows.Forms.TabPage();
            this.two = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboConfigSelect = new System.Windows.Forms.ComboBox();
            this.buttonDefault = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.SRPProperties = new System.Windows.Forms.PropertyGrid();
            this.Scheme = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelConnection = new System.Windows.Forms.Label();
            this.tabsParametersGroups.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Scheme)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabsParametersGroups
            // 
            this.tabsParametersGroups.Controls.Add(this.one);
            this.tabsParametersGroups.Controls.Add(this.two);
            this.tabsParametersGroups.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabsParametersGroups.ItemSize = new System.Drawing.Size(40, 20);
            this.tabsParametersGroups.Location = new System.Drawing.Point(0, 0);
            this.tabsParametersGroups.Name = "tabsParametersGroups";
            this.tabsParametersGroups.SelectedIndex = 0;
            this.tabsParametersGroups.Size = new System.Drawing.Size(310, 29);
            this.tabsParametersGroups.TabIndex = 1;
            this.tabsParametersGroups.SelectedIndexChanged += new System.EventHandler(this.tabsParametersGroups_SelectedIndexChanged);
            // 
            // one
            // 
            this.one.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.one.Location = new System.Drawing.Point(4, 24);
            this.one.Name = "one";
            this.one.Padding = new System.Windows.Forms.Padding(3);
            this.one.Size = new System.Drawing.Size(302, 1);
            this.one.TabIndex = 1;
            this.one.Text = "one";
            this.one.UseVisualStyleBackColor = true;
            // 
            // two
            // 
            this.two.Location = new System.Drawing.Point(4, 24);
            this.two.Name = "two";
            this.two.Size = new System.Drawing.Size(302, 1);
            this.two.TabIndex = 2;
            this.two.Text = "two";
            this.two.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboConfigSelect);
            this.panel1.Controls.Add(this.buttonDefault);
            this.panel1.Controls.Add(this.buttonSave);
            this.panel1.Controls.Add(this.buttonDelete);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(310, 24);
            this.panel1.TabIndex = 0;
            // 
            // comboConfigSelect
            // 
            this.comboConfigSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboConfigSelect.FormattingEnabled = true;
            this.comboConfigSelect.Items.AddRange(new object[] {
            "Pumping Unit",
            "Sucker Rod Pump",
            "Drive",
            "Well fluid",
            "Tubing",
            "Rod"});
            this.comboConfigSelect.Location = new System.Drawing.Point(0, 0);
            this.comboConfigSelect.Name = "comboConfigSelect";
            this.comboConfigSelect.Size = new System.Drawing.Size(172, 23);
            this.comboConfigSelect.TabIndex = 2;
            this.comboConfigSelect.SelectedIndexChanged += new System.EventHandler(this.comboConfigSelect_SelectedIndexChanged);
            this.comboConfigSelect.SelectionChangeCommitted += new System.EventHandler(this.comboConfigSelect_SelectionChangeCommitted);
            this.comboConfigSelect.TextUpdate += new System.EventHandler(this.comboConfigSelect_TextUpdate);
            this.comboConfigSelect.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboConfigSelect_KeyPress);
            // 
            // buttonDefault
            // 
            this.buttonDefault.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonDefault.FlatAppearance.BorderSize = 0;
            this.buttonDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDefault.Location = new System.Drawing.Point(172, 0);
            this.buttonDefault.Name = "buttonDefault";
            this.buttonDefault.Size = new System.Drawing.Size(46, 24);
            this.buttonDefault.TabIndex = 5;
            this.buttonDefault.Text = "Def";
            this.buttonDefault.UseVisualStyleBackColor = true;
            this.buttonDefault.Click += new System.EventHandler(this.buttonDefault_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonSave.FlatAppearance.BorderSize = 0;
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSave.Location = new System.Drawing.Point(218, 0);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(46, 24);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonDelete.FlatAppearance.BorderSize = 0;
            this.buttonDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDelete.Location = new System.Drawing.Point(264, 0);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(46, 24);
            this.buttonDelete.TabIndex = 4;
            this.buttonDelete.Text = "Del";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // SRPProperties
            // 
            this.SRPProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SRPProperties.Location = new System.Drawing.Point(0, 53);
            this.SRPProperties.Name = "SRPProperties";
            this.SRPProperties.Size = new System.Drawing.Size(310, 361);
            this.SRPProperties.TabIndex = 5;
            this.SRPProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.SRPProperties_PropertyValueChanged);
            this.SRPProperties.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.SRPProperties_SelectedGridItemChanged);
            // 
            // Scheme
            // 
            this.Scheme.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Scheme.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Scheme.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Scheme.Image = global::SRPSimulator.Properties.Resource.UnitGeometry;
            this.Scheme.Location = new System.Drawing.Point(0, 414);
            this.Scheme.Name = "Scheme";
            this.Scheme.Size = new System.Drawing.Size(310, 216);
            this.Scheme.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.Scheme.TabIndex = 7;
            this.Scheme.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelConnection);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 630);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(310, 70);
            this.panel2.TabIndex = 8;
            // 
            // labelConnection
            // 
            this.labelConnection.AllowDrop = true;
            this.labelConnection.Location = new System.Drawing.Point(4, 3);
            this.labelConnection.Name = "labelConnection";
            this.labelConnection.Size = new System.Drawing.Size(303, 64);
            this.labelConnection.TabIndex = 0;
            this.labelConnection.Text = "Disconnected";
            // 
            // PropertiesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SRPProperties);
            this.Controls.Add(this.Scheme);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabsParametersGroups);
            this.Name = "PropertiesPanel";
            this.Size = new System.Drawing.Size(310, 700);
            this.Load += new System.EventHandler(this.PropertiesPanel_Load);
            this.tabsParametersGroups.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Scheme)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

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
