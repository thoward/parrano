namespace Parrano.Api
{
    /// <summary>
    /// The possible destination document view types for a pdflink 
    /// </summary>
    public enum DocumentViewType
    {
        /// <summary>
        /// Fit to the page.
        /// </summary>
        fitpage,
        /// <summary>
        /// Fit to the page width.
        /// </summary>
        fitwidth,
        /// <summary>
        /// Fit to the page height.
        /// </summary>
        fitheight,
        /// <summary>
        /// Fit to the bounding box.
        /// </summary>
        fitbbox
    }
}