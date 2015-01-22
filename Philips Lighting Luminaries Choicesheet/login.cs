using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Philips_Lighting_Luminaries_Choicesheet
{
    public partial class login : Form
    {
        public Form1.userInfo curUser;

        public login(Form1 f)
        {
            InitializeComponent();
            curUser = f.curUser;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string iniFilePath = Application.StartupPath + @"\conf.ini";

            if (textBoxUserName.Text == String.Empty && textBoxPwd.Text == String.Empty)
            {
                curUser.name = "Anonymous";
                curUser.group = Form1.userGrp.UG_OPERATOR;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            //验证密码 
            else if (textBoxUserName.Text != string.Empty)
            {

                if (!File.Exists(iniFilePath))
                    return;

                StringBuilder temp = new StringBuilder(256);
                if (GetPrivateProfileString("Admin", "Name", "", temp, 256, iniFilePath) != 0)
                {
                    if (temp.ToString() == textBoxUserName.Text)
                    {
                        // Now get the pwd
                        if (GetPrivateProfileString("Admin", "Password", "", temp, 256, iniFilePath) != 0)
                        {
                            if (temp.ToString() == textBoxPwd.Text)
                            {
                                curUser.name = "admin";
                                curUser.group = Form1.userGrp.UG_ADMIN;

                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                        }
                    }
                }
                
            } 

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label_Click(object sender, EventArgs e)
        {
            if (sender.Equals(labelUserName))
            {
                textBoxUserName.Focus();
            }
            else if (sender.Equals(labelPwd))
            {
                textBoxPwd.Focus();
            }
        }

        private void textBoxFocusEnter(object sender, EventArgs e)
        {
            if (sender.Equals(textBoxUserName))
            {
                labelUserName.Visible = false;
            }
            else if (sender.Equals(textBoxPwd))
            {
                labelPwd.Visible = false;
            }

            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }

        private void textBoxFocusLeave(object sender, EventArgs e)
        {
            if (sender.Equals(textBoxUserName))
            {
                if (textBoxUserName.Text.Length < 1)
                    labelUserName.Visible = true;
            }
            else if (sender.Equals(textBoxPwd))
            {
                if(textBoxPwd.Text.Length < 1)
                    labelPwd.Visible = true;
            }
        }

        private void textBoxPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(sender, e);
        }
    }
}
