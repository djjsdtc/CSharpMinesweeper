using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CSharpMinesweeper
{
    class cppdll
    {
        //定义返回值的替代名称
        //由于C#没有define等预编译语句，因此只能用常量定义
        public const int ALLOK = 1000;
        public const int NOTINITSET = 1001;
        public const int NOTINITDAT = 1002;
        public const int CANNOTGAME = 2;
        public const int FILENOTFOUND = 1003;
        //错误提示的文本
        public const string FILENOTFOUND_STR ="找不到 QuestionAsker.dll 文件，尝试重新安装本程序以解决此问题。\n程序现在将退出。";
        public const string NOTINITSET_STR = "设置模块出现错误。请参考帮助文件以获取可能的解决方案。\n程序现在将退出。";
        public const string NOTINITDAT_STR = "统计模块出现错误。请参考帮助文件以获取可能的解决方案。\n程序现在将退出。";
        //C#导入DLL文件和其中的函数
        //可见比C++简单得多
        [DllImport("FunctionSelector.dll")]
        public static extern int SelectFunction();
    }
}
