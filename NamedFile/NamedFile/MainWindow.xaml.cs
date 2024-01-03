using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NamedFile
{
    public class FileInfoData
    {
        public string sourcePath { get; set; }
        public string sourceName { get; set; }
        public string newPath { get; set; }
        public string newName { get; set; }
    }

    public class RuleInfoData
    {
        public bool isEnable { get; set; }
        public string ruleName { get; set; }
        public string ruleDesc { get; set; }
    }
    public class IndexConverter : IValueConverter
    {
        public int AddValue { get; set; }

        public object Convert(object value, Type TargetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int curentvalue = (int)(value);
            curentvalue += AddValue;

            return curentvalue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int curentvalue = (int)(value);
            curentvalue -= AddValue;
            return curentvalue;

        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<FileInfoData> listSelectFiles;//当前组件绑定数据

        List<RuleInfo> listRuleInfo;    //当前规则详细信息
        public List<RuleInfoData> listRuleDatas; //当前组件绑定数据

        public bool justFilpath = false;

        public string appName = "文件名修改器";

        public string defRulesPath = ".\\rules\\default.inf";


        public MainWindow()
        {

            listRuleInfo = NamedFun.ReadRules(defRulesPath);
            listRuleDatas = NamedFun.GetRuleListDatas(listRuleInfo);

            InitializeComponent();

            lvRules.ItemsSource = listRuleDatas;

            SetColShowPath(justFilpath);
            string ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            this.Title = appName + " - " + ver;

            CheckEditDelShow();

        }

        //开始处理文件
        private void btnBNamed_Click(object sender, RoutedEventArgs e)
        {
            if (listSelectFiles == null || listSelectFiles.Count == 0)
                return;

            for (int i = 0; i < listSelectFiles.Count; i++)
            {
                FileInfoData fd = listSelectFiles[i];
                try
                {
                    if (File.Exists(fd.sourcePath))
                    {
                        File.Move(fd.sourcePath, fd.newPath);
                        //文件修改完毕后，需要把原文件修改了
                        fd.sourcePath = fd.newPath;
                        fd.sourceName = fd.newName;
                    }
                    else
                    {
                        Console.WriteLine("不存在的文件:" + fd.sourcePath);
                    }
                }
                catch
                {
                    Console.WriteLine("错误:" + fd.sourcePath);
                }
            }
            UpdatePreviewList();
        }

        void GetFiles(string[] filesname)
        {
            listSelectFiles = new List<FileInfoData>();
            for (int i = 0; i < filesname.Length; i++)
            {
                FileInfoData mydata = new FileInfoData
                {
                    sourcePath = filesname[i],
                    sourceName = Functions.GetFileName(filesname[i]),
                    newPath = "",
                    newName = ""
                };
                listSelectFiles.Add(mydata);
            }
            lvFileList.ItemsSource = listSelectFiles;

            if (listRuleInfo.Count == 0)
                AddDefaultRule();
            else
                UpdatePreviewList();
        }

        private void btnGetFiles_Click(object sender, RoutedEventArgs e)
        {
            GetFileDialog();
        }
        private void OnFileAddClick(object sender, RoutedEventArgs e)
        {
            GetFileDialog();
        }
        private void OnFileRemoveClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < lvFileList.Items.Count; i++)
            {
                if (lvFileList.SelectedItems.Contains(lvFileList.Items[i]))
                {
                    int indexDel = lvFileList.Items.IndexOf(lvFileList.Items[i]);
                    //lvFileList.Items.RemoveAt(indexDel);//删除
                    listSelectFiles.RemoveAt(indexDel);
                }
            }
            UpdatePreviewList();
        }
        private void OnFileClearClick(object sender, RoutedEventArgs e)
        {
            listSelectFiles.Clear();
            UpdatePreviewList();
        }
        void GetFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {

                string[] filesname = dialog.FileNames;
                GetFiles(filesname);
            }
        }
        private void Window_Drop(object sender, DragEventArgs e)
        {
            //string dropFile = "Drop";
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                int count = ((System.Array)e.Data.GetData(System.Windows.DataFormats.FileDrop)).Length;
                List<string> filesname = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    string f = ((System.Array)e.Data.GetData(System.Windows.DataFormats.FileDrop)).GetValue(i).ToString();
                    filesname.Add(f);

                }
                GetFiles(filesname.ToArray());
            }
        }




        //根据规则刷新新的预览文件列表
        void UpdatePreviewList()
        {
            if (listSelectFiles == null || listSelectFiles.Count == 0)
            {
                lvFileList.Items.Refresh();
                labTipHere.Visibility = Visibility.Visible;
                return;
            }
            labTipHere.Visibility = Visibility.Collapsed;


            for (int i = 0; i < listSelectFiles.Count; i++)
            {
                string filepath = listSelectFiles[i].sourcePath;
                string fname = listSelectFiles[i].sourceName;
                string nowStr = "";
                for (int j = 0; j < listRuleInfo.Count; j++)
                {
                    RuleInfo rule = listRuleInfo[j];
                    string type = rule.GetType().Name.ToString();
                    if (!rule.enable)
                        continue;
                    switch (type)
                    {
                        case "RuleInfoInsert":
                            nowStr = NamedFun.InsertProcess((RuleInfoInsert)rule, nowStr, fname, filepath);
                            break;
                        case "RuleInfoReplace":
                            nowStr = NamedFun.ReplaceProcess((RuleInfoReplace)rule, nowStr);
                            break;
                        case "RuleInfoDelete":
                            nowStr = NamedFun.DeleteProcess((RuleInfoDelete)rule, nowStr);
                            break;
                        case "RuleInfoUpLower":
                            nowStr = NamedFun.UpLowerProcess((RuleInfoUpLower)rule, nowStr);
                            break;
                        case "RuleInfoPinYin":
                            nowStr = NamedFun.PinYinProcess((RuleInfoPinYin)rule, nowStr);
                            break;
                        case "RuleInfoSerialize":
                            nowStr = NamedFun.SerializeProcess((RuleInfoSerialize)rule, nowStr, i);
                            break;
                    }
                }

                string newpath = filepath.Replace(fname, nowStr);
                string newname = fname.Replace(fname, nowStr);

                listSelectFiles[i].newPath = newpath;
                listSelectFiles[i].newName = newname;
            }
            lvFileList.Items.Refresh();

        }




        private void showFileNameClick(object sender, RoutedEventArgs e)
        {
            justFilpath = !justFilpath;
            SetColShowPath(justFilpath);
        }

        //对List列表进行显示和隐藏切换
        void SetColShowPath(bool show)
        {
            miShowName.IsChecked = !show;
            lvFileListView.Columns.Remove(gvcSFile);
            lvFileListView.Columns.Remove(gvcTFile);
            lvFileListView.Columns.Remove(gvcSPath);
            lvFileListView.Columns.Remove(gvcTPath);
            if (show)
            {
                //显示路径
                lvFileListView.Columns.Insert(0, gvcSPath);
                lvFileListView.Columns.Insert(1, gvcTPath);
            }
            else
            {
                lvFileListView.Columns.Insert(0, gvcSFile);
                lvFileListView.Columns.Insert(1, gvcTFile);
            }

        }
        #region 规则添加编辑等操作
        //右键菜单点击
        private void OnRuleMenuClick(object sender, RoutedEventArgs e)
        {
            string menuname = ((MenuItem)sender).Name;
            int index = lvRules.SelectedIndex;

            switch (menuname)
            {
                case "MenuItemRuleAdd":
                    AddRule();
                    break;

                case "MenuItemRuleEdit":
                    if (index == -1)
                        return;
                    ReadyEditRule(index);
                    break;
                case "MenuItemRuleDel":
                    if (index == -1)
                        return;
                    DeleteRule(index);
                    break;
            }
        }

        //打开规则添加界面
        void AddRule()
        {
            RuleSetting ruleWin = new RuleSetting();
            ruleWin.ShowDialog();
        }

        //显示规则编辑界面
        void ReadyEditRule(int index)
        {
            RuleSetting ruleWin = new RuleSetting();
            ruleWin.SelectOneRule(listRuleInfo[index], index);
            ruleWin.ShowDialog();
        }
        //删除规则
        void DeleteRule(int index)
        {
            //lvRules.Items.RemoveAt(index);
            listRuleInfo.RemoveAt(index);
            listRuleDatas.RemoveAt(index);
            UpdateRule();
            UpdatePreviewList();
        }

        void UpdateRule()
        {
            lvRules.Items.Refresh();
            NamedFun.SaveRules(defRulesPath, listRuleInfo);
        }


        //每次都需要一个原始的规则，如果添加文件后没有规则会添加一个默认规则
        void AddDefaultRule()
        {
            RuleInfoInsert nowInfo = new RuleInfoInsert();
            nowInfo.insertType = 2;
            nowInfo.placeType = 1;
            nowInfo.insertIgnoreExp = 0;
            AddRule(nowInfo);
        }

        //添加规则，从添加规则界面确定后回调回来的确认
        public void AddRule(RuleInfo nowInfo)
        {
            listRuleInfo.Add(nowInfo);

            listRuleDatas.Add(NamedFun.GetRuleData(nowInfo));
            UpdateRule();
            UpdatePreviewList();
        }
        //修改列表里的现有规则，从修改界面回调回来的。
        public void EditRule(RuleInfo nowInfo, int listRoleNo)
        {
            listRuleDatas[listRoleNo] = NamedFun.GetRuleData(nowInfo);
            UpdateRule();
            UpdatePreviewList();
        }

        //点击添加规则按钮
        private void btnRuleAdd_Click(object sender, RoutedEventArgs e)
        {
            AddRule();
        }
        //点击编辑规则按钮
        private void btnRuleEdit_Click(object sender, RoutedEventArgs e)
        {
            int index = lvRules.SelectedIndex;
            if (index == -1)
                return;
            ReadyEditRule(index);
        }
        //双击修改规则
        private void lbRules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = lvRules.SelectedIndex;
            if (index == -1)
                return;
            ReadyEditRule(index);
        }
        //点击删除规则按钮
        private void btnRuleDel_Click(object sender, RoutedEventArgs e)
        {
            int index = lvRules.SelectedIndex;

            if (index == -1)
                return;
            DeleteRule(index);
        }
        //选择规则后，检测按钮是否启用
        private void lbRules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckEditDelShow();
        }
        //取消选择或者鼠标其他地方点击关闭选择
        private void lbRules_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lvRules.SelectedIndex = -1;
        }
        //检测是否激活编辑和删除按钮
        void CheckEditDelShow()
        {
            int index = lvRules.SelectedIndex;

            if (index == -1)
            {
                btnRuleEdit.IsEnabled = false;
                btnRuleDel.IsEnabled = false;

                MenuItemRuleEdit.IsEnabled = false;
                MenuItemRuleDel.IsEnabled = false;
            }
            else
            {
                btnRuleEdit.IsEnabled = true;
                btnRuleDel.IsEnabled = true;

                MenuItemRuleEdit.IsEnabled = true;
                MenuItemRuleDel.IsEnabled = true;
            }
        }

        //规则启用和关闭
        void RuleEnable(int index, bool enable)
        {
            if (index == -1)
                return;
            listRuleInfo[index].enable = enable;
            listRuleDatas[index].isEnable = enable;
            UpdateRule();
            UpdatePreviewList();
        }

        //点击规则开关
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbBox = sender as CheckBox;
            if (cbBox != null)
            {
                //((TextBlock)cbBox.Content).Text
                int index = int.Parse(cbBox.Tag.ToString()) - 1;
                lvRules.SelectedIndex = index;
                RuleEnable(index, cbBox.IsChecked == true);
            }

        }

        #endregion


    }
}
