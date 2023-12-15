using Microsoft.Win32;
using System;
using System.CodeDom;
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
        List<string> listSelectFiles;
        List<RuleInfo> listRuleInfo;

        public bool justFilename = true;
        public MainWindow()
        {
            InitializeComponent();

            listRuleInfo = new List<RuleInfo>();
        }

        private void btnBNamed_Click(object sender, RoutedEventArgs e)
        {
            if (lbFiles == null)
                return;
            Console.WriteLine("开始");
            lbOutputs.Items.Clear();
            for (int i = 0; i < lbFiles.Items.Count; i++)
            {
                string filepath = (string)lbFiles.Items[i];
                string fname = Functions.GetFileName(filepath);
                
                string nm = PingYinHelper.ConvertToAllSpell(fname, PingYinHelper.UpLowerOrNormal.Upper);

                string fn = nm[0].ToString();
                string newname = fn + " - " + fname;
                string newpath = filepath.Replace(fname, newname);
                lbOutputs.Items.Add(newpath);

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
                lbFiles.Items.Clear();
                listSelectFiles = new List<string>();
                string[] filesname = dialog.FileNames;
                for (int i = 0; i < filesname.Length; i++)
                {
                    listSelectFiles.Add(filesname[i]);
                    if (!justFilename)
                        lbFiles.Items.Add(filesname[i]);
                    else
                        lbFiles.Items.Add(Functions.GetFileName(filesname[i]));
                }

                AddDefaultRule();
            }

        }

        private void btnRuleAdd_Click(object sender, RoutedEventArgs e)
        {
            RuleSetting ruleWin = new RuleSetting();
            ruleWin.ShowDialog();
        }

        //每次都需要一个原始的规则
        void AddDefaultRule()
        { 
            RuleInfoInsert nowInfo = new RuleInfoInsert();
            nowInfo.insertType = 2;
            nowInfo.placeType = 1;
            AddRule(nowInfo);
        }

        //添加规则
        public void AddRule(RuleInfo nowInfo,params object[] parm)
        {
            listRuleInfo.Add(nowInfo);
            string showtxt = FormatRuleString(nowInfo);
            lbRules.Items.Add(showtxt);
            PreviewList();
        }
        //修改列表里的现有规则
        public void EditRule(RuleInfo nowInfo, int listRoleNo)
        {
            string showtxt = FormatRuleString(nowInfo);
            lbRules.Items[listRoleNo] = showtxt;
            PreviewList();
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
                default:
                    showtxt = "Error : " + nowInfo.ruleType.ToString();
                    break;
            }
            return showtxt;
        }
        
        
        //删除规则
        void DeleteRule(int index)
        {
            lbRules.Items.RemoveAt(index);
            listRuleInfo.RemoveAt(index);
            PreviewList();
        }
        void ReadyEditRule(int index)
        {
            RuleSetting ruleWin = new RuleSetting();
            ruleWin.SelectOneRule(listRuleInfo[index], index);
            ruleWin.ShowDialog();

        }

        //根据规则刷新新的预览文件列表
        void PreviewList(bool toSave = false)
        {
            if (lbFiles == null || listRuleInfo.Count == 0)
                return;
            Console.WriteLine("开始PreviewList");
            lbOutputs.Items.Clear();
            
            for (int i = 0; i < lbFiles.Items.Count; i++)
            {
                string filepath = (string)lbFiles.Items[i];
                string fname = Functions.GetFileName(filepath);
                string nowStr = "";
                for (int j = 0; j < listRuleInfo.Count; j++)
                {
                    RuleInfo rule = listRuleInfo[j];
                    string type = rule.GetType().Name.ToString();
                    
                    switch (type)
                    {
                        case "RuleInfoInsert":
                            nowStr = NamedFun.InsertProcess((RuleInfoInsert)rule, nowStr, fname);
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
                    }
                }
                //string nm = PingYinHelper.ConvertToAllSpell(fname, PingYinHelper.UpLowerOrNormal.Upper);

                //string fn = nm[0].ToString();
                //string newname = fn + " - " + fname;
                string newpath = filepath.Replace(fname, nowStr);
                string newnew = fname.Replace(fname, nowStr);
                lbOutputs.Items.Add(newnew);

                if (toSave)
                {
                    try
                    {
                        if (File.Exists(filepath))
                        {
                            File.Move(filepath, newpath);

                        }
                        else
                        {
                            Console.WriteLine("不存在的文件:" + filepath);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("错误:" + filepath);
                    }
                }
            }
        }

        private void OnRuleMenuClick(object sender, RoutedEventArgs e)
        {
            string menuname = ((MenuItem)sender).Name;
            int index = lbRules.SelectedIndex;
            
            switch (menuname)
            {
                case "MenuItemRuleEdit":
                    ReadyEditRule(index);
                    break;
                case "MenuItemRuleDel":
                    DeleteRule(index);
                    break;
            }
        }

       
    }
}
