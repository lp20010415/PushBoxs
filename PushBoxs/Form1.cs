using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;

namespace PushBoxs
{
    public delegate void offlineModeSet();  // 委托-离线模式
    public delegate void ChangeEnabled1();  //委托-修改Enabled
    public delegate void CloseForm();       //委托-关闭窗口
    public partial class Form1 : Form
    {
        public static string LoginName = "";    //登录名
        string[] Surface = null;                //自制地图的地面数据
        string[] MapData = new string[10]{  
            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001111110000000001402310000000001111110000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
            "000000000000000000000000000000000000000000000000000000000000000001111100000000001000100000000011000110000000010222010000000010353010000000011111110000000000000000000000000000000000000000000000000000000000000000000000000000000",
            "000000000000000000000000000000000000000000000000011111111000000014000001000000010322301000000010233201000000010233201000000010322301000000010000001000000011111111000000000000000000000000000000000000000000000000000000000000000",
            "000000000000000000000000000000000000000000000000000000000000000011111111000000010000001000000010232301000000010323241000000010232301000000010000001000000011111111000000000000000000000000000000000000000000000000000000000000000",
            "000000000000000000000000000000000000000000000000000000000000000000111100000000011104100000000010020100000000010063100000000010063100000000010020100000000011100100000000000111100000000000000000000000000000000000000000000000000",
            "000000000000000000000000000000000000000000000000000000000000000000111110000000000100010000000011133410000000010001110000000010000010000000010202010000000011111110000000000000000000000000000000000000000000000000000000000000000",
            "000000000000000000000000000000000000000000000000000000000000000000011111000000001110001000001111333331000001402222201000001000001011000001111100010000000000111110000000000000000000000000000000000000000000000000000000000000000",
            "000000000000000000000000000000000000000000000000011111111000000010000001000000010366601000000010600601000000010600601000000010666201000000010000041000000011111111000000000000000000000000000000000000000000000000000000000000000",
            "000000000000000000000000000000000000000000000000111111111110001133333330010001022222224010001000101010110001010100000100001000111111100001111100000000000000000000000000000000000000000000000000000000000000000000000000000000000",
            "000000000000000000000000000000000111111111000001100010001100001000010000100001002010200100001000636000100001111343111100001000636000100001002010200100001000010000100001100010001100000111111111000000000000000000000000000000000" };                
        // 内置地图数据
        string[,] map = new string[15, 15];     //承载地图数据
        int level = 0;                          //关卡
        int level_total = 10;                    // 关卡总数
        bool write = false;                     //判断是否画了地图
        bool WriteDiyMap = false;               //上同
        int direction = 1;                      //方向0，1，2，3代表上下左右
        int getRow = 0, getCol = 0;             //获取角色的位置
        // MySql登录信息
        private string sqltext = "Server=1.117.74.238;port=3306;DataBase=c#Match_Data;user=root;password=SJMRLp0@0";

        public static offlineModeSet enterOfflineMode;  // 委托-离线模式
        public static ChangeEnabled1 change;    //委托-修改Enabled
        public static bool enabled = false;     //委托-修改Enabled-默认为false
        public static CloseForm close;          //委托-关闭窗口
        public Form1()
        {
            InitializeComponent();

            //使一些控件不会被获取到按键焦点
            SetGetFocus(button1);
            SetGetFocus(button2);
            SetGetFocus(button3);
            SetGetFocus(comboBox1);

            enterOfflineMode = new offlineModeSet(offlineMode);             // 委托-离线模式
            change = new ChangeEnabled1(ChangeShow);                        //委托-修改Enabled
            ChangeShow();                                                   //委托-修改Enabled
            close = new CloseForm(ShutDownFrom);                            //委托-关闭窗口
            new Form2().Show();                                             //启动登录窗口
            ConnectMapData(true);                                           //获取内置地图数据
            BackgroundImage = Image.FromFile(@".\images\Naruto.jpg");   //设置窗口背景

        }

