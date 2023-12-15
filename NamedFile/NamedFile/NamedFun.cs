using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NamedFile
{
    public enum RuleTypeEnum
    {
        Insert = 0,
        Replace = 1, Delete = 2, UpLower = 3, PinYin = 4,
    }
    public class RuleInfo
    {
        public RuleTypeEnum ruleType;
        public string ruleName;
        public int no;  //当前规则的顺序号
        public string ToString()
        {
            return ruleName;
        }
    }
    public class RuleInfoInsert : RuleInfo
    {
        public int insertType = 1;  //插入模式，1：固定，2：源文件名
        public string insertFixStr="";
        public int placeType = 1;   //插入位置，1：前方，2：后方

        public RuleInfoInsert() { ruleType = RuleTypeEnum.Insert; ruleName = "插入"; }

    }
    public class RuleInfoReplace : RuleInfo
    {
        public int replaceType = 1;  //模式，1：全部，2：前面第一个，3：后面第一个
        public string replaceFindText = "";  //查找的字符串
        public string replaceNewText = "";   //替换的字符串
        public int replaceIgnoreExp = 1;   //忽略扩展名字
        public RuleInfoReplace() { ruleType = RuleTypeEnum.Replace; ruleName = "替换"; }
    }

    public class RuleInfoDelete : RuleInfo
    {
        public int deleteType = 1;  //插入模式，1：删除到结束，2：到位置
        public int deleteBegin = 1;  //开始位置
        public int deleteToNum;  //如果是删除到位置，这个是位置
        public int deleteIgnoreExp = 1;   //忽略扩展名字
        public int deleteL2R = 1;   //从左到右
        public RuleInfoDelete() { ruleType = RuleTypeEnum.Delete; ruleName = "删除"; }
    }
    public class RuleInfoUpLower : RuleInfo
    {
        public int uplowerType = 1;  //模式，1：全部大写，2：首字母大写，3：全部小写，4：反转大小写
        public int uplowerExpType = 1;   //1：忽略扩展名字，2：扩展名大写，3：扩展名小写
        public RuleInfoUpLower() { ruleType = RuleTypeEnum.UpLower; ruleName = "大小写"; }
    }
    public class RuleInfoPinYin : RuleInfo
    {
        public int pinyinType = 1;  //模式，1：转换全拼，2：转首字母
        public int pinyinIgnoreExp = 1;   //忽略扩展名字
        public RuleInfoPinYin() { ruleType = RuleTypeEnum.PinYin; ruleName = "拼音"; }
    }
    internal class NamedFun
    {
        //根据插入规则返回合适的字符串
        public static string InsertProcess(RuleInfoInsert rule,string nowStr,string sourceName)
        {
            string outStr = nowStr;
            string newStr = "";
            switch (rule.insertType)
            {
                case 1:
                    newStr = rule.insertFixStr;
                    break;
                case 2: 
                    newStr = sourceName; 
                    break;
            }
            switch (rule.placeType)
            {
                case 1:
                    outStr = newStr + outStr;
                    break;
                case 2:
                    outStr = outStr + newStr;
                    break;
            }
            return outStr;
        }

        //替换内容
        public static string ReplaceProcess(RuleInfoReplace rule, string nowStr)
        {
            string outStr = "";
            string replaceStr = nowStr;
            string expName = "";
            if (rule.replaceIgnoreExp == 1)
                replaceStr = Functions.GetFileNameButExp(replaceStr, out expName);

            switch (rule.replaceType) ////模式，1：全部，2：前面第一个，3：后面第一个
            {
                case 1:
                    outStr = replaceStr.Replace(rule.replaceFindText, rule.replaceNewText);
                    break;
                case 2:
                    int ind = replaceStr.IndexOf(rule.replaceFindText);
                    if (ind != -1)
                    {
                        outStr = replaceStr.Substring(0,ind)+ rule.replaceNewText+ replaceStr.Substring(ind + 1);
                    }
                    else
                    {
                        outStr = replaceStr;
                    }
                    break;
                case 3:
                    int indlast = replaceStr.LastIndexOf(rule.replaceFindText);
                    if (indlast != -1)
                    {
                        outStr = replaceStr.Substring(0, indlast) + rule.replaceNewText + replaceStr.Substring(indlast + 1);
                    }
                    else
                    {
                        outStr = replaceStr;
                    }
                    break;
            }
            if (rule.replaceIgnoreExp == 1)
                outStr += "." + expName;
            return outStr;
        }

        //替换内容
        public static string DeleteProcess(RuleInfoDelete rule, string nowStr)
        {
            string outStr = "";
            string replaceStr = nowStr;
            string expName = "";
            if (rule.deleteIgnoreExp == 1)
                replaceStr = Functions.GetFileNameButExp(replaceStr, out expName);

            switch (rule.deleteType) //插入模式，1：删除到结束，2：到位置
            {
                case 1:
                    if(rule.deleteL2R == 1)
                        outStr = replaceStr.Substring (rule.deleteBegin-1);
                    else
                        outStr = replaceStr.Substring(replaceStr.Length - rule.deleteBegin-1);
                    break;
                case 2:
                    if (rule.deleteL2R == 1)
                        outStr = replaceStr.Substring(rule.deleteBegin-1,rule.deleteToNum - rule.deleteBegin-1);
                    else
                        outStr = replaceStr.Substring(replaceStr.Length - (rule.deleteToNum - rule.deleteBegin - 1), replaceStr.Length - rule.deleteBegin - 1);
                    break;
                
            }
            if (rule.deleteIgnoreExp == 1)
            {
                outStr += "." + expName;
            }
            return outStr;
        }

        //大小写
        public static string UpLowerProcess(RuleInfoUpLower rule, string nowStr)
        {
            string outStr = "";
            string replaceStr = nowStr;
            string expName = "";
            if (rule.uplowerExpType == 1)
                replaceStr = Functions.GetFileNameButExp(replaceStr, out expName);

            switch (rule.uplowerType) //模式，1：全部大写，2：首字母大写，3：全部小写，4：反转大小写
            {
                case 1:
                    outStr = replaceStr.ToUpper();
                    break;
                case 2:
                    outStr = replaceStr.Substring(0,1).ToUpper() + replaceStr.Substring(1).ToLower();
                    break;
                case 3:
                    outStr = replaceStr.ToLower();
                    break;
                case 4:
                    for (int i = 0; i < replaceStr.Length - 1; i++)
                    {
                        string str = replaceStr.Substring(i, i + 1);
                        if (str.ToUpper().Equals(str))
                            str = str.ToLower();
                        else 
                            str = str.ToUpper();
                        outStr += str;
                    }
                    break;
            }

            //1：忽略扩展名字，2：扩展名大写，3：扩展名小写
            if (rule.uplowerExpType == 1)
                outStr += "." + expName;
            else if (rule.uplowerExpType == 2)
                outStr += "." + expName.ToUpper();
            else if (rule.uplowerExpType == 3)
                outStr += "." + expName.ToLower();
            return outStr;
        }

        //拼音
        public static string PinYinProcess(RuleInfoPinYin rule, string nowStr)
        {
            string outStr = "";
            string replaceStr = nowStr;
            string expName = "";
            if (rule.pinyinIgnoreExp == 1)
                replaceStr = Functions.GetFileNameButExp(replaceStr, out expName);

            switch (rule.pinyinType) //模式，1：转换全拼，2：转首字母
            {
                case 1:
                    outStr = PingYinHelper.ConvertToAllSpell(replaceStr);

                    break;
                case 2:
                    outStr = PingYinHelper.ConvertToFirstSpell(replaceStr);
                    break;
                
            }

            //忽略扩展名字
            if (rule.pinyinIgnoreExp == 1)
                outStr += "." + expName;
            
            return outStr;
        }
    }
}
