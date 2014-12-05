using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Philips_Lighting_Luminaries_Choicesheet
{
    public partial class Form1 : Form
    {
        public enum OperationState
        {
            OS_NORMAL,
            OS_INSERT
        }

        private bool _firstColVisible;
        private const string PRI_KEY = "产品12NC";
        private const string CET_NO = "证书编号";
        private const string INDOOR = "Indoor (O&I)";
        private const string OUTDOOR = "Outdoor (R&T)";
        private OperationState _os;

        public Form1()
        {
            InitializeComponent();
        }

        private void SearchProduct(string item, string content)
        {
            if (!_firstColVisible)
            {
                _firstColVisible = true;
                this.dgvProduct.Columns[0].Visible = true;
            }

            try
            {
                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb"))
                {
                    DataSet ds = new DataSet();
                    OleDbDataAdapter da = new OleDbDataAdapter(String.Format("select * from [{0}] where {1} = '{2}'", this.rbIndoor.Checked ? INDOOR : OUTDOOR, item, content), con);
                    da.Fill(ds);
                    this.dgvProduct.DataSource = ds.Tables[0].DefaultView;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn12NC_Click(object sender, EventArgs e)
        {
            SearchProduct(PRI_KEY, this.txt12NC.Text.ToString());
        }

        private void btnNaming_Click(object sender, EventArgs e)
        {
           
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            
        }

        private void RefreshNormal()
        {
            _os = OperationState.OS_NORMAL;
            this.gbSearch.Enabled = true;
            this.btnAdd.Text = "添加";
            this.btnDelete.Enabled = false;
            this.btnDelete.Text = "删除";
            this.dgvProduct.AllowUserToAddRows = false;
            while (this.dgvProduct.Rows.Count > 0)
            {
                this.dgvProduct.Rows.RemoveAt(0);
            }
            for (int i = this.dgvProduct.Columns.Count - 1; i > 0;--i )
            {
                this.dgvProduct.Columns.RemoveAt(i);
            }
        }

        private bool Operate(string cmd)
        {
            bool result = false;
            try
            {
                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb"))
                {
                    con.Open();
                    OleDbCommand com = new OleDbCommand(cmd, con);
                    com.ExecuteNonQuery();
                }
                result = true;
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.dgvProduct.AllowUserToAddRows = false;
            _firstColVisible = false;
            this.btnDelete.Enabled = _firstColVisible;
            _os = OperationState.OS_NORMAL;
            this.rbIndoor.Checked = true;
        }

        private void dgvProduct_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                this.btnDelete.Enabled = EnableDeleteBtn();
            }
        }

        private bool EnableDeleteBtn()
        {
            bool result = false;
            foreach (DataGridViewRow cc in this.dgvProduct.Rows)
            {
                if (Convert.ToBoolean(cc.Cells[0].Value))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private void dgvProduct_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dgvProduct.IsCurrentCellDirty)
            {
                this.dgvProduct.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}