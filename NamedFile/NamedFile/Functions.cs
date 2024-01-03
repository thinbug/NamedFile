using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedFile
{
    internal class Functions
    {
        //根据全路径获得文件名
        public static string GetFileName(string fullPath)
        { 
            int index = fullPath.LastIndexOf('\\');
            if(index == -1) { return fullPath; }
            return fullPath.Substring(index + 1);
        }

        //获取除了扩展名的文件名
        public static string GetFileNameButExp(string filename,out string expname)
        {
            int index = filename.LastIndexOf('.');
            expname = "";
            if(index != -1)
            {
                expname = filename.Substring(index + 1);
                return filename.Substring(0, index);
            }
            return filename;
        }

        /// <summary>
        /// 获得地址所在文件的所有字符信息
        /// </summary>
        public static string GetFileText(string path)
        {
            if(System.IO.File.Exists(path))
                return System.IO.File.ReadAllText(path);
            return string.Empty;
        }

        public static void StreamWriter(string path, string data)
        {
            try
            {
                string forder = System.IO.Path.GetDirectoryName(path);
                bool exists = System.IO.Directory.Exists(forder);
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(forder);
                }

                System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
                sw.Write(data);
                //关闭StreamWriter
                sw.Close();
            }
            catch (System.Exception ee)
            {
                Console.WriteLine("文件写入失败:" + path + " Error:" + ee.Message);
            }
        }
    }
}
