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
using System.Threading;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Net;

namespace Philips_Lighting_Luminaries_Choicesheet
{
    public partial class Form1 : Form
    {
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

        public enum userGrp
        {
            UG_ADMIN,
            UG_OPERATOR,
            UG_UNKNOWN
        }

        public class userInfo
        {
            public String name;
            public userGrp group;
        }

        public userInfo curUser = new userInfo();

        private BackgroundWorker worker = null;

        public Form1()
        {
            InitializeComponent();
            curUser.name = "Anonymous";
            curUser.group = userGrp.UG_UNKNOWN;
        }

        private void DisableUserEditContent(DataGridView g)
        {
            for (int r = 0; r < this.dgvProduct.Rows.Count; r++)
            {
                if (!Convert.ToBoolean(this.dgvProduct.Rows[r].Cells[0].Value))
                {
                    this.dgvProduct.Rows[r].ReadOnly = false;
                    this.dgvProduct.Rows[r].DefaultCellStyle.BackColor = SystemColors.Control;
                }
            }

            g.Columns[0].ReadOnly = false;
            g.Columns["序列号"].ReadOnly = true;
            g.Columns["归档号"].ReadOnly = true;
            g.Columns["ProductFamily"].ReadOnly = true;
            g.Columns["证书编号"].ReadOnly = true;
            g.Columns["状态"].ReadOnly = true;
            g.Columns["Factory"].ReadOnly = true;
            g.Columns["规格/描述1(证书上)"].ReadOnly = true;
            g.Columns["规格/描述2(SAP上)"].ReadOnly = true;
            g.Columns["产品12NC"].ReadOnly = true;
        }

        private void EnableUserEditContent(DataGridView g)
        {
            g.Columns[0].ReadOnly = true;
            g.Columns["序列号"].ReadOnly = false;
            g.Columns["归档号"].ReadOnly = false;
            g.Columns["ProductFamily"].ReadOnly = false;
            g.Columns["证书编号"].ReadOnly = false;
            g.Columns["状态"].ReadOnly = false;
            g.Columns["Factory"].ReadOnly = false;
            g.Columns["规格/描述1(证书上)"].ReadOnly = false;
            g.Columns["规格/描述2(SAP上)"].ReadOnly = false;
            g.Columns["产品12NC"].ReadOnly = false;

            for (int r = 0; r < this.dgvProduct.Rows.Count; r++)
            {
                if (!Convert.ToBoolean(this.dgvProduct.Rows[r].Cells[0].Value))
                {
                    this.dgvProduct.Rows[r].ReadOnly = true;
                    this.dgvProduct.Rows[r].DefaultCellStyle.BackColor = Color.DarkGray;
                }
            }
        }

