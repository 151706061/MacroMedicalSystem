#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Macro.ImageServer.Web.Application.Pages.Common
{
    /// <summary>
    /// Generate a disk usage bar.
    /// </summary>
    /// <remarks>
    /// The usage and watermarks are specified in the Query string.
    /// To embed the generated image within another page, use <img src="BarChart.aspx?pct=xxxx&high=xxxx&low=xxxx"/>
    /// 
    /// </remarks>
    public partial class BarChart : System.Web.UI.Page
    {
        #region Private members

        private float _percentage;
        private float _high;
        private float _low;
        private int _width;
        private int _height;

        #endregion

        #region protected methods

        protected void Page_Load(object sender, EventArgs e)
        {
            // Read the input from the query string
            _percentage = float.Parse(Request.QueryString[ImageServerConstants.Pct]);
            _high = float.Parse(Request.QueryString[ImageServerConstants.High]);
            _low = float.Parse(Request.QueryString[ImageServerConstants.Low]);


            // set the ContentType appropriately, we are creating PNG image
            Response.ContentType = ImageServerConstants.ImagePng;

            // Load the background image
            Image bmp = Image.FromFile(Server.MapPath(ImageServerConstants.ImageURLs.UsageBar));
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            _width = bmp.Width;
            _height = bmp.Height;

            DrawUsageOverlay(ref graphics);

            // Save the image to memory stream (needed to do this if we are creating PNG image)
            MemoryStream MemStream = new MemoryStream();
            bmp.Save(MemStream, ImageFormat.Png);
            MemStream.WriteTo(Response.OutputStream);

            graphics.Dispose();
            bmp.Dispose();
        }

        /// <summary>
        /// Draw the disk usage bar.
        /// </summary>
        /// <param name="graphics"></param>
        protected void DrawUsageOverlay(ref Graphics graphics)
        {
            if (_percentage != float.NaN)
            {
                SolidBrush brush = new SolidBrush(Color.LightGray);

                int leftoffset = 1;

            
                if (_percentage < _low)
                    brush.Color = Color.FromArgb(150, 0x27, 0xFF, 0x0F);
                else if (_percentage < _high)
                    brush.Color = Color.FromArgb(150, 0xFF, 0xD8, 0x00);
                else
                    brush.Color = Color.FromArgb(150, 0xFF, 0x3A, 0x00);

                // overlay the "usage" bar on top
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.FillRectangle(brush, new Rectangle(leftoffset, 6, (int)(_width * _percentage / 100f), 8));

                // add watermark icons
                graphics.CompositingMode = CompositingMode.SourceOver;
                Image watermark = Image.FromFile(Server.MapPath(ImageServerConstants.ImageURLs.Watermark));


                graphics.DrawImageUnscaled(watermark, (int)(_width * _high / 100f) - watermark.Width / 2 + leftoffset, 12);
                graphics.DrawImageUnscaled(watermark, (int)(_width * _low / 100f) - watermark.Width / 2 + leftoffset, 12);

                watermark.Dispose();
            }

            
        }

        #endregion protected methods
    }
}