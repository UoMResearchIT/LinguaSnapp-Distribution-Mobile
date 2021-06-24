#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//   Copyright (c) 2005-2009 empira Software GmbH, Cologne (Germany)
// Modifications by:
//   Thomas Hövel
//   Copyright (c) 2015 TH Software, Troisdorf (Germany), http://developer.th-soft.com/developer/
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System.Text;
using PdfSharp.Drawing;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf.IO;

namespace PdfSharp.Drawing.Layout
{
    /// <summary>
    /// Represents a very simple text formatter.
    /// If this class does not satisfy your needs on formatting paragraphs I recommend to take a look
    /// at MigraDoc Foundation. Alternatively you should copy this class in your own source code and modify it.
    /// </summary>
    public class XTextFormatterEx
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XTextFormatter"/> class.
        /// </summary>
        public XTextFormatterEx(XGraphics gfx)
        {
            if (gfx == null)
                throw new ArgumentNullException("gfx");
            this.gfx = gfx;
        }
        XGraphics gfx;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        string text;

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        public XFont Font
        {
            get { return this.font; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("font");
                this.font = value;

                this.lineSpace = font.GetHeight();
                this.cyAscent = lineSpace * font.CellAscent / font.CellSpace;
                this.cyDescent = lineSpace * font.CellDescent / font.CellSpace;

                // HACK in XTextFormatter
                this.spaceWidth = gfx.MeasureString("x x", value).Width;
                this.spaceWidth -= gfx.MeasureString("xx", value).Width;
            }
        }
        XFont font;
        double lineSpace;
        double cyAscent;
        double cyDescent;
        double spaceWidth;

        private bool preparedText = false;

        /// <summary>
        /// Gets or sets the bounding box of the layout.
        /// </summary>
        public XRect LayoutRectangle
        {
            get { return this.layoutRectangle; }
            set { this.layoutRectangle = value; }
        }
        XRect layoutRectangle;

        /// <summary>
        /// Gets or sets the alignment of the text.
        /// </summary>
        public XParagraphAlignment Alignment
        {
            get { return this.alignment; }
            set { this.alignment = value; }
        }
        XParagraphAlignment alignment = XParagraphAlignment.Left;

        /// <summary>
        /// Prepares a given text for drawing, performs the layout, returns the index of the last fitting char and the needed height.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font to be used.</param>
        /// <param name="layoutRectangle">The layout rectangle. Set the correct width.
        /// Either set the available height to find how many chars will fit.
        /// Or set height to double.MaxValue to find which height will be needed to draw the whole text.</param>
        /// <param name="lastFittingChar">Index of the last fitting character. Can be -1 if the character was not determined. Will be -1 if the whole text can be drawn.</param>
        /// <param name="neededHeight">The needed height - either for the complete text or the used height of the given rect.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void PrepareDrawString(string text, XFont font, XRect layoutRectangle, out int lastFittingChar, out double neededHeight)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (font == null)
                throw new ArgumentNullException("font");

            Text = text;
            Font = font;
            LayoutRectangle = layoutRectangle;

            lastFittingChar = -1;
            neededHeight = double.MinValue;

            if (text.Length == 0)
                return;

            CreateBlocks();

            CreateLayout();

            preparedText = true;

