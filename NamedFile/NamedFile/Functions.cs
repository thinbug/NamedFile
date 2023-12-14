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
            return fullPath.Substring(index + 1);
        }
    }
}
