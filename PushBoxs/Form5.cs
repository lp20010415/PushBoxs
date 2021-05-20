using System;
using System.Collections;
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

namespace PushBoxs
{
    public delegate void DrawDoneMap();//委托-画已做的地图
    public delegate void ChangeLabelName();//委托-改变"当前地图名称"
    public delegate void RecoverMapModel();//委托-恢复默认绘制地图模板
    public delegate void ShutdownForm();//委托-关闭窗口
    public partial class Form5 : Form
    {
        private string code = "";//地面代码
        private string getUrl = "";//获取图片地址
        private bool CheckSave = false;//检测是否保存
        private string chooseSurface = "";//选择的地面
        private int x, y, x1, y1, x2, y2;//全局变量-pictureBox的位置
        private bool CheckClick = false;//picture移动
        private int fmpX = 5, fmpY = 10;//选择地面界面选项位置
        private int ImagesNumX = 0, ImagesNumY = 0;//pictureBox的位置
        public string LoginName = "";//登录名

        //控件
        private PictureBox p;//主界面-显示图片
        private PictureBox fmp;//选择地面界面-显示图片
        private Form fm;//选择地面界面

        //画地图定义
        public static string LabelName = "";//改变"当前地图名称"
        private Bitmap bmp;//绘制地图所需
        private Graphics g;//绘制地图所需
        private Bitmap bitmap;//绘制地图所需
        public static string[,] Map = new string[15, 15];//载入地图-地图数据
        public static string MapSurface = "";//载入地图-地面数据
        private string MapCode = "";//地图数据

        public static DrawDoneMap ddm;//委托-画已做的地图
        public static ChangeLabelName cln;//委托-改变"当前地图名称"
        public static RecoverMapModel rmm;//委托-恢复默认绘制地图模板
        public static ShutdownForm sf;//委托-关闭窗口
        public Form5()
        {
            InitializeComponent();

            //使一些控件不会被获取到按键焦点
            SetGetFocus(comboBox1);
            SetGetFocus(comboBox2);
            SetGetFocus(comboBox3);
            SetGetFocus(comboBox4);
            SetGetFocus(comboBox5);

            ddm = new DrawDoneMap(DrawDMap);//委托-画已做的地图
            cln = new ChangeLabelName(ChangeLName);//委托-改变"当前地图名称"
            rmm = new RecoverMapModel(DefaultMapModel);//委托-恢复默认绘制地图模板
            sf = new ShutdownForm(CloseForm);//委托-关闭窗口
            //获取登录账号
            label7.Text += LoginName;

            //画地图定义
            bmp = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            g = Graphics.FromImage(bmp);

            //创建选择地面界面
            fm = new Form();
            fm.Text = "请选择";
            fm.SetBounds(0, 0, 240, 170);
            fm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            fm.TopMost = true;
            fm.Show();

            //获取Surface
            string SurfacePath = @"..\..\images\Surface\";
            DirectoryInfo getSurface = new DirectoryInfo(SurfacePath);
            foreach (FileInfo f in getSurface.GetFiles())
            {
                string SurfaceImg = SurfacePath + f.Name;
                fmp = new PictureBox();
                fmp.BackgroundImageLayout = ImageLayout.Stretch;
                fmp.BackgroundImage = Image.FromFile(SurfaceImg);
                fmp.ImageLocation = SurfaceImg;
                fmp.SetBounds(fmpX, fmpY, 50, 50);
                fmp.MouseDown += Fmp_MouseDown;
                fm.Controls.Add(fmp);
                fmpX += 55;
                if (fmpX == 225)
                {
                    fmpX = 5;
                    fmpY += 60;
                }
            }
            pictureBox3.BringToFront();
        }

        //按钮提示
        private void button2_MouseEnter(object sender, EventArgs e)
        {
            Tip.Active = true;
            Tip.SetToolTip(button2, "仅保存，不能玩");
        }

        //按钮提示
        private void button3_MouseEnter(object sender, EventArgs e)
        {
            Tip.Active = true;
            Tip.SetToolTip(button3, "地图能玩");
        }

