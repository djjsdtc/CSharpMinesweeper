using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CSharpMinesweeper
{
    public partial class Form1 : Form
    {
        private bool firstclick = true;
        //是否为第一次单击雷区按钮
        private const int BOMB = 9, BOMBERROR = 10;
        //表示按钮状态的助记常量
        private int xmax = 0, ymax = 0, total = 0, countleft, flagleft;
        //雷区x、y坐标的最大值、地雷总数、未挖开的方块总数、标旗帜后地雷剩余数目
        private int time = -1;
        //时间初始值为-1，因为定时器启用时立即会给时间+1
        private bool leftbutton, rightbutton;
        //用于判断是否左右键同时按下
        private MyButton[,] btn = new MyButton[30, 16];
        //按钮阵（控件数组）
        //说明：C#中数组的定义方式不同于C++的直接给出下标，而必须用动态内存分配的方式
        //不需要用delete，.net framework会在程序结束的时候自动回收内存

        public Form1()
        //窗体类构造函数
        {
            InitializeComponent();
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 30; x++)
                {
                    btn[x, y] = new MyButton(x, y);
                    //控件数组赋值
                    //说明：定义类对象也必须用动态内存分配方式，在new后面是类构造函数的调用格式
                    btn[x, y].MouseUp += new MouseEventHandler(btnOnMouseUp);
                    btn[x, y].MouseDown += new MouseEventHandler(btnOnMouseDown);
                    //给按钮增加事件处理程序
                }
            }
        }

        void btnOnMouseDown(object sender, MouseEventArgs e)
        //鼠标按钮在雷区按下时的事件处理程序
        {
            if (leftbutton == false) leftbutton = (e.Button == MouseButtons.Left);
            if (rightbutton == false) rightbutton = (e.Button == MouseButtons.Right);
            //如果leftbutton=rightbutton=true，说明左右键同时按下
            //由于C#给鼠标状态分配的对象MouseEventArgs e的e.Button属性不可能同时等于MouseButtons.Left、MouseButtons.Right
            //因此只能通过此方法接收微小时间间隔内出现的左右按钮按下事件，以此表示同时按下
            //即：鼠标左右按钮同时按下时会有一个微小的时间差，在此时间差前先按下左/右键，记录下来
            //在此时间差过去后按下右/左键，记录下来
        }

        private void Form1_Load(object sender, EventArgs e)
        //窗体加载时的事件处理程序
        {
            LoadWindow(ref xmax, ref ymax, ref total);
            //答题模块、选择难度，用引用方式读入雷区大小和雷数
            countleft = xmax * ymax;
            //未翻开方块数等于总方块数
            flagleft = total;
            //未标记雷数等于总雷数
            
            for (int y = 0; y < ymax; y++)
            {
                for (int x = 0; x < xmax; x++)
                {
                    panel1.Controls.Add(btn[x, y]);
                    //显示按钮阵
                }
            }
            lblBombLeft.Text = flagleft.ToString();
            //显示剩余多少地雷
            
        }

        private void btnOnMouseUp(object sender, MouseEventArgs e)
        //鼠标按钮释放时的事件处理程序
        {
            if (leftbutton == rightbutton == true)
            {
                //如果左右键同时单击
                if (((MyButton)sender).isOpen == true) test(ref btn, (MyButton)sender);
                //只有在已经翻开的按钮上左右键同时单击才可以尝试翻开周围所有方块
            }
            else if (e.Button == MouseButtons.Left)
            {
                //左键单击
                if (firstclick == true)
                {
                    //如果是第一次单击，布雷
                    SetBomb(ref btn, xmax, ymax, ((MyButton)sender).x, ((MyButton)sender).y, total);
                    firstclick = false;
                    //取消第一次单击标志
                }
                if (((MyButton)sender).isOpen == false) ShowNumber(ref btn, xmax, ymax, (MyButton)sender);
                //如果不是第一次单击，翻开当前方块
            }
            else if (e.Button == MouseButtons.Right)
            {
                //右键单击
                flagleft -= ((MyButton)sender).SetFlag();
                //标记旗帜，并且根据是标记、取消还是无动作更新剩余未标记地雷数
                lblBombLeft.Text = flagleft.ToString();
                //更新界面显示
            }
            leftbutton = rightbutton = false;
            //重置两鼠标按钮标志
        }

        private void LoadWindow(ref int xmax, ref int ymax, ref int total)
        {
            //进入答题模块，根据返回值进行对应处理
            switch (cppdll.SelectFunction())
            {
                case cppdll.CANNOTGAME: Application.Exit(); break;
                case cppdll.FILENOTFOUND:
                    MessageBox.Show(cppdll.FILENOTFOUND_STR, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit(); break;
                case cppdll.NOTINITDAT:
                    MessageBox.Show(cppdll.NOTINITDAT_STR, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit(); break;
                case cppdll.NOTINITSET:
                    MessageBox.Show(cppdll.NOTINITSET_STR, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit(); break;
            }
            
            SettingDialog std = new SettingDialog();
            switch (std.ShowDialog())
            {
                //显示选择难度对话框，根据返回值设定对应的雷区大小和雷数
                //default表示难度对话框非正常关闭，退出程序
                case DialogResult.Abort: xmax = ymax = 9; total = 10; return;
                case DialogResult.Retry: xmax = ymax = 16; total = 40; return;
                case DialogResult.Ignore: xmax = 30; ymax = 16; total = 99; return;
                default: Application.Exit(); return;
            }
        }

        private void 返回主菜单RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();    //重启
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetBomb(ref MyButton[,] btn, int xmax, int ymax, int xnow, int ynow, int total)
        //布雷
        {
            int a, b; int count = 0;
            //随机数获得的横纵坐标、已经布下的雷数
            Random rd = new Random();
            //建立伪随机数类对象，其默认构造函数中已经按当前时间初始化好了伪随机数的种子
            do
            {
                a = rd.Next() % xmax;
                b = rd.Next() % ymax;
                //获取随机横纵坐标
                if (a != xnow && b != ynow && btn[a, b].state != BOMB)
                //能布雷的方块应该满足：不是我单击的地方，不是已经布下的地雷
                {
                    btn[a, b].state = BOMB;
                    //布雷
                    count++;
                    //已经布下的雷数+1
                }
            } while (count != total);
            //重复此操作直到布下了规定数量的雷
            for (a = 0; a < xmax; a++)
            {
                for (b = 0; b < ymax; b++)
                {
                    //以下是计算周围方块中雷的个数的代码
                    //大体思路：遍历周围的八个方块（一开始的几行表示如果是边框上的那就不应该是周围八个，否则下标越界）
                    //然后计算雷数，并且赋入本按钮
                    int mina, minb, maxa, maxb;
                    if (btn[a, b].state == BOMB) continue;
                    //如果自己是雷那就不要计算周围方块的雷数，用continue继续给下一个方块计算
                    if (a == 0) mina = 0;
                    else mina = a - 1;
                    if (a == xmax - 1) maxa = a;
                    else maxa = a + 1;
                    if (b == 0) minb = 0;
                    else minb = b - 1;
                    if (b == ymax - 1) maxb = b;
                    else maxb = b + 1;
                    //这里的count变量表示周围八个方块中的雷数
                    count = 0;
                    for (int ta = mina; ta <= maxa; ta++)
                    {
                        for (int tb = minb; tb <= maxb; tb++)
                        {
                            if (btn[ta, tb].state == BOMB) { count++; }
                        }
                    }
                    btn[a, b].state = count;
                }
            }
            timer1.Enabled = true;
            //开始游戏，开始计时
        }

        private void ShowNumber(ref MyButton[,] btn, int xmax, int ymax, MyButton sender)
        //翻开方块
        {
            //递归出口：遇到已经显示数字的方块时退出
            if (sender.isOpen == true) return;
            if (sender.isFlag == true) ; //单击旗子不能有任何动作
            else if (sender.state == BOMB)
            {
                //如果单击了炸弹
                EndGame(false);
                //表示按“输”的情况结束游戏
            }
            //有数字的话只显示本数字
            else
            {
                sender.ShowState();   //本按钮显示数字
                countleft--;   //未翻开方块数-1
                if (countleft == total)
                {
                    EndGame(true);
                    //如果未翻开方块数等于雷数，赢得胜利
                }
                if (sender.state == 0)
                {
                    //没有数字的话
                    int minx, miny, maxx, maxy;
                    if (sender.x == 0) minx = 0;
                    else minx = sender.x - 1;
                    if (sender.x == xmax - 1) maxx = sender.x;
                    else maxx = sender.x + 1;
                    if (sender.y == 0) miny = 0;
                    else miny = sender.y - 1;
                    if (sender.y == ymax - 1) maxy = sender.y;
                    else maxy = sender.y + 1;
                    //采用八个方向扩散单击的递归方法实现显示到全部数字区
                    for (int nowx = minx; nowx <= maxx; nowx++)
                        for (int nowy = miny; nowy <= maxy; nowy++)
                        {
                            ShowNumber(ref btn, xmax, ymax, btn[nowx, nowy]);
                        }
                }
            }
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about=new AboutBox();
            about.ShowDialog();
            //显示关于对话框
        }

        private void EndGame(bool isWin)
        //通用游戏结束事件处理函数，接受参数为true表示赢得胜利，false表示失败
        {
            timer1.Enabled = false;
            //游戏结束，停止计时
            for (int y = 0; y < ymax; y++)
            {
                for (int x = 0; x < xmax; x++)
                {
                    btn[x, y].Enabled = false;
                    //无论胜利还是失败，雷区不得再点击
                    if (isWin == false)
                    {
                        //如果失败
                        if (btn[x, y].state == BOMB && btn[x, y].isFlag == false) btn[x, y].ShowState();
                        //翻开所有地雷
                        if (btn[x, y].state != BOMB && btn[x, y].isFlag == true)
                        //如果有不是地雷但错标为地雷的
                        {
                            btn[x, y].SetState(BOMBERROR);
                            btn[x, y].ShowState();
                            //按钮上显示标错了
                        }
                    }
                    else
                    {
                        //如果获胜
                        if (btn[x, y].state == BOMB && btn[x, y].isFlag == false) btn[x, y].SetFlag();
                        //给所有地雷标旗
                    }
                }
            }
            //按输赢更新状态提示条
            if (isWin == true)
            {
                flagleft = 0;
                lblBombLeft.Text = flagleft.ToString();
                lblGameInfo.Text = "恭喜恭喜，你贏了！单击游戏->返回主菜单重新开始。";
            }
            else
            {
                lblGameInfo.Text = "很遗憾，你输了，请再接再厉！单击游戏->返回主菜单重新开始。";
            }
        }

        private void test(ref MyButton[,] btn, MyButton sender)
        {
            //左右键同时按下时尝试翻开周围的八个方块
            //按扫雷的规则，如果八个方块中无地雷则翻开
            //如果有地雷则不操作
            //如果有误标则自动触发所有地雷，游戏失败
            int minx, miny, maxx, maxy;
            if (sender.x == 0) minx = 0;
            else minx = sender.x - 1;
            if (sender.x == xmax - 1) maxx = sender.x;
            else maxx = sender.x + 1;
            if (sender.y == 0) miny = 0;
            else miny = sender.y - 1;
            if (sender.y == ymax - 1) maxy = sender.y;
            else maxy = sender.y + 1;
            bool nobomb = true;
            //如果周围八个方块均不是地雷则可以翻开，此标志用处是判断这一点
            for (int nowx = minx; nowx <= maxx; nowx++)
                for (int nowy = miny; nowy <= maxy; nowy++)
                {
                    if (btn[nowx, nowy].state == BOMB && btn[nowx, nowy].isFlag == false) nobomb = false;
                    //有地雷就不允许翻开
                    if (btn[nowx, nowy].state != BOMB && btn[nowx, nowy].isFlag == true)
                    {
                        //如果有误标则失败
                        EndGame(false);
                        return;
                    }
                }
            if (nobomb == true)
            {
                //如果确实没有地雷，翻开周围的八个方块
                for (int nowx = minx; nowx <= maxx; nowx++)
                    for (int nowy = miny; nowy <= maxy; nowy++)
                    {
                        ShowNumber(ref btn, xmax, ymax, btn[nowx, nowy]);
                    }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        //计时
        {
            time++;
            lblTime.Text=time.ToString();
            //更新时间
            switch (time % 15)
            {
                //15秒更新一次信息提示栏文字
                case 0: lblGameInfo.Text = "My Minesweeper 扫雷游戏"; break;
                case 5: lblGameInfo.Text = "游戏目标：找到雷区中的所有地雷，而不许踩到地雷。"; break;
                case 10: lblGameInfo.Text = "恭喜你完成了数学题目，请在扫雷游戏中尽情放松。"; break;
            }
            if (time == 999) timer1.Enabled = false;
            //时间达到上限999之后就不再往上计时
        }

        private void 帮助主题HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string helpfile = "CSharpMinesweeper.chm";
            //帮助文件的文件名
            if (File.Exists(helpfile)) System.Diagnostics.Process.Start(helpfile);
            //如果帮助文件存在就打开
            else MessageBox.Show("找不到帮助文件 CSharpMinesweeper.chm。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //否则报错
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
        }

    }
}
