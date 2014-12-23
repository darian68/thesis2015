using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotionPlayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Skeleton ske = new Skeleton();
            ske.ReadASFFile("D:/000-Thesis/01-GIT/07-Source_Code/SlimFramework/MotionPlayer/143.asf", Constants.MOCAP_SCALE);
        }
    }
}
