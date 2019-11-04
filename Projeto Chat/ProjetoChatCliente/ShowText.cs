using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjetoChatCliente
{
    public partial class ShowText : Form
    {
        public ShowText(string[] content)
        {
            InitializeComponent();
            richTextBox1.Lines = content;
        }

        private void ShowText_Load(object sender, EventArgs e)
        {
            richTextBox1.Anchor = (AnchorStyles.Top| AnchorStyles.Bottom| AnchorStyles.Left | AnchorStyles.Right) ;
        }

        private void SobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(" Desenvolvido por João Victor Godoi Bernardino" +
                " para matéria \n Linguagem de Programação 1 no curso de Engenharia de computação" +
                "\n No Instituto Federal Campus Piracicaba - outubro de 2019", "Sobre");
        }

        private void SalvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Random rd = new Random();
                    FileReadWrite.SaveBinaryFile(fbd.SelectedPath+"\\"+rd.Next()+".txt", richTextBox1.Lines);
                }
            }
        }
    }
}
