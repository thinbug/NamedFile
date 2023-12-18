using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<FileInfoData> listSelectFiles;
        List<RuleInfo> listRuleInfo;

        public bool justFilpath = false;

        public string appName = "文件名修改器";



        public MainWindow()
        {
            InitializeComponent();

            listRuleInfo = new List<RuleInfo>();
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
            PreviewList();
        }

        private void btnGetFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                listSelectFiles = new List<FileInfoData>();
                string[] filesname = dialog.FileNames;
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
                    PreviewList();
            }

        }


        string FormatRuleString(RuleInfo nowInfo)
        {
            string showrule = nowInfo.ruleName;
            string showtxt;
            switch (nowInfo.ruleType)
            {
                case RuleTypeEnum.Insert:
                    RuleInfoInsert rii = (RuleInfoInsert)nowInfo;
                    string fixstr = "";
                    if (rii.insertType == 1) fixstr = "固定文本:" + rii.insertFixStr;
                    else fixstr = "原文件";
                    showtxt = showrule + " - " + fixstr + ",位置:" + rii.placeType;
                    break;

                case RuleTypeEnum.Replace:
                    RuleInfoReplace rir = (RuleInfoReplace)nowInfo;
                    string rtype = "";
                    if (rir.replaceType == 1) rtype = "所有";
                    else if (rir.replaceType == 2) rtype = "最前一个";
                    else if (rir.replaceType == 3) rtype = "最后一个";
                    showtxt = showrule + " - " + rtype + "," + rir.replaceFindText + "->" + rir.replaceNewText;
                    if (rir.replaceIgnoreExp == 1)
                        showtxt += "," + "忽略扩展名";
                    break;
                case RuleTypeEnum.Delete:
                    RuleInfoDelete rid = (RuleInfoDelete)nowInfo;
                    string dtype = "";
                    if (rid.deleteType == 1) dtype = "到结束";
                    if (rid.deleteType == 2) dtype = "到位置：" + rid.deleteToNum.ToString();

                    showtxt = showrule + " - " + dtype;
                    if (rid.deleteIgnoreExp == 1) showtxt += "," + "忽略扩展名";
                    if (rid.deleteL2R == 1) showtxt += "," + "从左到右";
                    else showtxt += "," + "从右到左";
                    break;
                case RuleTypeEnum.UpLower:
                    RuleInfoUpLower riul = (RuleInfoUpLower)nowInfo;

                    string ultype = "";
                    switch (riul.uplowerType)
                    {
                        case 1: ultype = "全部大写"; break;
                        case 2: ultype = "首字母大写"; break;
                        case 3: ultype = "全部小写"; break;
                        case 4: ultype = "反转大小写"; break;
                    }
                    string exptype = "";
                    switch (riul.uplowerExpType)
                    {
                        case 1: exptype = "忽略扩展名"; break;
                        case 2: exptype = "扩展名小写"; break;
                        case 3: exptype = "扩展名大写"; break;
                    }
                    showtxt = showrule + " - " + ultype + "," + exptype;
                    break;
                case RuleTypeEnum.PinYin:
                    RuleInfoPinYin ripy = (RuleInfoPinYin)nowInfo;
                    string pytype = "";
                    switch (ripy.pinyinType)
                    {
                        case 1: pytype = "转换全拼"; break;
                        case 2: pytype = "转首字母"; break;
                    }
                    showtxt = showrule + " - " + pytype;
                    if (ripy.pinyinIgnoreExp == 1)
                        showtxt += "," + "忽略扩展名";
                    break;
                case RuleTypeEnum.Serialize:
                    RuleInfoSerialize ris = (RuleInfoSerialize)nowInfo;
                    string stype = "";
                    switch (ris.serializePlaceType)
                    {
                        case 1: stype = "插入前面"; break;
                        case 2: stype = "插入后面"; break;
                    }
                    showtxt = showrule + " - " + stype;
                    if (ris.serializeIgnoreExp == 1)
                        showtxt += "," + "忽略扩展名";
                    break;
                default:
                    showtxt = "Error : " + nowInfo.ruleType.ToString();
                    break;
            }
            return showtxt;
        }


        //根据规则刷新新的预览文件列表
        void PreviewList()
        {
            if (listSelectFiles == null || listSelectFiles.Count == 0)
                return;
            Console.WriteLine("开始PreviewList");
            //lbOutputs.Items.Clear();

            for (int i = 0; i < listSelectFiles.Count; i++)
            {
                string filepath = listSelectFiles[i].sourcePath;
                string fname = listSelectFiles[i].sourceName;
                string nowStr = "";
                for (int j = 0; j < listRuleInfo.Count; j++)
                {
                    RuleInfo rule = listRuleInfo[j];
                    string type = rule.GetType().Name.ToString();

                    switch (type)
                    {
                        case "RuleInfoInsert":
                            nowStr = NamedFun.InsertProcess((RuleInfoInsert)rule, nowStr, fname,filepath);
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
                            nowStr = NamedFun.SerializeProcess((RuleInfoSerialize)rule, nowStr,i);
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
            int index = lbRules.SelectedIndex;

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
            lbRules.Items.RemoveAt(index);
            listRuleInfo.RemoveAt(index);
            PreviewList();
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
            string showtxt = FormatRuleString(nowInfo);
            lbRules.Items.Add(showtxt);
            PreviewList();
        }
        //修改列表里的现有规则，从修改界面回调回来的。
        public void EditRule(RuleInfo nowInfo, int listRoleNo)
        {
            string showtxt = FormatRuleString(nowInfo);
            lbRules.Items[listRoleNo] = showtxt;
            PreviewList();
        }

        //点击添加规则按钮
        private void btnRuleAdd_Click(object sender, RoutedEventArgs e)
        {
            AddRule();
        }
        //点击编辑规则按钮
        private void btnRuleEdit_Click(object sender, RoutedEventArgs e)
        {
            int index = lbRules.SelectedIndex;
            if (index == -1)
                return;
            ReadyEditRule(index);
        }
        //双击修改规则
        private void lbRules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = lbRules.SelectedIndex;
            if (index == -1)
                return;
            ReadyEditRule(index);
        }
        //点击删除规则按钮
        private void btnRuleDel_Click(object sender, RoutedEventArgs e)
        {
            int index = lbRules.SelectedIndex;

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
            lbRules.SelectedIndex = -1;
        }
        //检测是否激活编辑和删除按钮
        void CheckEditDelShow()
        {
            int index = lbRules.SelectedIndex;

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




        #endregion

        
    }
}
