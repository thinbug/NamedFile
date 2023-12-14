using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace NamedFile
{
    public enum RuleTypeEnum
    { 
        Insert = 0,
        Replace = 1, Delete = 2, UpLower = 3 , PinYin = 4 ,
    }
    public class RuleInfo
    {
        public RuleTypeEnum ruleType;
        public string ruleName;

        public string ToString()
        { 
            return ruleName;
        }
    }
    public class RuleInfoInsert : RuleInfo
    {
        public int insertType = 1;  //插入模式，1：固定，2：源文件名
        public string insertFixStr;
        public int placeType = 1;   //插入位置，1：前方，2：后方
    }
    public class RuleInfoReplace : RuleInfo
    {
        public int replaceType = 1;  //插入模式，1：全部，2：前面第一个，3：后面第一个
        public string replaceFindText;  //查找的字符串
        public string replaceNewText;   //替换的字符串
        public int replaceIgnoreExp = 1;   //忽略扩展名字
    }
    /// <summary>
    /// RuleSetting.xaml 的交互逻辑
    /// </summary>
    public partial class RuleSetting : Window
    {
        Dictionary<int, RuleInfo> rulesDict;

        GroupBox nowGroup;
        RuleInfo nowInfo;
        public RuleSetting()
        {
            InitializeComponent();
            InitRules();
        }

        void InitRules()
        {
            rulesDict = new Dictionary<int, RuleInfo>();
            listRule.Items.Clear();
            RulesType_Insert();
            RulesType_Replace();
            RulesType_Delete();
            RulesType_UpLower();
            RulesType_Pinyin();
        }
        void RulesType_Insert()
        {
            RuleInfoInsert rule = new RuleInfoInsert();
            rule.ruleType = RuleTypeEnum.Insert;
            rule.ruleName = "插入";

            rulesDict.Add(listRule.Items.Count, rule);
            listRule.Items.Add(rule.ToString());

        }
        void RulesType_Replace()
        {
            RuleInfoReplace rule = new RuleInfoReplace();
            rule.ruleType = RuleTypeEnum.Replace;
            rule.ruleName = "替换";
            rulesDict.Add(listRule.Items.Count, rule);
            listRule.Items.Add(rule.ToString());
        }
        void RulesType_Delete()
        {
            RuleInfo rule = new RuleInfo();
            rule.ruleType = RuleTypeEnum.Delete;
            rule.ruleName = "删除";
            rulesDict.Add(listRule.Items.Count, rule);
            listRule.Items.Add(rule.ToString());
        }
        void RulesType_UpLower()
        {
            RuleInfo rule = new RuleInfo();
            rule.ruleType = RuleTypeEnum.UpLower;
            rule.ruleName = "大小写";
            rulesDict.Add(listRule.Items.Count, rule);
            listRule.Items.Add(rule.ToString());
        }
        void RulesType_Pinyin()
        {
            RuleInfo rule = new RuleInfo();
            rule.ruleType = RuleTypeEnum.PinYin;
            rule.ruleName = "拼音";
            rulesDict.Add(listRule.Items.Count, rule);
            listRule.Items.Add(rule.ToString());
            
        }

      

        
        //选择一个类型后
        private void listRule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectIdx = ((ListBox)sender).SelectedIndex;
            nowInfo = rulesDict[selectIdx];

            if (nowGroup != null)
                nowGroup.Visibility = Visibility.Hidden;
            nowGroup = (GroupBox)mainGrid.FindName("groupBox" + nowInfo.ruleType);
            nowGroup.Visibility = Visibility.Visible;
            switch (nowInfo.ruleType)
            {
                case RuleTypeEnum.Insert:
                    RuleInfoInsert rii = (RuleInfoInsert)nowInfo;
                    if (rii.insertType == 1)
                    {
                        insertTypeFix.IsChecked = true;
                        tbInsertFixStr.Text = rii.insertFixStr;
                    }
                    else
                    { 
                        insertTypeOri.IsChecked = true;
                    }
                    if (rii.placeType == 1)
                    {
                        placeFront.IsChecked = true;
                    }
                    else
                    {
                        placeBack.IsChecked = true;
                    }
                    break;
                case RuleTypeEnum.Replace:
                    RuleInfoReplace rir = (RuleInfoReplace)nowInfo;
                    tbReplaceFindText.Text = rir.replaceFindText;
                    tbReplaceNewText.Text = rir.replaceNewText;
                    switch (rir.replaceType)
                    {
                        case 1: rbReplaceAll.IsChecked = true; break;
                        case 2: rbReplaceFront.IsChecked = true; break;
                        case 3: rbReplaceBack.IsChecked = true; break;
                    }
                    cbReplaceIgnExp.IsChecked = rir.replaceIgnoreExp == 1;
                    break;
                case RuleTypeEnum.Delete:
                    break;
                case RuleTypeEnum.UpLower:
                    break;
                case RuleTypeEnum.PinYin:
                    break;
            }
        }

        //添加或者修改规则后
        private void btnRule_Click(object sender, RoutedEventArgs e)
        {
            if (nowInfo == null)
                return;
            switch (nowInfo.ruleType)
            {
                case RuleTypeEnum.Insert:
                    RuleInfoInsert rii = (RuleInfoInsert)nowInfo;

                    if (insertTypeFix.IsChecked == true)
                    {
                        rii.insertType = 1;
                        rii.insertFixStr = tbInsertFixStr.Text;
                    }
                    else
                    {
                        rii.insertType = 2;
                    }
                    if (placeFront.IsChecked == true)
                    {
                        rii.placeType = 1;
                    }
                    else
                    {
                        rii.placeType = 2;
                    }

                    break;
                
                case RuleTypeEnum.Replace:
                    RuleInfoReplace rir = (RuleInfoReplace)nowInfo;
                    rir.replaceFindText = tbReplaceFindText.Text  ;
                    rir.replaceNewText = tbReplaceNewText.Text;
                    if (rbReplaceAll.IsChecked == true) rir.replaceType = 1;
                    if (rbReplaceFront.IsChecked == true) rir.replaceType = 2;
                    if (rbReplaceBack.IsChecked == true) rir.replaceType = 3;
                    break;
                case RuleTypeEnum.Delete:
                    break;
                case RuleTypeEnum.UpLower:
                    break;
                case RuleTypeEnum.PinYin:
                    break;
            }

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.AddRule(nowInfo);

        }

    }
}