        private void SearchProduct(string item, string content)
        {//"12NC码",12NC码
            try
            {

                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb;Jet OleDb:DataBase Password=Philips2"))
                { //使用完毕自动释放
                    DataSet ds = new DataSet();
                    String cmdtext;
                    if(item.Equals("序列号") || item.Equals("归档号"))
                        cmdtext = String.Format("select * from [{0}] where [{1}] like '{2}'", PRODUCT, item, content);
                    else
                        cmdtext = String.Format("select * from [{0}] where [{1}] like '%{2}%'", PRODUCT, item, content);
                    //String cmdtext = String.Format("select * from [{0}] where trim(replace({1},' ','')) like trim(replace('%{2}%',' ',''))", PRODUCT, item, content);
                    OleDbDataAdapter da = new OleDbDataAdapter(cmdtext, con);
                    da.Fill(ds); //对ds添加数据
                    this.dgvProduct.DataSource = ds.Tables[0].DefaultView;
                    this.dgvProduct.Sort(this.dgvProduct.Columns[1], ListSortDirection.Ascending);
                }

                DisableUserEditContent(this.dgvProduct);
                this.btnDelete.Enabled = false;
                this.btnAdd.Text = "添加";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if(String.Compare(btn.Text, "添加") == 0)
            {
                if (curUser.group != userGrp.UG_ADMIN)
                    return;

                this.gbSearch.Enabled = false;
                this.btnExport.Enabled = false;
                this.btnSelectAll.Enabled = false;
                this.btnInverseAll.Enabled = false;
                this.buttonImport.Enabled = false;
                this.Column1.Visible = false;

                this.btnAdd.Text = "确定";
                this.btnDelete.Enabled = true;
                this.btnDelete.Text = "放弃";
                //while (this.dgvProduct.Rows.Count > 0)
                //{
                //    this.dgvProduct.Rows.RemoveAt(0);
                //}
                if(this.dgvProduct.DataSource != null)
                    this.dgvProduct.DataSource = null;

                this.dgvProduct.Rows.Clear();

                this.dgvProduct.AllowUserToAddRows = true;
                if (this.dgvProduct.Columns.Count <= 1)
                {
                    this.dgvProduct.Columns.Add("序列号", "序列号");
                    this.dgvProduct.Columns.Add("归档号", "归档号");
                    this.dgvProduct.Columns.Add("ProductFamily", "ProductFamily");
                    this.dgvProduct.Columns.Add("GroupLeader", "GroupLeader");
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
            else if (String.Compare(btn.Text, "确定") == 0)
            {
                //for (int r = 0; r < this.dgvProduct.Rows.Count; r++)
                //{
                    //this.dgvProduct.Rows[r].Cells[0].Selected = true;
                //    this.dgvProduct.Rows[r].Cells[0].Value = true;
                //}

                if (true == AddProduct(false))
                {
                    MessageBox.Show("添加成功");

                    if (this.dgvProduct.DataSource != null)
                        this.dgvProduct.DataSource = null;

                    this.dgvProduct.AllowUserToAddRows = false;
                    this.dgvProduct.Rows.Clear();
                    this.dgvProduct.Columns.Clear();
                    this.dgvProduct.Columns.Add(Column1);
                    this.Column1.Visible = false;

                    this.btnDelete.Text = "删除";
                    this.btnDelete.Enabled = false;
                    this.btnAdd.Text = "添加";

                    this.gbSearch.Enabled = true;
                    this.btnExport.Enabled = true;
                    this.btnSelectAll.Enabled = true;
                    this.btnInverseAll.Enabled = true;
                    this.buttonImport.Enabled = true;
                    this.Column1.Visible = false;

                    this.dgvUpdate(0);
                }
                    
            }
            else if (String.Compare(btn.Text, "编辑") == 0)
            {
                if (curUser.group != userGrp.UG_ADMIN)
                    return;

                EnableUserEditContent(this.dgvProduct);
                this.btnDelete.Text = "取消";
                this.btnAdd.Text = "提交";

                this.dgvProduct.Columns[0].ReadOnly = true;
                this.dgvProduct.Columns["序列号"].ReadOnly = true;

                this.gbSearch.Enabled = false;
                this.btnExport.Enabled = false;
                this.btnSelectAll.Enabled = false;
                this.btnInverseAll.Enabled = false;
                this.buttonImport.Enabled = false;
            }
            else if (String.Compare(btn.Text, "提交") == 0)
            {
                if (true == AddProduct(true))
                {
                    MessageBox.Show("修改成功");

                    this.gbSearch.Enabled = true;
                    this.btnExport.Enabled = true;
                    this.btnSelectAll.Enabled = true;
                    this.btnInverseAll.Enabled = true;
                    this.buttonImport.Enabled = true;

                    this.btnDelete.Text = "删除";
                    this.btnAdd.Text = "编辑";

                    DisableUserEditContent(this.dgvProduct);
                }
                    
            }
            else
            {
                String msg = String.Format("Unkown support type: {0}", btn.Text.ToString());
                MessageBox.Show(msg);
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

        private bool AddProduct(bool ignoreIndex)
        { //向数据库中添加
            bool result = false;
            int res = 0;

            string insertItem = "";
            try
            {
                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb;Jet OleDb:DataBase Password=Philips2"))
                {
                    con.Open();
                    for (int j = 0; j < ((false==ignoreIndex)? (this.dgvProduct.Rows.Count-1) : this.dgvProduct.Rows.Count);++j )
                    {
                        if ((ignoreIndex) && (false == Convert.ToBoolean(this.dgvProduct.Rows[j].Cells[0].Value)))
                            continue;

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
                                    foreach (char c in cc.Cells[INDEX].Value.ToString())
                                    {
                                        if (c < '0' || c > '9')
                                        {
                                            res++;
                                        }
                                    }

                                    if(res != 0)
                                        MessageBox.Show("序列号必须为数字！");

                                    if ((res == 0) && (!ignoreIndex) && isItemDup(cc.Cells[INDEX].Value.ToString()))
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
                        for (int j = 0; j < ((false == ignoreIndex) ? (this.dgvProduct.Rows.Count - 1) : this.dgvProduct.Rows.Count); ++j)
                        {
                            if ((ignoreIndex) && (false == Convert.ToBoolean(this.dgvProduct.Rows[j].Cells[0].Value)))
                                continue;

                            DataGridViewRow cc = this.dgvProduct.Rows[j];

                            if (ignoreIndex)
                            {
                                // this is update db
                                insertItem = String.Format("update {0} set [归档号]='{1}', [ProductFamily]='{2}', [GroupLeader]='{3}', [证书编号]='{4}', [状态]='{5}', [Factory]='{6}', [规格/描述1(证书上)]='{7}', [规格/描述2(SAP上)]='{8}', [产品12NC]='{9}'",
                                    PRODUCT,
                                    cc.Cells[2].Value.ToString(),
                                    cc.Cells[3].Value.ToString(),
                                    cc.Cells[4].Value.ToString(),
                                    cc.Cells[5].Value.ToString(),
                                    cc.Cells[6].Value.ToString(),
                                    cc.Cells[7].Value.ToString(),
                                    cc.Cells[8].Value.ToString(),
                                    cc.Cells[9].Value.ToString(),
                                    cc.Cells[10].Value.ToString());
                                insertItem += String.Format(" WHERE [{0}]={1}", INDEX, cc.Cells[1].Value.ToString());

                            }
                            else
                            {
                                // insert db
                                insertItem = String.Format("insert into [{0}] values({1}", PRODUCT, cc.Cells[1].Value == null ? "-1" : cc.Cells[1].Value.ToString());
                                for (int i = 2; i < cc.Cells.Count; ++i)
                                {
                                    insertItem += String.Format(",'{0}'", cc.Cells[i].Value == null ? "" : cc.Cells[i].Value.ToString());
                                }
                                insertItem += ")";
                            }

                            OleDbCommand com = new OleDbCommand(insertItem, con);
                            com.ExecuteNonQuery();

                            //this.dgvProduct.Rows.Remove(cc);
                            //--j;
                        }
                        result = true;
                    }
                    con.Close();
                }   
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return result;
        }

        private void worker_DoDeletion(object sender, DoWorkEventArgs e)
        {
            DataGridView dgv = e.Argument as DataGridView;
            const int linesPerOperation = 100;

            bool continueDelete = false;
            int pcs = 0;
            string eraseItem = "";
            int totalCnt = 0;
            int totalCntBack = 0;
            List<DataGridViewRow> targetRows = new List<DataGridViewRow>();

            foreach (DataGridViewRow cc in dgv.Rows)
            {
                if (Convert.ToBoolean(cc.Cells[0].Value))
                {
                    if ((pcs < linesPerOperation) && (!continueDelete))
                    {// can delete linesPerOperation items per time
                        eraseItem += String.Format("{0}={1} or ", INDEX, cc.Cells[INDEX].Value);
                        pcs++;
                    }
                    else
                    {
                        continueDelete = true;
                    }

                    targetRows.Add(cc);
                }
            }

            totalCnt = targetRows.Count;
            totalCntBack = totalCnt;

            if (!String.IsNullOrEmpty(eraseItem))
            {
                eraseItem = String.Format("delete from [{0}] where {1}", PRODUCT, eraseItem.Remove(eraseItem.Length - 4, 4));
                if (Operate(eraseItem))
                {
                    //int t = pcs - 1;
                    //while (t >= 0)
                    //{
                    //    dgv.Rows.Remove(targetRows[t]);
                    //    t--;
                    //}

                    targetRows.RemoveRange(0, pcs);
                    worker.ReportProgress(pcs);
                }
            }

            totalCnt -= pcs;

            if (continueDelete)
            {
                // all the item in the targetRows should be deleted
                while (totalCnt > 0)
                {
                    eraseItem = "";
                    pcs = 0;
                    foreach (DataGridViewRow cc in targetRows)
                    {
                        eraseItem += String.Format("{0}={1} or ", INDEX, cc.Cells[INDEX].Value);
                        if (++pcs >= linesPerOperation)
                            break;
                    }

                    eraseItem = String.Format("delete from [{0}] where {1}", PRODUCT, eraseItem.Remove(eraseItem.Length - 4, 4));
                    if (Operate(eraseItem))
                    {
                        //int t = pcs - 1;
                        //while (t >= 0)
                        //{
                        //    dgv.Rows.Remove(targetRows[t]);
                        //    t--;
                        //}

                        targetRows.RemoveRange(0, pcs);
                        worker.ReportProgress(pcs);
                    }

                    totalCnt -= pcs;

                    Thread.Sleep(50);
                }
            }
        }

        private void worker_DeleteProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //delete it from dgv
            int done = e.ProgressPercentage;

            int t = done - 1;
            while (t >= 0)
            {
                this.dgvProduct.Rows.RemoveAt(t);
                t--;
            }

            if (this.dgvProduct.Rows.Count == 0)
            {
                // no rows in current view
                this.btnDelete.Enabled = false;
                this.btnAdd.Text = "添加";
            }
        }

        private bool DeleteRefresh(int val)
        {
            this.button1_Click(null, null);
            return true;
        }

        private bool dgvUpdate(int val)
        {
            //this.button1_Click(null, null);

            if (this.dgvProduct.Rows.Count == 0)
            {
                // no rows in current view
                this.btnDelete.Enabled = false;
                this.btnAdd.Text = "添加";
            }

            // update the max index 
            Int32 max = GetMaxid();
            if (max != -1)
            {
                this.indicator.Text = "当前最大索引: " + max.ToString();
                this.indicator.ForeColor = SystemColors.ControlText;
                this.indicator.Update();
            } 

            return true;
        }

        private void doBtnDelete(object sender, EventArgs e, Int32 max)
        {
            try
            {
                //Show the progress bar
                progressBar progressForm = new progressBar(max);
                progressForm.Show();

                // Prepare the background worker for asynchronous prime number calculation
                worker = new BackgroundWorker();
                // Specify that the background worker provides progress notifications            
                worker.WorkerReportsProgress = true;
                // Specify that the background worker supports cancellation
                worker.WorkerSupportsCancellation = false;
                // The DoWork event handler is the main work function of the background thread
                worker.DoWork += new DoWorkEventHandler(worker_DoDeletion);
                // Specify the function to use to handle progress
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_DeleteProgressChanged);
                worker.ProgressChanged += new ProgressChangedEventHandler(progressForm.OnProgressChanged);
                // Specify the function to run when the background worker finishes
                // There are three conditions possible that should be handled in this function:
                // 1. The work completed successfully
                // 2. The work aborted with errors
                // 3. The user cancelled the process
                //worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(progressForm.OnProcessCompleted);

                //If your background operation requires a parameter, 
                //call System.ComponentModel.BackgroundWorker.RunWorkerAsync 
                //with your parameter. Inside the System.ComponentModel.BackgroundWorker.DoWork 
                //event handler, you can extract the parameter from the 
                //System.ComponentModel.DoWorkEventArgs.Argument property.
                worker.RunWorkerAsync(this.dgvProduct);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (String.Compare(btn.Text, "放弃") == 0)
            {
                this.dgvProduct.Rows.Clear();
                this.dgvProduct.Columns.Clear();
                this.dgvProduct.Columns.Add(Column1);
                this.Column1.Visible = false;
                this.dgvProduct.AllowUserToAddRows = false;

                this.btnDelete.Text = "删除";
                this.btnDelete.Enabled = false;
                this.btnAdd.Text = "添加";

                this.gbSearch.Enabled = true;
                this.btnExport.Enabled = true;
                this.btnSelectAll.Enabled = true;
                this.btnInverseAll.Enabled = true;
                this.buttonImport.Enabled = true;
            }
            else if(String.Compare(btn.Text, "取消") == 0)
            {
                this.btnDelete.Text = "删除";
                this.btnAdd.Text = "编辑";

                DisableUserEditContent(this.dgvProduct);

                this.gbSearch.Enabled = true;
                this.btnExport.Enabled = true;
                this.btnSelectAll.Enabled = true;
                this.btnInverseAll.Enabled = true;
                this.buttonImport.Enabled = true;
            }
            else if (String.Compare(btn.Text, "删除") == 0)
            {
                if (curUser.group != userGrp.UG_ADMIN)
                    return;

                Int32 cnt = 0;
                foreach (DataGridViewRow cc in this.dgvProduct.Rows)
                {
                    if (Convert.ToBoolean(cc.Cells[0].Value))
                    {
                        cnt++;
                    }
                }

                string msg = String.Format("选择了 {0} 条记录，确定要删除吗？", cnt.ToString());
                if (MessageBox.Show(msg, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    //doBtnDelete(sender, e, cnt); //暂时不用BackGroundWork的方法,代码保留

                    ThreadWithParam twp = new ThreadWithParam("", this);
                    Thread thd = new Thread(new ThreadStart(twp.DeleteProc));
                    thd.Start();
                }
                    

            }
            else
            {
                String msg = String.Format("Unkown support type: {0}", btn.Text.ToString());
                MessageBox.Show(msg);
            }

        }

        private void RefreshNormal()
        {
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
                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb;Jet OleDb:DataBase Password=Philips2"))
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

        private int GetMaxid()
        { 
            int id = -1;

            try
            {
                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb;Jet OleDb:DataBase Password=Philips2"))
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand("select max(序列号) from Product", con);
                    if (cmd.ExecuteScalar() == DBNull.Value)
                        id = 0;
                    else
                        id = Convert.ToInt32(cmd.ExecuteScalar());

                    con.Close();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }

            return id;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form f2 = new login(this);
            f2.ShowDialog();
            if (f2.DialogResult == DialogResult.OK)
            {
                this.dgvProduct.AllowUserToAddRows = false; //搜索界面不能添加行
                _firstColVisible = false;
                this.btnDelete.Enabled = _firstColVisible; //删除键不可用
                this.SelectItems.SelectedIndex = 0;

                // update the max index 
                Int32 max = GetMaxid();
                if (max != -1)
                {
                    this.indicator.Text = "当前最大索引: " + max.ToString();
                    this.indicator.ForeColor = SystemColors.ControlText;
                }   
            }
            else
                this.Close();
        }

        public void StatusUpdateTimerOnTick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
    
            this.toolStripStatusLabel1.Text = dt.ToString("yyyy年MM月dd日 HH:mm:ss");
            if(curUser.group == userGrp.UG_OPERATOR)
                this.toolStripStatusLabel1.Text += "  欢迎您，操作员！";
            else if (curUser.group == userGrp.UG_ADMIN)
                this.toolStripStatusLabel1.Text += "  欢迎您，管理员！";
            else 
                this.toolStripStatusLabel1.Text += "  欢迎您，非法用户！";
        }

        private void dgvProduct_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        { //值改变
            bool toSelected = false;
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            { //第零列改变
                if (Convert.ToBoolean(this.dgvProduct.Rows[e.RowIndex].Cells[0].Value))
                    toSelected = true;

                //{
                    if (toSelected == true)
                    {
                        this.btnDelete.Enabled = EnableDeleteBtn();

                        this.btnAdd.Text = "编辑";
                        this.btnDelete.Text = "删除";
                        //EnableUserEditContent(this.dgvProduct);
                    }
                    else
                    {
                        // check 有没有选中的了
                        bool stillSelect = false;
                        for (int r = 0; r < this.dgvProduct.Rows.Count; r++)
                            if (Convert.ToBoolean(this.dgvProduct.Rows[r].Cells[0].Value))
                                stillSelect = true;

                        if (stillSelect == false)
                        {
                            this.btnDelete.Enabled = false;
                            this.btnAdd.Text = "添加";
                        }
                    }
                    
                //}
                
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
            DataGridView dgv = (DataGridView)sender;
            if (dgv.CurrentCell.ColumnIndex != 0)
                return;
            
            if (this.dgvProduct.IsCurrentCellDirty) //修改数据未提交
            {
                this.dgvProduct.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.SearchNumber.Text != string.Empty)
            {
                this.dgvProduct.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                this.Column1.Visible = true;
                SearchProduct(this.SelectItems.SelectedItem.ToString(), this.SearchNumber.Text.ToString());
                this.dgvProduct.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            }
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
            using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb;Jet OleDb:DataBase Password=Philips2"))
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

        private progressBar myProcessBar = null;
        private delegate bool IncreaseHandle(int nValue);
        private IncreaseHandle myIncrease = null;
        private IncreaseHandle myDgvUpdate = null;
        private IncreaseHandle myDeletionRetrig = null;

        private void ShowImportProgressBar()
        {
            myProcessBar = new progressBar(0);
            myIncrease = new IncreaseHandle(myProcessBar.increase);
            myDgvUpdate = new IncreaseHandle(this.dgvUpdate);
            myDeletionRetrig = new IncreaseHandle(this.DeleteRefresh);
            myProcessBar.ShowDialog();
            myProcessBar = null;
        }

        public class ThreadWithParam
        {
            //要用到的属性，也就是我们要传递的参数
            private string fileName;
            Form1 frm;

            //包含参数的构造函数
            public ThreadWithParam(string text, Form1 f)
            {
                fileName = text;
                frm = f;
            }
            private bool progressBarAddVal(int val)
            {
                bool bIncreasd = false;
                object objRet = null;

                objRet = frm.Invoke(frm.myIncrease, new object[] { val });
                bIncreasd = (bool)objRet;

                return bIncreasd;
            }

            private bool mainFrmDgvUpdate(int val)
            {
                //delete it from dgv
                bool bIncreasd = false;
                object objRet = null;

                objRet = frm.Invoke(frm.myDgvUpdate, new object[] { val });
                bIncreasd = (bool)objRet;

                return bIncreasd;
            }

            private bool mainFrmDeleteRetrig(int val)
            {
                //delete it from dgv
                bool bIncreasd = false;
                object objRet = null;

                objRet = frm.Invoke(frm.myDeletionRetrig, new object[] { val });
                bIncreasd = (bool)objRet;

                return bIncreasd;
            }

            public void DeleteProc()
            {
                try
                {
                    MethodInvoker mi = new MethodInvoker(frm.ShowImportProgressBar);
                    frm.BeginInvoke(mi);

                    const int linesPerOperation = 100;

                    bool continueDelete = false;
                    int pcs = 0;
                    string eraseItem = "";
                    int totalCnt = 0;
                    int totalCntBack = 0;
                    List<DataGridViewRow> targetRows = new List<DataGridViewRow>();

                    foreach (DataGridViewRow cc in frm.dgvProduct.Rows)
                    {
                        if (Convert.ToBoolean(cc.Cells[0].Value))
                        {
                            if ((pcs < linesPerOperation) && (!continueDelete))
                            {// can delete linesPerOperation items per time
                                eraseItem += String.Format("{0}={1} or ", INDEX, cc.Cells[INDEX].Value);
                                pcs++;
                            }
                            else
                            {
                                continueDelete = true;
                            }

                            targetRows.Add(cc);
                        }
                    }

                    totalCnt = targetRows.Count;
                    totalCntBack = totalCnt;

                    if (!String.IsNullOrEmpty(eraseItem))
                    {
                        eraseItem = String.Format("delete from [{0}] where {1}", PRODUCT, eraseItem.Remove(eraseItem.Length - 4, 4));
                        if (frm.Operate(eraseItem))
                        {
                            targetRows.RemoveRange(0, pcs);
                            //worker.ReportProgress(pcs);
                            progressBarAddVal(100 * pcs / totalCntBack);
                        }
                    }

                    totalCnt -= pcs;

                    if (continueDelete)
                    {
                        // all the item in the targetRows should be deleted
                        while (totalCnt > 0)
                        {
                            eraseItem = "";
                            pcs = 0;
                            foreach (DataGridViewRow cc in targetRows)
                            {
                                eraseItem += String.Format("{0}={1} or ", INDEX, cc.Cells[INDEX].Value);
                                if (++pcs >= linesPerOperation)
                                    break;
                            }

                            eraseItem = String.Format("delete from [{0}] where {1}", PRODUCT, eraseItem.Remove(eraseItem.Length - 4, 4));
                            if (frm.Operate(eraseItem))
                            {
                                targetRows.RemoveRange(0, pcs);
                                //worker.ReportProgress(pcs);
                                progressBarAddVal(100 * pcs / totalCntBack);
                            }

                            totalCnt -= pcs;

                            Thread.Sleep(50);
                        }
                    }
                    progressBarAddVal(100);
                    mainFrmDeleteRetrig(100);
                    mainFrmDgvUpdate(100);
                }

                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            
            //要丢给线程执行的方法，本处无返回类型就是为了能让ThreadStart来调用
            public void ImportProc()
            {
                string[] sheetColumeName = {"序列号",
                                     "归档号",
                                     "ProductFamily",
                                     "GroupLeader",
                                     "证书编号",
                                     "状态",
                                     "Factory",
                                     "规格/描述1(证书上)",
                                     "规格/描述2(SAP上)",
                                     "产品12NC"};

                try
                {
                    MethodInvoker mi = new MethodInvoker(frm.ShowImportProgressBar);
                    frm.BeginInvoke(mi);

                    Int32 count = 0; // 记录数目
                    Int32 error = 0;
                    Int32 duplicate = 0;

                    Int32 sheetCnt = 0;
                    bool toContinue = true;

                    // 读取excel 数据并保存
                    using (SEHApplication app = new SEHApplication())
                    {
                        //打开Excel工作表
                        if (app.OpenWorkbook(fileName))
                        {
                            int index = 1; // start from 1
                            Excel.Worksheet worksheet;
                            DataTable dt = new System.Data.DataTable();

                            sheetCnt = app.GetSheetsCount();

                            while ((worksheet = app.OpenWorksheet(index)) != null)
                            {
                                // get data from current sheet
                                if (index++ == 1)
                                {
                                    for (int c = 1; c <= worksheet.Cells.CurrentRegion.Columns.Count; c++)
                                        dt.Columns.Add();
                                }

                                // Check the sheet format, must be in
                                //"序列号" "归档号" "Product Family" "Group Leader" "证书编号" "状态" "Factory" "规格/描述1(证书上)" "规格/描述2(SAP上)" "产品12NC"
                                for(int c = 1; c <= worksheet.Cells.CurrentRegion.Columns.Count; c++)
                                {
                                    Excel.Range temp = (Excel.Range)worksheet.Cells[1, c];
                                    string strv = temp.Text.ToString().Replace(" ", "");
                                    if (strv != string.Empty && sheetColumeName[c - 1] != strv)
                                        toContinue = false;
                                }

                                if (toContinue)
                                {
                                    Int32 granuality = sheetCnt * worksheet.Cells.CurrentRegion.Rows.Count / 50;

                                    if ((worksheet.Cells.CurrentRegion.Rows.Count <= granuality) || (granuality <= 0))
                                        progressBarAddVal(50 / sheetCnt);

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

                                        if ((granuality != 0) && (r % granuality) == 0)
                                            progressBarAddVal(1);
                                    }
                                }

                                app.CloseSheet(worksheet);
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                            }

                            //关闭工作表
                            app.CloseWorkbook();
                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            if (toContinue && (dt.Rows.Count > 0))
                            {
                                Int32 granuality = 0;
                                if (dt.Rows.Count > 50)
                                    granuality = dt.Rows.Count / 50;

                                // 打开mdb
                                using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data source=products.mdb;Jet OleDb:DataBase Password=Philips2"))
                                {
                                    con.Open();

                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        // go through each row and store in database
                                        if (frm.isRowValueValid(dt.Rows[i], dt.Columns.Count))
                                        {
                                            StoreResult res;
                                            if ((res = frm.storeRowValue2Database(con, dt.Rows[i], dt.Columns.Count)) != StoreResult.RES_SUCCESS)
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

                                        // proceed to progressBar
                                        if (granuality != 0)
                                        {
                                            if ((i % granuality) == 0)
                                                progressBarAddVal(1);
                                        }
                                        else
                                        {
                                            progressBarAddVal(50/dt.Rows.Count);
                                        }
                                    }

                                    con.Close();
                                }
                            }

                            progressBarAddVal(100);
                            mainFrmDgvUpdate(100);

                            String msg;
                            if (toContinue)
                                msg = String.Format("Insert {0} records into database \r\n\r\n {1} invalid, {2} duplicated records found.", count.ToString(), error.ToString(), duplicate.ToString());
                            else
                            {
                                msg = String.Format("Please Check Excel Format. The columns in sheet should be:\r\n\r\n");
                                for (int i = 0; i < sheetColumeName.Length; i++)
                                    msg += String.Format("{0}\r\n", sheetColumeName[i]);
                            }

                            MessageBox.Show(msg, "Confirm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Excel打开失败！");
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }


        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (curUser.group != userGrp.UG_ADMIN)
                return;

            try
            {
                //设置打开文件对话框属性
                OpenFileDialog openSysFileDialog = new OpenFileDialog();
                openSysFileDialog.FileName = "";
                openSysFileDialog.Filter = "Excel files(*.xls)|*.xls|All files(*.*)|*.*";
                openSysFileDialog.CheckFileExists = true;
                openSysFileDialog.CheckPathExists = true;
                openSysFileDialog.FilterIndex = 0;
                openSysFileDialog.RestoreDirectory = true;
                if (openSysFileDialog.ShowDialog() == DialogResult.OK)
                {

                    ThreadWithParam twp = new ThreadWithParam(openSysFileDialog.FileName, this);
                    Thread thd = new Thread(new ThreadStart(twp.ImportProc));
                    thd.Start();
                } // End if( openSysFileDialog.ShowDialog() == DialogResult.OK)
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void StatusUpdateTimerOnTick()
        {

        }

        private void SearchNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(sender, e);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Process.Start("AutoUpdate.exe", Application.ProductName.Replace(" ", ""));
        }
    }
}