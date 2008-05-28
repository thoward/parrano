using System;

namespace Parrano.Api
{
    /// <summary>
    /// The possible color types for use with the PS_setcolor API call.
    /// </summary>
    [Flags]
    public enum ColorUsage
    {
        /// <summary>
        /// Effects color for filling
        /// </summary>
        fill = 1,
        /// <summary>
        /// Effects color for drawing 
        /// </summary>
        stroke = 2,
        /// <summary>
        /// Effects color for both drawing and filling 
        /// </summary>
        fillstroke = fill | stroke,
        /// <summary>
        /// Effects color for both drawing and filling
        /// </summary>
        both = fillstroke
    }
}