        //获取地图数据库
        private void ConnectMapData(Boolean isInner)
        {
            int i = 0;//用于计数-方便写入值
            int getRow = 0;//用于定义数组大小
            if (isInner)
            {
                comboBox1.Items.Add("第一关");
                comboBox1.Items.Add("第二关");
                comboBox1.Items.Add("第三关");
                comboBox1.Items.Add("第四关");
                comboBox1.Items.Add("第五关");
                comboBox1.Items.Add("第六关");
                comboBox1.Items.Add("第七关");
                comboBox1.Items.Add("第八关");
                comboBox1.Items.Add("第九关");
                comboBox1.Items.Add("第十关");

                //try
                //{
                //    MySqlConnection conn = new MySqlConnection(sqltext);
                //    conn.Open();
                //    MySqlCommand command = conn.CreateCommand();
                //    command.CommandText = "select count(*) from InnerMapData";
                //    getRow = Convert.ToInt32(command.ExecuteScalar());
                //    command.CommandText = "select * from InnerMapData";

                //    if (getRow > 0)
                //    {
                //        MySqlDataReader reader = command.ExecuteReader();
                //        MapData = new string[getRow];
                //        while (reader.Read())
                //        {
                //            MapData[i] = reader["MapData"].ToString();
                //            comboBox1.Items.Add(reader["Name"].ToString());
                //            level_total = comboBox1.Items.Count;
                //            i++;
                //        }
                //        reader.Close();
                //    }
                //    else
                //    {
                //        MessageBox.Show("暂无地图");
                //    }
                //    conn.Close();

                //    //SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                //    //conn.Open();
                //    //SqlCommand command = conn.CreateCommand();
                //    //command.CommandText = "select count(*) from " + code;//sql语句
                //    //getRow = (int)command.ExecuteScalar();
                //    //conn.Close();
                //}
                //catch (MySqlException e)
                //{
                //    MessageBox.Show("您可能没有联网！可以考虑游玩离线模式！");
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.Message);
                //    MessageBox.Show("出错原因:" + e.Message);
                //}
            } else
            {
                try
                {
                    //SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                    //conn.Open();
                    //SqlCommand command = conn.CreateCommand();
                    //command.CommandText = "select * from " + code;//sql语句
                    //SqlDataReader reader = command.ExecuteReader();
                    string LoginName = label1.Text.Replace("当前登录账号:", "");
                    MySqlConnection conn = new MySqlConnection(sqltext);
                    conn.Open();
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "select count(*) from UserMapData where user_name = '" + LoginName + "'";
                    getRow = Convert.ToInt32(command.ExecuteScalar());
                    command.CommandText = "select * from UserMapData where user_name = '" + LoginName + "'";
                    if (getRow > 0)
                    {
                        MySqlDataReader reader = command.ExecuteReader();

                        MapData = new string[getRow];
                        Surface = new string[getRow];
                        while (reader.Read())
                        {
                            //if (Regex.IsMatch(code, "(MapData.MapData)+"))
                            //    Surface[i] = reader["Surface"].ToString();
                            Surface[i] = reader["surface"].ToString();
                            MapData[i] = reader["map_data"].ToString();
                            comboBox1.Items.Add(reader["map_name"].ToString());
                            level_total = comboBox1.Items.Count;
                            i++;
                        }
                        reader.Close();
                    } else
                    {
                        MessageBox.Show("暂无地图");
                    }
                    conn.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("出错原因:" + e.Message);
                }
            }
            
            
        }

