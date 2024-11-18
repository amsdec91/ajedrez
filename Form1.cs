using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ajedrez
{
    public partial class Form1 : Form
    {
        private casilla casillaSeleccionada = null;
        private bool esTurnoBlanco = true;

        public Form1()
        {
            InitializeComponent();
            GenerarTablero();
        }

        private void GenerarTablero()
        {
            int tamañoCuadro = 100;
            layout.Controls.Clear();

            for (int fila = 1; fila <= 8; fila++)
            {
                for (int columna = 1; columna <= 8; columna++)  // Ahora la columna es un número
                {
                    casilla casillaActual = CrearCasilla(columna, fila);  // Cambiamos el tipo de columna de char a int
                    Panel cuadro = CrearPanelCasilla(casillaActual, tamañoCuadro);
                    layout.Controls.Add(cuadro);
                }
            }
        }


        private Panel CrearPanelCasilla(casilla casillaActual, int tamañoCuadro)
        {
            Panel cuadro = new Panel
            {
                Size = new Size(tamañoCuadro, tamañoCuadro),
                BackColor = casillaActual.Color,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = casillaActual  // Asegurándonos de que el Tag contiene la casilla
            };

            if (casillaActual.Pieza != null)
            {
                PictureBox piezaVisual = CrearPictureBoxPieza(casillaActual.Pieza);
                cuadro.Controls.Add(piezaVisual);

                piezaVisual.Click += (sender, e) => SeleccionarPieza(cuadro);
            }

            cuadro.Click += (sender, e) => SeleccionarCasilla(cuadro);

            return cuadro;
        }


        private casilla CrearCasilla(int columna, int fila)
        {
            casilla nuevaCasilla = new casilla(columna, fila);  // Pasamos un int en lugar de un char

            if (fila == 2)
                nuevaCasilla.Pieza = new pieza("Peón", "Blanco", "peon_blanco.png");
            else if (fila == 7)
                nuevaCasilla.Pieza = new pieza("Peón", "Negro", "peon_negro.png");
            else if (fila == 1 || fila == 8)
            {
                string color = (fila == 1) ? "Blanco" : "Negro";
                string[] nombres = { "Torre", "Caballo", "Alfil", "Reina", "Rey", "Alfil", "Caballo", "Torre" };
                nuevaCasilla.Pieza = new pieza(nombres[columna - 1], color, $"{nombres[columna - 1].ToLower()}_{color.ToLower()}.png");
            }

            return nuevaCasilla;
        }


        private PictureBox CrearPictureBoxPieza(pieza pieza)
        {
            // Creamos el PictureBox para la pieza
            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(90, 90),
                Location = new Point(0, 0),
                Image = Image.FromFile(pieza.ImagenPath),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Aquí añadimos el evento de clic al PictureBox
            pictureBox.Click += (sender, e) =>
            {
                // Este código obtiene el Panel que contiene al PictureBox (la casilla)
                Panel casillaPanel = pictureBox.Parent as Panel;
                if (casillaPanel != null)
                {
                    // Al hacer clic en la pieza, se selecciona la casilla
                    SeleccionarCasilla(casillaPanel);
                }
            };

            return pictureBox;
        }


        private void SeleccionarPieza(Panel casillaPanel)
        {
            casilla casillaInfo = casillaPanel?.Tag as casilla;

            // Validar que la pieza seleccionada corresponde al turno actual
            if (!((esTurnoBlanco && casillaInfo.Pieza.Color == "Blanco") || (!esTurnoBlanco && casillaInfo.Pieza.Color == "Negro")))
            {
                MessageBox.Show($"Es el turno de las piezas {(esTurnoBlanco ? "blancas" : "negras")}. No puedes mover esa pieza.");
                return;
            }

            if (casillaInfo?.Pieza != null)
            {
                if (casillaSeleccionada != null)
                {
                    RestaurarColorCasillas();
                }

                // Resaltar casilla seleccionada y las casillas válidas
                casillaPanel.BackColor = Color.FromArgb(247, 198, 49);
                casillaSeleccionada = casillaInfo;
                ResaltarCasillasValidas(casillaSeleccionada);
            }
        }

        private void SeleccionarCasilla(Panel casillaPanel)
        {
            casilla casillaInfo = casillaPanel?.Tag as casilla;

            if (casillaInfo == null)
            {
                MessageBox.Show("Error: la casilla seleccionada es nula.");
                return;
            }

            if (casillaSeleccionada != null && casillaInfo != null)
            {
                // Intentar mover la pieza seleccionada a la casilla actual
                if (EsMovimientoValido(casillaSeleccionada, casillaInfo))
                {
                    // Realizamos el movimiento
                    casillaInfo.Pieza = casillaSeleccionada.Pieza;
                    casillaSeleccionada.Pieza = null;

                    // Actualizar el tablero visualmente
                    ActualizarTableroVisual(casillaSeleccionada, casillaInfo);

                    // Cambiar turno después de un movimiento válido
                    esTurnoBlanco = !esTurnoBlanco;

                    // Deseleccionar la pieza para permitir seleccionar otra
                    casillaSeleccionada = null;
                }
                else
                {
                    // Movimiento inválido: restablecemos la selección
                    MessageBox.Show("Movimiento inválido. Intenta de nuevo.");
                    RestaurarColorCasillas();
                    casillaSeleccionada = null;
                }
            }
            else
            {
                // Seleccionar una pieza si no hay ninguna seleccionada
                if (casillaInfo.Pieza != null)
                {
                    // Verificar que la pieza pertenece al turno actual
                    if ((esTurnoBlanco && casillaInfo.Pieza.Color == "Blanco") || (!esTurnoBlanco && casillaInfo.Pieza.Color == "Negro"))
                    {
                        casillaSeleccionada = casillaInfo;
                        ResaltarCasillasValidas(casillaSeleccionada);
                        casillaPanel.BackColor = Color.FromArgb(247, 198, 49); // Resaltar la casilla seleccionada
                    }
                    else
                    {
                        // No puedes mover una pieza del color contrario al turno actual
                        MessageBox.Show($"Es el turno de las piezas {(esTurnoBlanco ? "blancas" : "negras")}. No puedes mover esa pieza.");
                    }
                }
            }
        }


        private void ActualizarTableroVisual(casilla origen, casilla destino)
        {
            Panel panelOrigen = ObtenerPanelPorCasilla(origen);
            Panel panelDestino = ObtenerPanelPorCasilla(destino);

            // Limpiar la pieza visual de la casilla de origen
            if (panelOrigen != null)
            {
                panelOrigen.Controls.Clear(); // Eliminar cualquier control de pieza en la casilla de origen
            }

            // Limpiar la pieza visual de la casilla de destino, si la hay
            if (panelDestino != null)
            {
                panelDestino.Controls.Clear(); // Eliminar cualquier pieza previa en la casilla de destino
            }

            // Añadir la nueva pieza visual a la casilla de destino
            if (panelDestino != null && destino.Pieza != null)
            {
                PictureBox piezaVisual = CrearPictureBoxPieza(destino.Pieza);
                panelDestino.Controls.Add(piezaVisual); // Ahora añadimos la nueva pieza a la casilla de destino
            }

            // Restaurar colores después del movimiento
            RestaurarColorCasillas();
        }

        private Panel ObtenerPanelPorCasilla(casilla casillaInfo)
        {
            if (casillaInfo == null)
            {
                MessageBox.Show("La casilla seleccionada es nula.");
                return null;
            }

            var panelDestino = layout.Controls
                .OfType<Panel>()
                .FirstOrDefault(p => ((casilla)p.Tag).Columna == casillaInfo.Columna && ((casilla)p.Tag).Fila == casillaInfo.Fila);

            if (panelDestino == null)
            {
                MessageBox.Show($"No se encontró el panel para la casilla {casillaInfo.Columna}{casillaInfo.Fila}.");
                return null;
            }

            return panelDestino;
        }



        private void RestaurarColorCasillas()
        {
            foreach (Panel panel in layout.Controls)
            {
                casilla casillaInfo = panel?.Tag as casilla;
                if (casillaInfo != null)
                {
                    panel.BackColor = casillaInfo.Color;
                }
            }
            casillaSeleccionada = null;
        }

        private void ResaltarCasillasValidas(casilla casillaSeleccionada)
        {
            foreach (Panel panel in layout.Controls)
            {
                casilla casillaInfo = panel?.Tag as casilla;
                if (casillaInfo != null && EsMovimientoValido(casillaSeleccionada, casillaInfo))
                {
                    // Si la casilla tiene una pieza del oponente, resaltarla en rojo
                    if (casillaInfo.Pieza != null && casillaInfo.Pieza.Color != casillaSeleccionada.Pieza.Color)
                    {
                        panel.BackColor = Color.Red;
                    }
                    else
                    {
                        // Resaltar en azul para movimientos normales
                        panel.BackColor = Color.Blue;
                    }
                }
            }
        }


        private bool EsMovimientoValido(casilla origen, casilla destino)
        {
            if (destino.Pieza != null && destino.Pieza.Color == origen.Pieza.Color)
            {
                return false;
            }

            switch (origen.Pieza.Nombre)
            {
                case "Peón":
                    return EsMovimientoValidoPeon(origen, destino);
                case "Torre":
                    return EsMovimientoValidoTorre(origen, destino);
                case "Caballo":
                    return EsMovimientoValidoCaballo(origen, destino);
                case "Alfil":
                    return EsMovimientoValidoAlfil(origen, destino);
                case "Reina":
                    return EsMovimientoValidoReina(origen, destino);
                case "Rey":
                    return EsMovimientoValidoRey(origen, destino);
                default:
                    return false;
            }
        }

        private bool EsMovimientoValidoPeon(casilla origen, casilla destino)
        {
            int direccion = origen.Pieza.Color == "Blanco" ? 1 : -1;

            if (destino.Columna == origen.Columna && destino.Fila == origen.Fila + direccion && destino.Pieza == null)
            {
                return true;
            }

            if (origen.Fila == (origen.Pieza.Color == "Blanco" ? 2 : 7) && destino.Columna == origen.Columna && destino.Fila == origen.Fila + (direccion * 2) && destino.Pieza == null)
            {
                return true;
            }

            if (Math.Abs(destino.Columna - origen.Columna) == 1 && destino.Fila == origen.Fila + direccion && destino.Pieza != null && destino.Pieza.Color != origen.Pieza.Color)
            {
                return true;
            }

            return false;
        }

        private bool EsMovimientoValidoTorre(casilla origen, casilla destino)
        {
            if (origen.Columna == destino.Columna)
            {
                return EsRutaLibreVertical(origen, destino);
            }
            if (origen.Fila == destino.Fila)
            {
                return EsRutaLibreHorizontal(origen, destino);
            }
            return false;
        }

        private bool EsRutaLibreVertical(casilla origen, casilla destino)
        {
            int minFila = Math.Min(origen.Fila, destino.Fila);
            int maxFila = Math.Max(origen.Fila, destino.Fila);

            
            for (int fila = minFila + 1; fila < maxFila; fila++)
            {
                casilla casillaIntermedia = ObtenerCasilla(origen.Columna, fila);
                if (casillaIntermedia.Pieza != null)
                {
                    return false;
                }
                
            }
            return true;
        }


        private bool EsRutaLibreHorizontal(casilla origen, casilla destino)
        {
            int minColumna = Math.Min(origen.Columna, destino.Columna);
            int maxColumna = Math.Max(origen.Columna, destino.Columna);

            for (int columna = minColumna + 1; columna < maxColumna; columna++)  // Cambié de char a int
            {
                casilla casillaIntermedia = ObtenerCasilla(columna, origen.Fila);  // Usamos int en vez de char
                if (casillaIntermedia.Pieza != null)
                {
                    return false;
                }
            }
            return true;
        }



        private bool EsMovimientoValidoCaballo(casilla origen, casilla destino)
        {
            int dx = Math.Abs(destino.Columna - origen.Columna);
            int dy = Math.Abs(destino.Fila - origen.Fila);

            if ((dx == 2 && dy == 1) || (dx == 1 && dy == 2))
            {
                return true;
            }

            return false;
        }

        private bool EsMovimientoValidoAlfil(casilla origen, casilla destino)
        {
            int dx = Math.Abs(destino.Columna - origen.Columna);
            int dy = Math.Abs(destino.Fila - origen.Fila);

            if (dx == dy)
            {
                return EsRutaLibreDiagonal(origen, destino);
            }//holi

            return false;
        }

        private bool EsRutaLibreDiagonal(casilla origen, casilla destino)
        {
            int dx = Math.Abs(destino.Columna - origen.Columna);
            int dy = Math.Abs(destino.Fila - origen.Fila);

            int x = origen.Columna < destino.Columna ? 1 : -1;
            int y = origen.Fila < destino.Fila ? 1 : -1;

            for (int i = 1; i < dx; i++)
            {
                casilla casillaIntermedia = ObtenerCasilla((char)(origen.Columna + i * x), origen.Fila + i * y);
                if (casillaIntermedia.Pieza != null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool EsMovimientoValidoReina(casilla origen, casilla destino)
        {
            return EsMovimientoValidoTorre(origen, destino) || EsMovimientoValidoAlfil(origen, destino);
        }


        private bool EsMovimientoValidoRey(casilla origen, casilla destino)
        {
            int dx = Math.Abs(destino.Columna - origen.Columna);
            int dy = Math.Abs(destino.Fila - origen.Fila);

            return dx <= 1 && dy <= 1;
        }
        private casilla ObtenerCasilla(int columna, int fila)  // Cambié de char a int
        {
            return layout.Controls.OfType<Panel>()
                .FirstOrDefault(p => ((casilla)p.Tag).Columna == columna && ((casilla)p.Tag).Fila == fila)?.Tag as casilla;
        }
    }
}
