using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProjetoChatCliente
{
    public partial class ShowImage : Form
    {
        Image img;

        public ShowImage(Image img)
        {
            InitializeComponent();
            this.img = img;
        }

        private void ShowImage_Load(object sender, EventArgs e)
        {
            ToolTip tp = new ToolTip();
            tp.SetToolTip(pictureBox1, "Double-Click para salvar a imagem");
            this.Size = new System.Drawing.Size(img.Width+10, img.Height+10);
            pictureBox1.Size = new System.Drawing.Size(img.Width, img.Height);
            pictureBox1.Image = img;
        }

        private void PictureBox1_DoubleClick(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Random rd = new Random();
                    img.Save(fbd.SelectedPath + "\\" + Convert.ToString(rd.Next()) + ".png");
                }
            }
        }
    }
}
