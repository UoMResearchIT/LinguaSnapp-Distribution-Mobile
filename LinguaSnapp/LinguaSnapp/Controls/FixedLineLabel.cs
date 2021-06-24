using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.Controls
{
    /// <summary>
    /// An extended label which will wrap onto a fixed number of lines
    /// </summary>
    public class FixedLineLabel : Label
    {
        /// <summary>
        /// Number of lines the label should use
        /// </summary>
        public int NumberOfLines { get; set; } = 1;

        /// <summary>
        /// Ctor
        /// </summary>
        public FixedLineLabel() : base()
        {
        }
    }
}
