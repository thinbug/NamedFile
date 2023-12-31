﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPinyin;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace NamedFile
{
    public enum RuleTypeEnum
    {
        Insert = 0,
        Replace = 1, Delete = 2, UpLower = 3, PinYin = 4, Serialize = 5
    }
    public class RuleInfo
    {
        public RuleTypeEnum ruleType;
        public string ruleName;
        public bool enable = true;
        override public string ToString()
        {
            return ruleName;
        }
    }
    public class RuleInfoInsert : RuleInfo
    {
        public int insertType = 1;  //插入模式，1：固定，2：源文件名,3:文件所在目录名
        public string insertFixStr="";
        public int placeType = 1;   //插入位置，1：前方，2：后方
        public int insertIgnoreExp = 1;   //忽略扩展名字
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
    public class RuleInfoSerialize : RuleInfo
    {
        public int serializeBegin = 1;  //开始数值
        public int serializeAdd = 1;   //增量数字
        public int serializeFullZero = 1;  //填充0开关
        public int serializeFullZeroNumber = 2;  //填充几个0
        public int serializeIgnoreExp = 1;   //忽略扩展名字
        public int serializePlaceType = 1;   //插入位置，1：前方，2：后方,3:位置
        public int serializePlaceNumber = 1;    //如果是3，插入位置
        public RuleInfoSerialize() { ruleType = RuleTypeEnum.Serialize; ruleName = "序列化"; }
    }
    
    internal class NamedFun
    {
        //根据插入规则返回合适的字符串
        public static string InsertProcess(RuleInfoInsert rule,string nowStr,string sourceName,string sourcePath)
        {
        

            string outStr = "";
            string replaceStr = nowStr;
            string expName = "";
            
            if (rule.insertIgnoreExp == 1)
                replaceStr = Functions.GetFileNameButExp(replaceStr, out expName);

            //插入模式，1：固定，2：源文件名,3:文件所在目录名
            switch (rule.insertType)
            {
                case 1:
                    outStr = rule.insertFixStr;
                    break;
                case 2:
                    outStr = sourceName; 
                    break;
                case 3:
                    string pathname = Path.GetDirectoryName(sourcePath);
                    int idx = pathname.LastIndexOf('\\');
                    if (idx != -1)
                    {
                        outStr = pathname.Substring(idx+1);
                        break;
                    }
                    outStr = pathname;
                    break;
            }

            //插入位置，1：前方，2：后方
            switch (rule.placeType)
            {
                case 1:
                    outStr = outStr + replaceStr;
                    break;
                case 2:
                    outStr = replaceStr + outStr;
                    break;
            }
            if (rule.insertIgnoreExp == 1)
                outStr += "." + expName;
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
                    if (rule.deleteL2R == 1)
                    {
                        int end = rule.deleteBegin - 1;
                        if (end > replaceStr.Length)
                            end = replaceStr.Length;
                        outStr = replaceStr.Substring(0, end);
                    }
                    else
                    {
                        int bstart = replaceStr.Length - rule.deleteBegin + 1;
                        if (bstart < 0)
                        {
                            bstart = 0;
                        }
                        outStr = replaceStr.Substring(bstart);
                    }
                    break;
                case 2:
                    if (rule.deleteL2R == 1)
                    {
                        int bb = rule.deleteBegin - 1;
                        if (bb > replaceStr.Length)
                            bb = replaceStr.Length;
                        int ee = rule.deleteToNum - 1;
                        if (ee > replaceStr.Length)
                            ee = replaceStr.Length - bb;
                        string delstr = replaceStr.Substring(bb, ee);
                        int idx = replaceStr.IndexOf(delstr);
                        outStr = replaceStr.Substring(0, idx) + replaceStr.Substring(idx + delstr.Length);
                    }
                    else
                    {
                        int bb = replaceStr.Length - rule.deleteToNum + 1;
                        if (bb < 0 )
                            bb = 0;
                        int ee = rule.deleteToNum - rule.deleteBegin;
                        if (ee < 0)
                            ee = 0;
                        if (ee > replaceStr.Length - rule.deleteBegin)
                            ee = replaceStr.Length - rule.deleteBegin;
                        string delstr = replaceStr.Substring(bb, ee);
                        int idx = replaceStr.LastIndexOf(delstr);
                        outStr = replaceStr.Substring(0, idx) + replaceStr.Substring(idx + delstr.Length);
                    }
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
                        string str = replaceStr.Substring(i,  1);
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
        //拼音
        public static string SerializeProcess(RuleInfoSerialize rule, string nowStr,int no)
        {
            string outStr = "";
            string replaceStr = nowStr;
            string expName = "";
            if (rule.serializeIgnoreExp == 1)
                replaceStr = Functions.GetFileNameButExp(replaceStr, out expName);

            string strNum = "";
            int bnum = rule.serializeBegin + (rule.serializeAdd * no);
            strNum = bnum.ToString();
            if (rule.serializeFullZero == 1)
            {
                strNum = bnum.ToString().PadLeft(rule.serializeFullZeroNumber, '0'); ;
            }
            switch (rule.serializePlaceType) //模式，1：插入前面，2：后面，3：位置
            {
                case 1:
                    outStr = strNum + replaceStr;
                    break;
                case 2:
                    outStr = replaceStr + strNum;
                    break;
                case 3:
                    int bstr = rule.serializePlaceNumber - 1;
                    if(bstr > replaceStr.Length)
                        bstr = replaceStr.Length;
                    outStr = replaceStr.Substring(0, bstr) + strNum+ replaceStr.Substring(bstr);
                    break;
            }

            //忽略扩展名字
            if (rule.serializeIgnoreExp == 1)
                outStr += "." + expName;

            return outStr;
        }

        //显示描述
        public static string FormatRuleString(RuleInfo nowInfo)
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

        //根据当前ruleinfo列表获得ruleinfodata
        public static List<RuleInfoData> GetRuleListDatas(List<RuleInfo> infos)
        {
            List<RuleInfoData> datas = new List<RuleInfoData>();
            for (int i = 0; i < infos.Count; i++)
            {
                RuleInfo nowInfo = infos[i];
                RuleInfoData data = GetRuleData(nowInfo);
                data.no = i+1;
                datas.Add(data);
            }
            return datas;
        }

        public static RuleInfoData GetRuleData(RuleInfo nowInfo)
        {
            string showtxt = NamedFun.FormatRuleString(nowInfo);
            RuleInfoData d = (new RuleInfoData { isEnable = nowInfo.enable, ruleName = nowInfo.ruleName, ruleDesc = showtxt });
            return d;
        }

        public static void SaveRules(string ruleFile, List<RuleInfo> list)
        {
            string str = JsonConvert.SerializeObject(list);
            Functions.StreamWriter(ruleFile, str);
        }

        public static List<RuleInfo> ReadRules(string ruleFile)
        {
            List<RuleInfo> list = new List<RuleInfo>();
            string str = Functions.GetFileText(ruleFile);
            //这里要按照不同的类型去解析
            if (string.IsNullOrEmpty(str))
                return list;
            JArray jary = (JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(str);
            for (int i = 0; i < jary.Count; i++)
            {
                RuleInfo info = null;
                RuleTypeEnum nowType = jary[i]["ruleType"].ToObject<RuleTypeEnum>();
                switch (nowType)
                {
                    case RuleTypeEnum.Insert:
                        info = JsonConvert.DeserializeObject<RuleInfoInsert>(jary[i].ToString());
                        break;
                    case RuleTypeEnum.Replace:
                        info = JsonConvert.DeserializeObject<RuleInfoReplace>(jary[i].ToString());
                        break;
                    case RuleTypeEnum.Delete:
                        info = JsonConvert.DeserializeObject<RuleInfoDelete>(jary[i].ToString());
                        break;
                    case RuleTypeEnum.UpLower:
                        info = JsonConvert.DeserializeObject<RuleInfoUpLower>(jary[i].ToString());
                        break;
                    case RuleTypeEnum.PinYin:
                        info = JsonConvert.DeserializeObject<RuleInfoPinYin>(jary[i].ToString());
                        break;
                    case RuleTypeEnum.Serialize:
                        info = JsonConvert.DeserializeObject<RuleInfoSerialize>(jary[i].ToString());
                        break;
                }
        

                //list = JsonConvert.DeserializeObject<List<RuleInfo>>(str);
                //RuleInfo info = JsonConvert.DeserializeObject<RuleInfo>(jary[i].ToString());
                list.Add(info);
            }
            return list;
        }

        
    }

    
}
