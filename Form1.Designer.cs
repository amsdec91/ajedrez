using System.Drawing;

namespace ajedrez
{
    public class casilla
    {
        private static readonly Color Verde = Color.FromArgb(115, 149, 82);
        private static readonly Color Blanco = Color.FromArgb(235, 236, 208);

        public int Columna { get; }  // La columna de la casilla
        public int Fila { get; }  // La fila de la casilla
        public Color Color { get; }  // El color de la casilla
        public pieza Pieza { get; set; }  // La pieza en esta casilla (si hay alguna)

        public casilla(int columna, int fila)  // Constructor con coordenadas de columna y fila
        {
            Columna = columna;
            Fila = fila;

            Color = (fila + columna) % 2 == 0 ? Verde : Blanco;
        }
    }


    public class pieza
    {
        public string Nombre { get; set; }
        public string Color { get; set; }
        public string ImagenPath { get; set; }

        public pieza(string nombre, string color, string imagenPath)
        {
            Nombre = nombre;
            Color = color;
            ImagenPath = imagenPath;
        }
    }



    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.layout = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.Size = new System.Drawing.Size(853, 861);
            this.layout.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 861);
            this.Controls.Add(this.layout);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel layout;
    }
}

