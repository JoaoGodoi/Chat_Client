using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace ProjetoChatCliente
{
    public partial class Form1 : Form
    {
        static System.Windows.Forms.ListBox listaMensagens;
        public delegate void DelegateFromServer(string s);
        public delegate void DelegateFileFromServer(byte[] b);
        static List<ProjetoChat.Mensagem> mensagens;
        public string nick = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        public static void MesageRecievedFromServer(byte[] buffer)
        {
            if (listaMensagens.InvokeRequired)
            {
                DelegateFileFromServer del = new DelegateFileFromServer(MesageRecievedFromServer);
                listaMensagens.Invoke(del, buffer);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    ProjetoChat.Mensagem msg = new ProjetoChat.Mensagem(null, null, null, DateTime.Now, null);
                    try
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        msg = (ProjetoChat.Mensagem)bf.Deserialize(ms);
                        mensagens.Add(msg);
                        if (msg.getImage() != null)
                            listaMensagens.Items.Add("(" + msg.getDataEnvio() + ") " + msg.getRemetente() + " enviou uma imagem (double click para visualizar)");
                        else
                        {
                            if (msg.getContent().Equals(string.Empty))
                                listaMensagens.Items.Add("(" + msg.getDataEnvio() + ") " + msg.getRemetente() + " enviou um arquivo texto (double click para visualizar)");
                            else
                                listaMensagens.Items.Add("(" + msg.getDataEnvio().ToString() + ") " + msg.getRemetente() + " says: " + msg.getContent());
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Tivemos um erro ao receber a mensagem");
                    }
                    finally
                    {
                        ms.Close();
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listaMensagens = this.listBox1;
            btnOFD.Enabled = false;
            btnSendMSG.Enabled = false;
            mensagens = new List<ProjetoChat.Mensagem>();
            radioButton1.Checked = true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (txtIP.Text.Equals(string.Empty) || txtPort.Text.Equals(string.Empty))
                listaMensagens.Items.Add("Informe o IP e a porta do servidor.");
            else
            {
                if (radioButton1.Checked)
                    nick = "Anonimo";
                else
                {
                    if (!txtNick.Text.Equals(string.Empty))
                        nick = txtNick.Text;
                    else
                    {
                        MessageBox.Show("Nickname em branco", "ops...");
                        return;
                    }
                }
                try
                {
                    ClientTCP.ConnectToServer(txtIP.Text, Convert.ToInt32(txtPort.Text));
                    Thread.Sleep(100);
                    ProjetoChat.Mensagem msg = new ProjetoChat.Mensagem(string.Empty, null, nick, DateTime.Now, null);
                    ClientTCP.SendData(SerializeMensagem(msg));
                    btnConnect.Enabled = false;
                    btnOFD.Enabled = true;
                    btnSendMSG.Enabled = true;
                    this.Text = "Connected to IP: " + txtIP.Text + "/ Port: " + txtPort.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Não foi possivel se conectar ao servidor" +
                        "\n Verifique se o servidor está ativo ou" +
                        "\n se você inseriu o IP e a porta correta\n"+ex.Message, "ops...");
                }
            }
        }

        private void BtnSendMSG_Click(object sender, EventArgs e)
        {
            if (!txtMensagem.Text.Equals(string.Empty))
            {
                ProjetoChat.Mensagem msg = new ProjetoChat.Mensagem(txtMensagem.Text, null, nick, DateTime.Now, null);
                ClientTCP.SendData(SerializeMensagem(msg));
                txtMensagem.Text = string.Empty;
            }
        }

        private void BtnOFD_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image|*.jpg|Arquivo txt|*.txt";
                openFileDialog.Title = "Selecione uma imagem";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ProjetoChat.Mensagem msg = new ProjetoChat.Mensagem(string.Empty, null, nick, DateTime.Now, null);
                    if (Path.GetExtension(openFileDialog.FileName).Equals(".jpg"))
                    {
                        Image img = Image.FromFile(openFileDialog.FileName);
                        msg.setImage(img);
                    }
                    if (Path.GetExtension(openFileDialog.FileName).Equals(".txt"))
                    {
                        string[] conteudo = FileReadWrite.ReadTextFile(openFileDialog.FileName);
                        msg.setTxt(conteudo);
                    }
                    byte[] buffer = SerializeMensagem(msg);
                    if (buffer.Length >= 102400)
                        MessageBox.Show("Arquivo muito grande, não vai ser possivel fazer a transferencia", "ops...");
                    else
                        ClientTCP.SendData(buffer);
                }
            }
        }

        public static byte[] SerializeMensagem(ProjetoChat.Mensagem msg)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[1024 * 100];
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, msg);
                    buffer = ms.ToArray();
                }
                catch (Exception ex) { throw ex; }
                finally { ms.Close(); }
                return buffer;
            }
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listaMensagens.SelectedItem != null)
                if (listaMensagens.SelectedItem.ToString().Length != 0)
                {
                    if (mensagens[listaMensagens.SelectedIndex].getImage() != null)
                    {
                        ShowImage si = new ShowImage(mensagens[listaMensagens.SelectedIndex].getImage());
                        si.Show();
                    }
                    else
                    {
                        if (mensagens[listaMensagens.SelectedIndex].getContent().Equals(string.Empty))
                        {
                            ShowText st = new ShowText(mensagens[listaMensagens.SelectedIndex].getTxt());
                            st.Show();
                        }
                    }
                }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) { /*Do nothing*/}

        private void TxtNick_Click(object sender, EventArgs e) { radioButton2.Checked = true; }
    }
}
