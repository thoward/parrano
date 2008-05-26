namespace Parrano.Api
{
    /// <summary>
    /// The possible color types for use with the PS_setcolor API call.
    /// </summary>
    public enum SetColorScopeType
    {
        /// <summary>
        /// Effects color for both drawing and filling
        /// </summary>
        both,
        /// <summary>
        /// Effects color for filling
        /// </summary>
        fill,
        /// <summary>
        /// Effects color for drawing
        /// </summary>
        fillstroke
    }
}