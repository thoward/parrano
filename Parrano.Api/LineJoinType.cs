namespace Parrano.Api
{
    /// <summary>
    /// The possible line join types.
    /// </summary>
    public enum LineJoinType
    {
        /// <summary>
        /// A square 45 degree miter corner join 
        /// </summary>
        PS_LINEJOIN_MITER = 0,
        /// <summary>
        /// A rounded corner join 
        /// </summary>
        PS_LINEJOIN_ROUND = 1,
        /// <summary>
        /// A bevelled corner join
        /// </summary>
        PS_LINEJOIN_BEVEL = 2
    }
}