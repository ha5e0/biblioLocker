using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        static Lock locker;        public Form1()
        {
            InitializeComponent();
            locker = new Lock();
            locker.lockOn();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            locker.LogOff();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (locker.isLocked) locker.lockOff();
            this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            logOffTimer.Enabled = true;
            this.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!locker.isLocked) locker.lockOn();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (locker.isLocked) locker.lockOff();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void logOffTimer_Tick(object sender, EventArgs e)
        {
            logOffTimer.Enabled = false;
            //MessageBox.Show(new Form() { TopMost = true }, "Koniec czasu!");
            this.Visible = true;
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            if (textBoxNumer.Text == "000001" && textBoxData.Text == "2000")
            {
                MessageBox.Show(new Form() { TopMost = true }, "Dobrze!");
                logOffTimer.Enabled = true;
                this.Visible = false;
            } else
            {
                MessageBox.Show(new Form() { TopMost = true }, "Źle!");
            }
            textBoxNumer.Text = "";
            textBoxData.Text = "";
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (!locker.isLocked) locker.lockOn();
            } else
            {
                if (locker.isLocked) locker.lockOff();
            }
        }

        private void textBoxNumer_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBoxData_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBoxNumer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSubmit.PerformClick();
            }
        }

        private void textBoxData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSubmit.PerformClick();
            }
        }
    }
}
