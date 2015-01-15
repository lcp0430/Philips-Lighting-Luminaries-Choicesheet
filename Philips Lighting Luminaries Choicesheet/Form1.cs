using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using Excel = Microsoft.Office.Interop.Excel;
using SimpleExcelHelper;

namespace Philips_Lighting_Luminaries_Choicesheet
{
    public partial class Form1 : Form
    {
        public enum OperationState
        {
            OS_NORMAL,
            OS_INSERT
        }

        public enum StoreResult
        {
            RES_SUCCESS,
            RES_DUPLICATE,
            RES_INVALID
        }

        private bool _firstColVisible;
        private const string PRI_KEY = "产品12NC";
        private const string CET_NO = "证书编号";
        private const string PRODUCT = "Product";
        private const string INDEX = "序列号";
        private const int KEY_NUM = 12;
        private OperationState _os;

        public Form1()
        {
            InitializeComponent();
        }

        private void SearchProduct(string item, string content)
        {//"12NC码",12NC码
            if (!_firstColVisible)
            {//搜索时第一列可见
                _firstColVisible = true;
                this.dgvProduct.Columns[0].Visible = true;
            }

            try
            {
                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb"))
                { //使用完毕自动释放
                    DataSet ds = new DataSet();
                    String cmdtext = String.Format("select * from [{0}] where \"{1}\" like '%{2}%'", PRODUCT, item, content);
                    OleDbDataAdapter da = new OleDbDataAdapter(cmdtext, con);
                    da.Fill(ds); //对ds添加数据
                    this.dgvProduct.DataSource = ds.Tables[0].DefaultView;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_os == OperationState.OS_NORMAL)
            {
                _os = OperationState.OS_INSERT;
                this.gbSearch.Enabled = false;
                this.btnExport.Enabled = false;
                this.btnSelectAll.Enabled = false;
                this.btnInverseAll.Enabled = false;
                this.btnAdd.Text = "确定";
                this.btnDelete.Enabled = true;
                this.btnDelete.Text = "取消";
                while (this.dgvProduct.Rows.Count > 0)
                {
                    this.dgvProduct.Rows.RemoveAt(0);
                }
                this.dgvProduct.AllowUserToAddRows = true;
                if (this.dgvProduct.Columns.Count <= 1)
                {
                    this.dgvProduct.Columns.Add("归档号", "归档号");
                    this.dgvProduct.Columns.Add("ProductFamily", "ProductFamily");
                    this.dgvProduct.Columns.Add(CET_NO, CET_NO);
                    this.dgvProduct.Columns.Add("状态", "状态");
                    this.dgvProduct.Columns.Add("Factory", "Factory");
                    this.dgvProduct.Columns.Add("规格/描述1(证书上)", "规格/描述1(证书上)");
                    this.dgvProduct.Columns.Add("规格/描述2(SAP上)", "规格/描述2(SAP上)");
                    this.dgvProduct.Columns.Add(PRI_KEY, PRI_KEY);
                }
                else
                {
                    this.dgvProduct.Columns[0].Visible = false;
                    _firstColVisible = false;
                }
            }
            else
            {
                if (AddProduct())
                {
                    //RefreshNormal();  //添加成功后，恢复到搜索界面
                }
            }
        }

        private bool Is12NCCorrect(string no)
        {
            if (!String.IsNullOrEmpty(no))
            {
                if (no.Length != KEY_NUM)
                {
                    return false;
                }

                foreach (char c in no)
                {
                    if (c < '0' || c > '9')
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool AddProduct()
        { //向数据库中添加
            bool result = false;
            int res = 0;

            string insertItem = "";
            try
            {
                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb"))
                {
                    con.Open();
                    for (int j = 0; j < this.dgvProduct.Rows.Count - 1;++j )
                    {
                        DataGridViewRow cc = this.dgvProduct.Rows[j];
                        if (cc.Cells[CET_NO].Value != null)
                        {
                            if (!Is12NCCorrect(cc.Cells[PRI_KEY].Value != null ? cc.Cells[PRI_KEY].Value.ToString() : ""))
                            {
                                MessageBox.Show("产品12NC须12位数字！");
                                res++;
                            }
                            else
                            {
                                if (cc.Cells[INDEX].Value != null)
                                {
                                    if(isItemDup(cc.Cells[INDEX].Value.ToString()))
                                    {
                                        MessageBox.Show("序列号不能重复！");
                                        res++;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("证书描述不能为空！");
                                    res++;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("证书编号不能为空！");
                            res++;
                        }
                    }

                    if (0 == res)
                    {
                        for (int j = 0; j < this.dgvProduct.Rows.Count - 1; ++j)
                        {
                            DataGridViewRow cc = this.dgvProduct.Rows[j];
                            insertItem = String.Format("insert into [{0}] values({1}", PRODUCT, cc.Cells[1].Value == null ? "-1" : cc.Cells[1].Value.ToString());
                            for (int i = 2; i < cc.Cells.Count; ++i)
                            {
                                insertItem += String.Format(",'{0}'", cc.Cells[i].Value == null ? "" : cc.Cells[i].Value.ToString());
                            }
                            insertItem += ")";
                            OleDbCommand com = new OleDbCommand(insertItem, con);
                            com.ExecuteNonQuery();

                            this.dgvProduct.Rows.Remove(cc);
                            --j;
                        }
                        result = true;
                    } 
                }   
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return result;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_os == OperationState.OS_NORMAL)
            {
                string eraseItem = "";
                List<DataGridViewRow> targetRows = new List<DataGridViewRow>();
                foreach (DataGridViewRow cc in this.dgvProduct.Rows)
                {
                    if (Convert.ToBoolean(cc.Cells[0].Value))
                    {
                        eraseItem += String.Format("'{0}',", cc.Cells[PRI_KEY].Value);
                        targetRows.Add(cc);
                    }
                }
                if (!String.IsNullOrEmpty(eraseItem))
                {
                    eraseItem = String.Format("delete from [{0}] where {1} in ({2})", PRODUCT, PRI_KEY, eraseItem.Remove(eraseItem.Length - 1, 1));
                    if (Operate(eraseItem))
                    {
                        foreach (DataGridViewRow cc in targetRows)
                        {
                            this.dgvProduct.Rows.Remove(cc);
                        }
                    }
                }
            }
            else
            {
                RefreshNormal();
            }
        }

        private void RefreshNormal()
        {
            _os = OperationState.OS_NORMAL;
            this.gbSearch.Enabled = true;
            this.btnExport.Enabled = true;
            this.btnSelectAll.Enabled = true;
            this.btnInverseAll.Enabled = true;
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
            this.dgvProduct.AllowUserToAddRows = false; //搜索界面不能添加行
            _firstColVisible = false;
            this.btnDelete.Enabled = _firstColVisible; //删除键不可用
            _os = OperationState.OS_NORMAL;  //当前状态为查询状态而非添加状态
            this.SelectItems.SelectedIndex = 7;
        }

        private void dgvProduct_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        { //值改变
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            { //第零列改变
                this.btnDelete.Enabled = EnableDeleteBtn();
            }
        }

        private bool EnableDeleteBtn()
        { //删除键是否使能
            bool result = false;
            foreach (DataGridViewRow cc in this.dgvProduct.Rows)
            {  //每行搜索
                if (Convert.ToBoolean(cc.Cells[0].Value))
                {//每行第零个数据的值
                    result = true;
                    break;
                }
            }
            return result;
        }

        private void dgvProduct_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        { //缓存有修改
            if (this.dgvProduct.IsCurrentCellDirty) //修改数据未提交
            {
                this.dgvProduct.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(this.SearchNumber.Text != string.Empty)
                SearchProduct(this.SelectItems.SelectedItem.ToString(), this.SearchNumber.Text.ToString());
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            bool result = false;
            try
            {
                //获取导出数据（行数）
                int count = 1;
                foreach (DataGridViewRow row in this.dgvProduct.Rows)
                {
                    if (Convert.ToBoolean(row.Cells[0].Value))
                    {
                        ++count;
                    }
                }

                if (count > 1)
                {
                    //设置保存文件对话框属性
                    SaveFileDialog saveSysFileDialog = new SaveFileDialog();
                    saveSysFileDialog.FileName = "";
                    saveSysFileDialog.Filter = "Excel files(*.xlsx)|*.xlsx|All files(*.*)|*.*";
                    saveSysFileDialog.CheckFileExists = false;
                    saveSysFileDialog.CheckPathExists = false;
                    saveSysFileDialog.OverwritePrompt = false;
                    saveSysFileDialog.FilterIndex = 0;
                    saveSysFileDialog.RestoreDirectory = true;
                    if (saveSysFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (SEHApplication app = new SEHApplication())
                        {
                            //创建Excel工作表
                            if (app.CreateWorkbook(saveSysFileDialog.FileName))
                            {
                                //创建Sheet
                                Excel.Worksheet worksheet = app.CreateWorksheet(PRODUCT);
                                if (worksheet != null)
                                {
                                    object[,] data = new object[count, this.dgvProduct.Columns.Count - 1];
                                    for (int i = 1; i < this.dgvProduct.Columns.Count; ++i)
                                    {
                                        data[0, i - 1] = this.dgvProduct.Columns[i].HeaderText;
                                    }
                                    for (int i = 0, k = 1; i < this.dgvProduct.Rows.Count; ++i)
                                    {
                                        if (Convert.ToBoolean(this.dgvProduct.Rows[i].Cells[0].Value))
                                        {
                                            for (int j = 1; j < this.dgvProduct.Columns.Count; ++j)
                                            {
                                                data[k, j - 1] = this.dgvProduct.Rows[i].Cells[j].Value;
                                            }
                                            ++k;
                                        }
                                    }
                                    SEHRange range = app.GetRange(worksheet, 1, data.GetLength(0), 1, data.GetLength(1));
                                    if (range != null)
                                    {
                                        result = range.SetData(data);
                                    }
                                    //设置字体
                                    if (null != range && result)
                                    {
                                        result &= range.SetOutsideBorder(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin,
                                                        Excel.XlColorIndex.xlColorIndexAutomatic, Color.Black);
                                        result &= range.SetInsideBorder(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin,
                                                                            Excel.XlColorIndex.xlColorIndexAutomatic, Color.Black);
                                        result &= range.SetTextAlign(Excel.XlHAlign.xlHAlignCenter, Excel.XlVAlign.xlVAlignCenter, false);
                                        result &= range.SetColumnWidth(0, true);
                                        SEHFont sehFont = new SEHFont();
                                        sehFont.Name = "Arial";
                                        sehFont.Size = 10;
                                        result &= range.SetFont(sehFont);
                                        result &= range.SetDataFormat(SEHRange.SEHDataFormat.DfInt);
                                    }

                                    //特制表头
                                    if (result)
                                    {
                                        if (null != (range = app.GetRange(worksheet, 1, 1, 1, data.GetLength(1))))
                                        {
                                            result &= range.SetBackgroundColor(Color.FromArgb(255, 204, 153));
                                        }
                                    }

                                    //保存修改
                                    if (result)
                                    {
                                        result &= app.Save();
                                    }

                                    //关闭工作表
                                    app.CloseSheet(worksheet);

                                    if (!result)
                                    {
                                        MessageBox.Show("导出失败，检查是否已打开目标文件！");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Sheet创建失败！");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Excel导出失败！");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("导出内容不得为空！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (result)
            {
                MessageBox.Show("导出成功！");
            }
        }

        private void SelectItem(bool selected)
        {
            foreach (DataGridViewRow row in this.dgvProduct.Rows)
            {
                row.Cells[0].Value = selected;
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectItem(true);
        }

        private void btnInverseAll_Click(object sender, EventArgs e)
        {
            SelectItem(false);
        }

        private bool isRowValueValid(DataRow r, int c)
        {
            bool res = true;

            String str = r[0].ToString();
            int x;
            if (!(int.TryParse(str, out x)))
            {
                // 改行第一列不是数字, ignore
                res = false;
            }

            //str = r[c].ToString();
            //if (!(int.TryParse(str, out x)))
            //{
            //    // 改行最后一列不是数字, ignore
            //    res = false;
            //}

            //if (isDescription1Dup(r[5].ToString()))
            //    res = false;

            return res;
        }

        private bool isItemDup(String index)
        {
            bool res = false;

            // 打开mdb
            using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb"))
            {
                con.Open();

                String command = String.Format("select * from [{0}] where [序列号] like '{1}'", PRODUCT, index.ToString());
                //MessageBox.Show(command);
                OleDbCommand com = new OleDbCommand(command, con);
                //com.ExecuteNonQuery();
                OleDbDataReader read = com.ExecuteReader();
                if(read.Read())
                {
                    res = true;
                }

                con.Close();
            }

            return res;
        }

        private StoreResult storeRowValue2Database(OleDbConnection con, DataRow r, int c)
        {
            StoreResult res = StoreResult.RES_SUCCESS;

            try
            {
                if (isRowValueValid(r, c))
                {
                    // Add to mdb
                    String command = String.Format("insert into [{0}] values(", PRODUCT);
                    for (int i = 0; i < c; ++i)
                    {
                        if(i == 0)
                            command += String.Format("'{0}'", r[0].ToString());
                        else
                            command += String.Format(",'{0}'", r[i].ToString());
                    }
                    command += ")";

                    //MessageBox.Show(command);
                    OleDbCommand com = new OleDbCommand(command, con);
                    com.ExecuteNonQuery();
                }
                else
                {
                    res = StoreResult.RES_INVALID;
                }
            }

            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                if (ex.ToString().Contains("duplicate"))
                    res = StoreResult.RES_DUPLICATE;
                else
                    res = StoreResult.RES_INVALID;
            }

            return res;
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {

            try
            {
                //设置打开文件对话框属性
                OpenFileDialog openSysFileDialog = new OpenFileDialog();
                openSysFileDialog.FileName = "";
                openSysFileDialog.Filter = "Excel files(*.xlsx)|*.xlsx|All files(*.*)|*.*";
                openSysFileDialog.CheckFileExists = true;
                openSysFileDialog.CheckPathExists = true;
                openSysFileDialog.FilterIndex = 0;
                openSysFileDialog.RestoreDirectory = true;
                if (openSysFileDialog.ShowDialog() == DialogResult.OK)
                {
                        Int32 count = 0; // 记录数目
                        Int32 error = 0;
                        Int32 duplicate = 0;
                        String duplicateList = String.Format("");

                        // 读取excel 数据并保存
                        using (SEHApplication app = new SEHApplication())
                        {
                            //打开Excel工作表
                            if (app.OpenWorkbook(openSysFileDialog.FileName))
                            {
                                int index = 1; // start from 1
                                Excel.Worksheet worksheet;
                                DataTable dt = new System.Data.DataTable();

                                while ((worksheet = app.OpenWorksheet(index)) != null)
                                {
                                    // get data from current sheet
                                    if (index == 1)
                                    {
                                        for (int c = 1; c <= worksheet.Cells.CurrentRegion.Columns.Count; c++)
                                            dt.Columns.Add();
                                    }

                                    for (int r = 2; r <= worksheet.Cells.CurrentRegion.Rows.Count; r++)   //把工作表导入DataTable中
                                    {
                                        DataRow myRow = dt.NewRow();

                                        for (int c = 1; c <= worksheet.Cells.CurrentRegion.Columns.Count; c++)
                                        {
                                            Excel.Range temp = (Excel.Range)worksheet.Cells[r, c];
                                            string strValue = temp.Text.ToString();
                                            myRow[c - 1] = strValue;
                                        }
                                        dt.Rows.Add(myRow);
                                    }

                                    app.CloseSheet(worksheet);
                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);

                                    index++;
                                }

                                //关闭工作表
                                app.CloseWorkbook();

                                if (dt.Rows.Count > 0)
                                {
                                    // 打开mdb
                                    using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb"))
                                    {
                                        con.Open();

                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            // go through each row and store in database
                                            if (isRowValueValid(dt.Rows[i], dt.Columns.Count))
                                            {
                                                StoreResult res;
                                                if ((res = storeRowValue2Database(con, dt.Rows[i], dt.Columns.Count)) != StoreResult.RES_SUCCESS)
                                                {
                                                    if (res == StoreResult.RES_DUPLICATE)
                                                    {
                                                        duplicate++;
                                                    }
                                                    else if (res == StoreResult.RES_INVALID)
                                                    {
                                                        error++;
                                                    }
                                                }
                                                else
                                                {
                                                    count++;
                                                }
                                            }
                                            else
                                            {
                                                error++;
                                            }
                                        }

                                        con.Close();
                                    }
                                }

                                String msg = String.Format("Insert {0} records into database \r\n\r\n {1} invalid, {2} duplicated records found.", count.ToString(), error.ToString(), duplicate.ToString());
                                MessageBox.Show(msg, "Confirm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Excel打开失败！");
                            }

                            //System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                        }// End using (SEHApplication app = new SEHApplication())

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                } // End if( openSysFileDialog.ShowDialog() == DialogResult.OK)
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}