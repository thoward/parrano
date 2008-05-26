namespace Parrano.Api
{
    /// <summary>
    /// Indicates the masking role of a PNG image. 
    /// </summary>
    public enum PngMaskType
    {
        /// <summary>
        /// Image is a mask for another image.
        /// </summary>
        mask,
        /// <summary>
        /// Image is to be masked by another image.
        /// </summary>
        masked
    }
}