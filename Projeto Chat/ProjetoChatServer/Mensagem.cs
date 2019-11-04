using System;
using System.Drawing;

namespace ProjetoChat
{
    [Serializable]
    public class Mensagem
    {
        private string content;
        private Image image;
        private string remetente;
        private DateTime dataEnvio;
        private string[] txt;

        public Mensagem(string content, Image image, string remetente, DateTime dataEnvio, string[] txt)
        {
            this.content = content;
            this.image = image;
            this.remetente = remetente;
            this.dataEnvio = dataEnvio;
            this.txt = txt;
        }

        public void setContent(string content) { this.content = content; }
        public string getContent() { return this.content; }

        public void setImage(Image image) { this.image = image; }
        public Image getImage() { return this.image; }
        public void setRemetente(string remetente) { this.remetente = remetente; }
        public string getRemetente() { return this.remetente; }
        public void setDataEnvio(DateTime dataEnvio) { this.dataEnvio = dataEnvio; }
        public DateTime getDataEnvio() { return this.dataEnvio; }
        public void setTxt(string[] txt) { this.txt = txt; }
        public string[] getTxt() { return this.txt; }
    }
}
