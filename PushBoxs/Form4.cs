using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PushBoxs
{
    public partial class Form4 : Form
    {
        bool CheckPassword,CheckAgainPassword = false;
        public Form4()
        {
            InitializeComponent();
        }

        //校验码文本提示
        private void label2_MouseEnter(object sender, EventArgs e)
        {
            tip.SetToolTip(label2, "注册时设置的");
        }

        //窗口关闭事件
        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form2.enabled = true;
            Form2.change();
        }

        //"确定"按钮事件
        private void button1_Click(object sender, EventArgs e)
        {
            bool check = false;
            try
            {
                SqlConnection conn = new SqlConnection("server=.;database=c#Traning report;uid=sa;pwd=123456");
                conn.Open();
                SqlCommand comm = conn.CreateCommand();
                comm.CommandText = "select account,check_code from UserData where account="+textBox1.Text;
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0].ToString() == textBox1.Text && reader[1].ToString() == textBox2.Text) {
                        check = true;
                    }
                }
                reader.Close();
                if (check)
                {
                    if (CheckPassword && CheckAgainPassword)
                    {
                        comm.CommandText = "update UserData set password='" + textBox3.Text + "' where account=" + textBox1.Text;
                        int check_sql = comm.ExecuteNonQuery();
                        if (check_sql > 0)
                        {
                            textBox1.Clear();
                            textBox2.Clear();
                            textBox3.Clear();
                            textBox4.Clear();
                            CheckPassword = false;
                            CheckAgainPassword = false;
                            error.Clear();
                            MessageBox.Show("密码修改成功！", "提示");
                        }
                    }
                    else
                    {
                        MessageBox.Show("存在错误，请检查", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                else
                {
                    MessageBox.Show("账号不存在或校验码错误", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("出错了，原因是:"+ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //"新密码"输入框改变事件
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            error.Clear();
            error.SetIconAlignment(textBox3, ErrorIconAlignment.MiddleRight);
            error.SetIconPadding(textBox3, 3);
            if (textBox3.Text.Length < 6)
            {
                error.Icon = new Icon(@"..\..\images\error.ico");
                error.SetError(textBox3, "至少六位密码");
            }
            else if (textBox3.Text.Length > 16)
            {
                error.Icon = new Icon(@"..\..\images\error.ico");
                error.SetError(textBox3, "密码最多16位");
            }
            else
            {
                error.Icon = new Icon(@"..\..\images\right.ico");
                error.SetError(textBox3, "输入正确！");
                CheckPassword = true;
            }
        }

        //"新密码"输入框提示事件
        private void textBox3_MouseEnter(object sender, EventArgs e)
        {
            tip.Active = true;
            int codeLevel = 0;
            String getText = textBox3.Text;
            //含数字-含小写字母-含大写字母-含特殊符号
            if (Regex.IsMatch(getText, "\\d+") || Regex.IsMatch(getText, "[a-z]+") || Regex.IsMatch(getText, "((?=[\x21-\x7e]+)[^A-Za-z0-9])+") || Regex.IsMatch(getText, "[A-Z]+"))
            {
                codeLevel = 1;
            }
            //含小写字母母和数字-含大写字母和数字-含小写和大写字母-含特殊和字母-含特殊和数字
            if (Regex.IsMatch(getText, "(?=.*[a-z].*)(?=.*\\d.*)+") || Regex.IsMatch(getText, "(?=.*[A-Z].*)(?=.*\\d.*)+") || Regex.IsMatch(getText, "(?=.*[A-Z].*)(?=.*[a-z].*)+") || Regex.IsMatch(getText, "(?=.*((?=[\x21-\x7e]+)[^A-Za-z0-9]).*)(?=.*[a-z].*)+") || Regex.IsMatch(getText, "(?=.*((?=[\x21-\x7e]+)[^A-Za-z0-9]).*)(?=.*[A-Z].*)+") || Regex.IsMatch(getText, "(?=.*((?=[\x21-\x7e]+)[^A-Za-z0-9]).*)(?=.*\\d.*)+") || getText.Length > 10)
            {
                codeLevel = 2;
            }
            //含小写、大写字母和数字-含小写、大写字母和特殊-含数字、小写字母和特殊-含数字、大写字母和特殊
            if (Regex.IsMatch(getText, "(?=.*[a-z].*)(?=.*[A-Z].*)(?=.*\\d.*)+") || Regex.IsMatch(getText, "(?=.*[a-z].*)(?=.*[A-Z].*)(?=.*((?=[\x21-\x7e]+)[^A-Za-z0-9]).*)+") || Regex.IsMatch(getText, "(?=.*\\d.*)(?=.*[a-z].*)(?=.*((?=[\x21-\x7e]+)[^A-Za-z0-9]).*)+") || Regex.IsMatch(getText, "(?=.*[0-9].*)(?=.*[A-Z])(?=.*((?=[\x21-\x7e]+)[^A-Za-z0-9]).*)+") || Regex.IsMatch(getText, "(?=.*\\d.*)(?=.*[A-Z].*)(?=.*((?=[\x21-\x7e]+)[^A-Za-z0-9]).*)+"))
            {
                codeLevel = 3;
            }
            //含小写、大写字母、特殊符号和数字
            if (Regex.IsMatch(getText, "(?=.*[a-z].*)(?=.*[A-Z].*)(?=.*\\d.*)(?=.*((?=[\x21-\x7e]+)[^A-Za-z0-9]).*)+"))
            {
                codeLevel = 4;
            }
            switch (codeLevel)
            {
                case 1:
                    tip.SetToolTip(textBox3, "密码强度:弱"); break;
                case 2:
                    tip.SetToolTip(textBox3, "密码强度:中等"); break;
                case 3:
                    tip.SetToolTip(textBox3, "密码强度:高"); break;
                case 4:
                    tip.SetToolTip(textBox3, "密码强度:强"); break;
            }
        }

        //"再次输入密码"输入框改变事件
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if(textBox4.Text == textBox3.Text)
            {
                error.Icon = new Icon(@"..\..\images\right.ico");
                error.SetError(textBox4, "输入正确！");
                CheckAgainPassword = true;
            }
            else
            {
                error.Icon = new Icon(@"..\..\images\error.ico");
                error.SetError(textBox4, "不匹配！");
            }
        }
    }
}
