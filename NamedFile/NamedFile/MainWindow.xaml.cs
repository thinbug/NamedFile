using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NamedFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBNamed_Click(object sender, RoutedEventArgs e)
        {
            if (listFiles == null)
                return;
            Console.WriteLine("开始");
            listOutputs.Items.Clear();
            for (int i = 0; i < listFiles.Items.Count; i++)
            {
                string filepath = (string)listFiles.Items[i];
                string fname = Functions.GetFileName(filepath);
                
                string nm = PingYinHelper.ConvertToAllSpell(fname, PingYinHelper.UpLowerOrNormal.Upper);

                string fn = nm[0].ToString();
                string newname = fn + " - " + fname;
                string newpath = filepath.Replace(fname, newname);
                listOutputs.Items.Add(newpath);

                try
                {
                    if (File.Exists(filepath))
                    {
                        File.Move(filepath, newpath);

                    }
                    else
                    {
                        Console.WriteLine("不存在的文件:"+ filepath);
                    }
                }
                catch
                {
                    Console.WriteLine("错误:" + filepath);
                }
            }
            
        }

        private void btnGetFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                listFiles.Items.Clear();
                string[] filesname = dialog.FileNames;
                for (int i = 0; i < filesname.Length; i++)
                {
                    listFiles.Items.Add(filesname[i]);
                }
            }

        }
    }
}
