using System.Runtime.InteropServices;

namespace Parrano.Api
{
    /// <summary>
    /// A struct describing the dimensions of a string as returns from a call to PS_string_geometry().
    /// <para>
    /// Note: This struct is used to replace the dimension parameter in the PS_string_geometry call,
    /// which is defined in the API as a float[]. The purpose of this is to be more
    /// clear what the values mean. This struct is not part of the compatible pslib API, and if you
    /// plan to port code to a different language or binding, you shoudl use the compatibility overload
    /// that returns dimension as a float[], instead of this struct...
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct StringGeometry
    {
        /// <summary>
        /// The width of the string.
        /// </summary>
        public float width;
        /// <summary>
        /// The ascender of the string. 
        /// </summary>
        public float ascender;
        /// <summary>
        /// The descender of the string. 
        /// </summary>
        public float descender;

        public float[] ToFloatArray()
        {
            return new float[] { width, ascender, descender };
        }
    }
}