using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApplication6;

using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;


namespace WindowsFormsApplication6
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private Bitmap bitmap1 = null;
        private Bitmap bitmap11 = null;
        private string full_name;
        public double[,] HSV_h;
        public double[,] HSV_s;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //SaveFileDialog sfd = new SaveFileDialog();
            openFileDialog1.FileName = "";
            openFileDialog1.DefaultExt = "bmp";
            openFileDialog1.Filter = "Image files (*.bmp;*.PNG;*.jpg)|*.bmp;*.PNG;*.jpg|All files (*.*)|*.*";
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    bitmap1 = new Bitmap(openFileDialog1.FileName);
                    full_name = openFileDialog1.FileName;
                    picture1.Image = bitmap1;
                    //  imageBox1.Image = bitmap1.I
                    toolStripButton2.Enabled = true;
                    toolStripButton3.Enabled = true;

                    toolStripSplitButton2.Enabled = true;
                    Image<Bgr, Byte> image = new Image<Bgr, Byte>(bitmap1);
                    imageBox1.Image = image;
                    
                    picture1.Visible = false;
                }
                else return;
            }
            catch 
            {
                 MessageBox.Show("Недопустимый формат файла", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public struct YCbCrColor
        {
            public int Y { set; get; }
            public int Cb { set; get; }
            public int Cr { set; get; }

            public YCbCrColor(int y, int cb, int cr)
                : this()
            {
                Y = y;
                Cb = cb;
                Cr = cr;
            }

            public Color ToRgbColor()
            {
                int r = Convert.ToInt32((double)this.Y +
                    1.402 * (double)(this.Cr - 128));
                int g = Convert.ToInt32((double)this.Y -
                    0.34414 * (double)(this.Cb - 128) -
                    0.71414 * (double)(this.Cr - 128));
                int b = Convert.ToInt32((double)this.Y +
                    1.772 * (double)(this.Cb - 128));

                return Color.FromArgb(r, g, b);
            }

            public static YCbCrColor FromRgbColor(Color color)
            {
                int y = Convert.ToInt32(0.299 * (double)color.R +
                    0.587 * (double)color.G +
                    0.114 * (double)color.B);
                int cb = Convert.ToInt32(128 - 0.168736 * (double)color.R -
                    0.331264 * (double)color.G +
                    0.5 * (double)color.B);
                int cr = Convert.ToInt32(128 + 0.5 * (double)color.R -
                    0.418688 * (double)color.G -
                    0.081312 * (double)color.B)-1;

                return new YCbCrColor(y, cb, cr);
            }
        }
        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
        private double hMin = 0;
        private double hMax = 50;
        private double hMin2 = 93;
        private double hMax2 = 100;
        private double sMin = 0.23;
        private double sMax = 0.68;
        
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            
            segment _obuch = new segment();
            _obuch.PacF.Text = full_name;
            
            _obuch.Width += 250;
            _obuch.Show();
            
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            int rr, gg, bb,kol,KOL, flag, sh,vis;
            int[,] r_mas;
            int[,] g_mas;
            int[,] b_mas;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bitmap11 = new Bitmap(openFileDialog1.FileName);                
            }
            FastBitmap bbb;
            YCbCrColor yyy;
            bbb = new FastBitmap(bitmap11);
            r_mas = new int[bitmap11.Height, bitmap11.Width];
            g_mas = new int[bitmap11.Height, bitmap11.Width];
            b_mas = new int[bitmap11.Height, bitmap11.Width];
            sh = bitmap11.Width;
            vis = bitmap11.Height;
           // strproba = strproba.Replace("ляляля", "777");
            for (int i = 0; i < sh; i++)
            {
                for (int j = 0; j < vis; j++)
                {
                    Color orig_mas = bbb.GetColor(i, j);
                    r_mas[i, j] = orig_mas.R;
                    g_mas[i, j] = orig_mas.G;
                    b_mas[i, j] = orig_mas.B;
                }
            }



            
            
            for (int i = 0; i < sh; i++)
            {
                for (int j = 0; j < vis; j++)
                {
                    //Color orig = bbb.GetColor(i, j);
                    if (r_mas[i, j] == 0 && g_mas[i, j] == 0 && b_mas[i, j] == 0)
                    {
                        rr = 123;
                        continue;
                    }
                    Color orig = Color.FromArgb(r_mas[i, j],g_mas[i, j],b_mas[i, j]);
                    string[] str = new string[30];
                    char[] DataSeparator = { '\n' };
                    StreamReader sr = File.OpenText(Application.StartupPath + "\\baza.txt");
                    str = sr.ReadLine().Trim().Split(DataSeparator);
                    KOL = Int32.Parse(str[0]);
                    DataSeparator[0] = ' ';
                    flag = 0;
                    while (true)
                    {
                        string st = sr.ReadLine();
                        if (st == null)
                            break;

                        str = st.Trim().Split(DataSeparator);
                        rr = Int32.Parse(str[0]);
                        gg = Int32.Parse(str[1]);
                        bb = Int32.Parse(str[2]);
                        kol = Int32.Parse(str[5]);
                        if (r_mas[i, j] == rr && b_mas[i, j] == bb && g_mas[i, j] == gg)
                        {
                            
                            yyy = YCbCrColor.FromRgbColor(orig);
                            sr.Close();
                            string strproba = string.Empty;
                            using (System.IO.StreamReader reader = System.IO.File.OpenText(Application.StartupPath + "\\baza.txt"))
                            {
                                strproba = reader.ReadToEnd();
                                reader.Close();
                            }
                           
                            string s1 = (r_mas[i, j] + " " + g_mas[i, j] + " " + b_mas[i, j] + " " + yyy.Cb + " " + yyy.Cr +" " + kol);
                            string s2 = (r_mas[i, j] + " " + g_mas[i, j] + " " + b_mas[i, j] + " " + yyy.Cb + " " + yyy.Cr + " " +(kol+1));
                            strproba = strproba.Replace(s1, s2);
                            //s1 = (""+KOL);
                           // s2 = ("" + (KOL+1));

                           // strproba = strproba.Replace(s1, s2);
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\baza.txt"))
                            {
                                file.Write(strproba);
                                
                            }
                            //File.WriteAllLines(Application.StartupPath + @"\Rate.txt", str);
                            flag = 1;
                             break;
                        }
                       
                    }
                    
                    
                    if (flag == 0)
                    {
                        sr.Close();
                        StreamWriter sw = File.AppendText(Application.StartupPath + "\\baza.txt"); 
                        yyy = YCbCrColor.FromRgbColor(orig);
                        sw.WriteLine(r_mas[i, j] +  " " + g_mas[i, j] + " " + b_mas[i, j] +" " + yyy.Cb + " " +yyy.Cr +" " + 1);
                        sw.Close();
                    }

                    
                    
                }
            }
            MessageBox.Show("Данные занесены в базу", "Процесс завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
          
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            int rr, gg, bb, kol, KOL, flag, dlina, stroki,sh, vis;
            int[,] r_mas;
            int[,] g_mas;
            int[,] b_mas;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bitmap11 = new Bitmap(openFileDialog1.FileName);
            }
            else return;
            FastBitmap bbb;
            YCbCrColor yyy;
            bbb = new FastBitmap(bitmap11);
            //int[][] mas = new int[6][];
            r_mas = new int[bitmap11.Width, bitmap11.Height];
            g_mas = new int[bitmap11.Width, bitmap11.Height];
            b_mas = new int[bitmap11.Width, bitmap11.Height];
            sh = bitmap11.Width;
            vis = bitmap11.Height;
            Color orig_mas;
            for (int i = 0; i < bitmap11.Width; i++)
            {
                for (int j = 0; j < bitmap11.Height; j++)
                {
                    orig_mas = bbb.GetColor(i, j);
                    r_mas[i, j] = orig_mas.R;
                    g_mas[i, j] = orig_mas.G;
                    b_mas[i, j] = orig_mas.B;
                }
            }

            flag = 0;
            int ii, jj;
            string strproba = string.Empty;
            using (System.IO.StreamReader reader = System.IO.File.OpenText(Application.StartupPath + "\\baza.txt"))
            {
                strproba = reader.ReadToEnd();
                reader.Close();
            }
            dlina = strproba.Length;
            stroki = 0;
            for (int i = 0; i < dlina; i++)
                if (strproba[i] == '\n')
                {
                    stroki = stroki + 1;
                }
            string[] proba2 = new string[stroki];
            string[] proba3 = new string[30];
            char[] DataSeparator = { '\n' };
            string s1,s2;
            proba2 = strproba.Trim().Split(DataSeparator);
            //proba2[0] = proba2[0].Replace("\n", "");
            KOL = 0;
           /* KOL = bitmap11.Width * bitmap11.Height;
            s1 = ("" + KOL);
            s2 = proba2[0].Replace("\n", "");

            int p = strproba.IndexOf(s2);
            if (p != -1)
            {
                strproba = strproba.Remove(p, s2.Length).Insert(p, s1);
            }*/
            //strproba = strproba.Replace(s2, s1);

            int kol_p2 = proba2.Length;
            for (int i = 0; i < sh; i++)
            {
                for (int j = 0; j < vis; j++)
                {
                    if (r_mas[i, j] < 20 || g_mas[i, j] < 20 || b_mas[i, j] < 20)
                    {
                      
                        continue;
                    }
                    Color origg = Color.FromArgb(r_mas[i, j],g_mas[i, j],b_mas[i, j]);
                    yyy = YCbCrColor.FromRgbColor(origg);
                    flag = 0;
                    KOL = KOL + 1;
                    DataSeparator[0] = '\n';
                    proba2 = strproba.Trim().Split(DataSeparator);
                    // proba2[4] = proba2[4].Replace("\n", "");
                    kol_p2 = proba2.Length;

                    for (int q = 1; q < kol_p2; q++)
                    {
                        proba2[q] = proba2[q].Replace("\n", "");
                        DataSeparator[0] = ' ';
                        proba3 = proba2[q].Trim().Split(DataSeparator);
                        rr = Int32.Parse(proba3[0]);
                        gg = Int32.Parse(proba3[1]);
                        bb = Int32.Parse(proba3[2]);
                        kol = Int32.Parse(proba3[5]);
                        if (origg.R == rr && origg.B == bb && origg.G == gg)
                        {
                            //yyy = YCbCrColor.FromRgbColor(origg);
                            s1 = (r_mas[i, j] + " " + g_mas[i, j] + " " + b_mas[i, j] + " " + yyy.Cb + " " + yyy.Cr + " " + kol);
                            s2 = (r_mas[i, j] + " " + g_mas[i, j] + " " + b_mas[i, j] + " " + yyy.Cb + " " + yyy.Cr + " " + (kol + 1));
                            strproba = strproba.Replace(s1, s2);
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 0)
                    {
                        //yyy = YCbCrColor.FromRgbColor(origg);
                       // sw.WriteLine(origg.R +  " " + origg.G + " " + origg.B +" " + yyy.Cb + " " +yyy.Cr +" " + 1);

                        strproba = strproba + (r_mas[i, j] + " " + g_mas[i, j] + " " + b_mas[i, j] + " " + yyy.Cb + " " + yyy.Cr + " " + 1) + "\r\n";
                    
                    }
                }

            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\baza.txt"))
            {

                s2 = proba2[0];
                s1 = (Int32.Parse(s2) + KOL+"\r");
                

                int p = strproba.IndexOf(s2);
                if (p != -1)
                {
                    strproba = strproba.Remove(p, s2.Length).Insert(p, s1);
                }
                file.Write(strproba);
                MessageBox.Show("Данные занесены в базу", "Процесс завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //char KOLl = strproba[2];
        }






        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            string[] fullfilesPath;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                // листаем все файлы в выбранной папке
                foreach (string File in Directory.GetFiles(folderBrowserDialog1.SelectedPath))
                {
                    // показываем имя файла
                    bitmap11 = new Bitmap(Image.FromFile(File));
                    // MessageBox.Show(File);

                    //fullfilesPath = Directory.GetFiles(folderBrowserDialog1.SelectedPath); 


                    //  fullfilesPath = Directory.GetFiles(Application.StartupPath + "\\pr");  

                    /*if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        bitmap11 = new Bitmap(openFileDialog1.FileName);
                        else return;
                    }*/


                    FastBitmap bbb;
                    bbb = new FastBitmap(bitmap11);
                    int KOL, sh, vis;
                    int[,] r_mas;
                    int[,] g_mas;
                    int[,] b_mas;
                    r_mas = new int[bitmap11.Width, bitmap11.Height];
                    g_mas = new int[bitmap11.Width, bitmap11.Height];
                    b_mas = new int[bitmap11.Width, bitmap11.Height];
                    sh = bitmap11.Width;
                    vis = bitmap11.Height;
                    Color orig_mas;
                    for (int i = 0; i < bitmap11.Width; i++)
                    {
                        for (int j = 0; j < bitmap11.Height; j++)
                        {
                            orig_mas = bbb.GetColor(i, j);
                            r_mas[i, j] = orig_mas.R;
                            g_mas[i, j] = orig_mas.G;
                            b_mas[i, j] = orig_mas.B;
                        }
                    }
                    // Задаем экземпляр класса.
                    Pix w = new Pix();
                    w.rrr = 0;
                    w.ggg = 0;
                    w.bbb = 0;
                    w.kol = 0;
                    Pix w2 = new Pix();
                    w2.rrr = 0;
                    w2.ggg = 0;
                    w2.bbb = 0;
                    w2.kol = 0;

                    // Сериализуем класс.
                    /*FileStream fs = new FileStream(Application.StartupPath + "\\ser.txt", FileMode.Open, FileAccess.Write);
                    IFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, w);
                    fs.Close();

                    // Десериализуем класс.
                    fs = new FileStream(Application.StartupPath + "\\ser.txt", FileMode.Open, FileAccess.Read);
                    pixx w1 = (pixx)bf.Deserialize(fs);
                    textBox1.Text = ("age: " + w1.rrr + ", yoe: " + w1.ggg);
                    fs.Close(); */


                    List<Pix> students = new List<Pix>();
                    List<Pix> students2 = new List<Pix>();
                    SerializableObject obj = new SerializableObject();
                    obj.Students = students;
                    MySerializer serializer = new MySerializer();
                    /* students.Add(w);


            
                     serializer.SerializeObject(Application.StartupPath + "\\ser.txt", obj);*/

                    obj = serializer.DeserializeObject(Application.StartupPath + "\\ser.txt");
                    students2 = obj.Students;
                    //textBox1.Text = students2[0].rrr +"";
                    int kkk;
                    KOL = 0;
                    //students.Add(w);
                    for (int i = 0; i < sh; i++)
                    {
                        for (int j = 0; j < vis; j++)
                        {
                            if (r_mas[i, j] < 20 && g_mas[i, j] < 20 && b_mas[i, j] < 20)
                            {

                                continue;
                            }
                            Color origg = Color.FromArgb(r_mas[i, j], g_mas[i, j], b_mas[i, j]);
                            
                            kkk = students2.FindIndex(
                            delegate(Pix bk)
                            {
                                return (bk.rrr == r_mas[i, j] && bk.ggg == g_mas[i, j] && bk.bbb == b_mas[i, j]);
                            });
                            if (kkk != -1)
                            {
                                students2[kkk].kol = students2[kkk].kol + 1;
                            }
                            else
                            {
                                w.rrr = r_mas[i, j];
                                w.ggg = g_mas[i, j];
                                w.bbb = b_mas[i, j];
                                w.kol = 1;
                                KOL = KOL + 1;
                                //students2.Add(w);
                                students2.Add(new Pix() { rrr = w.rrr, ggg = w.ggg, bbb = w.bbb, kol = 1 });
                                students2.IndexOf(w2);
                            }
                        }
                    }
                    //MessageBox.Show("Данные добавлены в выборку", "Процесс завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    obj.Students = students2;
                    serializer.SerializeObject(Application.StartupPath + "\\ser.txt", obj);
                    obj = serializer.DeserializeObject(Application.StartupPath + "\\ser.txt");
                    students2 = obj.Students;
                    /* students2.ForEach(delegate(Pix bk)
                     {
                         textBox1.Text = textBox1.Text + bk.rrr+" ";
                     });*/
                    string strproba = string.Empty;
                    using (System.IO.StreamReader reader = System.IO.File.OpenText(Application.StartupPath + "\\kol.txt"))
                    {
                        strproba = reader.ReadToEnd();
                        reader.Close();
                    }
                    string[] proba2 = new string[50];


                    string s1, s2;
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\kol.txt"))
                    {

                        s2 = strproba;
                        s1 = (Int32.Parse(s2) + KOL + "\r");


                        int p = strproba.IndexOf(s2);
                        if (p != -1)
                        {
                            strproba = strproba.Remove(p, s2.Length).Insert(p, s1);
                        }
                        file.Write(strproba);
                        //MessageBox.Show("Данные занесены в базу", "Процесс завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    
                }
            }
            MessageBox.Show("Данные занесены в базу", "Процесс завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            Pix w = new Pix();
            w.rrr = 0;
            w.ggg = 0;
            w.bbb = 0;
            w.kol = 0;
            List<Pix> students = new List<Pix>();

            SerializableObject obj = new SerializableObject();
            obj.Students = students;
            MySerializer serializer = new MySerializer();
            students.Add(w);
            serializer.SerializeObject(Application.StartupPath + "\\ser.txt", obj);
            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\kol.txt"))
            {

                file.Write("0");
                MessageBox.Show("Данные занесены в базу", "Процесс завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bitmap11 = new Bitmap(openFileDialog1.FileName);
                picture1.Image = bitmap11;
            }
            else return;
            
            var bmp = picture1.Image as Bitmap;
            if (bmp == null) return;

            FastBitmap bbb;
            bbb = new FastBitmap(bmp);
            int[,] r_mas;
            int[,] g_mas;
            int[,] b_mas;
            
            YCbCrColor yyy;
            int sh, vis,KOL;
           // bb.Dispose();
           // bbb = new FastBitmap(bitmap11);
            //int[][] mas = new int[6][];
            r_mas = new int[bitmap11.Width, bitmap11.Height];
            g_mas = new int[bitmap11.Width, bitmap11.Height];
            b_mas = new int[bitmap11.Width, bitmap11.Height];
            
            string strproba = string.Empty;
            using (System.IO.StreamReader reader = System.IO.File.OpenText(Application.StartupPath + "\\kol.txt"))
            {
                strproba = reader.ReadToEnd();
                reader.Close();
            }
            KOL = Int32.Parse(strproba);
            Color orig_mas;
            for (int i = 0; i < bitmap11.Width; i++)
            {
                for (int j = 0; j < bitmap11.Height; j++)
                {
                    orig_mas = bbb.GetColor(i, j);
                    r_mas[i, j] = orig_mas.R;
                    g_mas[i, j] = orig_mas.G;
                    b_mas[i, j] = orig_mas.B;
                }
            }
            List<Pix> students = new List<Pix>();
            List<Pix> students2 = new List<Pix>();
            SerializableObject obj = new SerializableObject();
            obj.Students = students;
            MySerializer serializer = new MySerializer();

            

            obj = serializer.DeserializeObject(Application.StartupPath + "\\ser.txt");
            students2 = obj.Students;
            sh = bitmap11.Width;
            vis = bitmap11.Height;
            int kkk;
                    //students.Add(w);
            for (int i = 0; i < sh; i++)
            {
                 for (int j = 0; j < vis; j++)
                    {
                        if (r_mas[i, j] < 20 && g_mas[i, j] < 20 && b_mas[i, j] < 20)
                        {

                            continue;
                        }
                        kkk = students2.FindIndex(
                               delegate(Pix bk)
                               {
                                   return (bk.rrr == r_mas[i, j] && bk.ggg == g_mas[i, j] && bk.bbb == b_mas[i, j]);
                               });
                        if (kkk != -1)
                        {
                            //if ((students2[kkk].kol / KOL) / (1-students2[kkk].kol / KOL) > 1)
                            bbb.SetColor(i, j, Color.White);
                        }
                        else
                        {
                            bbb.SetColor(i, j, Color.Black);
                        }

                    }
            }
            bbb.Dispose();




        }

        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
           
            
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            

        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            
        }

        private void добавитьВВыборкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] fullfilesPath;
            YCbCrColor yyy;

            FastBitmap bbb;
            int KOL, sh, vis;
            int[,] r_mas;
            int[,] g_mas;
            int[,] b_mas;
            Color orig_mas;
            PixCbCr w = new PixCbCr();

            PixCbCr w2 = new PixCbCr();


            w2.kol = 0;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                // листаем все файлы в выбранной папке
                foreach (string File in Directory.GetFiles(folderBrowserDialog1.SelectedPath))
                {
                    // показываем имя файла
                    bitmap11 = new Bitmap(Image.FromFile(File));
                    // MessageBox.Show(File);

                    //fullfilesPath = Directory.GetFiles(folderBrowserDialog1.SelectedPath); 


                    //  fullfilesPath = Directory.GetFiles(Application.StartupPath + "\\pr");  

                    /*if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        bitmap11 = new Bitmap(openFileDialog1.FileName);
                        else return;
                    }*/


                    bbb = new FastBitmap(bitmap11);

                    r_mas = new int[bitmap11.Width, bitmap11.Height];
                    g_mas = new int[bitmap11.Width, bitmap11.Height];
                    b_mas = new int[bitmap11.Width, bitmap11.Height];
                    sh = bitmap11.Width;
                    vis = bitmap11.Height;

                    for (int i = 0; i < bitmap11.Width; i++)
                    {
                        for (int j = 0; j < bitmap11.Height; j++)
                        {
                            orig_mas = bbb.GetColor(i, j);
                            r_mas[i, j] = orig_mas.R;
                            g_mas[i, j] = orig_mas.G;
                            b_mas[i, j] = orig_mas.B;
                        }
                    }
                    // Задаем экземпляр класса.

                    w.cb = 0;
                    w.cr = 0;

                    w.kol = 0;

                    w2.cb = 0;
                    w2.cr = 0;

                    w2.kol = 0;

                    // Сериализуем класс.
                    /*FileStream fs = new FileStream(Application.StartupPath + "\\ser.txt", FileMode.Open, FileAccess.Write);
                    IFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, w);
                    fs.Close();

                    // Десериализуем класс.
                    fs = new FileStream(Application.StartupPath + "\\ser.txt", FileMode.Open, FileAccess.Read);
                    pixx w1 = (pixx)bf.Deserialize(fs);
                    textBox1.Text = ("age: " + w1.rrr + ", yoe: " + w1.ggg);
                    fs.Close(); */


                    List<PixCbCr> students = new List<PixCbCr>();
                    List<PixCbCr> students2 = new List<PixCbCr>();
                    SerializableObject2 obj = new SerializableObject2();
                    obj.Students = students;
                    MySerializer2 serializer = new MySerializer2();
                    /* students.Add(w);


            
                     serializer.SerializeObject(Application.StartupPath + "\\ser.txt", obj);*/

                    obj = serializer.DeserializeObject2(Application.StartupPath + "\\ser.txt");
                    students2 = obj.Students;
                    //textBox1.Text = students2[0].rrr +"";
                    int kkk;
                    KOL = 0;
                    //students.Add(w);
                    for (int i = 0; i < sh; i++)
                    {
                        for (int j = 0; j < vis; j++)
                        {
                            if (r_mas[i, j] < 20 || g_mas[i, j] < 20 || b_mas[i, j] < 20)
                            {

                                continue;
                            }
                            if (r_mas[i, j] > 230 || g_mas[i, j] > 230 || b_mas[i, j] > 230)
                            {

                                continue;
                            }
                            KOL = KOL + 1;
                            Color origg = Color.FromArgb(r_mas[i, j], g_mas[i, j], b_mas[i, j]);
                            yyy = YCbCrColor.FromRgbColor(origg);
                            kkk = students2.FindIndex(
                            delegate(PixCbCr bk)
                            {
                                return (bk.cb == yyy.Cb && bk.cr == yyy.Cr);
                            });
                            if (kkk != -1)
                            {
                                students2[kkk].kol = students2[kkk].kol + 1;
                            }
                            else
                            {
                                //w.rrr = r_mas[i, j];
                                //w.ggg = g_mas[i, j];
                                //w.bbb = b_mas[i, j];
                                w.kol = 1;

                                //students2.Add(w);
                                students2.Add(new PixCbCr() { cb = yyy.Cb, cr = yyy.Cr, kol = 1 });
                                students2.IndexOf(w2);
                            }
                        }
                    }
                    //MessageBox.Show("Данные добавлены в выборку", "Процесс завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    obj.Students = students2;
                    serializer.SerializeObject2(Application.StartupPath + "\\ser.txt", obj);
                    obj = serializer.DeserializeObject2(Application.StartupPath + "\\ser.txt");
                    students2 = obj.Students;
                    /* students2.ForEach(delegate(Pix bk)
                     {
                         textBox1.Text = textBox1.Text + bk.rrr+" ";
                     });*/
                    string strproba = string.Empty;
                    using (System.IO.StreamReader reader = System.IO.File.OpenText(Application.StartupPath + "\\kol.txt"))
                    {
                        strproba = reader.ReadToEnd();
                        reader.Close();
                    }
                    string[] proba2 = new string[50];


                    string s1, s2;
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\kol.txt"))
                    {

                        s2 = strproba;
                        s1 = (Int32.Parse(s2) + KOL + "\r");


                        int p = strproba.IndexOf(s2);
                        if (p != -1)
                        {
                            strproba = strproba.Remove(p, s2.Length).Insert(p, s1);
                        }
                        file.Write(strproba);
                        //MessageBox.Show("Данные занесены в базу", "Процесс завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }


                }
                MessageBox.Show("Данные занесены в базу", "Процесс завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void удалитьВыборкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить данную запить?", "Удаление записи",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                PixCbCr w = new PixCbCr();
                w.cb = 0;
                w.cr = 0;

                w.kol = 0;
                List<PixCbCr> students = new List<PixCbCr>();

                SerializableObject2 obj = new SerializableObject2();
                obj.Students = students;
                MySerializer2 serializer = new MySerializer2();
                students.Add(w);
                serializer.SerializeObject2(Application.StartupPath + "\\ser.txt", obj);
                //textBox1.Text = students[0].rrr + " " + students[0].ggg + " " + students[0].bbb + " " + students[0].kol + " ";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\kol.txt"))
                {

                    file.Write("0");
                    MessageBox.Show("Данные удалены", "Процесс завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void лицаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bmp = picture1.Image as Bitmap;
            if (bmp == null) return;
            Image<Bgr, Byte> image = new Image<Bgr, Byte>(imageBox1.Image.Bitmap);
            // Image<Bgr, Byte> image = picture1;
            Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>();
            FastBitmap bb;
            bb = new FastBitmap(bmp);
            int hh, ww, hh2, ww2, kol, KOL;
            int kol2 = 0;
            double xz;
            HaarCascade Cascade1 = new HaarCascade("haarcascade_frontalface_alt2.xml");
            //imageBox1.Image = picture1.Image;
            //CascadeClassifier Cascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            //CascadeClassifier CascadeEye = new CascadeClassifier("haarcascade_eye.xml");
            //Берем кадр и делаем его из цветного в серый

            //Ищем признаки лица
            //Rectangle[] facesDetected = Cascade.DetectMultiScale(grayImage, 1.2, 10, new Size(50, 50), Size.Empty);
            //Cascade.DetectMultiScale(grayImage, 0.1, 1, 1, 20);
            var Faces = grayImage.DetectHaarCascade(Cascade1)[0];
            foreach (var face in Faces)
            {

                //Eсли есть - обводим его. Первый аргумент - координаты, второй - цвет линии, третий - толщина
                image.Draw(face.rect, new Bgr(0, 255, 255), 10);
                kol2 = kol2 + 1;
                toolStripButton4.Enabled = true;

            }
            bb.Dispose();
            //Ищем признаки глаз
            /*var Eyes = grayImage.DetectHaarCascade(CascadeEye)[0];
            foreach (var eye in Eyes)
            {
                //Обводим
                image.Draw(eye.rect, new Bgr(0, 0, 255), 3);
            }*/
            if (kol2 == 0)
                MessageBox.Show("На изображении не обнаружено лиц", "Обработка завершена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            //Выводим обработаное приложение
            imageBox1.Image = image;
            //picture1.Image = imageBox1.Image;
        }

        private void рукиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bmp = picture1.Image as Bitmap;
            if (bmp == null) return;
            int kol = 0;
            HaarCascade Cascade1 = new HaarCascade("hc_hand.xml");
            //imageBox1.Image = picture1.Image;
            //CascadeClassifier Cascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            //CascadeClassifier CascadeEye = new CascadeClassifier("haarcascade_eye.xml");
            //Берем кадр и делаем его из цветного в серый
            Image<Bgr, Byte> image = new Image<Bgr, Byte>(imageBox1.Image.Bitmap);
            // Image<Bgr, Byte> image = picture1;
            Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>();
            //Ищем признаки лица
            //Rectangle[] facesDetected = Cascade.DetectMultiScale(grayImage, 1.2, 10, new Size(50, 50), Size.Empty);
            //Cascade.DetectMultiScale(grayImage, 0.1, 1, 1, 20);
            var Faces = grayImage.DetectHaarCascade(Cascade1)[0];
            foreach (var face in Faces)
            {
                //Eсли есть - обводим его. Первый аргумент - координаты, второй - цвет линии, третий - толщина
                image.Draw(face.rect, new Bgr(255, 0, 0), 7);
                kol = kol + 1;
                toolStripButton4.Enabled = true;
            }
            //Ищем признаки глаз
            /*var Eyes = grayImage.DetectHaarCascade(CascadeEye)[0];
            foreach (var eye in Eyes)
            {
                //Обводим
                image.Draw(eye.rect, new Bgr(0, 0, 255), 3);
            }*/
            if (kol == 0)
                MessageBox.Show("На изображении не обнаружено рук", "Обработка завершена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            //Выводим обработаное приложение
            imageBox1.Image = image;
            //picture1.Image = imageBox1.Image;
        }

        private void лицаСПроверкойНаКожуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bmp = picture1.Image as Bitmap;
            if (bmp == null) return;
            Image<Bgr, Byte> image = new Image<Bgr, Byte>(imageBox1.Image.Bitmap);
            // Image<Bgr, Byte> image = picture1;
            Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>();
            FastBitmap bb;
            bb = new FastBitmap(bmp);
            int hh, ww, hh2, ww2, kol, KOL;
            double xz;
            int kol2 = 0;
            HaarCascade Cascade1 = new HaarCascade("haarcascade_frontalface_alt.xml");
            //imageBox1.Image = picture1.Image;
            //CascadeClassifier Cascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            //CascadeClassifier CascadeEye = new CascadeClassifier("haarcascade_eye.xml");
            //Берем кадр и делаем его из цветного в серый

            //Ищем признаки лица
            //Rectangle[] facesDetected = Cascade.DetectMultiScale(grayImage, 1.2, 10, new Size(50, 50), Size.Empty);
            //Cascade.DetectMultiScale(grayImage, 0.1, 1, 1, 20);
            var Faces = grayImage.DetectHaarCascade(Cascade1)[0];
            foreach (var face in Faces)
            {
                hh = face.rect.Y;
                hh2 = hh + face.rect.Height;
                ww = face.rect.X;
                ww2 = ww + face.rect.Width;
                KOL = face.rect.Height * face.rect.Width;
                kol = 0;
                for (int i = ww; i < ww2; i++)
                {
                    for (int j = hh; j < hh2; j++)
                    {
                        Color original = bb.GetColor(i, j);
                        //original = {Name=ff3278c8, ARGB=(255, 50, 120, 200)}
                        int max = Math.Max(original.R, Math.Max(original.G, original.B));
                        int min = Math.Min(original.R, Math.Min(original.G, original.B));


                        double hue, hue2;
                        double saturation, saturation2;
                        double value, lightness;
                        //hsl тон, насыщенность и светлота. 
                        //hsv тон, насыщенность и яркость. 

                        ColorToHSV(original, out hue, out saturation, out value);
                        /* if ((hue >= hMin && hue <= hMax || hue >= hMin2 && hue <= hMax2) && saturation >= sMin && saturation <= sMax)
                         {
                             bb.SetColor(i, j, Color.White);
                         }
                         else bb.SetColor(i, j, Color.Black);*/
                        //HSLColor hslColor1 = new HSLColor(original);
                        // hue = hslColor1.Hue;
                        hue2 = original.GetHue();
                        saturation2 = original.GetSaturation();
                        lightness = original.GetBrightness();
                        /*if ((hue2 >= hMin && hue2 <= hMax || hue2 >= hMin2 && hue2 <= hMax2) && saturation2 >= sMin && saturation2 <= sMax)
                        {
                            bb.SetColor(i, j, Color.White);
                        }
                        else bb.SetColor(i, j, Color.Black);*/
                        //bb.SetColor(i, j, Color.Blue);
                        // bb.Dispose();
                        YCbCrColor yy;
                        yy = YCbCrColor.FromRgbColor(original);
                        // if (yy.Cr>=139 && yy.Cr<188 && yy.Cb>=104 && yy.Cb<130 && yy.Y>=26 && yy.Y<222)
                        // if (yy.Cr>=138 && yy.Cr<178 && (yy.Cb + 0.6*yy.Cr)>199 && (yy.Cb + 0.6*yy.Cr)<215)
                        // if (yy.Cr >= 135 && yy.Cr < 180 && yy.Cb >= 90 && yy.Cb < 135 && yy.Y >= 80 && yy.Y < 210)
                        if (original.R > 95 && original.G > 40 && original.B > 20 && (max - min) > 15 && Math.Abs(original.R - original.G) > 15
                                          && original.R > original.G && original.R > original.B && yy.Cr >= 135 && yy.Cr < 170 && yy.Cb >= 90 && yy.Cb < 115)
                        // if (yy.Cr >= 135 && yy.Cr < 170 && yy.Cb >= 90 && yy.Cb < 115)
                        {
                            kol = kol + 1;
                        }
                        //else KOL = KOL + 1;

                    }
                }
                xz = (double)kol / (double)KOL;
                if (xz > 0.2)
                {
                    //Eсли есть - обводим его. Первый аргумент - координаты, второй - цвет линии, третий - толщина
                    image.Draw(face.rect, new Bgr(0, 255, 255), 10);
                    kol2 = kol2 + 1;
                    toolStripButton4.Enabled = true;
                }

            }
            bb.Dispose();
            //Ищем признаки глаз
            /*var Eyes = grayImage.DetectHaarCascade(CascadeEye)[0];
            foreach (var eye in Eyes)
            {
                //Обводим
                image.Draw(eye.rect, new Bgr(0, 0, 255), 3);
            }*/
            if (kol2 == 0)
                MessageBox.Show("На изображении не обнаружено лиц", "Обработка завершена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            //Выводим обработаное приложение
            imageBox1.Image = image;
            //picture1.Image = imageBox1.Image;
        }

        private void рукиСПроверкойНаКожуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bmp = picture1.Image as Bitmap;
            if (bmp == null) return;
            HaarCascade Cascade1 = new HaarCascade("hc_hand.xml");
            int hh, ww, hh2, ww2, kol, KOL;
            double xz;
            int kol2 = 0;
            //imageBox1.Image = picture1.Image;
            //CascadeClassifier Cascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            //CascadeClassifier CascadeEye = new CascadeClassifier("haarcascade_eye.xml");
            //Берем кадр и делаем его из цветного в серый
            Image<Bgr, Byte> image = new Image<Bgr, Byte>(imageBox1.Image.Bitmap);
            // Image<Bgr, Byte> image = picture1;
            Image<Gray, Byte> grayImage = image.Convert<Gray, Byte>();
            //Ищем признаки лица
            //Rectangle[] facesDetected = Cascade.DetectMultiScale(grayImage, 1.2, 10, new Size(50, 50), Size.Empty);
            //Cascade.DetectMultiScale(grayImage, 0.1, 1, 1, 20);
            var Faces = grayImage.DetectHaarCascade(Cascade1)[0];
            FastBitmap bb;
            bb = new FastBitmap(bmp);
            foreach (var face in Faces)
            {
                hh = face.rect.Y;
                hh2 = hh + face.rect.Height;
                ww = face.rect.X;
                ww2 = ww + face.rect.Width;
                KOL = face.rect.Height * face.rect.Width;
                kol = 0;
                for (int i = ww; i < ww2; i++)
                {
                    for (int j = hh; j < hh2; j++)
                    {
                        Color original = bb.GetColor(i, j);
                        //original = {Name=ff3278c8, ARGB=(255, 50, 120, 200)}
                        int max = Math.Max(original.R, Math.Max(original.G, original.B));
                        int min = Math.Min(original.R, Math.Min(original.G, original.B));


                        double hue, hue2;
                        double saturation, saturation2;
                        double value, lightness;
                        //hsl тон, насыщенность и светлота. 
                        //hsv тон, насыщенность и яркость. 

                        ColorToHSV(original, out hue, out saturation, out value);
                        /* if ((hue >= hMin && hue <= hMax || hue >= hMin2 && hue <= hMax2) && saturation >= sMin && saturation <= sMax)
                         {
                             bb.SetColor(i, j, Color.White);
                         }
                         else bb.SetColor(i, j, Color.Black);*/
                        //HSLColor hslColor1 = new HSLColor(original);
                        // hue = hslColor1.Hue;
                        hue2 = original.GetHue();
                        saturation2 = original.GetSaturation();
                        lightness = original.GetBrightness();
                        /*if ((hue2 >= hMin && hue2 <= hMax || hue2 >= hMin2 && hue2 <= hMax2) && saturation2 >= sMin && saturation2 <= sMax)
                        {
                            bb.SetColor(i, j, Color.White);
                        }
                        else bb.SetColor(i, j, Color.Black);*/
                        //bb.SetColor(i, j, Color.Blue);
                        // bb.Dispose();
                        YCbCrColor yy;
                        yy = YCbCrColor.FromRgbColor(original);
                        // if (yy.Cr>=139 && yy.Cr<188 && yy.Cb>=104 && yy.Cb<130 && yy.Y>=26 && yy.Y<222)
                        // if (yy.Cr>=138 && yy.Cr<178 && (yy.Cb + 0.6*yy.Cr)>199 && (yy.Cb + 0.6*yy.Cr)<215)
                        // if (yy.Cr >= 135 && yy.Cr < 180 && yy.Cb >= 90 && yy.Cb < 135 && yy.Y >= 80 && yy.Y < 210)
                        if (original.R > 95 && original.G > 40 && original.B > 20 && (max - min) > 15 && Math.Abs(original.R - original.G) > 15
                                          && original.R > original.G && original.R > original.B && yy.Cr >= 135 && yy.Cr < 170 && yy.Cb >= 90 && yy.Cb < 115)
                        // if (yy.Cr >= 135 && yy.Cr < 170 && yy.Cb >= 90 && yy.Cb < 115)
                        {
                            kol = kol + 1;
                        }
                        //else KOL = KOL + 1;
                    }
                }
                xz = (double)kol / (double)KOL;
                if (xz > 0.2)
                {
                    //Eсли есть - обводим его. Первый аргумент - координаты, второй - цвет линии, третий - толщина
                    image.Draw(face.rect, new Bgr(0, 255, 255), 10);
                    kol2 = kol2 + 1;
                    toolStripButton4.Enabled = true;
                }


            }

            bb.Dispose();
            //Выводим обработаное приложение
            if (kol2 == 0)
                MessageBox.Show("На изображении не обнаружено рук", "Обработка завершена!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            imageBox1.Image = image;
            //picture1.Image = imageBox1.Image;
        }

        private void toolStripButton3_Click_2(object sender, EventArgs e)
        {
            
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "bmp";
            sfd.Filter = "Image files (*.bmp;*.jpg;*.PNG)|*.bmp;*.jpg;*.PNG|All files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
                
                // bmpSave.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                imageBox1.Image.Save(sfd.FileName);
        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            bitmap1 = new Bitmap(full_name);
            Image<Bgr, Byte> image = new Image<Bgr, Byte>(bitmap1);
            imageBox1.Image = image;

            picture1.Image = bitmap1;
            picture1.Visible = false;
        }

    
    }
}
