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
    
    /// <summary>
    /// RuleSetting.xaml 的交互逻辑
    /// </summary>
    public partial class RuleSetting : Window
    {
        Dictionary<int, RuleInfo> rulesDict;    //当前所有类型的列表
        Dictionary<RuleTypeEnum, RuleInfo> rulesDictEnum;    //当前所有类型的列表

        GroupBox nowGroup;
        RuleInfo nowInfo;
        int ruleListboxIndex = -1;
        public RuleSetting()
        {
            InitializeComponent();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            this.Top =  mainWindow.Top + (mainWindow.Height - Height) / 2 ;
            this.Left = mainWindow.Left + (mainWindow.Width - Width) / 2;
            Title = "规则设置";
            InitRules();
            IsAdd(true);

        }
        private void Window_Activated(object sender, EventArgs e)
        {
            if (ruleListboxIndex != -1)
                listRule.IsEnabled = false;
            else
                listRule.IsEnabled = true;
        }

        void InitRules()
        {
            rulesDict = new Dictionary<int, RuleInfo>();
            rulesDictEnum = new Dictionary<RuleTypeEnum, RuleInfo>();
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
            rule.no = listRule.Items.Count;
            rulesDict.Add(rule.no, rule);
            rulesDictEnum.Add(rule.ruleType, rule);
            listRule.Items.Add(rule.ToString());

        }
        void RulesType_Replace()
        {
            RuleInfoReplace rule = new RuleInfoReplace();
            rule.no = listRule.Items.Count;
            rulesDict.Add(rule.no, rule);
            rulesDictEnum.Add(rule.ruleType, rule);
            listRule.Items.Add(rule.ToString());
        }
        void RulesType_Delete()
        {
            RuleInfoDelete rule = new RuleInfoDelete();
            rule.no = listRule.Items.Count;
            rulesDict.Add(rule.no, rule);
            rulesDictEnum.Add(rule.ruleType, rule);
            listRule.Items.Add(rule.ToString());
        }
        void RulesType_UpLower()
        {
            RuleInfoUpLower rule = new RuleInfoUpLower();
            rule.no = listRule.Items.Count;
            rulesDict.Add(rule.no, rule);
            rulesDictEnum.Add(rule.ruleType, rule);
            listRule.Items.Add(rule.ToString());
        }
        void RulesType_Pinyin()
        {
            RuleInfoPinYin rule = new RuleInfoPinYin();
            rule.no = listRule.Items.Count;
            rulesDict.Add(rule.no, rule);
            rulesDictEnum.Add(rule.ruleType, rule);
            listRule.Items.Add(rule.ToString());
        }

      

        
        //选择一个类型后,显示数据
        private void listRule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectIdx = ((ListBox)sender).SelectedIndex;
            nowInfo = rulesDict[selectIdx];

            ShowGroup(nowInfo);
        }

        void IsAdd(bool isAdd)
        {
            if (isAdd)
            {
                btnAddRule.Visibility = Visibility.Visible;
                btnEditRule.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnAddRule.Visibility = Visibility.Collapsed;
                btnEditRule.Visibility = Visibility.Visible;
            }
        }

        public void SelectOneRule(RuleInfo rule , int _ruleListboxIndex)
        {
            ruleListboxIndex = _ruleListboxIndex;   //当前已经添加的规则里的序号
           
            
            IsAdd(false);
            ShowGroup(rule);    //刷新数据 
        }

        void ShowGroup(RuleInfo rule)
        {
            nowInfo = rule;
            if (nowGroup != null)
                nowGroup.Visibility = Visibility.Collapsed;
            nowGroup = (GroupBox)mainGrid.FindName("groupBox" + nowInfo.ruleType);
            nowGroup.Visibility = Visibility.Visible;
            switch (nowInfo.ruleType)
            {
                case RuleTypeEnum.Insert:
                    RuleInfoInsert rii = (RuleInfoInsert)nowInfo;
                    if (rii.insertType == 1) { insertTypeFix.IsChecked = true; tbInsertFixStr.Text = rii.insertFixStr; }
                    else { insertTypeOri.IsChecked = true; }
                    if (rii.placeType == 1) { placeFront.IsChecked = true; }
                    else { placeBack.IsChecked = true; }
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
                    RuleInfoDelete rid = (RuleInfoDelete)nowInfo;
                    tbDelBegin.Text = rid.deleteBegin.ToString();
                    switch (rid.deleteType)
                    {
                        case 1: rbDelToEnd.IsChecked = true; break;
                        case 2: rbDelNumEnd.IsChecked = true; tbDelEnd.Text = rid.deleteToNum.ToString(); break;
                    }
                    cbDelIgnoreExp.IsChecked = rid.deleteIgnoreExp == 1;
                    cbDelL2R.IsChecked = rid.deleteL2R == 1;
                    break;
                case RuleTypeEnum.UpLower:
                    RuleInfoUpLower riul = (RuleInfoUpLower)nowInfo;
                    switch (riul.uplowerType)
                    {
                        case 1: rbUpLowerAllBig.IsChecked = true; break;
                        case 2: rbUpLowerFirstBig.IsChecked = true; break;
                        case 3: rbUpLowerAllSmall.IsChecked = true; break;
                        case 4: rbUpLowerAllFlip.IsChecked = true; break;
                    }
                    switch (riul.uplowerExpType)
                    {
                        case 1: rbUpLowerExpIgnore.IsChecked = true; break;
                        case 2: rbUpLowerExpSmall.IsChecked = true; break;
                        case 3: rbUpLowerExpBig.IsChecked = true; break;
                    }

                    break;
                case RuleTypeEnum.PinYin:
                    RuleInfoPinYin ripy = (RuleInfoPinYin)nowInfo;
                    switch (ripy.pinyinType)
                    {
                        case 1: rbPinYinAll.IsChecked = true; break;
                        case 2: rbPinYinFirst.IsChecked = true; break;
                    }
                    cbPinYinIgnoreExp.IsChecked = ripy.pinyinIgnoreExp == 1;
                    break;
            }
        }

        //添加规则后
        private void btnAddRule_Click(object sender, RoutedEventArgs e)
        {
            if (nowInfo == null)
                return;
            UpdateNowRule();

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.AddRule(nowInfo);

            this.Close();
        }
        private void btnEditRule_Click(object sender, RoutedEventArgs e)
        {
            if (nowInfo == null)
                return;
            UpdateNowRule();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.EditRule(nowInfo, ruleListboxIndex);
            ruleListboxIndex = -1;
            this.Close();
        }

        void UpdateNowRule()
        {
            switch (nowInfo.ruleType)
            {
                case RuleTypeEnum.Insert:
                    RuleInfoInsert rii = (RuleInfoInsert)nowInfo;
                    if (insertTypeFix.IsChecked == true) { rii.insertType = 1; rii.insertFixStr = tbInsertFixStr.Text; }
                    else { rii.insertType = 2; }
                    if (placeFront.IsChecked == true) { rii.placeType = 1; }
                    else { rii.placeType = 2; }
                    break;

                case RuleTypeEnum.Replace:
                    RuleInfoReplace rir = (RuleInfoReplace)nowInfo;
                    rir.replaceFindText = tbReplaceFindText.Text;
                    rir.replaceNewText = tbReplaceNewText.Text;
                    rir.replaceIgnoreExp = cbReplaceIgnExp.IsChecked == true ? 1 : 0;
                    if (rbReplaceAll.IsChecked == true) rir.replaceType = 1;
                    if (rbReplaceFront.IsChecked == true) rir.replaceType = 2;
                    if (rbReplaceBack.IsChecked == true) rir.replaceType = 3;
                    break;
                case RuleTypeEnum.Delete:
                    RuleInfoDelete rid = (RuleInfoDelete)nowInfo;
                    rid.deleteBegin = int.Parse(tbDelBegin.Text);
                    if (rbDelToEnd.IsChecked == true) rid.deleteType = 1;
                    if (rbDelNumEnd.IsChecked == true) { rid.deleteType = 2; rid.deleteToNum = int.Parse(tbDelEnd.Text); }
                    rid.deleteIgnoreExp = cbDelIgnoreExp.IsChecked == true ? 1 : 0;
                    rid.deleteL2R = cbDelL2R.IsChecked == true ? 1 : 0;
                    break;
                case RuleTypeEnum.UpLower:
                    RuleInfoUpLower riul = (RuleInfoUpLower)nowInfo;
                    if (rbUpLowerAllBig.IsChecked == true) riul.uplowerType = 1;
                    if (rbUpLowerFirstBig.IsChecked == true) riul.uplowerType = 2;
                    if (rbUpLowerAllSmall.IsChecked == true) riul.uplowerType = 3;
                    if (rbUpLowerAllFlip.IsChecked == true) riul.uplowerType = 4;
                    if (rbUpLowerExpIgnore.IsChecked == true) riul.uplowerExpType = 1;
                    if (rbUpLowerExpSmall.IsChecked == true) riul.uplowerExpType = 2;
                    if (rbUpLowerExpBig.IsChecked == true) riul.uplowerExpType = 3;
                    break;
                case RuleTypeEnum.PinYin:
                    RuleInfoPinYin ripy = (RuleInfoPinYin)nowInfo;
                    if (rbPinYinAll.IsChecked == true) ripy.pinyinType = 1;
                    if (rbPinYinFirst.IsChecked == true) ripy.pinyinType = 2;
                    ripy.pinyinIgnoreExp = cbPinYinIgnoreExp.IsChecked == true ? 1 : 0;
                    break;
            }
        }

       
    }
}
