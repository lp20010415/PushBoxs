using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PushBoxs
{

    public delegate void ChangeEnabled();//委托-修改Enabled
    public partial class Form2 : Form
    {
        public static ChangeEnabled change;//委托-修改Enabled
        public static bool enabled;//委托-修改Enabled
        public Form2()
        {
            InitializeComponent();
            change = new ChangeEnabled(ChangeShow);//委托-修改Enabled
        }

        //窗口是否可用
        public void ChangeShow() {
            Enabled = enabled;
        }

        //登录
        private void button1_Click(object sender, EventArgs e)
        {
            String sqltext = "Server=.;database=c#Traning report;uid=sa;pwd=123456";
            bool check = false;
            try
            {
                SqlConnection conn = new SqlConnection(sqltext);
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "select * from UserData";
                SqlDataReader sqlreader = comm.ExecuteReader();
                while (sqlreader.Read())
                {
                    if (sqlreader["account"].ToString() == textBox1.Text && sqlreader["password"].ToString() == textBox2.Text)
                    {
                        check = true;
                    }
                }
                if (check)
                {
                    MessageBox.Show("登录成功！", "提示");
                    Form1.LoginName = textBox1.Text;
                    Form1.enabled = true;
                    Form1.change();
                    Close();
                }
                else
                {
                    MessageBox.Show("账号或密码错误！", "提示", MessageBoxButtons.OK , MessageBoxIcon.Information);
                }
                conn.Close();
                sqlreader.Close();
            }
            catch (MySqlException e1)
            {
                MessageBox.Show("您可能没有联网！可以考虑游玩离线模式！");
            }
            catch(Exception ex)
            {
                MessageBox.Show("出错原因:"+ex.Message);
            }
        }

        //进入"注册"界面
        private void label3_Click(object sender, EventArgs e)
        {
            new Form3().Show();
            Enabled = false;
        }

        //进入"忘记密码"界面
        private void label4_Click(object sender, EventArgs e)
        {
            new Form4().Show();
            Enabled = false;
        }

        //关闭玩的界面
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!Form1.enabled)
            {
                Form1.close();
            }
        }

        //按"Enter"登录
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(button1, e);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否进入离线模式？(有些功能无法使用)", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {

                Form1.enabled = true;
                Form1.change();
                Close();
                Form1.enterOfflineMode();
            }
        }
    }
}
