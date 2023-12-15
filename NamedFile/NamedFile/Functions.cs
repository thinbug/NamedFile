using System;
using System.Collections.Generic;
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
    }
}
