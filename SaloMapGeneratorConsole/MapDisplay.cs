using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salo.SaloMapGeneratorConsole
{
    public partial class MapDisplay : Form
    {
        private System.Drawing.Graphics formGraphics;

        public MapDisplay()
        {
            InitializeComponent();
            formGraphics = CreateGraphics();
        }

        

        public void DrawCircle(int x, int y, System.Drawing.Color color, int size){
            System.Drawing.Pen defaultPen = new System.Drawing.Pen(color);
            if (color != System.Drawing.Color.Black)
                defaultPen.Width = 2.0F;
            formGraphics.DrawEllipse(defaultPen, x, y, size, size);
        }

        public void DrawBackground()
        {
            formGraphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Red), new System.Drawing.Rectangle(0, 0, 250, 250));
        }
    }
}
