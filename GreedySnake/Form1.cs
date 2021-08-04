using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreedySnake
{
    public partial class Form1 : MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }
        GreedySnakeCore Core;
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Core = new GreedySnakeCore(panel1.CreateGraphics());
            Core.EatFoodEvent += Scoring;
            Core.EatFoodEvent += ScoringUI;
            Stars = 0;
            ScoringUI();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Core.ClockForward();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Core.ModifyDirection(keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        }
        int Stars = 0;
        void Scoring() {
            Stars++;
        }
        void ScoringUI()
        {
            htmlLabel1.Invoke((Action)(() =>
            {
                htmlLabel1.Text = Stars.ToString();
            }));
        }
    }
}
