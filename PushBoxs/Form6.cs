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
    public delegate void reshowDoneMapA();   // 委托-重新展示完成制作的地图
    public delegate void InFocus();         // 委托-获得焦点
    public partial class FormSc : Form
    {
        private Label label;            //文本
        private Button button;          //按钮
        private Button delMap;          // 删除地图按钮
        private TextBox textBox;        //输入框
        private PictureBox pictureBox;  //图片框

        // MySql登录信息
        private string sqltext = "Server=1.117.74.238;port=3306;DataBase=c#Match_Data;user=root;password=SJMRLp0@0";

        public static string[,] GetMapData = null;    //地图数据
        public static string GetMapName = "";   //地图数据
        public static string LoginName = "";    //登录名
        public static string MapCode = "";      //地图数据
        public static int mapId = 0;            // 地图id
        public static string chooseSurface = "";//获取地面
        public static int check = 0;            //检测窗口是否打开

        public static reshowDoneMapA reshowMap; // 委托-重新展示完成制作的地图
        public static InFocus focus;            //委托-获得焦点
        public FormSc()
        {
            InitializeComponent();
            focus = new InFocus(GetFocus);//委托-获得焦点
            reshowMap = new reshowDoneMapA(reshowDoneMap);  // 委托-重新展示完成制作的地图
        }

        //按钮事件
        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Parent.Text)
            {
                case "保存":
                    switch (btn.Text)
                    {
                        case "保存":
                            string MapName = textBox.Text;
                            bool checkName = false;
                            try
                            {
                                MySqlConnection conn = new MySqlConnection(sqltext);
                                conn.Open();
                                MySqlCommand comm = conn.CreateCommand();
                                comm.CommandText = "select count(*) from UserMapData where user_name='" + LoginName + "' and map_name = '" + MapName + "'";

                                //SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                                //conn.Open();
                                //SqlCommand comm = conn.CreateCommand();
                                //comm.CommandText = ("select count(*) from MapData.MapData_" + LoginName + " where Name='" + MapName + "'");

                                int checkRow = Convert.ToInt32(comm.ExecuteScalar());
                                if (checkRow > 0)
                                {
                                    checkName = true;
                                    MessageBox.Show("已存在该名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                conn.Close();
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show("出错原因:" + ex.Message, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            if (!checkName)
                            {
                                try
                                {
                                    //SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                                    //conn.Open();
                                    //SqlCommand comm = conn.CreateCommand();
                                    //comm.CommandText = ("Insert into MapData.MapData_" + LoginName + "(Name, MapData, Surface) values ('" + MapName + "', '" + MapCode + "', '" + chooseSurface + "')");

                                    MySqlConnection conn = new MySqlConnection(sqltext);
                                    conn.Open();
                                    MySqlCommand comm = conn.CreateCommand();
                                    comm.CommandText = "Insert into UserMapData(user_name, map_name, map_data, surface) values ('" + LoginName + "', '" + MapName + "', '" + MapCode + "', '" + chooseSurface + "'); select last_insert_id() from UserMapData where user_name = " + LoginName;

                                    int getId = Convert.ToInt32(comm.ExecuteScalar());
                                    if (getId > 0)
                                        MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    conn.Close();

                                    Form5.mapId = getId;
                                    Form5.LabelName = MapName;
                                    Form5.cln();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("出错原因:" + ex.Message, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            check -= 2;
                            Dispose();
                            break;
                        case "取消":
                            check -= 2;
                            Dispose();
                            break;
                    }
                    break;
                case "保存-关闭窗口":
                    switch (btn.Text)
                    {
                        case "保存":
                            string MapName = textBox.Text;
                            bool checkName = false;
                            try
                            {
                                MySqlConnection conn = new MySqlConnection(sqltext);
                                conn.Open();
                                MySqlCommand comm = conn.CreateCommand();
                                comm.CommandText = "select count(*) from UserMapData where user_name='" + LoginName + "' and map_name = '" + MapName + "'";


                                //SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                                //conn.Open();
                                //SqlCommand comm = conn.CreateCommand();
                                //comm.CommandText = ("select count(*) from MapData.MapData_" + LoginName + " where Name='" + MapName + "'");

                                int checkRow = (int)comm.ExecuteScalar();
                                if (checkRow > 0)
                                {
                                    checkName = true;
                                    MessageBox.Show("已存在该名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                conn.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("出错原因:" + ex.Message, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            if (!checkName)
                            {
                                try
                                {
                                    //SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                                    //conn.Open();
                                    //SqlCommand comm = conn.CreateCommand();
                                    //comm.CommandText = ("Insert into MapData.MapData_" + LoginName + "(Name, MapData, Surface) values ('" + MapName + "', '" + MapCode + "', '" + chooseSurface + "')");

                                    MySqlConnection conn = new MySqlConnection(sqltext);
                                    conn.Open();
                                    MySqlCommand comm = conn.CreateCommand();
                                    comm.CommandText = "Insert into UserMapData(user_name, map_name, map_data, surface) values ('" + LoginName + "', '" + MapName + "', '" + MapCode + "', '" + chooseSurface + "');";


                                    int checkRow = comm.ExecuteNonQuery();
                                    if (checkRow > 0)
                                        MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    conn.Close();
                                    Form5.sf();
                                    Form1.enabled = true;
                                    Form1.change();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("出错原因:" + ex.Message, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            check -= 2;
                            Dispose();
                            break;
                        case "取消":
                            Form5.sf();
                            Form1.enabled = true;
                            Form1.change();
                            check -= 2;
                            Dispose();
                            break;
                    }
                    break;
                case "完成制作":
                    switch (btn.Text)
                    {
                        case "完成制作":
                            string MapName = textBox.Text;
                            bool checkName = false;
                            try
                            {
                                //SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                                //conn.Open();
                                //SqlCommand comm = conn.CreateCommand();
                                //comm.CommandText = ("select count(*) from MapData.MapData_" + LoginName + " where Name='" + MapName + "'");

                                MySqlConnection conn = new MySqlConnection(sqltext);
                                conn.Open();
                                MySqlCommand comm = conn.CreateCommand();
                                comm.CommandText = "select count(*) from UserMapData where user_name = '" + LoginName + "' and map_name= '" + MapName + "'";

                                int checkRow = Convert.ToInt32(comm.ExecuteScalar());
                                if (checkRow != 0)
                                    checkName = true;
                                conn.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("出错原因:" + ex.Message, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            if (!checkName)
                            {
                                try
                                {
                                    //SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                                    //conn.Open();
                                    //SqlCommand comm = conn.CreateCommand();
                                    //comm.CommandText = ("Insert into MapData.MapData_" + LoginName + "(Name, MapData, Surface) values ('" + MapName + "', '" + MapCode + "', '" + chooseSurface + "')");

                                    MySqlConnection conn = new MySqlConnection(sqltext);
                                    conn.Open();
                                    MySqlCommand comm = conn.CreateCommand();
                                    comm.CommandText = "Insert into UserMapData(user_name, map_name, map_data, surface) values ('" + LoginName + "', '" + MapName + "', '" + MapCode + "', '" + chooseSurface + "');";


                                    int checkRow = comm.ExecuteNonQuery();
                                    if (checkRow > 0)
                                        MessageBox.Show("已完成制作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    conn.Close();
                                    Form5.mapId = 0;
                                    Form5.LabelName = "";
                                    Form5.cln();
                                    Form5.rmm();
                                    Form5.comboBox2IsEnabled = true;
                                    Form5.comboBox2Enabled();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("出错原因:" + ex.Message, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                                MessageBox.Show("已存在该名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            check -= 3;
                            Dispose();
                            break;
                        case "取消":
                            check -= 3;
                            Dispose();
                            break;
                    }
                    break;
            }
        }

        //添加控件-根据"做地图"界面决定
        private void FormSc_Load(object sender, EventArgs e)
        {
            switch (((Form)sender).Text)
            {
                case "已做的地图":

                    //连接数据库获取地图数据
                    string[,] MapData;//地图数据
                    int k = 0;//用于计数-方便写入值
                    int getRow = 0;//用于定义数组大小
                    //获取行数
                    //try
                    //{
                    //    //SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                    //    //conn.Open();
                    //    //SqlCommand comm = conn.CreateCommand();
                    //    //comm.CommandText = "Select Count(*) from MapData.MapData_" + LoginName;

                    //    MySqlConnection conn = new MySqlConnection(sqltext);
                    //    conn.Open();
                    //    MySqlCommand comm = conn.CreateCommand();
                    //    conn.Close();
                    //}
                    //catch(Exception ex)
                    //{
                    //    MessageBox.Show("出错原因:" + ex.Message, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
                    //开始获取数据
                    try
                    {
                        //SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                        //conn.Open();
                        //SqlCommand comm = conn.CreateCommand();
                        //comm.CommandText = "Select * from MapData.MapData_" + LoginName;

                        MySqlConnection conn = new MySqlConnection(sqltext);
                        conn.Open();
                        MySqlCommand comm = conn.CreateCommand();
                        comm.CommandText = "select count(*) from UserMapData where user_name='" + LoginName + "'";

                        getRow = Convert.ToInt32(comm.ExecuteScalar());
                        Console.WriteLine(getRow);
                        comm.CommandText = "select * from UserMapData where user_name='" + LoginName + "'";

                        MapData = new string[getRow, 4];
                        MySqlDataReader reader = comm.ExecuteReader();
                        while (reader.Read())
                        {
                            MapData[k, 0] = reader["map_name"].ToString();
                            MapData[k, 1] = reader["map_data"].ToString();
                            MapData[k, 2] = reader["surface"].ToString();
                            MapData[k, 3] = reader["id"].ToString();
                            k++;
                        }
                        GetMapData = MapData;
                        conn.Close();
                        reader.Close();
                        draw_DoneMap();
                        
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("出错原因:" + ex.Message, "出错了", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;
                case "保存":
                    Controls.Clear();
                    label = new Label();
                    label.Text = "请输入地图名";
                    label.Font = new Font("宋体", 13.8f, FontStyle.Regular);
                    label.SetBounds(30, 5, 150, 25);
                    Controls.Add(label);

                    label = new Label();
                    label.Text = "地图名:";
                    label.Font = new Font("宋体", 10.8f, FontStyle.Regular);
                    label.SetBounds(0, 35, 60, 25);
                    Controls.Add(label);

                    textBox = new TextBox();
                    textBox.SetBounds(60, 30, 120, 30);
                    Controls.Add(textBox);

                    button = new Button();
                    button.Text = "保存";
                    button.SetBounds(10, 60, 80, 30);
                    button.Click += btn_Click;
                    Controls.Add(button);

                    button = new Button();
                    button.Text = "取消";
                    button.SetBounds(105, 60, 80, 30);
                    button.Click += btn_Click;
                    Controls.Add(button);

                    break;
                case "保存-关闭窗口":
                    Controls.Clear();
                    label = new Label();
                    label.Text = "请输入地图名";
                    label.Font = new Font("宋体", 13.8f, FontStyle.Regular);
                    label.SetBounds(30, 5, 150, 25);
                    Controls.Add(label);

                    label = new Label();
                    label.Text = "地图名:";
                    label.Font = new Font("宋体", 10.8f, FontStyle.Regular);
                    label.SetBounds(0, 35, 60, 25);
                    Controls.Add(label);

                    textBox = new TextBox();
                    textBox.SetBounds(60, 30, 120, 30);
                    Controls.Add(textBox);

                    button = new Button();
                    button.Text = "保存";
                    button.SetBounds(10, 60, 80, 30);
                    button.Click += btn_Click;
                    Controls.Add(button);

                    button = new Button();
                    button.Text = "取消";
                    button.SetBounds(105, 60, 80, 30);
                    button.Click += btn_Click;
                    Controls.Add(button);

                    break;
                case "完成制作":
                    Controls.Clear();
                    label = new Label();
                    label.Text = "请输入地图名";
                    label.Font = new Font("宋体", 13.8f, FontStyle.Regular);
                    label.SetBounds(30, 5, 150, 25);
                    Controls.Add(label);

                    label = new Label();
                    label.Text = "地图名:";
                    label.Font = new Font("宋体", 10.8f, FontStyle.Regular);
                    label.SetBounds(0, 35, 60, 25);
                    Controls.Add(label);

                    textBox = new TextBox();
                    textBox.SetBounds(60, 30, 120, 30);
                    Controls.Add(textBox);

                    button = new Button();
                    button.Text = "完成制作";
                    button.SetBounds(10, 60, 80, 30);
                    button.Click += btn_Click;
                    Controls.Add(button);

                    button = new Button();
                    button.Text = "取消";
                    button.SetBounds(105, 60, 80, 30);
                    button.Click += btn_Click;
                    Controls.Add(button);

                    break;
            }
        }

        private void draw_DoneMap()
        {
            Controls.Clear();
            label = new Label();
            label.Text = "单击地图名称可进行修改";
            label.Font = new Font("宋体", 13.8f, FontStyle.Regular);
            label.AutoSize = true;
            Controls.Add(label);
            label.SetBounds((Width / 2) - (label.Width / 2), 9, 0, 0);

            // 删除地图按钮
            delMap = new Button();
            delMap.Text = "删除地图";
            delMap.Font = new Font("宋体", 11.8f, FontStyle.Regular);
            delMap.AutoSize = true;
            delMap.Click += delMap_Click;
            Controls.Add(delMap);
            delMap.SetBounds(10, 7, 0, 0);

            int xNum, yNum, GetMapCount;//位置
            xNum = 0;
            yNum = 0;
            GetMapCount = GetMapData.GetLength(0);
            for (int num = 0; num < GetMapCount; num++)
            {
                string SurfaceName = GetMapData[num, 2];
                pictureBox = new PictureBox();
                pictureBox.BorderStyle = BorderStyle.FixedSingle;
                pictureBox.SetBounds(12 + xNum * 308, 71 + yNum * 324, 302, 282);
                Controls.Add(pictureBox);
                Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
                Graphics g = Graphics.FromImage(bmp);
                Bitmap bitmap = new Bitmap(@".\images\Surface\SurfaceOne.png");
                //画图
                string[,] Map = new string[15, 15];
                /*先解析数据*/
                for (int i = 0; i < 15; i++)
                {
                    string code = GetMapData[num, 1].Substring(i * 30, 30);
                    for (int j = 0; j < 15; j++)
                    {
                        Map[i, j] = code.Substring(j * 2, 2);
                    }
                }
                /*开始画图*/
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        switch (Map[i, j])
                        {
                            case "S1":
                                bitmap = new Bitmap(@".\images\Surface\SurfaceOne.png");
                                break;
                            case "S2":
                                bitmap = new Bitmap(@".\images\Surface\SurfaceTwo.png");
                                break;
                            case "S3":
                                bitmap = new Bitmap(@".\images\Surface\SurfaceThree.png");
                                break;
                            case "S4":
                                bitmap = new Bitmap(@".\images\Surface\SurfaceFour.png");
                                break;
                            case "S5":
                                bitmap = new Bitmap(@".\images\Surface\SurfaceFive.png");
                                break;
                            case "S6":
                                bitmap = new Bitmap(@".\images\Surface\SurfaceSix.png");
                                break;
                            case "S7":
                                bitmap = new Bitmap(@".\images\Surface\SurfaceSeven.png");
                                break;
                            case "S8":
                                bitmap = new Bitmap(@".\images\Surface\SurfaceEight.png");
                                break;
                            case "B1":
                                bitmap = new Bitmap(@".\images\Box\BoxOne\" + SurfaceName + "-BoxOne-1.png");
                                break;
                            case "B2":
                                bitmap = new Bitmap(@".\images\Box\BoxTwo\" + SurfaceName + "-BoxTwo-1.png");
                                break;
                            case "B3":
                                bitmap = new Bitmap(@".\images\Box\BoxThree\" + SurfaceName + "-BoxThree-1.png");
                                break;
                            case "H4":
                                bitmap = new Bitmap(@".\images\Box\BoxOne\" + SurfaceName + "-BoxOne-2.png");
                                break;
                            case "I4":
                                bitmap = new Bitmap(@".\images\Box\BoxOne\" + SurfaceName + "-BoxOne-2.png");
                                break;
                            case "J4":
                                bitmap = new Bitmap(@".\images\Box\BoxOne\" + SurfaceName + "-BoxOne-2.png");
                                break;
                            case "K4":
                                bitmap = new Bitmap(@".\images\Box\BoxOne\" + SurfaceName + "-BoxOne-2.png");
                                break;
                            case "L4":
                                bitmap = new Bitmap(@".\images\Box\BoxOne\" + SurfaceName + "-BoxOne-2.png");
                                break;
                            case "M4":
                                bitmap = new Bitmap(@".\images\Box\BoxOne\" + SurfaceName + "-BoxOne-2.png");
                                break;
                            case "N4":
                                bitmap = new Bitmap(@".\images\Box\BoxOne\" + SurfaceName + "-BoxOne-2.png");
                                break;
                            case "O4":
                                bitmap = new Bitmap(@".\images\Box\BoxOne\" + SurfaceName + "-BoxOne-2.png");
                                break;
                            case "H5":
                                bitmap = new Bitmap(@".\images\Box\BoxTwo\" + SurfaceName + "-BoxTwo-2.png");
                                break;
                            case "I5":
                                bitmap = new Bitmap(@".\images\Box\BoxTwo\" + SurfaceName + "-BoxTwo-2.png");
                                break;
                            case "J5":
                                bitmap = new Bitmap(@".\images\Box\BoxTwo\" + SurfaceName + "-BoxTwo-2.png");
                                break;
                            case "K5":
                                bitmap = new Bitmap(@".\images\Box\BoxTwo\" + SurfaceName + "-BoxTwo-2.png");
                                break;
                            case "L5":
                                bitmap = new Bitmap(@".\images\Box\BoxTwo\" + SurfaceName + "-BoxTwo-2.png");
                                break;
                            case "M5":
                                bitmap = new Bitmap(@".\images\Box\BoxTwo\" + SurfaceName + "-BoxTwo-2.png");
                                break;
                            case "N5":
                                bitmap = new Bitmap(@".\images\Box\BoxTwo\" + SurfaceName + "-BoxTwo-2.png");
                                break;
                            case "O5":
                                bitmap = new Bitmap(@".\images\Box\BoxTwo\" + SurfaceName + "-BoxTwo-2.png");
                                break;
                            case "H6":
                                bitmap = new Bitmap(@".\images\Box\BoxThree\" + SurfaceName + "-BoxThree-2.png");
                                break;
                            case "I6":
                                bitmap = new Bitmap(@".\images\Box\BoxThree\" + SurfaceName + "-BoxThree-2.png");
                                break;
                            case "J6":
                                bitmap = new Bitmap(@".\images\Box\BoxThree\" + SurfaceName + "-BoxThree-2.png");
                                break;
                            case "K6":
                                bitmap = new Bitmap(@".\images\Box\BoxThree\" + SurfaceName + "-BoxThree-2.png");
                                break;
                            case "L6":
                                bitmap = new Bitmap(@".\images\Box\BoxThree\" + SurfaceName + "-BoxThree-2.png");
                                break;
                            case "M6":
                                bitmap = new Bitmap(@".\images\Box\BoxThree\" + SurfaceName + "-BoxThree-2.png");
                                break;
                            case "N6":
                                bitmap = new Bitmap(@".\images\Box\BoxThree\" + SurfaceName + "-BoxThree-2.png");
                                break;
                            case "O6":
                                bitmap = new Bitmap(@".\images\Box\BoxThree\" + SurfaceName + "-BoxThree-2.png");
                                break;
                            case "E1":
                                bitmap = new Bitmap(@".\images\End\EndBeige\" + SurfaceName + "-EndBeige.png");
                                break;
                            case "E2":
                                bitmap = new Bitmap(@".\images\End\EndBlack\" + SurfaceName + "-EndBlack.png");
                                break;
                            case "E3":
                                bitmap = new Bitmap(@".\images\End\EndBlue\" + SurfaceName + "-EndBlue.png");
                                break;
                            case "E4":
                                bitmap = new Bitmap(@".\images\End\EndBrown\" + SurfaceName + "-EndBrown.png");
                                break;
                            case "E5":
                                bitmap = new Bitmap(@".\images\End\EndGray\" + SurfaceName + "-EndGray.png");
                                break;
                            case "E6":
                                bitmap = new Bitmap(@".\images\End\EndPurple\" + SurfaceName + "-EndPurple.png");
                                break;
                            case "E7":
                                bitmap = new Bitmap(@".\images\End\EndRed\" + SurfaceName + "-EndRed.png");
                                break;
                            case "E8":
                                bitmap = new Bitmap(@".\images\End\EndYellow\" + SurfaceName + "-EndYellow.png");
                                break;
                            case "C1":
                                bitmap = new Bitmap(@".\images\Character\Up\" + SurfaceName + "-Up.gif");
                                break;
                            case "C2":
                                bitmap = new Bitmap(@".\images\Character\Down\" + SurfaceName + "-Down.gif");
                                break;
                            case "C3":
                                bitmap = new Bitmap(@".\images\Character\Left\" + SurfaceName + "-Left.gif");
                                break;
                            case "C4":
                                bitmap = new Bitmap(@".\images\Character\Right\" + SurfaceName + "-Right.gif");
                                break;
                            case "CH":
                                bitmap = new Bitmap(@".\images\Character\EndCharacter\EndCharacter-Beige.png");
                                break;
                            case "CI":
                                bitmap = new Bitmap(@".\images\Character\EndCharacter\EndCharacter-Black.png");
                                break;
                            case "CJ":
                                bitmap = new Bitmap(@".\images\Character\EndCharacter\EndCharacter-Blue.png");
                                break;
                            case "CK":
                                bitmap = new Bitmap(@".\images\Character\EndCharacter\EndCharacter-Brown.png");
                                break;
                            case "CL":
                                bitmap = new Bitmap(@".\images\Character\EndCharacter\EndCharacter-Gray.png");
                                break;
                            case "CM":
                                bitmap = new Bitmap(@".\images\Character\EndCharacter\EndCharacter-Purple.png");
                                break;
                            case "CN":
                                bitmap = new Bitmap(@".\images\Character\EndCharacter\EndCharacter-Red.png");
                                break;
                            case "CO":
                                bitmap = new Bitmap(@".\images\Character\EndCharacter\EndCharacter-Yellow.png");
                                break;
                            case "W1":
                                bitmap = new Bitmap(@".\images\Wall\WallOne.png");
                                break;
                            case "W2":
                                bitmap = new Bitmap(@".\images\Wall\WallTwo.png");
                                break;
                            case "W3":
                                bitmap = new Bitmap(@".\images\Wall\WallThree.png");
                                break;
                            case "W4":
                                bitmap = new Bitmap(@".\images\Wall\WallFour.png");
                                break;
                        }
                        g.DrawImage(bitmap, j * 20, i * 20, 20, 20);
                    }
                }
                pictureBox.Image = bmp;


                label = new Label();
                label.Click += Label_Click;
                label.AutoSize = true;
                label.Text = GetMapData[num, 0];
                label.Font = new Font("宋体", 12f, FontStyle.Regular);
                Controls.Add(label);
                int labelX = ((pictureBox.Width / 2 + pictureBox.Bounds.X) - (label.Width / 2));
                label.SetBounds(labelX, 47 + yNum * 324, 0, 0);

                if (xNum == 2)
                {
                    xNum = 0;
                    yNum += 1;
                }
                else
                    xNum += 1;

            }
        }

        // 删除地图
        private void delMap_Click(object sender, EventArgs e)
        {
            new DelMap().Show();
        }

        //载入地图
        private void Label_Click(object sender, EventArgs e)
        {
            string MapName = ((Label)sender).Text;
            if (MessageBox.Show("是否要修改地图-" + MapName + "?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //获取单击地图的数据
                for (int i = 0;i < GetMapData.GetLength(0);i++)
                {
                    if (GetMapData[i, 0] == MapName)
                    {
                        string[,] Map = new string[15, 15];
                        for (int j = 0;j < 15;j++)
                        {
                            string code = GetMapData[i, 1].Substring(j * 30, 30);
                            for (int k = 0;k < 15;k++)
                            {
                                Map[j, k] = code.Substring(k * 2, 2);
                            }
                        }
                        Form5.mapId = Convert.ToInt32(GetMapData[i, 3]);
                        Form5.MapSurface = GetMapData[i, 2];
                        Form5.LabelName = MapName;
                        Form5.cln();
                        Form5.Map = Map;
                        Form5.ddm();
                    }
                }
                check -= 1;
                Dispose();
            }
        }

        //防止打开多个窗口
        private void FormSc_FormClosed(object sender, FormClosedEventArgs e)
        {
            switch (Text)
            {
                case "已做的地图":
                    check -= 1;
                    break;
                case "保存":
                    check -= 2;
                    break;
                case "完成制作":
                    check -= 3;
                    break;
            }
        }

        //获取焦点
        public void GetFocus()
        {
            this.Focus();
        }

        // 委托-重新展示完成制作的地图
        public void reshowDoneMap()
        {
            draw_DoneMap();
        }
    }
}
