using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Drawing;

namespace CSharpMinesweeper
{
    public class MyButton : Button
    //继承于Button类的自定义控件，在系统给的按钮的基础上增加以下功能
    {
        public int x, y;
        //按钮所在的阵列横坐标、纵坐标
        public int state;
        //按钮状态
        public bool isFlag;
        //是否被标旗帜（右击）
        public bool isOpen;
        //是否已经被翻开
        public MyButton(int xres,int yres)
        //构造函数
        {
            x = xres; y = yres; state = 0;
            //传入表示坐标的参数，初始化按钮状态
            isFlag = false; isOpen = false;
            //没有标旗，没有翻开
            Width = Height = 25;
            //按钮的长宽
            Point pt = new Point(25*x, 25*y);
            Location = pt;
            //设置按钮状态
            FlatStyle = FlatStyle.Popup;
            //设置按钮风格为Popup风格
            TabStop = false;
            //雷区的按钮不需要在按Tab键的时候获得焦点
        }

        public void SetState(int mystate)
        {
            state = mystate;
            //设置状态
        }
        public void ShowState()
        //翻开按钮时执行的操作
        {
            switch (state)
            {
                //根据状态设置按钮显示的图标
                //样式0～8分别表示周围八个方块中有0～8个雷
                //样式9表示地雷，样式10表示标记了旗帜但实际上不是地雷
                case 0: BackgroundImage = (Image)Properties.Resources.Image0; break;
                case 1: BackgroundImage = (Image)Properties.Resources.Image1; break;
                case 2: BackgroundImage = (Image)Properties.Resources.Image2; break;
                case 3: BackgroundImage = (Image)Properties.Resources.Image3; break;
                case 4: BackgroundImage = (Image)Properties.Resources.Image4; break;
                case 5: BackgroundImage = (Image)Properties.Resources.Image5; break;
                case 6: BackgroundImage = (Image)Properties.Resources.Image6; break;
                case 7: BackgroundImage = (Image)Properties.Resources.Image7; break;
                case 8: BackgroundImage = (Image)Properties.Resources.Image8; break;
                case 9: BackgroundImage = (Image)Properties.Resources.Bomb; break;
                case 10:BackgroundImage = (Image)Properties.Resources.BombError; break;
            }
            FlatStyle = FlatStyle.Flat;
            //改变按钮样式为Flat
            isOpen = true;
            //已经翻开按钮
        }
        public int SetFlag()
        //给按钮设置旗帜时执行的操作
        //返回值：未操作返回0，设置旗帜返回1，取消旗帜返回-1
        {
            if (isOpen == true) return 0;
            //已经翻开的按钮不准设置为旗帜
            if (isFlag == false)
            {
                //设置旗帜
                isFlag = true;
                BackgroundImage = (Image)Properties.Resources.Flag;
                FlatStyle = FlatStyle.Flat;
                return 1;
            }
            else
            {
                //取消旗帜
                isFlag = false;
                BackgroundImage = null;
                FlatStyle = FlatStyle.Popup;
                return -1;
            }
        }
    }
}
