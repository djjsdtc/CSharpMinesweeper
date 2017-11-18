using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

/* 不规则窗体移动说明：
 * 由于不规则窗体没有标题栏，因此只能靠拖动窗体本身移动。
 * 移动的大题思路是：在鼠标按下时记录当前鼠标的位置
 * 然后鼠标移动多少，窗体跟着移动多少
 * 这就需要将按下鼠标时的位置记录为偏移量，并且为当时鼠标位置的相反数
 * 然后移动的时候将鼠标到达的位置叠加上刚才的偏移量，
 * 形成鼠标移动的距离，然后窗体跟着移动这么多距离
 * 这种方法移动窗体时如果拖到屏幕外再拖回来，会发生窗体闪动现象
 * 解决方法是双缓存，即窗体的DoubleBuffed属性设为True
 */

namespace CSharpMinesweeper
{
    public partial class AboutBox : Form
    {
        private Point mouse_offset;
        //Point类型变量mouse_offset记录鼠标位移的偏移量
        public AboutBox()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                //获取当前鼠标指针的位置
                mousePos.Offset(mouse_offset.X, mouse_offset.Y);
                //设置偏移
                Location = mousePos;
                //移动窗体
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //运行“系统信息”程序msinfo32.exe
            System.Diagnostics.Process.Start("msinfo32.exe");
            this.Close();
        }

        private void AboutBox_MouseDown(object sender, MouseEventArgs e)
        {
            //Point类型变量mouse_offset记录鼠标位移的偏移量
            mouse_offset = new Point(-e.X, -e.Y);
        }
    }
}
