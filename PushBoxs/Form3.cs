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
    public partial class Form3 : Form
    {
        int c1, c2, c3, c4 = 0;
        public Form3()
        {
            InitializeComponent();
        }

        //账号提示
        private void label5_MouseEnter(object sender, EventArgs e)
        {
            tip.Active = true;
            tip.SetToolTip(label5,"用于找回密码");
        }
        
        //密码提示
        private void label3_MouseEnter(object sender, EventArgs e)
        {
            tip.Active = true;
            tip.SetToolTip(label3,"可以设置多种情况的密码！");
        }
        
        //注册
        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("c1:"+c1+"c2:"+ c2 +"c3:"+ c3 +"c4:"+ c4);
            String sqltext = "Server=.;database=c#Traning report;uid=sa;pwd=123456";
            try
            {
                if (c1 == 1 && c2 == 1 && c3 == 1 && c4 == 1)
                {   
                    SqlConnection conn = new SqlConnection(sqltext);
                    conn.Open();
                    SqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "insert into UserData values ('"+textBox1.Text+"','"+textBox2.Text+"','"+textBox3.Text+ "');"+ "create table MapData.MapData_" + textBox1.Text + "(Name varchar(255), MapData varchar(2555), Surface varchar(255))";

                    int check = comm.ExecuteNonQuery();
                    if (check > 0)
                    {
                        MessageBox.Show("注册成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("输入信息有误", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    conn.Close();
                    
                }
                else
                {
                    if (textBox1.Text == "")
                    {
                        MessageBox.Show("账号未输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }else if (textBox2.Text == "")
                    {
                        MessageBox.Show("密码未输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }else if (textBox3.Text == "")
                    {
                        MessageBox.Show("再次输入为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }else if (textBox4.Text == "")
                    {
                        MessageBox.Show("校验码未输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("报错原因:"+ex.Message);
            }
            
        }
        
        //密码强度检测
        private void textBox2_MouseEnter(object sender, EventArgs e)
        {
            tip.Active = true;
            int codeLevel = 0;
            String getText = textBox2.Text;
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
                    tip.SetToolTip(textBox2, "密码强度:弱"); break;
                case 2:
                    tip.SetToolTip(textBox2, "密码强度:中等"); break;
                case 3:
                    tip.SetToolTip(textBox2, "密码强度:高"); break;
                case 4:
                    tip.SetToolTip(textBox2, "密码强度:强"); break;
            }
        }
        
        //校验码输入检测
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            error4.Clear();
            error4.SetIconPadding(textBox4, 3);
            error4.SetIconAlignment(textBox4, ErrorIconAlignment.MiddleRight);
            if (Regex.IsMatch(textBox4.Text,"\\d+"))
            {
                int i = textBox4.Text.Length;
                if (i >= 0 && i < 6)
                {
                    error4.Icon = new Icon(@"..\..\images\error.ico");
                    error4.SetError(textBox4, "请输入6位数字!");

                }
                else if (i > 6)
                {
                    error4.Icon = new Icon(@"..\..\images\error.ico");
                    error4.SetError(textBox4, "请输入6位数字!");
                }
                else
                {
                    error4.Icon = new Icon(@"..\..\images\right.ico");
                    error4.SetError(textBox4, "输入正确！");
                    c4 = 1;
                }
            }
            else
            {
                error4.Icon = new Icon(@"..\..\images\error.ico");
                error4.SetError(textBox4, "请输入6位数字!");
            }
            
        }
        
        //再次输入密码的输入检测
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            error3.Clear();
            error3.SetIconPadding(textBox3, 3);
            error3.SetIconAlignment(textBox3, ErrorIconAlignment.MiddleRight);
            if (textBox3.Text != textBox2.Text)
            {
                error3.Icon = new Icon(@"..\..\images\error.ico");
                error3.SetError(textBox3, "不匹配！");
            }
            else
            {
                error3.Icon = new Icon(@"..\..\images\right.ico");
                error3.SetError(textBox3, "输入正确！");
                c3 = 1;
            }
        }
        
        //密码输入检测
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            error2.Clear();
            error2.SetIconAlignment(textBox2, ErrorIconAlignment.MiddleRight);
            error2.SetIconPadding(textBox2, 3);
            if (textBox2.Text.Length < 6)
            {
                error2.Icon = new Icon(@"..\..\images\error.ico");
                error2.SetError(textBox2, "至少六位密码");
            }
            else if (textBox2.Text.Length > 16)
            {
                error2.Icon = new Icon(@"..\..\images\error.ico");
                error2.SetError(textBox2, "密码最多16位");
            }
            else
            {
                error2.Icon = new Icon(@"..\..\images\right.ico");
                error2.SetError(textBox2, "输入正确！");
                c2 = 1;
            }
        }
        
        //账号输入检测
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            error1.Clear();
            error1.SetIconPadding(textBox1, 3);
            error1.SetIconAlignment(textBox1, ErrorIconAlignment.MiddleRight);
            if (Regex.IsMatch(textBox1.Text, "[ ]"))
            {
                error1.Icon = new Icon(@"..\..\images\error.ico");
                error1.SetError(textBox1, "存在空格！");
            }
            else if (textBox1.Text.Length > 10)
            {
                error1.Icon = new Icon(@"..\..\images\error.ico");
                error1.SetError(textBox1, "账号最多十位！");
            }
            else
            {
                error1.Icon = new Icon(@"..\..\images\right.ico");
                error1.SetError(textBox1, "输入正确！");
                c1 = 1;
            }
        }
        
        //窗口关闭事件
        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form2.enabled = true;
            Form2.change();
        }

    }
}
