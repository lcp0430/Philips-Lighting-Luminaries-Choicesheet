namespace Philips_Lighting_Luminaries_Choicesheet
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn12NC = new System.Windows.Forms.Button();
            this.btnNaming = new System.Windows.Forms.Button();
            this.txt12NC = new System.Windows.Forms.TextBox();
            this.txtNaming = new System.Windows.Forms.TextBox();
            this.gbSearch = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvProduct = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbOutdoor = new System.Windows.Forms.RadioButton();
            this.rbIndoor = new System.Windows.Forms.RadioButton();
            this.gbSearch.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduct)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(113, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search by 12NC";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(540, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Search by Prisma Naming";
            // 
            // btn12NC
            // 
            this.btn12NC.AutoSize = true;
            this.btn12NC.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn12NC.BackgroundImage")));
            this.btn12NC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn12NC.Location = new System.Drawing.Point(265, 63);
            this.btn12NC.Name = "btn12NC";
            this.btn12NC.Size = new System.Drawing.Size(32, 34);
            this.btn12NC.TabIndex = 2;
            this.btn12NC.UseVisualStyleBackColor = true;
            this.btn12NC.Click += new System.EventHandler(this.btn12NC_Click);
            // 
            // btnNaming
            // 
            this.btnNaming.AutoSize = true;
            this.btnNaming.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnNaming.BackgroundImage")));
            this.btnNaming.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNaming.Location = new System.Drawing.Point(694, 63);
            this.btnNaming.Name = "btnNaming";
            this.btnNaming.Size = new System.Drawing.Size(32, 34);
            this.btnNaming.TabIndex = 3;
            this.btnNaming.UseVisualStyleBackColor = true;
            this.btnNaming.Click += new System.EventHandler(this.btnNaming_Click);
            // 
            // txt12NC
            // 
            this.txt12NC.Location = new System.Drawing.Point(113, 69);
            this.txt12NC.Name = "txt12NC";
            this.txt12NC.Size = new System.Drawing.Size(146, 20);
            this.txt12NC.TabIndex = 4;
            // 
            // txtNaming
            // 
            this.txtNaming.Location = new System.Drawing.Point(542, 69);
            this.txtNaming.Name = "txtNaming";
            this.txtNaming.Size = new System.Drawing.Size(146, 20);
            this.txtNaming.TabIndex = 5;
            // 
            // gbSearch
            // 
            this.gbSearch.Controls.Add(this.label2);
            this.gbSearch.Controls.Add(this.txtNaming);
            this.gbSearch.Controls.Add(this.label1);
            this.gbSearch.Controls.Add(this.txt12NC);
            this.gbSearch.Controls.Add(this.btn12NC);
            this.gbSearch.Controls.Add(this.btnNaming);
            this.gbSearch.Location = new System.Drawing.Point(12, 13);
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.Size = new System.Drawing.Size(833, 128);
            this.gbSearch.TabIndex = 6;
            this.gbSearch.TabStop = false;
            this.gbSearch.Text = "Search";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnAdd);
            this.groupBox2.Controls.Add(this.btnDelete);
            this.groupBox2.Controls.Add(this.dgvProduct);
            this.groupBox2.Location = new System.Drawing.Point(12, 147);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1121, 374);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Product Family";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(823, 334);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(125, 25);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(959, 334);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(125, 25);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dgvProduct
            // 
            this.dgvProduct.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvProduct.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvProduct.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvProduct.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProduct.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvProduct.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvProduct.Location = new System.Drawing.Point(17, 22);
            this.dgvProduct.Name = "dgvProduct";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvProduct.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvProduct.RowHeadersVisible = false;
            this.dgvProduct.RowTemplate.Height = 23;
            this.dgvProduct.Size = new System.Drawing.Size(1086, 298);
            this.dgvProduct.TabIndex = 0;
            this.dgvProduct.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProduct_CellValueChanged);
            this.dgvProduct.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvProduct_CurrentCellDirtyStateChanged);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column1.HeaderText = "选择";
            this.Column1.Name = "Column1";
            this.Column1.Visible = false;
            this.Column1.Width = 50;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbOutdoor);
            this.groupBox1.Controls.Add(this.rbIndoor);
            this.groupBox1.Location = new System.Drawing.Point(851, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(282, 128);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mode";
            // 
            // rbOutdoor
            // 
            this.rbOutdoor.AutoSize = true;
            this.rbOutdoor.Location = new System.Drawing.Point(117, 70);
            this.rbOutdoor.Name = "rbOutdoor";
            this.rbOutdoor.Size = new System.Drawing.Size(63, 18);
            this.rbOutdoor.TabIndex = 1;
            this.rbOutdoor.TabStop = true;
            this.rbOutdoor.Text = "Outdoor";
            this.rbOutdoor.UseVisualStyleBackColor = true;
            // 
            // rbIndoor
            // 
            this.rbIndoor.AutoSize = true;
            this.rbIndoor.Location = new System.Drawing.Point(117, 33);
            this.rbIndoor.Name = "rbIndoor";
            this.rbIndoor.Size = new System.Drawing.Size(55, 18);
            this.rbIndoor.TabIndex = 0;
            this.rbIndoor.TabStop = true;
            this.rbIndoor.Text = "Indoor";
            this.rbIndoor.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1145, 534);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbSearch);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Philips Lighting Luminaries Choicesheet";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gbSearch.ResumeLayout(false);
            this.gbSearch.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduct)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn12NC;
        private System.Windows.Forms.Button btnNaming;
        private System.Windows.Forms.TextBox txt12NC;
        private System.Windows.Forms.TextBox txtNaming;
        private System.Windows.Forms.GroupBox gbSearch;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvProduct;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbOutdoor;
        private System.Windows.Forms.RadioButton rbIndoor;
    }
}