            double dy = cyDescent + cyAscent;
            int count = this.blocks.Count;
            for (int idx = 0; idx < count; idx++)
            {
                Block block = (Block)this.blocks[idx];
                if (block.Stop)
                {
                    // We have a Stop block, so only part of the text will fit. We return the index of the last fitting char (and the height of the block, if available).
                    lastFittingChar = 0;
                    int idx2 = idx - 1;
                    while (idx2 >= 0)
                    {
                        Block block2 = (Block)this.blocks[idx2];
                        if (block2.EndIndex >= 0)
                        {
                            lastFittingChar = block2.EndIndex;
                            neededHeight = dy + block2.Location.Y; // Test this!!!!!
                            return;
                        }
                        --idx2;
                    }
                    return;
                }
                if (block.Type == BlockType.LineBreak)
                    continue;
                //gfx.DrawString(block.Text, font, brush, dx + block.Location.x, dy + block.Location.y);
                neededHeight = dy + block.Location.Y; // Test this!!!!! Performance optimization?
            }
        }

        /// <summary>
        /// Draws the text that was previously prepared by calling PrepareDrawString or by passing a text to DrawString.
        /// </summary>
        /// <param name="brush">The brush used for drawing the text.</param>
        public void DrawString(XBrush brush)
        {
            DrawString(brush, XStringFormats.TopLeft);
        }

        /// <summary>
        /// Draws the text that was previously prepared by calling PrepareDrawString or by passing a text to DrawString.
        /// </summary>
        /// <param name="brush">The brush used for drawing the text.</param>
        /// <param name="format">Not yet implemented.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void DrawString(XBrush brush, XStringFormat format)
        {
            // TODO: Do we need "XStringFormat format" at PrepareDrawString or at DrawString? Not yet used anyway, but probably already needed at PrepareDrawString.
            if (!preparedText)
                throw new ArgumentException("PrepareDrawString must be called first.");
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (format.Alignment != XStringAlignment.Near || format.LineAlignment != XLineAlignment.Near)
                throw new ArgumentException("Only TopLeft alignment is currently implemented.");

            if (text.Length == 0)
                return;

            double dx = layoutRectangle.Location.X;
            double dy = layoutRectangle.Location.Y + cyAscent;
            int count = this.blocks.Count;
            for (int idx = 0; idx < count; idx++)
            {
                Block block = (Block)this.blocks[idx];
                if (block.Stop)
                    break;
                if (block.Type == BlockType.LineBreak)
                    continue;
                gfx.DrawString(block.Text, font, brush, dx + block.Location.X, dy + block.Location.Y);
            }
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle)
        {
            DrawString(text, font, brush, layoutRectangle, XStringFormats.TopLeft);
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        /// <param name="format">The format. Must be <c>XStringFormat.TopLeft</c></param>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
        {
            int dummy1;
            double dummy2;
            PrepareDrawString(text, font, layoutRectangle, out dummy1, out dummy2);

            DrawString(brush);
        }

        void CreateBlocks()
        {
            this.blocks.Clear();
            int length = this.text.Length;
            bool inNonWhiteSpace = false;
            int startIndex = 0, blockLength = 0;
            for (int idx = 0; idx < length; idx++)
            {
                char ch = text[idx];

                // Treat CR and CRLF as LF
                if (ch == Chars.CR)
                {
                    if (idx < length - 1 && text[idx + 1] == Chars.LF)
                        idx++;
                    ch = Chars.LF;
                }
                if (ch == Chars.LF)
                {
                    if (blockLength != 0)
                    {
                        string token = text.Substring(startIndex, blockLength);
                        this.blocks.Add(new Block(token, BlockType.Text,
                          this.gfx.MeasureString(token, this.font).Width,
                          startIndex, startIndex + blockLength - 1));
                    }
                    startIndex = idx + 1;
                    blockLength = 0;
                    this.blocks.Add(new Block(BlockType.LineBreak));
                }
                else if (Char.IsWhiteSpace(ch))
                {
                    if (inNonWhiteSpace)
                    {
                        string token = text.Substring(startIndex, blockLength);
                        this.blocks.Add(new Block(token, BlockType.Text,
                          this.gfx.MeasureString(token, this.font).Width,
                          startIndex, startIndex + blockLength - 1));
                        startIndex = idx + 1;
                        blockLength = 0;
                    }
                    else
                    {
                        blockLength++;
                    }
                }
                else
                {
                    inNonWhiteSpace = true;
                    blockLength++;
                }
            }
            if (blockLength != 0)
            {
                string token = text.Substring(startIndex, blockLength);
                this.blocks.Add(new Block(token, BlockType.Text,
                                this.gfx.MeasureString(token, this.font).Width,
                                startIndex, startIndex + blockLength - 1));
            }
        }

        void CreateLayout()
        {
            double rectWidth = this.layoutRectangle.Width;
            double rectHeight = this.layoutRectangle.Height - this.cyAscent - this.cyDescent /*- lineSpace*/;
            int firstIndex = 0;
            double x = 0, y = 0;
            int count = this.blocks.Count;
            for (int idx = 0; idx < count; idx++)
            {
                Block block = (Block)this.blocks[idx];
                if (block.Type == BlockType.LineBreak)
                {
                    if (Alignment == XParagraphAlignment.Justify)
                        ((Block)this.blocks[firstIndex]).Alignment = XParagraphAlignment.Left;
                    AlignLine(firstIndex, idx - 1, rectWidth);
                    firstIndex = idx + 1;
                    x = 0;
                    y += this.lineSpace;
                    if (y > rectHeight)
                    {
                        block.Stop = true;
                        break;
                    }
                }
                else
                {
                    double width = block.Width; //!!!modTHHO 19.11.09 don't add this.spaceWidth here
                    if ((x + width <= rectWidth || x == 0) && block.Type != BlockType.LineBreak)
                    {
                        block.Location = new XPoint(x, y);
                        x += width + spaceWidth; //!!!modTHHO 19.11.09 add this.spaceWidth here
                    }
                    else
                    {
                        AlignLine(firstIndex, idx - 1, rectWidth);
                        firstIndex = idx;
                        y += lineSpace;
                        if (y > rectHeight)
                        {
                            block.Stop = true;
                            break;
                        }
                        block.Location = new XPoint(0, y);
                        x = width + spaceWidth; //!!!modTHHO 19.11.09 add this.spaceWidth here
                    }
                }
            }
            if (firstIndex < count && Alignment != XParagraphAlignment.Justify)
                AlignLine(firstIndex, count - 1, rectWidth);
        }

        /// <summary>
        /// Align center, right or justify.
        /// </summary>
        void AlignLine(int firstIndex, int lastIndex, double layoutWidth)
        {
            XParagraphAlignment blockAlignment = ((Block)(this.blocks[firstIndex])).Alignment;
            if (this.alignment == XParagraphAlignment.Left || blockAlignment == XParagraphAlignment.Left)
                return;

            int count = lastIndex - firstIndex + 1;
            if (count == 0)
                return;

            double totalWidth = -this.spaceWidth;
            for (int idx = firstIndex; idx <= lastIndex; idx++)
                totalWidth += ((Block)(this.blocks[idx])).Width + this.spaceWidth;

            double dx = Math.Max(layoutWidth - totalWidth, 0);
            //Debug.Assert(dx >= 0);
            if (this.alignment != XParagraphAlignment.Justify)
            {
                if (this.alignment == XParagraphAlignment.Center)
                    dx /= 2;
                for (int idx = firstIndex; idx <= lastIndex; idx++)
                {
                    Block block = (Block)this.blocks[idx];
                    block.Location += new XSize(dx, 0);
                }
            }
            else if (count > 1) // case: justify
            {
                dx /= count - 1;
                for (int idx = firstIndex + 1, i = 1; idx <= lastIndex; idx++, i++)
                {
                    Block block = (Block)this.blocks[idx];
                    block.Location += new XSize(dx * i, 0);
                }
            }
        }

        readonly List<Block> blocks = new List<Block>();

        enum BlockType
        {
            Text, Space, Hyphen, LineBreak,
        }

        /// <summary>
        /// Represents a single word.
        /// </summary>
        class Block
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Block"/> class.
            /// </summary>
            /// <param name="text">The text of the block.</param>
            /// <param name="type">The type of the block.</param>
            /// <param name="width">The width of the text.</param>
            /// <param name="startIndex"></param>
            /// <param name="endIndex"></param>
            public Block(string text, BlockType type, double width, int startIndex, int endIndex)
            {
                Text = text;
                Type = type;
                Width = width;
                StartIndex = startIndex;
                EndIndex = endIndex;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Block"/> class.
            /// </summary>
            /// <param name="type">The type.</param>
            public Block(BlockType type)
            {
                Type = type;
            }

            /// <summary>
            /// The text represented by this block.
            /// </summary>
            public string Text;

            public int StartIndex = -1;
            public int EndIndex = -1;

            /// <summary>
            /// The type of the block.
            /// </summary>
            public BlockType Type;

            /// <summary>
            /// The width of the text.
            /// </summary>
            public double Width;

            /// <summary>
            /// The location relative to the upper left corner of the layout rectangle.
            /// </summary>
            public XPoint Location;

            /// <summary>
            /// The alignment of this line.
            /// </summary>
            public XParagraphAlignment Alignment;

            /// <summary>
            /// A flag indicating that this is the last block that fits in the layout rectangle.
            /// </summary>
            public bool Stop;
        }
    }
}