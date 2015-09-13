using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Data.OleDb;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace schindlerPrint
{
    public partial class Form1 : Form
    {
        private string SamplePath;
        private string ExcelPath;
        private DataSet myDataSet;
        private string[,] Prm;

        public Form1()
        {
            InitializeComponent();

            PrintDocument print = new PrintDocument();
            string sDefault = print.PrinterSettings.PrinterName;               //默认打印机名
            comboBox1.Text = sDefault;

            foreach (string sPrint in PrinterSettings.InstalledPrinters)       //获取打印机列表
            {
                comboBox1.Items.Add(sPrint);
                if (sPrint == sDefault)
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(sPrint);
            }
        }

        private void 模板路径设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择打印模板文件所在路径";
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SamplePath = folderBrowserDialog.SelectedPath;
            }
            Properties.Settings.Default.SamplePathSetting = SamplePath;
            Properties.Settings.Default.Save();
        }

        private void 标签内容表格路径设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择标签内容表格所在路径";
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ExcelPath = folderBrowserDialog.SelectedPath;
            }
            Properties.Settings.Default.ExcelPathSetting = ExcelPath;
            Properties.Settings.Default.Save();
        }

        private void GetPrn()                                                   //读入Prn内容
        {
            SamplePath = Properties.Settings.Default.SamplePathSetting;
            string path = SamplePath + "/";
            string[] text = File.ReadAllLines(path + "/" + "area.prn");
        }

        private void GetExcel()                                              //读入Excel内容
        {
            string SamplePath;
            SamplePath = Properties.Settings.Default.SamplePathSetting;
            string path = SamplePath + "/";
            string strCon = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = " + path + "PLC.xls;Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'";    //创建一个数据链接
            OleDbConnection myConn = new OleDbConnection(strCon);
            string strCom = " SELECT * FROM [Sheet1$] ";
            //try
            //{
            myConn.Open();
            OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);                  //打开数据链接，得到一个数据集
            myDataSet = new DataSet();                                                          //创建一个 DataSet对象
            myCommand.Fill(myDataSet, "[Sheet1$]");                                             //得到自己的DataSet对象
            myConn.Close();                                                                     //关闭此数据链接

            Prm = new string[myDataSet.Tables[0].Rows.Count, 8];

            for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)                            //读取Sheet1里的配置信息
            {
                Prm[i, 0] = myDataSet.Tables[0].Rows[i].ItemArray[2].ToString();            //配置物料的名称
                Prm[i, 1] = myDataSet.Tables[0].Rows[i].ItemArray[1].ToString();            //配置物料的ID号
                Prm[i, 2] = myDataSet.Tables[0].Rows[i].ItemArray[3].ToString();            //配置物料第一命令地址
                Prm[i, 3] = myDataSet.Tables[0].Rows[i].ItemArray[4].ToString();            //配置物料第二命令地址
                Prm[i, 4] = myDataSet.Tables[0].Rows[i].ItemArray[5].ToString();            //配置物料第三命令地址
                Prm[i, 5] = myDataSet.Tables[0].Rows[i].ItemArray[6].ToString();            //配置物料测试完成确认地址
                Prm[i, 6] = myDataSet.Tables[0].Rows[i].ItemArray[7].ToString();            //配置物料线束连接提示
                Prm[i, 7] = myDataSet.Tables[0].Rows[i].ItemArray[8].ToString();            //配置物料断路器闭合提示
            }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //BarCodePrint lpt = new BarCodePrint();
            //string mycommanglines = File.ReadAllText("area.prn");
            //lpt.Open();
            //lpt.Write(mycommanglines);
            //lpt.Close();

            string cmd = File.ReadAllText("area.prn");

            ZebraPrintHelper.SendFileToPrinter(comboBox1.Text, "area.prn");
            //if (!printer.Open())
            //{
            //    MessageBox.Show("未能连接打印机，请确认打印机是否安装正确并接通电源。");
            //    return;
            //}
            //printer.Write(cmd);
            //if (!printer.Close())
            //{
            //    MessageBox.Show("未能关闭与打印机之间的连接，这可能意味着严重的错误，请重启电脑及打印机。");
            //    return;
            //}
        }
    }
}
