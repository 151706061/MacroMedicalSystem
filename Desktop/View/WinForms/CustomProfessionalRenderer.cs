using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Macro.Desktop.View.WinForms
{
    public class CustomProfessionalRenderer : ToolStripProfessionalRenderer
    {
        private Color _color = Color.Red;
        public CustomProfessionalRenderer()
            : base()
        {
        }
        public CustomProfessionalRenderer(Color color)
            : base()
        {
            _color = color;
        }

        //渲染背景 包括menustrip背景 toolstripDropDown背景
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;//抗锯齿
            Rectangle bounds = e.AffectedBounds;
            LinearGradientBrush lgbrush = new LinearGradientBrush(new Point(0, 0), new Point(0, toolStrip.Height), _color, _color);
            if (toolStrip is MenuStrip)
            {
                //由menuStrip的Paint方法定义 这里不做操作
            }
            else if (toolStrip is ToolStripDropDown)
            {
                int diameter = 10;//直径
                GraphicsPath path = new GraphicsPath();
                Rectangle rect = new Rectangle(Point.Empty, toolStrip.Size);
                Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

                path.AddLine(0, 0, 10, 0);
                // 右上角
                arcRect.X = rect.Right - diameter;
                path.AddArc(arcRect, 270, 90);

                // 右下角
                arcRect.Y = rect.Bottom - diameter;
                path.AddArc(arcRect, 0, 90);

                // 左下角
                arcRect.X = rect.Left;
                path.AddArc(arcRect, 90, 90);
                path.CloseFigure();
                toolStrip.Region = new Region(path);
                g.FillPath(lgbrush, path);
            }
            else
            {
                base.OnRenderToolStripBackground(e);
            }
        }


        //渲染箭头 更改箭头颜色
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = _color;
            base.OnRenderArrow(e);
        }

    }

    public partial class CustomContrlsMenuStrip : MenuStrip
    {
        private Color _themeColor = Color.Gray;
        public CustomContrlsMenuStrip()
        {
            this.Renderer = new CustomProfessionalRenderer(_themeColor);
        }
        public Color ThemeColor
        {
            get { return _themeColor; }
            set
            {
                _themeColor = value;
                this.Renderer = new CustomProfessionalRenderer(_themeColor);
            }
        }
    }
}
