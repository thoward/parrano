namespace Parrano.Api
{
    /// <summary>
    /// The Postscript color space for the PS_setcolor API call.
    /// </summary>
    public enum ColorSpace
    {
        /// <summary>
        /// Grayscale color space.
        /// </summary>
        gray,
        /// <summary>
        /// RGB (Red, Green, Blue) color space.
        /// </summary>
        rgb,
        /// <summary>
        /// CMYK (Cyan, Magenta, Yellow, Black) color space.
        /// </summary>
        cmyk,
        /// <summary>
        /// Spot color space.
        /// </summary>
        spot,
        /// <summary>
        /// Pattern color space.
        /// </summary>
        pattern
    }
}