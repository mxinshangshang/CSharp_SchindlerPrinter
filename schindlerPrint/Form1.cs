using System;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Drawing.Printing;
using System.Text.RegularExpressions;

namespace schindlerPrint
{
    public partial class Form1 : Form
    {
        private string SamplePath;
        private string ExcelPath;
        private DataSet myDataSet;

        public Form1()
        {
            InitializeComponent();

            PrintDocument print = new PrintDocument();
            string sDefault = print.PrinterSettings.PrinterName;               //获取默认打印机名
            comboBox1.Text = sDefault;

            foreach (string sPrint in PrinterSettings.InstalledPrinters)       //获取打印机列表
            {
                comboBox1.Items.Add(sPrint);
                if (sPrint == sDefault)
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(sPrint);
            }
            textBox1.Text = "59410916";
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

        private string GetPrn(string[] Prm)                                                   //整合Prn内容
        {
            SamplePath = Properties.Settings.Default.SamplePathSetting;
            string path = SamplePath + "/";
            string[] text = File.ReadAllLines(path + "/" + "area.prn");

            for (int u = 0; u < text.Length; u++)
            {
                if (text[u].Contains("area"))
                {
                    int s = Int32.Parse(Regex.Match(text[u], @"area([\s\S]*?)""").Groups[1].Value);
                    text[u] = text[u].Replace(Regex.Match(text[u], @"""([\s\S]*?)""").Groups[1].ToString(), Prm[s-1]);

                    //text[u].Replace("A90", "1231231231231");
                    //string a = Regex.Match(text[u], @"""([\s\S]*?)""").Groups[1].ToString();
                }
                else continue;
            }
            return string.Join("\r\n", text);
        }

        private string GetExcel(string target)                                              //读入Excel内容
        {
            int index = 0;
            string[] Prm;
            string cmd;
            ExcelPath = Properties.Settings.Default.ExcelPathSetting;
            string path = ExcelPath + "/";
            string strCon = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = " + path + "AREA.xls;Extended Properties='Excel 8.0;HDR=NO;IMEX=1;'";    //创建一个数据链接
            OleDbConnection myConn = new OleDbConnection(strCon);
            string strCom = " SELECT * FROM [Sheet1$] ";
            //try
            //{
            myConn.Open();
            OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);                  //打开数据链接，得到一个数据集
            myDataSet = new DataSet();                                                          //创建一个 DataSet对象
            myCommand.Fill(myDataSet, "[Sheet1$]");                                             //得到自己的DataSet对象
            myConn.Close();                                                                     //关闭此数据链接

            Prm = new string[myDataSet.Tables[0].Columns.Count];

            for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)
            {
                if (target == myDataSet.Tables[0].Rows[i].ItemArray[1].ToString())              //获取要打印的ID号所在的行号
                {
                    index = i;
                    break;
                }
            }

            for (int i = 0; i < myDataSet.Tables[0].Columns.Count; i++)                         //获取ID对应的所有需要打印的信息
            {
                Prm[i] = myDataSet.Tables[0].Rows[index].ItemArray[i].ToString();
            }
            cmd = GetPrn(Prm);
            return cmd;
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string cmd = GetExcel(textBox1.Text)+"\r\n";
            //ZebraPrintHelper.SendStringToPrinter(comboBox1.Text, cmd);

            //string mycommanglines = File.ReadAllText("area.prn");
            ZebraPrintHelper.SendStringToPrinter(comboBox1.Text, cmd);

            //string cmd = File.ReadAllText("area.prn");
            //ZebraPrintHelper.SendFileToPrinter(comboBox1.Text, "area.prn");

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