        //画内置地图
        private void DrawMap_InnerMap()
        {
            if (level_total == level)
            {
                pictureBox1.Image = null;
                MessageBox.Show("您已通关所有关卡！");

            } else
            {

                comboBox1.SelectedIndex = level;
                label2.Text = comboBox1.Text;
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);//创建矢量图
                Graphics g = Graphics.FromImage(bmp);//载入矢量图
                Bitmap bitmp;
                //解析地图数据并写入二维数组中
                if (write)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        string first = MapData[level].Substring(15 * i, 15);
                        for (int j = 0; j < 15; j++)
                        {
                            map[i, j] = first.Substring(j, 1);
                        }
                    }
                    write = false;
                }
                //开始画地图
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        bitmp = new Bitmap(@".\images\Surface\SurfaceOne.png");
                        switch (map[i, j])
                        {
                            case "0":
                                bitmp = new Bitmap(@".\images\Surface\SurfaceOne.png");
                                break;
                            case "1":
                                bitmp = new Bitmap(@".\images\Wall\WallOne.png");
                                break;
                            case "2":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\SurfaceOne-BoxOne-1.png");
                                break;
                            case "3":
                                bitmp = new Bitmap(@".\images\End\EndBlue\SurfaceOne-EndBlue.png");
                                break;
                            case "4":
                                switch (direction)
                                {
                                    case 0:
                                        bitmp = new Bitmap(@".\images\Character\Up\SurfaceOne-Up.gif");
                                        break;
                                    case 1:
                                        bitmp = new Bitmap(@".\images\Character\Down\SurfaceOne-Down.gif");
                                        break;
                                    case 2:
                                        bitmp = new Bitmap(@".\images\Character\Left\SurfaceOne-Left.gif");
                                        break;
                                    case 3:
                                        bitmp = new Bitmap(@".\images\Character\Right\SurfaceOne-Right.gif");
                                        break;
                                }
                                getRow = i;
                                getCol = j;
                                break;

                            case "5":
                                bitmp = new Bitmap(@".\images\Character\EndCharacter.png");
                                getRow = i;
                                getCol = j;
                                break;
                            case "6":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\SurfaceOne-BoxOne-2.png"); break;
                        }
                        g.DrawImage(bitmp, j * 25, i * 25, 25, 25);
                    }

                }
                pictureBox1.Image = bmp;
                pictureBox1.Focus();
                CheckWin();
            }
            
        }

        //画自制地图
        private void DrawMap_DiyMap()
        {
            if (level == level_total)
            {

                pictureBox1.Image = null;
                MessageBox.Show("恭喜您已通关所有关卡！");
            } else
            {

                comboBox1.SelectedIndex = level;
                label2.Text = comboBox1.Text;
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);//创建矢量图
                Graphics g = Graphics.FromImage(bmp);//载入矢量图
                Bitmap bitmp = new Bitmap(@".\images\Surface\SurfaceOne.png");
                //解析地图数据并写入二维数组中
                if (WriteDiyMap)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        string first = MapData[level].Substring(30 * i, 30);
                        for (int j = 0; j < 15; j++)
                        {
                            map[i, j] = first.Substring(j * 2, 2);
                        }
                    }
                    WriteDiyMap = false;
                }
                for (int i = 0; i < 15; i++)
                {
                    string checkLookCode = "";
                    for (int j = 0; j < 15; j++)
                    {
                        checkLookCode += map[i, j];
                    }
                    Console.WriteLine(checkLookCode);
                }

                //开始画地图
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        switch (map[i, j])
                        {
                            case "S1":
                                bitmp = new Bitmap(@".\images\Surface\SurfaceOne.png");
                                break;
                            case "S2":
                                bitmp = new Bitmap(@".\images\Surface\SurfaceOne.png");
                                break;
                            case "S3":
                                bitmp = new Bitmap(@".\images\Surface\SurfaceTwo.png");
                                break;
                            case "S4":
                                bitmp = new Bitmap(@".\images\Surface\SurfaceThree.png");
                                break;
                            case "S5":
                                bitmp = new Bitmap(@".\images\Surface\SurfaceFour.png");
                                break;
                            case "S6":
                                bitmp = new Bitmap(@".\images\Surface\SurfaceFive.png");
                                break;
                            case "S7":
                                bitmp = new Bitmap(@".\images\Surface\SurfaceSix.png");
                                break;
                            case "S8":
                                bitmp = new Bitmap(@".\images\Surface\SurfaceSeven.png");
                                break;
                            case "B1":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\" + Surface[level] + "-BoxOne-1.png");
                                break;
                            case "B2":
                                bitmp = new Bitmap(@".\images\Box\BoxTwo\" + Surface[level] + "-BoxTwo-1.png");
                                break;
                            case "B3":
                                bitmp = new Bitmap(@".\images\Box\BoxThree\" + Surface[level] + "-BoxThree-1.png");
                                break;
                            case "H4":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\" + Surface[level] + "-BoxOne-2.png");
                                break;
                            case "I4":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\" + Surface[level] + "-BoxOne-2.png");
                                break;
                            case "J4":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\" + Surface[level] + "-BoxOne-2.png");
                                break;
                            case "K4":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\" + Surface[level] + "-BoxOne-2.png");
                                break;
                            case "L4":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\" + Surface[level] + "-BoxOne-2.png");
                                break;
                            case "M4":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\" + Surface[level] + "-BoxOne-2.png");
                                break;
                            case "N4":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\" + Surface[level] + "-BoxOne-2.png");
                                break;
                            case "O4":
                                bitmp = new Bitmap(@".\images\Box\BoxOne\" + Surface[level] + "-BoxOne-2.png");
                                break;
                            case "H5":
                                bitmp = new Bitmap(@".\images\Box\BoxTwo\" + Surface[level] + "-BoxTwo-2.png");
                                break;
                            case "I5":
                                bitmp = new Bitmap(@".\images\Box\BoxTwo\" + Surface[level] + "-BoxTwo-2.png");
                                break;
                            case "J5":
                                bitmp = new Bitmap(@".\images\Box\BoxTwo\" + Surface[level] + "-BoxTwo-2.png");
                                break;
                            case "K5":
                                bitmp = new Bitmap(@".\images\Box\BoxTwo\" + Surface[level] + "-BoxTwo-2.png");
                                break;
                            case "L5":
                                bitmp = new Bitmap(@".\images\Box\BoxTwo\" + Surface[level] + "-BoxTwo-2.png");
                                break;
                            case "M5":
                                bitmp = new Bitmap(@".\images\Box\BoxTwo\" + Surface[level] + "-BoxTwo-2.png");
                                break;
                            case "N5":
                                bitmp = new Bitmap(@".\images\Box\BoxTwo\" + Surface[level] + "-BoxTwo-2.png");
                                break;
                            case "O5":
                                bitmp = new Bitmap(@".\images\Box\BoxTwo\" + Surface[level] + "-BoxTwo-2.png");
                                break;
                            case "H6":
                                bitmp = new Bitmap(@".\images\Box\BoxThree\" + Surface[level] + "-BoxThree-2.png");
                                break;
                            case "I6":
                                bitmp = new Bitmap(@".\images\Box\BoxThree\" + Surface[level] + "-BoxThree-2.png");
                                break;
                            case "J6":
                                bitmp = new Bitmap(@".\images\Box\BoxThree\" + Surface[level] + "-BoxThree-2.png");
                                break;
                            case "K6":
                                bitmp = new Bitmap(@".\images\Box\BoxThree\" + Surface[level] + "-BoxThree-2.png");
                                break;
                            case "L6":
                                bitmp = new Bitmap(@".\images\Box\BoxThree\" + Surface[level] + "-BoxThree-2.png");
                                break;
                            case "M6":
                                bitmp = new Bitmap(@".\images\Box\BoxThree\" + Surface[level] + "-BoxThree-2.png");
                                break;
                            case "N6":
                                bitmp = new Bitmap(@".\images\Box\BoxThree\" + Surface[level] + "-BoxThree-2.png");
                                break;
                            case "O6":
                                bitmp = new Bitmap(@".\images\Box\BoxThree\" + Surface[level] + "-BoxThree-2.png");
                                break;
                            case "E1":
                                bitmp = new Bitmap(@".\images\End\EndBeige\" + Surface[level] + "-EndBeige.png");
                                break;
                            case "E2":
                                bitmp = new Bitmap(@".\images\End\EndBlack\" + Surface[level] + "-EndBlack.png");
                                break;
                            case "E3":
                                bitmp = new Bitmap(@".\images\End\EndBlue\" + Surface[level] + "-EndBlue.png");
                                break;
                            case "E4":
                                bitmp = new Bitmap(@".\images\End\EndBrown\" + Surface[level] + "-EndBrown.png");
                                break;
                            case "E5":
                                bitmp = new Bitmap(@".\images\End\EndGray\" + Surface[level] + "-EndGray.png");
                                break;
                            case "E6":
                                bitmp = new Bitmap(@".\images\End\EndPurple\" + Surface[level] + "-EndPurple.png");
                                break;
                            case "E7":
                                bitmp = new Bitmap(@".\images\End\EndRed\" + Surface[level] + "-EndRed.png");
                                break;
                            case "E8":
                                bitmp = new Bitmap(@".\images\End\EndYellow\" + Surface[level] + "-EndYellow.png");
                                break;
                            case "C1":
                                bitmp = new Bitmap(@".\images\Character\Up\" + Surface[level] + "-Up.gif");
                                getRow = i;
                                getCol = j;
                                break;
                            case "C2":
                                bitmp = new Bitmap(@".\images\Character\Down\" + Surface[level] + "-Down.gif");
                                getRow = i;
                                getCol = j;
                                break;
                            case "C3":
                                bitmp = new Bitmap(@".\images\Character\Left\" + Surface[level] + "-Left.gif");
                                getRow = i;
                                getCol = j;
                                break;
                            case "C4":
                                bitmp = new Bitmap(@".\images\Character\Right\" + Surface[level] + "-Right.gif");
                                getRow = i;
                                getCol = j;
                                break;
                            case "CH":
                                bitmp = new Bitmap(@".\images\End\EndCharacter\EndCharacter-Beige.png");
                                getRow = i;
                                getCol = j;
                                break;
                            case "CI":
                                bitmp = new Bitmap(@".\images\End\EndCharacter\EndCharacter-Black.png");
                                getRow = i;
                                getCol = j;
                                break;
                            case "CJ":
                                bitmp = new Bitmap(@".\images\End\EndCharacter\EndCharacter-Blue.png");
                                getRow = i;
                                getCol = j;
                                break;
                            case "CK":
                                bitmp = new Bitmap(@".\images\End\EndCharacter\EndCharacter-Brown.png");
                                getRow = i;
                                getCol = j;
                                break;
                            case "CL":
                                bitmp = new Bitmap(@".\images\End\EndCharacter\EndCharacter-Gray.png");
                                getRow = i;
                                getCol = j;
                                break;
                            case "CM":
                                bitmp = new Bitmap(@".\images\End\EndCharacter\EndCharacter-Purple.png");
                                getRow = i;
                                getCol = j;
                                break;
                            case "CN":
                                bitmp = new Bitmap(@".\images\End\EndCharacter\EndCharacter-Red.png");
                                getRow = i;
                                getCol = j;
                                break;
                            case "CO":
                                bitmp = new Bitmap(@".\images\End\EndCharacter\EndCharacter-Yellow.png");
                                getRow = i;
                                getCol = j;
                                break;
                            case "W1":
                                bitmp = new Bitmap(@".\images\Wall\WallOne.png");
                                break;
                            case "W2":
                                bitmp = new Bitmap(@".\images\Wall\WallTwo.png");
                                break;
                            case "W3":
                                bitmp = new Bitmap(@".\images\Wall\WallThree.png");
                                break;
                            case "W4":
                                bitmp = new Bitmap(@".\images\Wall\WallFour.png");
                                break;
                        }
                        g.DrawImage(bitmp, j * 25, i * 25, 25, 25);
                    }
                }
                pictureBox1.Image = bmp;
                pictureBox1.Focus();
                CheckWin();
            }
            
        }

        //选择关卡
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (button1.Text == "切换到自制地图")
            {
                if (comboBox1.SelectedIndex >= 0)
                {
                    level = comboBox1.SelectedIndex;
                    write = true;
                    pictureBox1.Focus();
                    DrawMap_InnerMap();
                }
            }
            else if (button1.Text == "切换到内置地图")
            {
                if (comboBox1.SelectedIndex >= 0)
                {
                    level = comboBox1.SelectedIndex;
                    WriteDiyMap = true;
                    pictureBox1.Focus();
                    DrawMap_DiyMap();
                }
            }
        }

        //移动
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int row = 0, col = 0, row1 = 0, col1 = 0;
            if (pictureBox1.Image != null)
            {
                if (button1.Text == "切换到自制地图")
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Up:
                            direction = 0;
                            row = getRow - 1;
                            col = getCol;
                            row1 = getRow - 2;
                            col1 = getCol;
                            break;
                        case Keys.Down:
                            direction = 1;
                            row = getRow + 1;
                            col = getCol;
                            row1 = getRow + 2;
                            col1 = getCol;
                            break;
                        case Keys.Left:
                            direction = 2;
                            row = getRow;
                            col = getCol - 1;
                            row1 = getRow;
                            col1 = getCol - 2;
                            break;
                        case Keys.Right:
                            direction = 3;
                            row = getRow;
                            col = getCol + 1;
                            row1 = getRow;
                            col1 = getCol + 2;
                            break;
                        case Keys.W:
                            direction = 0;
                            row = getRow - 1;
                            col = getCol;
                            row1 = getRow - 2;
                            col1 = getCol;
                            break;
                        case Keys.S:
                            direction = 1;
                            row = getRow + 1;
                            col = getCol;
                            row1 = getRow + 2;
                            col1 = getCol;
                            break;
                        case Keys.A:
                            direction = 2;
                            row = getRow;
                            col = getCol - 1;
                            row1 = getRow;
                            col1 = getCol - 2;
                            break;
                        case Keys.D:
                            direction = 3;
                            row = getRow;
                            col = getCol + 1;
                            row1 = getRow;
                            col1 = getCol + 2;
                            break;

                    }
                    CharacterMove_InnerMap(row, col, row1, col1);
                }
                else if (button1.Text == "切换到内置地图")
                {

                    switch (e.KeyCode)
                    {
                        case Keys.Up:

                            direction = 0;
                            row = getRow - 1;
                            col = getCol;
                            row1 = getRow - 2;
                            col1 = getCol;
                            break;
                        case Keys.Down:

                            direction = 1;
                            row = getRow + 1;
                            col = getCol;
                            row1 = getRow + 2;
                            col1 = getCol;
                            break;
                        case Keys.Left:

                            direction = 2;
                            row = getRow;
                            col = getCol - 1;
                            row1 = getRow;
                            col1 = getCol - 2;
                            break;
                        case Keys.Right:

                            direction = 3;
                            row = getRow;
                            col = getCol + 1;
                            row1 = getRow;
                            col1 = getCol + 2;
                            break;
                        case Keys.W:

                            direction = 0;
                            row = getRow - 1;
                            col = getCol;
                            row1 = getRow - 2;
                            col1 = getCol;
                            break;
                        case Keys.S:

                            direction = 1;
                            row = getRow + 1;
                            col = getCol;
                            row1 = getRow + 2;
                            col1 = getCol;
                            break;
                        case Keys.A:

                            direction = 2;
                            row = getRow;
                            col = getCol - 1;
                            row1 = getRow;
                            col1 = getCol - 2;
                            break;
                        case Keys.D:

                            direction = 3;
                            row = getRow;
                            col = getCol + 1;
                            row1 = getRow;
                            col1 = getCol + 2;
                            break;
                    }
                    CharacterMove_DiyMap(row, col, row1, col1);
                }
            }
        }

        //人物移动(内置地图)-x0,y0人物下一个位置、x1,y1下下个位置
        private void CharacterMove_InnerMap(int x0, int y0, int x1, int y1)
        {
            if (map[getRow, getCol] == "5")//人物在终点上
                map[getRow, getCol] = "3";//人物移动，终点还原

            if (x0 + y0 < 30 && x1 + y1 < 30 && x0 + y0 > 0 && x1 + y1 >0)
            {
                switch (map[x0, y0])
                {
                    //人物前面是地面
                    case "0":
                        if (map[getRow, getCol] != "3")
                            map[getRow, getCol] = "0";
                        map[x0, y0] = "4";
                        DrawMap_InnerMap();
                        break;
                    //人物前面是箱子
                    case "2":
                        switch (map[x1, y1])
                        {
                            //人物前前面是地面
                            case "0":
                                if (map[getRow, getCol] != "3")
                                    map[getRow, getCol] = "0";
                                map[x1, y1] = map[x0, y0];
                                map[x0, y0] = "4";
                                break;
                            //人物前前面是终点
                            case "3":
                                if (map[getRow, getCol] != "3")
                                    map[getRow, getCol] = "0";
                                map[x1, y1] = "6";
                                map[x0, y0] = "4";
                                break;
                        }
                        break;
                    //人物前面是终点
                    case "3":
                        if (map[getRow, getCol] != "3")
                            map[getRow, getCol] = "0";
                        map[x0, y0] = "5";
                        break;
                    //人物前面是到达终点的箱子
                    case "6":
                        switch (map[x1, y1])
                        {
                            //人物前前面是地面
                            case "0":
                                if (map[getRow, getCol] != "3")
                                    map[getRow, getCol] = "0";
                                map[x1, y1] = map[x0, y0];
                                map[x0, y0] = "5";
                                break;
                            //人物前前面是终点
                            case "3":
                                if (map[getRow, getCol] != "3")
                                    map[getRow, getCol] = "0";
                                map[x1, y1] = "6";
                                map[x0, y0] = "5";
                                break;
                        }
                        break;
                }
            }
            DrawMap_InnerMap();
        }

        //人物移动(自制地图)
        private void CharacterMove_DiyMap(int x0, int y0, int x1, int y1)
        {
            Console.WriteLine("x0:" + x0 + "---y0:" + y0 + "----x1:" + x1 + "----y1:" + y1);

            //还原终点
            //string getEndOne = "";
            //string getEndTwo = "";

            //if (Regex.IsMatch(map[x0, y0], "(?=.*[0-9].*)(?=.*(E).*)+"))
            //    getEndOne = map[x0, y0];
            //if (Regex.IsMatch(map[x1, y1], "(?=.*[0-9].*)(?=.*(E).*)+"))
            //    getEndTwo = map[x1, y1];

            /*
             * x1,y1 
             * x0,x0 移动到的位置
             * getRow, getCol 起始位置
             */
            Console.WriteLine("位置3:" + x1 + "---" + y1);
            Console.WriteLine("位置2:" + x0 + "---" + y0);
            Console.WriteLine("位置1:" + getRow + "---" + getCol);

            /*人物移动，终点还原*/
            if (map[getRow, getCol] == "CH" || map[getRow, getCol] == "CI" || map[getRow, getCol] == "CJ" || map[getRow, getCol] == "CK" || map[getRow, getCol] == "CL" || map[getRow, getCol] == "CM" || map[getRow, getCol] == "CN" || map[getRow, getCol] == "CO")//人物在终点上
            {
                switch (map[getRow, getCol])
                {
                    case "CH":
                        map[getRow, getCol] = "E1";
                        break;
                    case "CI":
                        map[getRow, getCol] = "E2";
                        break;
                    case "CJ":
                        map[getRow, getCol] = "E3";
                        break;
                    case "CK":
                        map[getRow, getCol] = "E4";
                        break;
                    case "CL":
                        map[getRow, getCol] = "E5";
                        break;
                    case "CM":
                        map[getRow, getCol] = "E6";
                        break;
                    case "CN":
                        map[getRow, getCol] = "E7";
                        break;
                    case "CO":
                        map[getRow, getCol] = "E8";
                        break;
                }
            }

            //开始移动
            if (x0 + y0 < 30 && x1 + y1 < 30 && x0 + y0 > 0 && x1 + y1 > 0)
            {
                switch (map[x0, y0])
                {
                    //人物前面是地面
                    case "S1":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = ChangeCharacter(direction);
                        break;
                    case "S2":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = ChangeCharacter(direction);
                        break;
                    case "S3":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = ChangeCharacter(direction);
                        break;
                    case "S4":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = ChangeCharacter(direction);
                        break;
                    case "S5":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = ChangeCharacter(direction);
                        break;
                    case "S6":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = ChangeCharacter(direction);
                        break;
                    case "S7":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = ChangeCharacter(direction);
                        break;
                    case "S8":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        Console.WriteLine("笑死了--->" + map[getRow, getCol]);
                        map[x0, y0] = ChangeCharacter(direction);
                        break;
                    //人物前面是箱子
                    case "B1":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        Console.WriteLine("---->" + map[x0,y0]);
                        break;
                    case "B2":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "B3":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    //人物前面是到达终点的箱子
                    case "H4":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "I4":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "J4":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "K4":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "L4":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "M4":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "N4":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "O4":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "H5":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "I5":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "J5":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "K5":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "L5":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "M5":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "N5":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "O5":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "H6":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "I6":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "J6":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "K6":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "L6":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "M6":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "N6":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    case "O6":
                        map[getRow, getCol] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item1;
                        map[x1, y1] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item2;
                        map[x0, y0] = CheckMoveSecondly(map[x0, y0], map[x1, y1], map[getRow, getCol]).Item3;
                        break;
                    //人物前面是终点
                    case "E1":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = "CH";
                        break;
                    case "E2":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = "CI";
                        break;
                    case "E3":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = "CJ";
                        break;
                    case "E4":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = "CK";
                        break;
                    case "E5":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = "CL";
                        break;
                    case "E6":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = "CM";
                        break;
                    case "E7":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = "CN";
                        break;
                    case "E8":
                        map[getRow, getCol] = CheckMoveOneEnd(map[getRow, getCol]);
                        map[x0, y0] = "CO";
                        break;
                }
            }
            DrawMap_DiyMap();
        }

        //检测是否为终点-自制地图移动
        private string CheckMoveOneEnd(string code)
        {
            if (code != "E1" && code != "E2" && code != "E3" && code != "E4" && code != "E5" && code != "E6" && code != "E7" && code != "E8")
            {
                switch (Surface[level])
                {
                    case "SurfaceOne":
                        return "S1";
                    case "SurfaceTwo":
                        return "S2";
                    case "SurfaceThree":
                        return "S3";
                    case "SurfaceFour":
                        return "S4";
                    case "SurfaceFive":
                        return "S5";
                    case "SurfaceSix":
                        return "S6";
                    case "SurfaceSeven":
                        return "S7";
                    case "SurfaceEight":
                        return "S8";
                }
            }
            return code;
        }

        //检测第二位置-自制地图移动
        private (string, string, string) CheckMoveSecondly(string first, string secondly, string code)
        {
            string one, two, three;
            switch (secondly)
            {
                // 表面
                case "S1":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "S2":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "S3":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "S4":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "S5":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "S6":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "S7":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "S8":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                // 箱子
                case "B1":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "B2":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "B3":
                    one = CheckMoveOneEnd(code);
                    two = first;
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                // 终点
                case "E1":
                    one = CheckMoveOneEnd(code);
                    two = "H6";
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "E2":
                    one = CheckMoveOneEnd(code);
                    two = "I6";
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "E3":
                    one = CheckMoveOneEnd(code);
                    two = "J6";
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "E4":
                    one = CheckMoveOneEnd(code);
                    two = "K6";
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "E5":
                    one = CheckMoveOneEnd(code);
                    two = "L6";
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "E6":
                    one = CheckMoveOneEnd(code);
                    two = "M6";
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "E7":
                    one = CheckMoveOneEnd(code);
                    two = "N6";
                    three = ChangeCharacter(direction);
                    return (one, two, three);
                case "E8":
                    one = CheckMoveOneEnd(code);
                    two = "O6";
                    three = ChangeCharacter(direction);
                    return (one, two, three);
            }
            return (first, secondly, code);
        }

        //改变人物方向-自制地图移动
        private string ChangeCharacter(int code)
        {
            switch (code)
            {
                case 0:
                    return "C1";
                case 1:
                    return "C2";
                case 2:
                    return "C3";
                case 3:
                    return "C4";
            }
            return "";
        }

        //判断是否获胜
        private void CheckWin()
        {
            string checkText = ""; 
            for (int i = 0;i < 15;i++)
            {
                for (int j = 0;j < 15;j++)
                {
                    checkText += map[i,j];
                }
            }
            if (button1.Text == "切换到自制地图")
            {
                if (!Regex.IsMatch(checkText, "[3]+") && !Regex.IsMatch(checkText, "[5]+"))
                {
                    MessageBox.Show("恭喜通关了！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    write = true;
                    level += 1;
                    DrawMap_InnerMap();
                }
            }
            else if (button1.Text == "切换到内置地图")
            {
                if (!Regex.IsMatch(checkText, "(E1)+") && !Regex.IsMatch(checkText, "(E2)+") && !Regex.IsMatch(checkText, "(E3)+") && !Regex.IsMatch(checkText, "(E4)+") && !Regex.IsMatch(checkText, "(E5)+") && !Regex.IsMatch(checkText, "(E6)+") && !Regex.IsMatch(checkText, "(E7)+") && !Regex.IsMatch(checkText, "(E8)+") && !Regex.IsMatch(checkText, "(C5)+"))
                {
                    MessageBox.Show("恭喜通关了！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    WriteDiyMap = true;
                    level += 1;
                    DrawMap_DiyMap();
                }
            }
            
        }

        //重新开始游戏
        private void button2_Click(object sender, EventArgs e)
        {
            if (button1.Text == "切换到自制地图")
            {
                write = true;
                DrawMap_InnerMap();
            }
            else if (button1.Text == "切换到内置地图")
            {
                if (pictureBox1.Image == null)
                {
                    level = comboBox1.SelectedIndex;
                }
                WriteDiyMap = true;
                DrawMap_DiyMap();
            }
        }

        // 委托-窗口是否可用
        public void ChangeShow()
        {
            Enabled = enabled;
            if (enabled)
            {
                button1.Enabled = true;
                button3.Enabled = true;
                button4.Text = "退出登录";
                button4.Visible = true;
                Show();
                label1.Text = "当前登录账号:" + LoginName;
            }
        }

        //切换到自制地图
        private void button1_Click(object sender, EventArgs e)
        {
            
            if (button1.Text == "切换到自制地图")
            {
                comboBox1.Items.Clear();
                pictureBox1.Image = null;
                button1.Text = "切换到内置地图";
                Text = "推箱子-自制地图";
                ConnectMapData(false);
            }
            else if (button1.Text == "切换到内置地图")
            {
                comboBox1.Items.Clear();
                pictureBox1.Image = null;
                button1.Text = "切换到自制地图";
                Text = "推箱子-内置地图";
                ConnectMapData(true);
            }
        }

        //打开"做地图"界面
        private void button3_Click(object sender, EventArgs e)
        {
            string getName = label1.Text.Replace("当前登录账号:", "");
            Form5 form5 = new Form5();
            form5.LoginName = LoginName;
            form5.Enabled = false;
            form5.Show();
            Hide();
        }

        //使按钮不会被获取到按键焦点
        private void SetGetFocus(Control button)
        {
            MethodInfo methodinfo = button.GetType().GetMethod("SetStyle", BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.InvokeMethod);
            methodinfo.Invoke(button, BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.InvokeMethod, null, new object[]
                {ControlStyles.Selectable,false}, Application.CurrentCulture);
        }

        //需要登录提示
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

            if (!this.Enabled)
            {
                MessageBox.Show("请登录!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 退出登录
        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            label1.Text = "当前登录账号:";
            enabled = false;
            ChangeShow();
            new Form2().Show();
        }

        // 委托-关闭窗口
        private void ShutDownFrom()
        {
            Close();
        }

        // 委托-离线模式
        private void offlineMode()
        {
            label1.Text = "当前离线模式，无法进入自制地图及做地图，抱歉。";
            button1.Enabled = false;
            button3.Enabled = false;
            button4.Text = "登录";
        }

    }
}
