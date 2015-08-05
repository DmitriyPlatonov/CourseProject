using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace course_project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static double MyPow(double x, double n) // Возведение в степень
        {
            if (x >= 0)
                return Math.Pow(x, n);
            else
                return -Math.Pow(Math.Abs(x), n);
        }
        static public void print(string s) // Печать ошибки
        {
            MessageBox.Show(s, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        static bool is_delta_plus(double[] mas) // Проверка оценок на неотрицательность
        {
            bool flag = false;
            for (int i = 0; i < mas.Length; i++)
            {
                if (mas[i] >= 0)
                    flag = true;
                else
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }
        static int min_index(double[] mas) // Поиск индекса минимального элемента массива
        {
            double min_elem = mas[0];
            int k = 0;
            for (int i = 0; i < mas.Length; i++)
                if (mas[i] < min_elem)
                {
                    min_elem = mas[i];
                    k = i;
                }
            return k;
        }
        static double[] Simplex(int I, int J, double[,] M, int[] B, double[] Cb, double[] Xb, double[] Cel_func, double free) // Симплекс - метод
        {
            double[] X_opt = new double[J];
            for (int j = 0; j < J; j++)
                X_opt[j] = 0;

            double[] delta = new double[J];
            bool f = false;
            double[] Q = new double[I];
            double cel_val = 0;
            int k = 0;

            while ((f == false)&&(k<30))
            {
                k++;
                if (k == 30)
                {
                    print("Симплекс - задача не имеет допустимого решения.");
                    throw new System.Exception();
                }
                for (int i = 0; i < J; i++)
                    delta[i] = 0;

                for (int i = 0; i < J; i++)
                {
                    for (int j = 0; j < I; j++)
                        delta[i] += Cb[j] * M[j, i];
                    delta[i] = delta[i] - Cel_func[i];
                }

                if (is_delta_plus(delta) == true)
                {
                    for (int i = 0; i < I; i++)
                        X_opt[B[i]] = Xb[i];
                    for (int i = 0; i < J; i++)
                        cel_val += X_opt[i] * Cel_func[i];
                    cel_val = cel_val + free;
                    f = true;
                }
                // в противном случае преобразовываем таблицу
                else
                {
                    int var_in = min_index(delta); // НОМЕР переменной, входящей в базис

                    for (int i = 0; i < I; i++)
                        if (M[i, var_in] <= 0)
                        {
                            if (i == I - 1)
                            {
                                print("Целевая функция неограниченна.");
                                throw new System.Exception();
                            }
                        }
                        else break;

                    for (int i = 0; i < I; i++)
                    {
                        if (M[i, var_in] != 0)
                        {
                            Q[i] = Xb[i] / M[i, var_in];
                            if (Q[i] < 0) Q[i] = 1000000;
                        }
                        else Q[i] = 100000;
                    }

                    int var_out = min_index(Q); // ИНДЕКС переменной, выходящей из базиса - B[var_out]
                    B[var_out] = var_in; // замена переменной в базисе на новую
                    Cb[var_out] = Cel_func[var_in];

                    // Жордановы преобразования:
                    double mn1 = M[var_out, var_in];
                    for (int j = 0; j < J; j++)
                        M[var_out, j] = M[var_out, j] / mn1;
                    Xb[var_out] = Xb[var_out] / mn1;

                    for (int i = 0; i < I; i++)
                    {
                        double mn2 = M[i, var_in];
                        for (int j = 0; j < J; j++)
                            if (i != var_out)
                                M[i, j] = M[i, j] - M[var_out, j] * mn2;

                        if (i != var_out)
                            Xb[i] = Xb[i] - Xb[var_out] * mn2;
                    }
                }
            }
            return X_opt;
        }
        static double[] ArtificialSimplex(int I, int J, double[,] M, int[] B, double[] Cb, double[] Xb, double[] Cel_func, double free) // Симплекс - метод с искусственным базисом
        {
            double[] X_opt = new double[J];
            for (int j = 0; j < J; j++)
                X_opt[j] = 0;

            double[] delta = new double[J];
            bool f = false;
            double[] Q = new double[I];
            int k = 0;

            while ((f == false)&&(k<30))
            {
                k++;
                if (k == 30)
                {
                    print("Симплекс - задача не имеет допустимого решения.");
                    throw new System.Exception();
                }

                for (int i = 0; i < J; i++)
                    delta[i] = 0;

                for (int i = 0; i < J; i++)
                    for (int j = 0; j < I; j++)
                        delta[i] += Cb[j] * M[j, i];

                double cel_val = 0;
                for (int i = 0; i < I; i++)
                    cel_val += Cb[i] * Xb[i];
                if (cel_val == 0)
                {
                    if (is_delta_plus(delta) == true)
                    {
                        f = true;
                        for (int i = 0; i < I; i++)
                            if (B[i] == 100)
                            {
                                print("Симплекс - задача не имеет решения ввиду невозможности исключения искусственных переменных.");
                                throw new System.Exception();
                            }
                        X_opt = Simplex(I, J, M, B, Cb, Xb, Cel_func, free);
                    }
                }

                else
                {
                    int var_in = min_index(delta); // НОМЕР переменной, входящей в базис
                    for (int i = 0; i < I; i++)
                        if (M[i, var_in] <= 0)
                        {
                            if (i == I - 1)
                            {
                                print("Целевая функция неограниченна");
                                throw new System.Exception();
                            }
                        }
                        else break;

                    for (int i = 0; i < I; i++)
                    {
                        if (M[i, var_in] != 0)
                        {
                            Q[i] = Xb[i] / M[i, var_in];
                            if (Q[i] < 0) Q[i] = 10000000;
                        }
                    }

                    int var_out = min_index(Q); // ИНДЕКС переменной, выходящей из базиса - B[var_out]
                    B[var_out] = var_in; // замена переменной в базисе на новую
                    Cb[var_out] = 0;

                    // Жордановы преобразования:
                    double mn1 = M[var_out, var_in];
                    for (int j = 0; j < J; j++)
                        M[var_out, j] = M[var_out, j] / mn1;
                    Xb[var_out] = Xb[var_out] / mn1;

                    for (int i = 0; i < I; i++)
                    {
                        double mn2 = M[i, var_in];
                        for (int j = 0; j < J; j++)
                            if (i != var_out)
                                M[i, j] = M[i, j] - M[var_out, j] * mn2;

                        if (i != var_out)
                            Xb[i] = Xb[i] - Xb[var_out] * mn2;
                    }
                }
            }
            return X_opt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double gamma, omega, omega_s, ro, pi, Ebj, L, K, N, F, Y1, Y2, B1, B2, V1, V2;
            try
            {
                gamma = Convert.ToDouble(textBox1.Text);
                omega = Convert.ToDouble(textBox2.Text);
                omega_s = Convert.ToDouble(textBox3.Text);
                ro = Convert.ToDouble(textBox4.Text);
                pi = Convert.ToDouble(textBox5.Text);
                Ebj = Convert.ToDouble(textBox6.Text);
                L = Convert.ToDouble(textBox7.Text);
                K = Convert.ToDouble(textBox8.Text);
                N = Convert.ToDouble(textBox9.Text);
                F = Convert.ToDouble(textBox10.Text);
            }
            catch (System.Exception)
            {
                print("Не заполнены вся поля, либо заполнены неверно.");
                return;
            }
            Y1 = Form3.Y1;
            Y2 = Form3.Y2;
            V1 = Form3.V1;
            V2 = Form3.V2;
            B1 = Form3.B1;
            B2 = Form3.B2;
            if ((gamma < 0) || (omega < 0) || (omega_s < 0) || (ro < 0) || (pi < 0) || (Ebj < 0) || (L < 0) || (K < 0) || (N < 0) || (F < 0) || (Y1 < 0) || (Y2 < 0) || (B1 < 0) || (B2 < 0) || (V1 < 0) || (V2 < 0))
            {
                print("Вводимые данные должны принимать только положительные значения.");
                return;
            }
            if ((omega == 0) || (omega_s == 0))
            {
                print("Размер средней номинальной з/п, или затраты на создание рабочего места не могут равняться 0.");
                return;
            }
            if ((ro == 0) || (ro == 1))
            {
                print("Ставка налога на фонд оплаты труда не может принимать значение 0 или 1.");
                return;
            }
            if (L>N)
            {
                print("Требуемое количество ресурсов не может превышать численность незанятого населения.");
                return;
            }

            // коэффициенты функции Кобба-Дугласа
            double alfa = 0.6;
            double beta = 0.4;

            // целевая функция
            double y_coef = 1 - Ebj;
            double v_coef = -omega * F / ((1 - ro) * omega_s);
            double b_coef = -F * gamma;
            double free = (-omega * L / (1 - ro)) - (gamma * (1 - gamma) * K);

            // произвольная точка x0 и коэффициенты при переменных
            double[] x_nul = new double[] { 5,4,3 };
            double[] y = new double[12] {0,0,0,0,0,0,0,0,0,0,0,0};
            double[] v = new double[12] { 0, 0, 0, 0, 0, 0, 0, 0,0,0,0,0 };
            double[] b = new double[12] { 0, 0, 0, 0, 0, 0, 0, 0,0,0,0,0 };
            double[] ffree = new double[12] { 0, 0, 0, 0, 0, 0, 0, 0,0,0,0,0 };

            v[0] = 1;
            ffree[0] = (N - L) * omega_s / F;
            y[1] = 1;
            v[1] = (-0.4 * F * omega * MyPow((x_nul[2] * F - gamma * K + K), alfa)) / (omega_s * MyPow((omega * (F * x_nul[1] / omega_s + L)), alfa));
            b[1] = (-0.6 * F * MyPow(omega * (F * x_nul[1] / omega_s + L), beta)) / MyPow((x_nul[2] * F - gamma * K + K), beta);
            ffree[1] = -((x_nul[0] - MyPow(((1 - gamma) * K + F * x_nul[2]), alfa) * MyPow((omega * L + omega * F * x_nul[1] / omega_s), beta)) - x_nul[0] * y[1] - x_nul[1] * v[1] - x_nul[2] * b[1]);
            y[2] = Ebj - 1;
            v[2] = (F * omega) / (omega_s - ro * omega_s);
            b[2] = F * gamma;
            ffree[2] = -pi - (-x_nul[0] * (1 - Ebj) + (omega / (1 - ro)) * (L + F * x_nul[1] / omega_s) + gamma * ((1 - gamma) * K + x_nul[2] * F) - x_nul[0] * y[2] - x_nul[1] * v[2] - x_nul[2] * b[2]);
            v[3] = 1;
            b[3] = 1;
            ffree[3] = 1;
            y[4] = -1;
            ffree[4] = -Y1;
            y[5] = 1;
            ffree[5] = Y2;
            b[6] = 1;
            ffree[6] = 1;
            v[7] = 1;
            ffree[7] = 1;
            b[8] = -1;
            ffree[8] = -B1;
            b[9] = 1;
            ffree[9] = B2;
            v[10] = -1;
            ffree[10] = -V1;
            v[11] = 1;
            ffree[11] = V2;

            // симплекс-метод:
            const int I = 12; // число строк (ограничений)
            const int J = 15; // число столбцов (переменных)
            double[,] M = new double[I, J]; // симплекс-таблица
            for (int i = 0; i < I; i++)
                for (int j = 0; j < J; j++)
                {
                    if (j == 0) M[i, j] = y[i];
                    if (j == 1) M[i, j] = v[i];
                    if (j == 2) M[i, j] = b[i];
                    if (j > 2)
                    {
                        if (i == j - 3) M[i, j] = 1;
                        else M[i, j] = 0;
                    }
                }
            double[] Cb = new double[I];
            double[] Xb = new double[I];
            double[] solution = new double[J];
            int[] B = new int[I] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            for (int i = 0; i < I; i++)
            {
                Xb[i] = ffree[i];
                Cb[i] = 0;
            }
            double[] X_opt = new double[J] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            double[] Cel_func = new double[J] { y_coef, v_coef, b_coef,0,0,0,0,0,0,0,0,0,0,0,0};
            
            bool myflag = true;
            for (int i = 0; i < ffree.Length; i++) // приводим к каноническому виду, если требуется
                if (Xb[i] < 0)
                {
                    Xb[i] = -Xb[i];
                    for (int j = 0; j < J; j++)
                    M[i,j] = -M[i,j];
                    B[i] = 100;
                    Cb[i] = -1;
                    myflag = false;
                }

            try
            {
                if (myflag == true)
                    solution = Simplex(I, J, M, B, Cb, Xb, Cel_func, free);
                else
                    solution = ArtificialSimplex(I, J, M, B, Cb, Xb, Cel_func, free);
            }
            catch (Exception)
            {
                return;
            }
            double celznach = 0;
            for (int i = 0; i < I; i++)
                celznach += Cel_func[i] * solution[i];
            celznach = celznach + free;

            label11.Text = "y = " + (Math.Round(solution[0],4)).ToString("R");
            label12.Text = "δ = " + (Math.Round(solution[2],4)).ToString("R");
            label13.Text = "ν = " + (Math.Round(solution[1],4)).ToString("R");
            label14.Text = "G = " + (Math.Round(celznach,4)).ToString("R");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string mytext = String.Empty;
            string[] ss = new string[10];
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    mytext = text;
                }
                catch (IOException)
                {
                }
                MatchCollection collection = Regex.Matches(mytext, @"\d*\,?\d+");
                int i = 0;
                foreach (Match match in collection)
                {
                    try
                    {
                    ss[i] = match.Value;
                    i++;
                    }
                    catch (System.Exception)
                    {
                        print("Ошибка считывания из файла, проверьте правильность ввода.");
                        return;
                    }
                }
                textBox1.Text = ss[0]; textBox2.Text = ss[1]; textBox3.Text = ss[2]; textBox4.Text = ss[3]; textBox5.Text = ss[4];
                textBox6.Text = ss[5]; textBox7.Text = ss[6]; textBox8.Text = ss[7]; textBox9.Text = ss[8]; textBox10.Text = ss[9];
            }
        }
    }
}