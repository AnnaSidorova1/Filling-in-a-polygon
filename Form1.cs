using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace КГ_лаба5
{
    public partial class Form1 : Form
    {
        List<int[]> Figure = new List<int[]>();
        int[] Pixel = new int[2];
        Queue<int[]> Que = new Queue<int[]>();
        List<int[]> Filled_Pixels = new List<int[]>();
        List<int[]> Border = new List<int[]>();

        //матрицы возврата обратно в исходную позицию
        int[,] return_matrix = { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
        int[,] matrix_move = { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
            matrix_Ox = { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
            matrix_Oy = { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
            matrix_XY = { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };

        public Form1()
        {
            InitializeComponent();
        }

        private void Draw()
        {
            for(int i = 0; i < Filled_Pixels.Count; ++i)
            {
                Pixel[0] = Filled_Pixels[i][0];
                Pixel[1] = Filled_Pixels[i][1];
                Thread.Sleep(200);
                Draw_Pixel();
                zgc.Refresh();
            }
        }

        private void Set()
        {
            //считывание координат многоугольника и отрезка без проверки правильности введенных данных
            //обязателен enter при вводе координат

            //многоугольник
            try
            {
                Figure.Clear();
                string str = mnTB.Text;

                int pos = str.IndexOf(' ');
                int pos2 = str.IndexOf('\r');
                int X1 = Convert.ToInt32(str.Substring(0, pos));
                int Y1 = Convert.ToInt32(str.Substring(pos + 1, pos2 - pos - 1));
                int[] coordinates = { X1, Y1, 1 };
                Figure.Add(coordinates);
                str = str.Substring(pos2 + 2);
                int rx1 = X1, ry1 = Y1;

                while (str != "")
                {
                    pos = str.IndexOf(' ');
                    pos2 = str.IndexOf('\r');
                    X1 = Convert.ToInt32(str.Substring(0, pos));
                    Y1 = Convert.ToInt32(str.Substring(pos + 1, pos2 - pos - 1));
                    int[] coordinates_ = { X1, Y1, 1 };
                    Figure.Add(coordinates_);
                    str = str.Substring(pos2 + 2);
                }
                Figure.Add(coordinates);

                //пискель
                int xx = Convert.ToInt32(x1.Text);
                int yy = Convert.ToInt32(y1.Text);

                Pixel[0] = xx;
                Pixel[1] = yy;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
        }

        private void Draw_Line()
        {
            //настройка области
            GraphPane pane = zgc.GraphPane;
            pane.XAxis.Title.Text = "Ось X";
            pane.YAxis.Title.Text = "Ось Y";
            pane.XAxis.MajorGrid.IsZeroLine = true;

            //координаты отображаемой области
            pane.XAxis.Scale.Min = -10;
            pane.XAxis.Scale.Max = 10;
            pane.YAxis.Scale.Min = -10;
            pane.YAxis.Scale.Max = 10;

            //списки точек 
            
            PointPairList list2 = new PointPairList();

            for (int i = 0; i < Figure.Count; ++i)
            {
                list2.Add(Figure[i][0], Figure[i][1]);
            }
            list2.Add(Figure[0][0], Figure[0][1]);

            LineItem myFig = pane.AddCurve("", list2, Color.Black, SymbolType.Circle);
            zgc.Invalidate();
            zgc.AxisChange();
        }

        private void Draw_Pixel()
        {
            GraphPane pane = zgc.GraphPane;
            PointPairList list = new PointPairList();

            list.Add(Pixel[0], Pixel[1]);
            list.Add(Pixel[0] + 1, Pixel[1]);
            list.Add(Pixel[0] + 1, Pixel[1] + 1);
            list.Add(Pixel[0], Pixel[1] + 1);
            list.Add(Pixel[0], Pixel[1]);



            LineItem line_ = pane.AddCurve("", list, Color.Lime, SymbolType.None);
            line_.Line.Fill = new ZedGraph.Fill(Color.Aquamarine);
            zgc.Invalidate();
            zgc.AxisChange();
        }

        private void Draw_Pixel_2()
        {
            GraphPane pane = zgc.GraphPane;
            PointPairList list = new PointPairList();

            list.Add(Pixel[0], Pixel[1]);
            list.Add(Pixel[0] + 1, Pixel[1]);
            list.Add(Pixel[0] + 1, Pixel[1] + 1);
            list.Add(Pixel[0], Pixel[1] + 1);
            list.Add(Pixel[0], Pixel[1]);


            LineItem line_ = pane.AddCurve("", list, Color.Gray, SymbolType.None);
            line_.Line.Fill = new ZedGraph.Fill(Color.LightGray);
            zgc.Invalidate();
            zgc.AxisChange();
        }

        private void Return_in_start()
        {
            //функция возврата отрезка и точек в исходное положение
            int[] new_m;
            for (int i = 0; i < 3; ++i)
            {
                //возврат отражения по х = у
                int[] v = { return_matrix[0, i], return_matrix[1, i], return_matrix[2, i] };
                new_m = Multiplication_Matrix(v, matrix_XY);
                return_matrix[0, i] = new_m[0];
                return_matrix[1, i] = new_m[1];
                return_matrix[2, i] = new_m[2];
            }
            for (int i = 0; i < 3; ++i)
            {
                //возврат отражения по оси Ох
                int[] v = { return_matrix[0, i], return_matrix[1, i], return_matrix[2, i] };
                new_m = Multiplication_Matrix(v, matrix_Ox);
                return_matrix[0, i] = new_m[0];
                return_matrix[1, i] = new_m[1];
                return_matrix[2, i] = new_m[2];
            }
            for (int i = 0; i < 3; ++i)
            {
                //возврат отражения по оси Оу
                int[] v = { return_matrix[0, i], return_matrix[1, i], return_matrix[2, i] };
                new_m = Multiplication_Matrix(v, matrix_Oy);
                return_matrix[0, i] = new_m[0];
                return_matrix[1, i] = new_m[1];
                return_matrix[2, i] = new_m[2];
            }
            for (int i = 0; i < 3; ++i)
            {
                //возврат сдвига
                int[] v = { return_matrix[0, i], return_matrix[1, i], return_matrix[2, i] };
                new_m = Multiplication_Matrix(v, matrix_move);
                return_matrix[0, i] = new_m[0];
                return_matrix[1, i] = new_m[1];
                return_matrix[2, i] = new_m[2];
            }
        }

        private void Moving_along_axisX(ref int[] a, ref int[] b)
        {
            //перенос в начало координат
            try
            {
                int koeff = a[0];
                int koeff2 = a[1];
                int[] Points1 = new int[3];
                int[] Points2 = new int[3];
                int[,] matrix = { { 1, 0, -koeff }, { 0, 1, -koeff2 }, { 0, 0, 1 } };
                Points1 = (Multiplication_Matrix(a, matrix));
                Points2 = (Multiplication_Matrix(b, matrix));
                a = Points1;
                b = Points2;
                int[,] matrix2 = { { 1, 0, koeff }, { 0, 1, koeff2 }, { 0, 0, 1 } };
                matrix_move = matrix2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + " - у вас ошибка");
            }
        }

        private void Reflection_Ox(ref int[] a, ref int[] b)
        {
            //отражение относительно оси Х
            try
            {
                int[] Points1 = new int[3];
                int[] Points2 = new int[3];
                int[,] matrix = { { 1, 0, 0 }, { 0, -1, 0 }, { 0, 0, 1 } };
                Points1 = (Multiplication_Matrix(a, matrix));
                Points2 = (Multiplication_Matrix(b, matrix));
                a = Points1;
                b = Points2;
                //матрица для возврата в исходную позицию всех элементов
                int[,] matrix2 = { { 1, 0, 0 }, { 0, -1, 0 }, { 0, 0, 1 } };
                matrix_Ox = matrix2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + " - у вас ошибка");
            }
        }

        private void Reflection_Oy(ref int []a, ref int []b)
        {
            //отражение относительно оси У
            try
            {
                int[] Points1 = new int[3];
                int[] Points2 = new int[3];
                int[,] matrix = { { -1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
                Points1 = (Multiplication_Matrix(a, matrix));
                Points2 = (Multiplication_Matrix(b, matrix));
                a = Points1;
                b = Points2;
                //матрица для возврата в исходную позицию всех элементов
                int[,] matrix2 = { { -1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
                matrix_Oy = matrix2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + " - у вас ошибка");
            }
        }

        private void Reflection_XY(ref int[] a, ref int[] b)
        {
            //отражение относительно прямой х=у
            try
            {
                int[] Points1 = new int[3];
                int[] Points2 = new int[3];
                int[,] matrix = { { 0, 1, 0 }, { 1, 0, 0 }, { 0, 0, 1 } };
                Points1 = (Multiplication_Matrix(a, matrix));
                Points2 = (Multiplication_Matrix(b, matrix));
                a = Points1;
                b = Points2;
                //матрица для возврата в исходную позицию всеx элементов
                int[,] matrix2 = { { 0, 1, 0 }, { 1, 0, 0 }, { 0, 0, 1 } };
                matrix_XY = matrix2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + " - у вас ошибка");
            }
        }

        private int[] Multiplication_Matrix(int[] vector, int[,] matrix)
        {
            //умножение вектора на матрицу
            int[] newvector = new int[3];
            int tmp = 0;
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    tmp += vector[j] * matrix[i, j];
                }
                newvector[i] = tmp;
                tmp = 0;
            }
            return newvector;
        }

        private void BresenhamLine(int x0, int y0, int x1, int y1)
        {
            //нахождение координат точек
            try
            {
                //заполнение координат отрезка для его построения
                GraphPane pane = zgc.GraphPane;
                pane.CurveList.Clear();

                int[] a = { x0, y0, 1 };
                int[] b = { x1, y1, 1 };
                bool paral_x, paral_y;
                if (y0 == y1) paral_x = true;
                else paral_x = false;
                if (x0 == x1) paral_y = true;
                else paral_y = false;

                //if (Math.Sqrt(a[0] * a[0] + a[1] * a[1]) > Math.Sqrt(b[0] * b[0] + b[1] * b[1])
                //    || (Math.Sqrt(a[0] * a[0] + a[1] * a[1]) == Math.Sqrt(b[0] * b[0] + b[1] * b[1])
                //    && b[0] > b[0]))
                //{
                //    int tt_x = a[0];
                //    a[0] = b[0];
                //    b[0] = tt_x;

                //    int tt_y = a[1];
                //    a[1] = b[1];
                //    b[1] = tt_y;
                //}

                List<int[]> Points = new List<int[]>();

                if (a[0] == b[0] && a[1] == b[1])
                    throw new Exception("Точки совпадают");

                bool XY = false, X = false, Y = false;
                //в начало координат
                Moving_along_axisX(ref a, ref b);

                if (b[0] < 0)
                {
                    Reflection_Oy(ref a, ref b);
                    Y = true;
                }
                if (b[1] < 0)
                {
                    Reflection_Ox(ref a, ref b);
                    X = true;
                }
                if (Math.Abs(b[0] - a[0]) <= Math.Abs(b[1] - a[1]))
                {
                    Reflection_XY(ref a, ref b);
                    XY = true;
                }

                int x0_ = a[0];
                int y0_ = a[1];

                List<int[]> Points_Node_reserve = new List<int[]>();

                x0_++;
                double D0 = 2 * (b[1] - a[1]) - (b[0] - a[0]);
                if (D0 >= 0)
                {
                    y0_ += 1;
                    D0 += (2 * (b[1] - a[1]) - 2 * (b[0] - a[0]));
                }
                else
                {
                    D0 += 2 * (b[1] - a[1]);
                }
                int[] t2 = { x0_, y0_, 1 };

                Points_Node_reserve.Add(t2);
                for (int i = x0_; i < b[0]; ++i)
                {
                    if (D0 >= 0)
                    {
                        y0_ += 1;
                        D0 += (2 * (b[1] - a[1]) - 2 * (b[0] - a[0]));
                    }
                    else
                    {
                        D0 += 2 * (b[1] - a[1]);
                    }
                    x0_++;
                    int[] t1 = { x0_, y0_, 1 };
                    Points_Node_reserve.Add(t1);
                }

                Return_in_start();

                int[] Points1 = new int[3];
                int[] Points2 = new int[3];
                Points1 = (Multiplication_Matrix(a, return_matrix));
                Points2 = (Multiplication_Matrix(b, return_matrix));
                a = Points1;
                b = Points2;


                List<int[]> Points_N = new List<int[]>();
                for (int i = 0; i < Points_Node_reserve.Count(); ++i)
                {
                    Points_N.Add(Multiplication_Matrix(Points_Node_reserve[i], return_matrix));
                }
                Points_Node_reserve = Points_N;

                for(int i = 0; i < Points_Node_reserve.Count; ++i)
                {
                    if(Y && !X && !XY && !paral_x)
                    {
                        Points_Node_reserve[i][1] -= 1;
                    }
                    if (X && !Y && !XY)
                    {
                        Points_Node_reserve[i][0] -= 1;
                    }
                    if(XY && !X && !Y && !paral_y)
                    {
                        Points_Node_reserve[i][0] -= 1;
                        Points_Node_reserve[i][1] -= 1;
                    }
                    if (XY && !X && !Y && paral_y)
                    {
                        Points_Node_reserve[i][1] -= 1;
                    }

                    if (Y && XY && !X)
                    {
                        Points_Node_reserve[i][1] -= 1;
                    }
                    if (X && XY && !Y)
                    {
                        Points_Node_reserve[i][0] -= 1;
                    }
                    if(!X && !Y && !XY)
                    {
                        Points_Node_reserve[i][0] -= 1;
                        Points_Node_reserve[i][1] -= 1;
                    }
                }

                for (int i = 0; i < Points_Node_reserve.Count; ++i)
                    Border.Add(Points_Node_reserve[i]);
                

                //освобождение всех массивов для след отрезков
                return_matrix = new int[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
                matrix_move = new int[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
                matrix_Ox = new int[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
                matrix_Oy = new int[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
                matrix_XY = new int[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + " - у вас ошибка");
            }
        }

        private void Rastr()
        {
            Border.Clear();
            Filled_Pixels.Clear();
            for (int i = 0; i < Figure.Count - 1; ++i)
            {
                BresenhamLine(Figure[i][0], Figure[i][1], Figure[i + 1][0], Figure[i + 1][1]);
            }

            //отрисовка границы
            for (int i = 0; i < Border.Count; ++i)
            {
                Pixel[0] = Border[i][0];
                Pixel[1] = Border[i][1];
                Draw_Pixel_2();
            }
        }

        private bool IsFilled(int[] pix)
        {
            for (int i = 0; i < Filled_Pixels.Count; ++i)
            {
                if (Filled_Pixels[i][0] == pix[0] &&
                    Filled_Pixels[i][1] == pix[1])
                    return true;
            }
            return false;
        }

        private bool IsBorder(int[] pix)
        {
            for (int i = 0; i < Border.Count; ++i)
            {
                if (Border[i][0] == pix[0] &&
                    Border[i][1] == pix[1])
                    return true;
            }
            return false;
        }
/*
-5 -5
6 -3
7 8
-7 4
pixel 2 2
*/

        private void Algorithm()
        {
            try
            {
                int xx = Convert.ToInt32(x1.Text);
                int yy = Convert.ToInt32(y1.Text);
                int[] el = { xx, yy };
                Que.Enqueue(el);
                Pixel[0] = xx;
                Pixel[1] = yy;

                Filled_Pixels.Add(el);

                Draw_Pixel();

                while (Que.Count != 0)
                {
                    el = Que.Dequeue();

                    int[] t1 = { el[0] + 1, el[1] };
                    if (!IsFilled(t1) && !IsBorder(t1))
                    {
                        Que.Enqueue(t1);
                        Filled_Pixels.Add(t1);
                    }
                    int[] t2 = { el[0] - 1, el[1] };
                    if (!IsFilled(t2) && !IsBorder(t2))
                    {
                        Que.Enqueue(t2);
                        Filled_Pixels.Add(t2);

                    }
                    int[] t3 = { el[0], el[1] - 1 };
                    if (!IsFilled(t3) && !IsBorder(t3))
                    {
                        Que.Enqueue(t3);
                        Filled_Pixels.Add(t3);
                    }
                    int[] t4 = { el[0], el[1] + 1 };
                    if (!IsFilled(t4) && !IsBorder(t4))
                    {
                        Que.Enqueue(t4);
                        Filled_Pixels.Add(t4);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + " - у вас ошибка");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            GraphPane pane = zgc.GraphPane;
            pane.CurveList.Clear();
            Border.Clear();
            Set();
            Rastr();

            Draw_Line();
            Algorithm();
            Draw();
            //Draw_Pixel();
        }
    }
}
