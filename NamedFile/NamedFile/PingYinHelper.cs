using System.Text;
using System;

public class PingYinHelper
{
    private static Encoding gb2312 = Encoding.GetEncoding("GB2312");

    public enum UpLowerOrNormal
    { 
        Normal = 0,
        Upper = 1,
        Lower = 2,
        FirstUpper = 3
    }
    /// <summary>
    /// 汉字转全拼
    /// </summary>
    /// <param name="strChinese"></param>
    /// <returns></returns>
    public static string ConvertToAllSpell(string strChinese, UpLowerOrNormal flag = UpLowerOrNormal.Normal)
    {
        try
        {
            if (strChinese.Length != 0)
            {
                StringBuilder fullSpell = new StringBuilder();
                for (int i = 0; i < strChinese.Length; i++)
                {
                    var chr = strChinese[i];
                    fullSpell.Append(GetSpell(chr));
                }
                string outstr = "";
                switch (flag)
                { 
                    case UpLowerOrNormal.Normal:
                        outstr = fullSpell.ToString();
                        break;
                    case UpLowerOrNormal.Upper:
                        outstr = fullSpell.ToString().ToUpper();
                        break;
                    case UpLowerOrNormal.Lower:
                        outstr = fullSpell.ToString().ToLower();
                        break;
                }
                return outstr;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("出错！" + e.Message);
        }
        return string.Empty;
    }

    /// <summary>
    /// 汉字转首字母
    /// </summary>
    /// <param name="strChinese"></param>
    /// <returns></returns>
    public static string GetFirstSpell(string strChinese)
    {
        try
        {
            if (strChinese.Length != 0)
            {
                StringBuilder fullSpell = new StringBuilder();
                for (int i = 0; i < strChinese.Length; i++)
                {
                    var chr = strChinese[i];
                    fullSpell.Append(GetSpell(chr)[0]);
                }
                return fullSpell.ToString().ToUpper();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("出错！" + e.Message);
        }

        return string.Empty;
    }

    private static string GetSpell(char chr)
    {
        var coverchr = NPinyin.Pinyin.GetPinyin(chr);
        return coverchr;
    }
}
