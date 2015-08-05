using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace course_project
{
    public partial class Form3 : Form
    {
        static public double Y1 = 1;
        static public double Y2 = 4;
        static public double B1 = 0.15;
        static public double B2 = 0.9;
        static public double V1 = 0.2;
        static public double V2 = 0.85;
        public Form3()
        {
            InitializeComponent();
            this.richTextBox1.Text = Y1.ToString();
            this.richTextBox2.Text = Y2.ToString();
            this.richTextBox3.Text = V1.ToString();
            this.richTextBox4.Text = V2.ToString();
            this.richTextBox5.Text = B1.ToString();
            this.richTextBox6.Text = B2.ToString();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Y1 = Convert.ToDouble(richTextBox1.Text);
                Y2 = Convert.ToDouble(richTextBox2.Text);
                V1 = Convert.ToDouble(richTextBox3.Text);
                V2 = Convert.ToDouble(richTextBox4.Text);
                B1 = Convert.ToDouble(richTextBox5.Text);
                B2 = Convert.ToDouble(richTextBox6.Text);
            }
            catch (System.Exception)
                {
                    Form1.print("Неправильно введены ограничения.");
                    return;
                }
            if ((Y1 >= Y2) || (V1 >= V2) || (B1 >= B2) || (B1 < 0) || (B2 > 1) || (V1 < 0) || (V2 > 1))
            {
                Form1.print("Неверные значения ограничений.");
                return;
            }
            this.Hide();
        }
    }
}
