using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shichang
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            //chart1.Series.
            DataTable dt = default(DataTable);
            dt = CreateDataTable();

            //设置图表的数据源
            chart1.DataSource = dt;

            //设置图表Y轴对应项
            chart1.Series[0].YValueMembers = "Volume1";
            //chart1.Series[1].YValueMembers = "Volume2";

            //设置图表X轴对应项
            chart1.Series[0].XValueMember = "Date";

            //绑定数据
            chart1.DataBind();
            
        }
        private DataTable CreateDataTable()
        {
            richTextBox1.Text = "";
            DataTable dt = new DataTable();
            dt.Columns.Add("Date");
            dt.Columns.Add("Volume1");
            DataRow dr;

            int A = Convert.ToInt32(textBox1.Text);//均值
            int B = Convert.ToInt32(textBox2.Text);//振幅
            int T = Convert.ToInt32(textBox3.Text);//周期
            int P = Convert.ToInt32(textBox4.Text);//偏移
            int Biaocz = Convert.ToInt32(textBox5.Text); //标准差         
            int TS = Convert.ToInt32(textBox6.Text);//突发事件持续时长
            int TF = Convert.ToInt32(textBox7.Text);//突发振幅
            int TG = Convert.ToInt32(textBox8.Text);//突发概率
            int TFBC = Convert.ToInt32(textBox10.Text);//突发概率的标准差

            int Tian = Convert.ToInt32(textBox9.Text);//生成多少天数
           
            GaussianRNG g = new GaussianRNG();

            //突发需求的四个临时变量（如果需求是一天一天求的话要将这四个保存到数据库中，如果需求是一下子求个几百年存到数据库的话，这四个变量无需存）
            bool flag = false;//今天在突发中吗
            int tt = 0;//突发第几天
            double  tx = 0;//第几天的振幅
            double tfbc = 0;//最大振幅


            Random ran = new Random(unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < Tian; i++) {
                if (flag == false) {
                    tx = 0;

                    int RandKey = ran.Next(0, 100);
                    
                    if (RandKey < TG)
                    {
                        
                        tfbc =Convert.ToInt32(TF + g.Next() * TFBC+0.5);
                        flag = true;
                        tt = 0;
                        richTextBox1.Text += "第" + i + "天，概率值:" + RandKey.ToString() + ",最大振幅为："+ tfbc+",";
                    }
                    else tx = 0;
                }
                if (flag == true) {
                    tx = Math.Floor(tfbc * Math.Sin(Math.PI/TS * tt)+0.5);
                    richTextBox1.Text += "tx:" + tx.ToString() + ";";
                    tt++;
                    if (tt == TS) {
                        richTextBox1.Text += "\n";
                        flag = false;
                        //tx = 0;
                        tt = 0;
                        tfbc = 0;
                    }
                }
                dr = dt.NewRow();               
                dr["Date"] = i+1;
                dr["Volume1"] = A+B*Math.Sin(2*Math.PI/T*(i+P))+ Math.Floor(g.Next() * Biaocz + 0.5)+tx;
               
                dt.Rows.Add(dr);
            }
                      
           
            return dt;
        }
        public class GaussianRNG
        {
            int iset;
            double gset;
            Random r1, r2;
            public GaussianRNG()
            {
                r1 = new Random(unchecked((int)DateTime.Now.Ticks));
                r2 = new Random(~unchecked((int)DateTime.Now.Ticks));
                iset = 0;
            }
            public double Next()
            {
                double fac, rsq, v1, v2;
                if (iset == 0)
                {
                    do
                    {
                        v1 = 2.0 * r1.NextDouble() - 1.0;
                        v2 = 2.0 * r2.NextDouble() - 1.0;
                        rsq = v1 * v1 + v2 * v2;
                    } while (rsq >= 1.0 || rsq == 0.0);
                    fac = Math.Sqrt(-2.0 * Math.Log(rsq) / rsq);
                    gset = v1 * fac; iset = 1;
                    return v2 * fac;
                }
                else { iset = 0; return gset; }
            }
        }
    }
}
