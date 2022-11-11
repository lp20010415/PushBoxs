using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PushBoxs
{
    public partial class DelMap : Form
    {
        public int mapId = 0;
        public static string[,] mapData = null;
        public DelMap()
        {
            InitializeComponent();
            mapData = FormSc.GetMapData;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string del_mapName = textBox1.Text;
            if (del_mapName != "" || mapData != null)
            {
                // 查找地图名是否存在
                Boolean isExist = false;
                int mapId = 0;
                int del_index = 0;
                for (int i = 0;i < mapData.GetLength(0);i++)
                {
                    if (mapData[i, 0] == del_mapName)
                    {
                        isExist = true;
                        del_index = i;
                        mapId = Convert.ToInt32(mapData[i, 3]);

                    }

                }
                if (isExist)
                {
                    MySqlConnection conn = new MySqlConnection("Server=1.117.74.238;port=3306;DataBase=c#Match_Data;user=root;password=SJMRLp0@0");
                    conn.Open();
                    MySqlCommand comm = conn.CreateCommand();
                    comm.CommandText = "delete from UserMapData where id = " + mapId;
                    if (comm.ExecuteNonQuery() > 0)
                    {
                        string[,] newMapData = new string[mapData.GetLength(0) - 1, 4];
                        for (int i = 0; i < mapData.GetLength(0) - 1; i++)
                        {
                            Console.WriteLine(i);
                            if (del_index != i)
                            {
                                newMapData[i,0] = mapData[i, 0];
                                newMapData[i,1] = mapData[i, 1];
                                newMapData[i,2] = mapData[i, 2];
                                newMapData[i,3] = mapData[i, 3];
                            }

                        }
                        FormSc.GetMapData = newMapData;
                        FormSc.reshowMap();
                        MessageBox.Show("删除成功！");
                    }
                    conn.Close();

                } else
                {
                    MessageBox.Show("该地图名不存在");
                }
            }

        }
    }
}
