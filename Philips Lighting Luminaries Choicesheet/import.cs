using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Philips_Lighting_Luminaries_Choicesheet
{
    public partial class import : Form
    {
        public import()
        {
            InitializeComponent();
        }

        public bool increase(int nValue)
        {
            if(nValue > 0)
            {
                if(progressBar1.Value + nValue < progressBar1.Maximum)
                {
                    progressBar1.Value += nValue;
                    String text = String.Format("{0}%", progressBar1.Value);
                    labelProgress.Text = text;
                    return true;
                }
                else
                {
                    progressBar1.Value = progressBar1.Maximum;
                    labelProgress.Text = "100%";
                    this.Close();
                    return false;
                }
            }

            return false;
        }
    }
}
