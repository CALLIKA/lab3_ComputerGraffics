using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace labCompGraf3
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            pictureBox1.BackColor = Color.DarkSeaGreen;
        }

        private void Swap(ref int x,ref int y)
        {
            int tmp = x;
            x = y;
            y = tmp;
        }

        public void BresenhamLine(int x0, int y0, int x1, int y1, Bitmap bmp, Color color)
        {

            bool checkM = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

            if (checkM)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }

            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            double dx = x1 - x0;
            double dy = Math.Abs(y1 - y0);
            double m = dy / dx;
            double e = m - 0.5;
            int ySign = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                bmp.SetPixel(checkM ? y : x, checkM ? x : y, color);
                if (e >= 0)
                {
                    y += ySign;
                    e += m - 1;
                }
                else e += m;
            }
        }

        public void BezierDrawPoints(PointF[] pointF, double t, Bitmap bmp)
        {            
            if (pointF.Length > 1)
            {
                PointF[] tmp = new PointF[pointF.Length - 1];
                for (int i = 0; i < pointF.Length - 1; i++)
                {
                    tmp[i].X = pointF[i].X + (pointF[i + 1].X - pointF[i].X) * (float)t;
                    tmp[i].Y = pointF[i].Y + (pointF[i + 1].Y - pointF[i].Y) * (float)t;
                }
                BezierDrawPoints(tmp, t, bmp);
            }
            else bmp.SetPixel((int)pointF[0].X, (int)pointF[0].Y, Color.Red);
            return;
        }
               
        public void BezierCurve(PointF[] pointF, Bitmap bmp)
        {
            for (double t = 0.001; t < 1; t += 0.001)
            {
                BezierDrawPoints(pointF, t, bmp);
            }
        }

        public void Draw8Pixels(int x, int y, int x0, int y0, Bitmap bmp, Color color)
        {
            bmp.SetPixel(x + x0, y + y0, color);
            bmp.SetPixel(x + x0, -y + y0, color);
            bmp.SetPixel(-x + x0, y + y0, color);
            bmp.SetPixel(-x + x0, -y + y0, color);
            bmp.SetPixel(y + x0, x + y0, color);
            bmp.SetPixel(y + x0, -x + y0, color);
            bmp.SetPixel(-y + x0, x + y0, color);
            bmp.SetPixel(-y + x0, -x + y0, color);
        }

        public void BrezenhamCircle(int x0, int y0, int R, Bitmap bmp, Color color)
        {
            int x = 0;
            int y = R;
            int d = 3 - 2 * R;
            for(; y>=x; x++)
            { 
                Draw8Pixels(x, y, x0, y0, bmp, color);
                if (d <= 0)
                {
                    d += 4 * x + 6;
                }
                else
                {
                    d += 4 * (x - y) + 10;
                    y--;
                }
            }
        }

        //  Закраска узором
        public void Pattern(int x, int y, Bitmap bmp, Color[,] color_pattern, int w, int h)
        {
            Color backcolor = bmp.GetPixel(x, y);
            int xl = x;
            int xr = x;

            //Двигаемся влево, пока не встретим границу             
            while ((xl > 0) && (bmp.GetPixel(xl - 1, y) == backcolor))
            {
                xl--;
            }

            //Двигаемся вправо, пока не встретим границу             
            while ((xr < bmp.Width - 1) && (bmp.GetPixel(xr + 1, y) == backcolor))
            {
                xr++;
            }

            // Закрашиваем все внутри отрезка
            for (int tmpX = xl; tmpX <= xr; tmpX++)
            {
                bmp.SetPixel(tmpX, y, color_pattern[tmpX % w, y % h]);
            }

            for (int tmpX = xl; tmpX <= xr; tmpX++)
            {
                if (y > 0 && bmp.GetPixel(tmpX, y - 1) == backcolor)
                {
                    Pattern(tmpX, y - 1, bmp, color_pattern, w, h);
                }

                if (y < bmp.Height - 1 && bmp.GetPixel(tmpX, y + 1) == backcolor)
                {
                    Pattern(tmpX, y + 1, bmp, color_pattern, w, h);
                }
            }
        }

        public int w = 30;
        public int h = 8;

        //Рисование черного столбца
        private Color[,] PlusColumn(int numColumn, Color[,] changeable )
        {
            for (int i = 0; i < h-2; i++)
            {
                changeable[numColumn, i] = Color.Black;
            }
            return changeable;
        }
        //Рисование черной строки
        private Color[,] PlusLine(int numColumn, int numLine,  Color[,] changeable)
        {
            for (int i = 0; i < 3; i++)
            {
                changeable[i + numColumn, numLine] = Color.Black;
            }
            return changeable;
        }
        //Создание целого узора
        private Color[,] MakePattern(Color[,] tmp)
        {
            Color[,] CALLIKA = tmp;
            for (int i = 0; i < CALLIKA.GetLength(0); ++i)
            {
                for (int j = 0; j < CALLIKA.GetLength(1); ++j) {
                    CALLIKA[i, j] = Color.Pink; 
                }
            }

            CALLIKA = PlusColumn(0, CALLIKA);
            CALLIKA = PlusColumn(4, CALLIKA);
            CALLIKA = PlusColumn(6, CALLIKA);
            CALLIKA = PlusColumn(8, CALLIKA);
            CALLIKA = PlusColumn(12, CALLIKA);
            CALLIKA = PlusColumn(16, CALLIKA);
            CALLIKA = PlusColumn(18, CALLIKA);
            CALLIKA = PlusColumn(23, CALLIKA);
            CALLIKA = PlusColumn(25, CALLIKA);
            CALLIKA = PlusLine(0, 0, CALLIKA);
            CALLIKA = PlusLine(0, 4, CALLIKA);
            CALLIKA = PlusLine(0, 0, CALLIKA);
            CALLIKA = PlusLine(4, 0, CALLIKA);
            CALLIKA = PlusLine(4, 2, CALLIKA);
            CALLIKA = PlusLine(8, 4, CALLIKA);
            CALLIKA = PlusLine(12, 4, CALLIKA);
            CALLIKA = PlusLine(23, 0, CALLIKA);
            CALLIKA = PlusLine(23, 2, CALLIKA);
            CALLIKA[19, 2] = Color.Black;
            CALLIKA[20, 1] = Color.Black;
            CALLIKA[20, 3] = Color.Black;
            CALLIKA[21, 0] = Color.Black;
            CALLIKA[21, 4] = Color.Black;

            return CALLIKA;
        } 



        bool first = true;
        Random rnd = new Random();
        PointF[] BezierPoints = new PointF[]
        {
            new PointF(300,180),
            new PointF(270,300),
            new PointF(400,15),
            new PointF(300,300),

        };
        private void RunClickStartButton()
        {
            pictureBox1.BackColor = Color.DarkSeaGreen;

            Bitmap bmp = first ? new Bitmap(pictureBox1.Width, pictureBox1.Height) : new Bitmap(pictureBox1.Image);
            first = false;

            Color[,] CALLIKA = new Color[w, h];

            CALLIKA = MakePattern(CALLIKA);

            int r = rnd.Next(25, 75);
            int x = rnd.Next(r + 1, pictureBox1.Width - r - 1);
            int y = rnd.Next(r + 1, pictureBox1.Height - r - 1);
            if (bmp.GetPixel(x, y).ToArgb() != new Color().ToArgb()) return;

            BezierCurve(BezierPoints, bmp);

            BresenhamLine((int)BezierPoints[0].X, (int)BezierPoints[0].Y, (int)BezierPoints[1].X, (int)BezierPoints[1].Y, bmp, Color.Black);
            BresenhamLine((int)BezierPoints[1].X, (int)BezierPoints[1].Y, (int)BezierPoints[2].X, (int)BezierPoints[2].Y, bmp, Color.Black);
            BresenhamLine((int)BezierPoints[2].X, (int)BezierPoints[2].Y, (int)BezierPoints[3].X, (int)BezierPoints[3].Y, bmp, Color.Black);

            //BrezenhamCircle(x, y, r, bmp, Color.Yellow);
            //Pattern(x, y, bmp, CALLIKA, w, h);

            pictureBox1.Image = bmp;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            const int stackSize = (1 << 20) * 256;
            var thread = new Thread(RunClickStartButton, stackSize);
            thread.Start();
            thread.Join();
            //RunClickStartButton();
        }

        public void PatternBrute(int x, int y, Bitmap bmp, Color[,] color_pattern, int w, int h)
        {
            Color backcolor = bmp.GetPixel(x, y);

            // Закрашиваем все внутри отрезка
            bmp.SetPixel(x, y, color_pattern[x % w, y % h]);

            if (x > 0 && bmp.GetPixel(x - 1, y) == backcolor) PatternBrute(x - 1, y, bmp, color_pattern, w, h);
            if (x < bmp.Width - 1 && bmp.GetPixel(x + 1, y) == backcolor) PatternBrute(x + 1, y, bmp, color_pattern, w, h);

            if (y > 0 && bmp.GetPixel(x, y - 1) == backcolor)
            {
                PatternBrute(x, y - 1, bmp, color_pattern, w, h);
            }

            if (y < bmp.Height - 1 && bmp.GetPixel(x, y + 1) == backcolor)
            {
                PatternBrute(x, y + 1, bmp, color_pattern, w, h);
            }
        }

        public void PatternQueue(int xs, int ys, Bitmap bmp, Color[,] color_pattern, int w, int h)
        {
            Color backcolor = bmp.GetPixel(xs, ys);

            Queue<Point> q = new Queue<Point>();
            q.Enqueue(new Point(xs, ys));

            while (q.Count > 0)
            {
                Point p = q.Dequeue();
                int x = p.X, y = p.Y;

                if (bmp.GetPixel(x, y) != backcolor) continue;

                // Закрашиваем все внутри отрезка
                bmp.SetPixel(x, y, color_pattern[p.X % w, p.Y % h]);

                if (x > 0 && bmp.GetPixel(x - 1, y) == backcolor) q.Enqueue(new Point(x - 1, y));
                if (x < bmp.Width - 1 && bmp.GetPixel(x + 1, y) == backcolor) q.Enqueue(new Point(x + 1, y));

                if (y > 0 && bmp.GetPixel(x, y - 1) == backcolor)
                {
                    q.Enqueue(new Point(x, y - 1));
                }

                if (y < bmp.Height - 1 && bmp.GetPixel(x, y + 1) == backcolor)
                {
                    q.Enqueue(new Point(x, y + 1));
                }
            }
        }
    }
}