        //按钮事件
        private void button_Click(object sender, EventArgs e)
        {
            //显示选项卡及隐藏面板
            comboBox1.Show();
            comboBox2.Show();
            comboBox3.Show();
            comboBox4.Show();
            comboBox5.Show();
            panel1.Hide();
            panel2.Hide();
            panel3.Hide();
            panel4.Hide();
            panel5.Hide();
            bool checkBlock = false;//检测地图是否箱子、终点、人物各一个。
            if (Regex.IsMatch(MapCode, "(?=.*[B-C].*)(?=.*(E).*)(?=.*[H-O].*)+"))
                checkBlock = true;
            else
                MessageBox.Show("箱子、终点、人物需各一个", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //开始事件
            switch (((Button)sender).Text)
            {
                case "查看已做的地图":
                    if (FormSc.check == 0)
                    {
                        FormSc.LoginName = LoginName;
                        FormSc.check += 1;
                        Form sc = new FormSc();
                        sc.Text = "已做的地图";
                        sc.AutoScroll = true;
                        sc.SetBounds(500, 500, 972, 735);
                        sc.Show();
                    }
                    else
                        FormSc.focus();
                    break;
                case "保存":
                    if (checkBlock)
                    {
                        if (FormSc.check == 0)
                        {
                            string check = label8.Text.Replace("当前地图名称:", "");
                            if (check == "")//无地图名称-需在地图数据后面加上"Save"
                            {
                                FormSc.LoginName = LoginName;
                                FormSc.MapCode = MapCode + "Save";
                                FormSc.chooseSurface = chooseSurface;
                                Form sc = new FormSc();
                                sc.Text = "保存";
                                sc.SetBounds(500, 500, 210, 135);
                                sc.AutoScroll = false;
                                sc.Show();
                                FormSc.check += 2;
                            }
                            else//有地图名称
                            {
                                if (MessageBox.Show("是否保存?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                                {
                                    try
                                    {
                                        SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                                        conn.Open();
                                        SqlCommand comm = conn.CreateCommand();
                                        comm.CommandText = "Update MapData.MapData_" + LoginName + "set MapData='" + MapCode + "' where Name='" + check + "'";
                                        int checks = comm.ExecuteNonQuery();
                                        if (checks > 0)
                                            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK);
                                        conn.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("出错原因:" + ex.Message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                        }
                        else
                            FormSc.focus();
                    }
                    break;
                case "完成制作":
                    if (checkBlock)
                    {
                        string getMapName = label8.Text.Replace("当前地图名称:", "");//获取地图名称
                        if (FormSc.check == 0)
                        {
                            if (getMapName == "")
                            {
                                FormSc.LoginName = LoginName;
                                FormSc.MapCode = MapCode + "Done";
                                FormSc.chooseSurface = chooseSurface;
                                Form sc = new FormSc();
                                sc.Text = "完成制作";
                                sc.SetBounds(500, 500, 210, 135);
                                sc.AutoScroll = false;
                                sc.Show();
                                FormSc.check += 3;
                            }
                            else
                            {
                                if (MessageBox.Show("是否完成制作?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                                {
                                    //地图数据处理
                                    MapCode = MapCode.Remove(225, 4);
                                    MapCode = MapCode.Insert(225, "Done");
                                    //选择地面数据处理
                                    string ChooseSurface = chooseSurface;
                                    //数据库连接等操作
                                    try
                                    {
                                        SqlConnection conn = new SqlConnection("Server=.;Database=c#Traning report;uid=sa;pwd=123456");
                                        conn.Open();
                                        SqlCommand comm = conn.CreateCommand();
                                        comm.CommandText = "Update MapData.MapData_" + LoginName + " set MapData ='" + MapCode + "' where Name ='" + getMapName + "'";
                                        int check = comm.ExecuteNonQuery();
                                        if (check > 0)
                                            MessageBox.Show("已完成制作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        conn.Close();
                                        DefaultMapModel();
                                        label8.Text = "当前地图名称:";
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("出错原因:" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                        }
                    }
                    break;

            }
        }

        //加载数据
        private void Fmp_MouseDown(object sender, MouseEventArgs e)
        {
            string pattern = "..\\..\\images\\Surface\\";
            string get = ((PictureBox)sender).ImageLocation;
            chooseSurface = get.Replace(pattern, "");
            chooseSurface = chooseSurface.Replace(".png", "");
            this.Enabled = true;

            //加载箱子
            string BoxPath = @"..\..\images\Box\";
            DirectoryInfo GetBox = new DirectoryInfo(BoxPath);
            foreach (DirectoryInfo f in GetBox.GetDirectories())
            {
                string DirectoryName = f.Name;
                string PathSon = BoxPath + DirectoryName;
                DirectoryInfo GetFile = new DirectoryInfo(PathSon);
                foreach (FileInfo fi in GetFile.GetFiles())
                {
                    string FileName = fi.Name;
                    string PathSonFile = PathSon + "\\" + FileName;
                    if (!Regex.IsMatch(PathSonFile, "[2]+") && Regex.IsMatch(PathSonFile, "(" + chooseSurface + ")+"))
                    {
                        p = new PictureBox();
                        p.ImageLocation = PathSonFile;
                        p.SizeMode = PictureBoxSizeMode.StretchImage;
                        p.SetBounds(ImagesNumX * 25, ImagesNumY * 25, 25, 25);
                        p.MouseDown += P_MouseDown;
                        panel1.Controls.Add(p);
                        ImagesNumX += 1;
                        if (ImagesNumX == 5)
                        {
                            ImagesNumX = 0;
                            ImagesNumY += 1;
                        }
                    }

                }
            }

            //加载人物
            ImagesNumX = 0; ImagesNumY = 0;
            string CharacterPath = @"..\..\images\Character\";
            DirectoryInfo GetCharacrer = new DirectoryInfo(CharacterPath);
            foreach (DirectoryInfo f in GetCharacrer.GetDirectories())
            {
                string DirectoryName = f.Name;
                string PathSon = CharacterPath + DirectoryName;
                DirectoryInfo GetFile = new DirectoryInfo(PathSon);
                foreach (FileInfo fi in GetFile.GetFiles())
                {
                    string FileName = fi.Name;
                    string PathSonFile = PathSon + "\\" + FileName;
                    if (!Regex.IsMatch(PathSonFile, "[2]+") && Regex.IsMatch(PathSonFile, "(" + chooseSurface + ")+"))
                    {
                        p = new PictureBox();
                        p.ImageLocation = PathSonFile;
                        p.SizeMode = PictureBoxSizeMode.StretchImage;
                        p.SetBounds(ImagesNumX * 25, ImagesNumY * 25, 25, 25);
                        p.MouseDown += P_MouseDown;
                        panel2.Controls.Add(p);
                        ImagesNumX += 1;
                        if (ImagesNumX == 5)
                        {
                            ImagesNumX = 0;
                            ImagesNumY += 1;
                        }
                    }
                }
            }

            //加载表面
            ImagesNumX = 0; ImagesNumY = 0;
            p = new PictureBox();
            p.ImageLocation = get;
            p.SizeMode = PictureBoxSizeMode.StretchImage;
            p.SetBounds(ImagesNumX * 25, ImagesNumY * 25, 25, 25);
            p.MouseDown += P_MouseDown;
            panel4.Controls.Add(p);

            //加载墙
            ImagesNumX = 0; ImagesNumY = 0;
            string WallPath = @"..\..\images\Wall\";
            DirectoryInfo GetWall = new DirectoryInfo(WallPath);
            foreach (FileInfo f in GetWall.GetFiles())
            {
                string PathSonFile = GetWall + f.Name;
                p = new PictureBox();
                p.ImageLocation = PathSonFile;
                p.SizeMode = PictureBoxSizeMode.StretchImage;
                p.SetBounds(ImagesNumX * 25, ImagesNumY * 25, 25, 25);
                p.MouseDown += P_MouseDown;
                panel3.Controls.Add(p);
                ImagesNumX += 1;
                if (ImagesNumX == 5)
                {
                    ImagesNumX = 0;
                    ImagesNumY += 1;
                }
            }

            //加载终点
            ImagesNumX = 0; ImagesNumY = 0;
            string EndPath = @"..\..\images\End\";
            DirectoryInfo GetEnd = new DirectoryInfo(EndPath);
            foreach (DirectoryInfo f in GetEnd.GetDirectories())
            {
                string DirectoryName = f.Name;
                string PathSon = EndPath + DirectoryName;
                DirectoryInfo GetFile = new DirectoryInfo(PathSon);
                foreach (FileInfo fi in GetFile.GetFiles())
                {
                    string FileName = fi.Name;
                    string PathSonFile = PathSon + "\\" + FileName;
                    if (!Regex.IsMatch(PathSonFile, "[2]+") && Regex.IsMatch(PathSonFile, "(" + chooseSurface + ")+"))
                    {
                        p = new PictureBox();
                        p.ImageLocation = PathSonFile;
                        p.SizeMode = PictureBoxSizeMode.StretchImage;
                        p.SetBounds(ImagesNumX * 25, ImagesNumY * 25, 25, 25);
                        p.MouseDown += P_MouseDown;
                        panel5.Controls.Add(p);
                        ImagesNumX += 1;
                        if (ImagesNumX == 5)
                        {
                            ImagesNumX = 0;
                            ImagesNumY += 1;
                        }
                    }
                }
            }

            //地面代码
            if (Regex.IsMatch(get, "(One)+"))
                code = "S1";
            else if (Regex.IsMatch(get, "(Two)+"))
                code = "S2";
            else if (Regex.IsMatch(get, "(Three)+"))
                code = "S3";
            else if (Regex.IsMatch(get, "(Four)+"))
                code = "S4";
            else if (Regex.IsMatch(get, "(Five)+"))
                code = "S5";
            else if (Regex.IsMatch(get, "(Six)+"))
                code = "S6";
            else if (Regex.IsMatch(get, "(Seven)+"))
                code = "S7";
            else if (Regex.IsMatch(get, "(Eight)+"))
                code = "S8";
            getUrl = get;
            DefaultMapModel();
            fm.Close();

        }

        //移动操作
        private void P_MouseDown(object sender, MouseEventArgs e)
        {
            //位置获取
            x1 = ((PictureBox)sender).Bounds.X;
            y1 = ((PictureBox)sender).Bounds.Y;
            switch (((PictureBox)sender).Parent.Name)//获取父控件来决定值
            {
                case "panel1":
                    x = panel1.Left + x1;
                    y = panel1.Top + y1;
                    break;
                case "panel2":
                    x = panel2.Left + x1;
                    y = panel2.Top + y1;
                    break;
                case "panel3":
                    x = panel3.Left + x1;
                    y = panel3.Top + y1;
                    break;
                case "panel4":
                    x = panel4.Left + x1;
                    y = panel4.Top + y1;
                    break;
                case "panel5":
                    x = panel5.Left + x1;
                    y = panel5.Top + y1;
                    break;
            }
            x2 = e.X;
            y2 = e.Y;

            //选中相关设置
            pictureBox3.Visible = true;
            pictureBox3.ImageLocation = ((PictureBox)sender).ImageLocation;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SetBounds(x, y, 25, 25);
            pictureBox3.Show();
            pictureBox3_MouseDown(pictureBox3, e);

            switch (((PictureBox)sender).Parent.Name)//获取父控件来决定
            {
                case "panel1":
                    comboBox1.Show();
                    panel1.Hide();
                    break;
                case "panel2":
                    comboBox2.Show();
                    panel2.Hide();
                    break;
                case "panel3":
                    comboBox3.Show();
                    panel3.Hide();
                    break;
                case "panel4":
                    comboBox4.Show();
                    panel4.Hide();
                    break;
                case "panel5":
                    comboBox5.Show();
                    panel5.Hide();
                    break;
            }

            pictureBox1.ImageLocation = ((PictureBox)sender).ImageLocation;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        //窗口加载
        private void Form5_Load(object sender, EventArgs e)
        {
            label7.Text = "当前登录账号:" + LoginName;
        }

        //移动操作
        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            int i, j;
            i = pictureBox3.Left + e.X - x2;
            j = pictureBox3.Top + e.Y - y2;
            if (CheckClick)
            {
                pictureBox3.SetBounds(i, j, 25, 25);
                //pictureBox3.Location = new Point(i, j);
            }
        }
        
        //是否保存-显示玩的界面
        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool checkBlock = false;//检测地图是否箱子、终点、人物各一个。
            if (Regex.IsMatch(MapCode, "(?=.*[B-C].*)(?=.*(E).*)(?=.*[H-O].*)+"))
                checkBlock = true;
            else
                MessageBox.Show("箱子、终点、人物需各一个", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (checkBlock)
            {
                string check = label8.Text.Replace("当前地图名称:", "");
                if (FormSc.check == 0)
                {
                    if (check == "")//无地图名称-需在地图数据后面加上"Save"
                    {
                        Form sc = new FormSc();
                        sc.Text = "保存-关闭窗口";
                        sc.SetBounds(500, 500, 210, 135);
                        sc.AutoScroll = false;
                        sc.Show();
                        FormSc.LoginName = LoginName;
                        FormSc.MapCode = MapCode + "Save";
                        FormSc.chooseSurface = chooseSurface;
                        FormSc.check += 2;
                    }
                    else//有地图名称
                    {
                        if (MessageBox.Show("是否保存?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            try
                            {
                                SqlConnection conn = new SqlConnection("Server=.;DataBase=c#Traning report;uid=sa;pwd=123456");
                                conn.Open();
                                SqlCommand comm = conn.CreateCommand();
                                comm.CommandText = "Update MapData.MapData_" + LoginName + "set MapData='" + MapCode + "' where Name=" + check;
                                int checks = comm.ExecuteNonQuery();
                                if (checks > 0)
                                    MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK);
                                conn.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("出错原因:" + ex.Message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }

                Console.WriteLine(e.CloseReason);
                switch (e.CloseReason)
                {
                    case CloseReason.UserClosing:
                        e.Cancel = true;
                        break;
                }
                Enabled = false;
            }
        }

        //绘制地图相关操作
        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            int xx, xxx, yy, yyy;
            xx = pictureBox2.Left;
            yy = pictureBox2.Top;
            xxx = pictureBox2.Right;
            yyy = pictureBox2.Bottom;

            int xx1, yy1;
            xx1 = pictureBox3.Bounds.X + e.X;
            yy1 = pictureBox3.Bounds.Y + e.Y;

            String FileUrl = ((PictureBox)sender).ImageLocation;

            if (CheckClick)
            {
                if (xx1 > xx && xx1 < xxx && yy1 > yy && yy < yyy)
                {
                    int jX, jY;
                    jX = (int)(xx1 - xx) / 25;
                    jY = (int)(yy1 - yy) / 25;

                    Console.WriteLine(jX + "-" + jY);

                    //箱子
                    if (Regex.IsMatch(FileUrl, "(Box)+"))
                    {
                        bool CheckChoose = false;
                        switch (Map[jX, jY])//判断该项是否为人物
                        {
                            case "C1"://上
                                comboBox2.Enabled = true;
                                break;
                            case "C2"://下
                                comboBox2.Enabled = true;
                                break;
                            case "C3"://左
                                comboBox2.Enabled = true;
                                break;
                            case "C4"://右
                                comboBox2.Enabled = true;
                                break;
                            //人物在终点上
                            case "CH":
                                comboBox2.Enabled = true;
                                break;
                            case "CI":
                                comboBox2.Enabled = true;
                                break;
                            case "CJ":
                                comboBox2.Enabled = true;
                                break;
                            case "CK":
                                comboBox2.Enabled = true;
                                break;
                            case "CL":
                                comboBox2.Enabled = true;
                                break;
                            case "CM":
                                comboBox2.Enabled = true;
                                break;
                            case "CN":
                                comboBox2.Enabled = true;
                                break;
                            case "CO":
                                comboBox2.Enabled = true;
                                break;
                        }

                        if (Regex.IsMatch(Map[jX, jY], "(E)+"))
                        {
                            if (MessageBox.Show("是否放在终点上?", "选择", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                CheckChoose = true;
                            else
                                CheckChoose = false;
                        }

                        if (CheckChoose)
                        {
                            if (Regex.IsMatch(FileUrl, "(One)+"))
                            {
                                MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                                string get = GetEndCode(FileUrl, Map[jX, jY]);
                                MapCode = MapCode.Insert(jX * 2 + jY * 30, get);
                                Map[jX, jY] = get;
                                FileUrl = FileUrl.Replace("1", "2");
                            }
                            else if (Regex.IsMatch(FileUrl, "(Two)+"))
                            {
                                MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                                string get = GetEndCode(FileUrl, Map[jX, jY]);
                                MapCode = MapCode.Insert(jX * 2 + jY * 30, get);
                                Map[jX, jY] = get;
                                FileUrl = FileUrl.Replace("1", "2");
                            }
                            else if (Regex.IsMatch(FileUrl, "(Three)+"))
                            {
                                MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                                string get = GetEndCode(FileUrl, Map[jX, jY]);
                                MapCode = MapCode.Insert(jX * 2 + jY * 30, get);
                                Map[jX, jY] = get;
                                FileUrl = FileUrl.Replace("1", "2");
                            }
                        }
                        else
                        {
                            if (Regex.IsMatch(FileUrl, "(One)+"))
                            {
                                MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                                MapCode = MapCode.Insert(jX * 2 + jY * 30, "B1");
                                Map[jX, jY] = "B1";
                            }
                            else if (Regex.IsMatch(FileUrl, "(Two)+"))
                            {
                                MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                                MapCode = MapCode.Insert(jX * 2 + jY * 30, "B2");
                                Map[jX, jY] = "B2";
                            }
                            else if (Regex.IsMatch(FileUrl, "(Three)+"))
                            {
                                MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                                MapCode = MapCode.Insert(jX * 2 + jY * 30, "B3");
                                Map[jX, jY] = "B3";
                            }
                        }
                    }
                    //人物
                    else if (Regex.IsMatch(FileUrl, ("Character+")))
                    {
                        bool CheckChoose = false;
                        if (Regex.IsMatch(Map[jX, jY], "(E)+"))
                        {
                            if (MessageBox.Show("是否放在终点上?", "选择", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                CheckChoose = true;
                            else
                                CheckChoose = false;
                        }
                        if (CheckChoose)
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "C5");
                            switch (Map[jX, jY])
                            {
                                case "E1":
                                    Map[jX, jY] = "CH";
                                    FileUrl = @"..\..\images\End\EndCharacter\EndCharacter-Beige.png";
                                    break;
                                case "E2":
                                    Map[jX, jY] = "CI";
                                    FileUrl = @"..\..\images\End\EndCharacter\EndCharacter-Black.png";
                                    break;
                                case "E3":
                                    Map[jX, jY] = "CJ";
                                    FileUrl = @"..\..\images\End\EndCharacter\EndCharacter-Blue.png";
                                    break;
                                case "E4":
                                    Map[jX, jY] = "CK";
                                    FileUrl = @"..\..\images\End\EndCharacter\EndCharacter-Brown.png";
                                    break;
                                case "E5":
                                    Map[jX, jY] = "CL";
                                    FileUrl = @"..\..\images\End\EndCharacter\EndCharacter-Gray.png";
                                    break;
                                case "E6":
                                    Map[jX, jY] = "CM";
                                    FileUrl = @"..\..\images\End\EndCharacter\EndCharacter-Purple.png";
                                    break;
                                case "E7":
                                    Map[jX, jY] = "CN";
                                    FileUrl = @"..\..\images\End\EndCharacter\EndCharacter-Red.png";
                                    break;
                                case "E8":
                                    Map[jX, jY] = "CO";
                                    FileUrl = @"..\..\images\End\EndCharacter\EndCharacter-Yellow.png";
                                    break;
                            }
                            comboBox2.Enabled = false;
                        }
                        else
                        {
                            if (Regex.IsMatch(FileUrl, "(Up)+"))//上
                            {
                                Console.WriteLine(MapCode);
                                MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                                Console.WriteLine(MapCode);
                                MapCode = MapCode.Insert(jX * 2 + jY * 30, "C1");
                                Console.WriteLine(MapCode);
                                Map[jX, jY] = "C1";
                                comboBox2.Enabled = false;
                            }
                            if (Regex.IsMatch(FileUrl, "(Down)+"))//下
                            {
                                MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                                MapCode = MapCode.Insert(jX * 2 + jY * 30, "C2");
                                Map[jX, jY] = "C2";
                                comboBox2.Enabled = false;
                            }
                            if (Regex.IsMatch(FileUrl, "(Left)+"))//左
                            {
                                MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                                MapCode = MapCode.Insert(jX * 2 + jY * 30, "C3");
                                Map[jX, jY] = "C3";
                                comboBox2.Enabled = false;
                            }
                            if (Regex.IsMatch(FileUrl, "(Right)+"))//右
                            {
                                MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                                MapCode = MapCode.Insert(jX * 2 + jY * 30, "C4");
                                Map[jX, jY] = "C4";
                                comboBox2.Enabled = false;
                            }
                        }

                    }
                    //墙
                    else if (Regex.IsMatch(FileUrl, "(Wall)+"))
                    {
                        switch (Map[jX, jY])//判断该项是否为人物
                        {
                            case "C1"://上
                                comboBox2.Enabled = true;
                                break;
                            case "C2"://下
                                comboBox2.Enabled = true;
                                break;
                            case "C3"://左
                                comboBox2.Enabled = true;
                                break;
                            case "C4"://右
                                comboBox2.Enabled = true;
                                break;
                            //人物在终点上
                            case "CH":
                                comboBox2.Enabled = true;
                                break;
                            case "CI":
                                comboBox2.Enabled = true;
                                break;
                            case "CJ":
                                comboBox2.Enabled = true;
                                break;
                            case "CK":
                                comboBox2.Enabled = true;
                                break;
                            case "CL":
                                comboBox2.Enabled = true;
                                break;
                            case "CM":
                                comboBox2.Enabled = true;
                                break;
                            case "CN":
                                comboBox2.Enabled = true;
                                break;
                            case "CO":
                                comboBox2.Enabled = true;
                                break;
                        }

                        if (Regex.IsMatch(FileUrl, "One+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "W1");
                            Map[jX, jY] = "W1";
                        }
                        if (Regex.IsMatch(FileUrl, "Two+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "W2");
                            Map[jX, jY] = "W2";
                        }
                        if (Regex.IsMatch(FileUrl, "Three+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "W3");
                            Map[jX, jY] = "W3";
                        }
                        if (Regex.IsMatch(FileUrl, "Four+"))
                        {
                            Console.WriteLine(MapCode);
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            Console.WriteLine(MapCode);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "W4");
                            Console.WriteLine(MapCode);
                            Map[jX, jY] = "W4";
                        }
                    }
                    //终点
                    else if (Regex.IsMatch(FileUrl, "(End)+"))
                    {
                        switch (Map[jX, jY])//判断该项是否为人物
                        {
                            case "C1"://上
                                comboBox2.Enabled = true;
                                break;
                            case "C2"://下
                                comboBox2.Enabled = true;
                                break;
                            case "C3"://左
                                comboBox2.Enabled = true;
                                break;
                            case "C4"://右
                                comboBox2.Enabled = true;
                                break;
                            //人物在终点上
                            case "CH":
                                comboBox2.Enabled = true;
                                break;
                            case "CI":
                                comboBox2.Enabled = true;
                                break;
                            case "CJ":
                                comboBox2.Enabled = true;
                                break;
                            case "CK":
                                comboBox2.Enabled = true;
                                break;
                            case "CL":
                                comboBox2.Enabled = true;
                                break;
                            case "CM":
                                comboBox2.Enabled = true;
                                break;
                            case "CN":
                                comboBox2.Enabled = true;
                                break;
                            case "CO":
                                comboBox2.Enabled = true;
                                break;
                        }

                        if (Regex.IsMatch(FileUrl, "EndBeige+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "E1");
                            Map[jX, jY] = "E1";
                        }
                        if (Regex.IsMatch(FileUrl, "EndBlack+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "E2");
                            Map[jX, jY] = "E2";
                        }
                        if (Regex.IsMatch(FileUrl, "EndBlue+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "E3");
                            Map[jX, jY] = "E3";
                        }
                        if (Regex.IsMatch(FileUrl, "EndBrown+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "E4");
                            Map[jX, jY] = "E4";
                        }
                        if (Regex.IsMatch(FileUrl, "EndGray+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "E5");
                            Map[jX, jY] = "E5";
                        }
                        if (Regex.IsMatch(FileUrl, "EndPurple+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "E6");
                            Map[jX, jY] = "E6";
                        }
                        if (Regex.IsMatch(FileUrl, "EndRed+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "E7");
                            Map[jX, jY] = "E7";
                        }
                        if (Regex.IsMatch(FileUrl, "EndYellow+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "E8");
                            Map[jX, jY] = "E8";
                        }
                    }
                    //地面
                    else
                    {
                        switch (Map[jX, jY])//判断该项是否为人物
                        {
                            case "C1"://上
                                comboBox2.Enabled = true;
                                break;
                            case "C2"://下
                                comboBox2.Enabled = true;
                                break;
                            case "C3"://左
                                comboBox2.Enabled = true;
                                break;
                            case "C4"://右
                                comboBox2.Enabled = true;
                                break;
                            //人物在终点上
                            case "CH":
                                comboBox2.Enabled = true;
                                break;
                            case "CI":
                                comboBox2.Enabled = true;
                                break;
                            case "CJ":
                                comboBox2.Enabled = true;
                                break;
                            case "CK":
                                comboBox2.Enabled = true;
                                break;
                            case "CL":
                                comboBox2.Enabled = true;
                                break;
                            case "CM":
                                comboBox2.Enabled = true;
                                break;
                            case "CN":
                                comboBox2.Enabled = true;
                                break;
                            case "CO":
                                comboBox2.Enabled = true;
                                break;
                        }

                        if (Regex.IsMatch(FileUrl, "One+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "S1");
                            Map[jX, jY] = "S1";
                        }
                        if (Regex.IsMatch(FileUrl, "Two+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "S2");
                            Map[jX, jY] = "S2";
                        }
                        if (Regex.IsMatch(FileUrl, "Three+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "S3");
                            Map[jX, jY] = "S3";
                        }
                        if (Regex.IsMatch(FileUrl, "Four+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "S4");
                            Map[jX, jY] = "S4";
                        }
                        if (Regex.IsMatch(FileUrl, "Five+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "S5");
                            Map[jX, jY] = "S5";
                        }
                        if (Regex.IsMatch(FileUrl, "Six+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "S6");
                            Map[jX, jY] = "S6";
                        }
                        if (Regex.IsMatch(FileUrl, "Seven+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "S7");
                            Map[jX, jY] = "S7";
                        }
                        if (Regex.IsMatch(FileUrl, "Eight+"))
                        {
                            MapCode = MapCode.Remove(jX * 2 + jY * 30, 2);
                            MapCode = MapCode.Insert(jX * 2 + jY * 30, "S8");
                            Map[jX, jY] = "S8";
                        }
                    }

                    CheckSave = false;
                    bitmap = new Bitmap(FileUrl);
                    g.DrawImage(bitmap, jX * 25, jY * 25, 25, 25);
                    pictureBox2.Image = bmp;
                    pictureBox1.Image = null;
                }
                pictureBox3.Hide();
                CheckClick = false;
            }
            else
                CheckClick = true;
        }

        //显示选项卡及隐藏面板
        private void Form5_MouseDown(object sender, MouseEventArgs e)
        {
            comboBox1.Show();
            comboBox2.Show();
            comboBox3.Show();
            comboBox4.Show();
            comboBox5.Show();
            panel1.Hide();
            panel2.Hide();
            panel3.Hide();
            panel4.Hide();
            panel5.Hide();
        }

        //需要选择地面提示
        private void Form5_MouseClick(object sender, MouseEventArgs e)
        {
            if (!this.Enabled)
            {
                MessageBox.Show("请选择一个表面!", "提示", MessageBoxButtons.OK);
            }
        }

        //选项卡操作
        private void comboBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (((ComboBox)sender).Name)
            {
                case "comboBox1":
                    comboBox1.Hide();
                    comboBox2.Show();
                    comboBox3.Show();
                    comboBox4.Show();
                    comboBox5.Show();
                    panel1.Show();
                    panel2.Hide();
                    panel3.Hide();
                    panel4.Hide();
                    panel5.Hide();

                    break;
                case "comboBox2":
                    comboBox2.Hide();
                    comboBox1.Show();
                    comboBox3.Show();
                    comboBox4.Show();
                    comboBox5.Show();
                    panel2.Show();
                    panel1.Hide();
                    panel3.Hide();
                    panel4.Hide();
                    panel5.Hide();
                    break;
                case "comboBox3":
                    comboBox3.Hide();
                    comboBox1.Show();
                    comboBox2.Show();
                    comboBox4.Show();
                    comboBox5.Show();
                    panel3.Show();
                    panel1.Hide();
                    panel2.Hide();
                    panel4.Hide();
                    panel5.Hide();
                    break;
                case "comboBox4":
                    comboBox4.Hide();
                    comboBox1.Show();
                    comboBox2.Show();
                    comboBox3.Show();
                    comboBox5.Show();
                    panel4.Show();
                    panel1.Hide();
                    panel2.Hide();
                    panel3.Hide();
                    panel5.Hide();
                    break;
                case "comboBox5":
                    comboBox5.Hide();
                    comboBox1.Show();
                    comboBox2.Show();
                    comboBox3.Show();
                    comboBox4.Show();
                    panel5.Show();
                    panel1.Hide();
                    panel2.Hide();
                    panel3.Hide();
                    panel4.Hide();
                    break;
            }
        }
        
        //获取终点代码
        private string GetEndCode(string check, string code)
        {
            if (Regex.IsMatch(check, "(One)+"))
            {
                switch (code)
                {
                    case "E1":
                        return "H5";
                    case "E2":
                        return "I5";
                    case "E3":
                        return "J5";
                    case "E4":
                        return "K5";
                    case "E5":
                        return "L5";
                    case "E6":
                        return "M5";
                    case "E7":
                        return "N5";
                    case "E8":
                        return "O5";
                }
            }
            else if (Regex.IsMatch(check, "(Two)+"))
            {
                switch (code)
                {
                    case "E1":
                        return "H6";
                    case "E2":
                        return "I6";
                    case "E3":
                        return "J6";
                    case "E4":
                        return "K6";
                    case "E5":
                        return "L6";
                    case "E6":
                        return "M6";
                    case "E7":
                        return "N6";
                    case "E8":
                        return "O6";
                }
            }
            else if (Regex.IsMatch(check, "(Three)+"))
            {
                switch (code)
                {
                    case "E1":
                        return "H4";
                    case "E2":
                        return "I4";
                    case "E3":
                        return "J4";
                    case "E4":
                        return "K4";
                    case "E5":
                        return "L4";
                    case "E6":
                        return "M4";
                    case "E7":
                        return "N4";
                    case "E8":
                        return "O4";
                }
            }
            return "";
        }

        //默认绘制地图模板
        public void DefaultMapModel()
        {
            Console.WriteLine("Hello");
            Bitmap ty = new Bitmap(getUrl);
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    MapCode += code;
                    Map[i, j] = code;
                    g.DrawImage(ty, i * 25, j * 25, 25, 25);
                }
            }
            pictureBox2.Image = bmp;
        }

        //委托-画已做的地图
        public void DrawDMap()
        {
            string mapcode = "";
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    switch (Map[i, j])
                    {
                        case "S1":
                            bitmap = new Bitmap(@"..\..\images\Surface\SurfaceOne.png");
                            break;
                        case "S2":
                            bitmap = new Bitmap(@"..\..\images\Surface\SurfaceOne.png");
                            break;
                        case "S3":
                            bitmap = new Bitmap(@"..\..\images\Surface\SurfaceTwo.png");
                            break;
                        case "S4":
                            bitmap = new Bitmap(@"..\..\images\Surface\SurfaceThree.png");
                            break;
                        case "S5":
                            bitmap = new Bitmap(@"..\..\images\Surface\SurfaceFour.png");
                            break;
                        case "S6":
                            bitmap = new Bitmap(@"..\..\images\Surface\SurfaceFive.png");
                            break;
                        case "S7":
                            bitmap = new Bitmap(@"..\..\images\Surface\SurfaceSix.png");
                            break;
                        case "S8":
                            bitmap = new Bitmap(@"..\..\images\Surface\SurfaceSeven.png");
                            break;
                        case "B1":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxOne\" + MapSurface + "-BoxOne-1.png");
                            break;
                        case "B2":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxTwo\" + MapSurface + "-BoxTwo-1.png");
                            break;
                        case "B3":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxThree\" + MapSurface + "-BoxThree-1.png");
                            break;
                        case "H4":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxOne\" + MapSurface + "-BoxOne-2.png");
                            break;
                        case "I4":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxOne\" + MapSurface + "-BoxOne-2.png");
                            break;
                        case "J4":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxOne\" + MapSurface + "-BoxOne-2.png");
                            break;
                        case "K4":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxOne\" + MapSurface + "-BoxOne-2.png");
                            break;
                        case "L4":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxOne\" + MapSurface + "-BoxOne-2.png");
                            break;
                        case "M4":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxOne\" + MapSurface + "-BoxOne-2.png");
                            break;
                        case "N4":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxOne\" + MapSurface + "-BoxOne-2.png");
                            break;
                        case "O4":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxOne\" + MapSurface + "-BoxOne-2.png");
                            break;
                        case "H5":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxTwo\" + MapSurface + "-BoxTwo-2.png");
                            break;
                        case "I5":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxTwo\" + MapSurface + "-BoxTwo-2.png");
                            break;
                        case "J5":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxTwo\" + MapSurface + "-BoxTwo-2.png");
                            break;
                        case "K5":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxTwo\" + MapSurface + "-BoxTwo-2.png");
                            break;
                        case "L5":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxTwo\" + MapSurface + "-BoxTwo-2.png");
                            break;
                        case "M5":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxTwo\" + MapSurface + "-BoxTwo-2.png");
                            break;
                        case "N5":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxTwo\" + MapSurface + "-BoxTwo-2.png");
                            break;
                        case "O5":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxTwo\" + MapSurface + "-BoxTwo-2.png");
                            break;
                        case "H6":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxThree\" + MapSurface + "-BoxThree-2.png");
                            break;
                        case "I6":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxThree\" + MapSurface + "-BoxThree-2.png");
                            break;
                        case "J6":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxThree\" + MapSurface + "-BoxThree-2.png");
                            break;
                        case "K6":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxThree\" + MapSurface + "-BoxThree-2.png");
                            break;
                        case "L6":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxThree\" + MapSurface + "-BoxThree-2.png");
                            break;
                        case "M6":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxThree\" + MapSurface + "-BoxThree-2.png");
                            break;
                        case "N6":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxThree\" + MapSurface + "-BoxThree-2.png");
                            break;
                        case "O6":
                            bitmap = new Bitmap(@"..\..\images\Box\BoxThree\" + MapSurface + "-BoxThree-2.png");
                            break;
                        case "E1":
                            bitmap = new Bitmap(@"..\..\images\End\EndBeige\" + MapSurface + "-EndBeige.png");
                            break;
                        case "E2":
                            bitmap = new Bitmap(@"..\..\images\End\EndBlack\" + MapSurface + "-EndBlack.png");
                            break;
                        case "E3":
                            bitmap = new Bitmap(@"..\..\images\End\EndBlue\" + MapSurface + "-EndBlue.png");
                            break;
                        case "E4":
                            bitmap = new Bitmap(@"..\..\images\End\EndBrown\" + MapSurface + "-EndBrown.png");
                            break;
                        case "E5":
                            bitmap = new Bitmap(@"..\..\images\End\EndGray\" + MapSurface + "-EndGray.png");
                            break;
                        case "E6":
                            bitmap = new Bitmap(@"..\..\images\End\EndPurple\" + MapSurface + "-EndPurple.png");
                            break;
                        case "E7":
                            bitmap = new Bitmap(@"..\..\images\End\EndRed\" + MapSurface + "-EndRed.png");
                            break;
                        case "E8":
                            bitmap = new Bitmap(@"..\..\images\End\EndYellow\" + MapSurface + "-EndYellow.png");
                            break;
                        case "C1":
                            bitmap = new Bitmap(@"..\..\images\Character\Up\" + MapSurface + "-Up.gif");
                            break;
                        case "C2":
                            bitmap = new Bitmap(@"..\..\images\Character\Down\" + MapSurface + "-Down.gif");
                            break;
                        case "C3":
                            bitmap = new Bitmap(@"..\..\images\Character\Left\" + MapSurface + "-Left.gif");
                            break;
                        case "C4":
                            bitmap = new Bitmap(@"..\..\images\Character\Right\" + MapSurface + "-Right.gif");
                            break;
                        case "CH":
                            bitmap = new Bitmap(@"..\..\images\Character\EndCharacter\EndCharacter-Beige.png");
                            break;
                        case "CI":
                            bitmap = new Bitmap(@"..\..\images\Character\EndCharacter\EndCharacter-Black.png");
                            break;
                        case "CJ":
                            bitmap = new Bitmap(@"..\..\images\Character\EndCharacter\EndCharacter-Blue.png");
                            break;
                        case "CK":
                            bitmap = new Bitmap(@"..\..\images\Character\EndCharacter\EndCharacter-Brown.png");
                            break;
                        case "CL":
                            bitmap = new Bitmap(@"..\..\images\Character\EndCharacter\EndCharacter-Gray.png");
                            break;
                        case "CM":
                            bitmap = new Bitmap(@"..\..\images\Character\EndCharacter\EndCharacter-Purple.png");
                            break;
                        case "CN":
                            bitmap = new Bitmap(@"..\..\images\Character\EndCharacter\EndCharacter-Red.png");
                            break;
                        case "CO":
                            bitmap = new Bitmap(@"..\..\images\Character\EndCharacter\EndCharacter-Yellow.png");
                            break;
                        case "W1":
                            bitmap = new Bitmap(@"..\..\images\Wall\WallOne.png");
                            break;
                        case "W2":
                            bitmap = new Bitmap(@"..\..\images\Wall\WallTwo.png");
                            break;
                        case "W3":
                            bitmap = new Bitmap(@"..\..\images\Wall\WallThree.png");
                            break;
                        case "W4":
                            bitmap = new Bitmap(@"..\..\images\Wall\WallFour.png");
                            break;
                    }
                    mapcode += Map[i, j];
                    g.DrawImage(bitmap, j * 25, i * 25, 25, 25);
                }
            }
            MapCode = mapcode;
            pictureBox2.Image = bmp;
        }

        //委托-改变"当前地图名称"
        public void ChangeLName()
        {
            label8.Text = "当前地图名称:" + LabelName;
        }
        
        //委托-关闭窗口
        public void CloseForm()
        {
            Dispose();
        }
        
        //使按钮不会被获取到按键焦点
        private void SetGetFocus(Control c)
        {
            MethodInfo methodinfo = c.GetType().GetMethod("SetStyle", BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.InvokeMethod);
            methodinfo.Invoke(c, BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.InvokeMethod, null, new object[]
                {ControlStyles.Selectable,false}, Application.CurrentCulture);
        }
    }
}
