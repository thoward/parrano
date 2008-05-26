using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Parrano.Api
{
    public static class PSLib
    {
        /// <summary>
        /// Returns the major version number of the pslib dll.
        /// </summary>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_get_majorversion();

        /// <summary>
        /// returns the minor version number of the pslib dll.
        /// </summary>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_get_minorversion();

        /// <summary>
        /// returns the subminor version number of the pslib dll.
        /// </summary>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_get_subminorversion();

        /// <summary>
        /// Initialize the library
        /// </summary>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_boot();

        /// <summary>
        /// Final clean up of library
        /// </summary>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_shutdown();

        /// <summary>
        /// Sets information fields of document
        /// <para>Valid fields are Keywords, Subject, Title, Creator, Author, BoundingBox, and Orientation</para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        [DllImport("pslib.dll", EntryPoint = "PS_set_info", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Auto)]
        private static extern void PS_set_info_private(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string key,
                                                       [MarshalAs(UnmanagedType.LPStr)] string val);

        public static void PS_set_info(IntPtr psdoc, string key, string value)
        {
            if (InfoFields.IsValidFieldName(key))
            {
                PS_set_info_private(psdoc, key, value);
            }
        }

        public static void PS_set_info(IntPtr psdoc, InfoFields infoFields)
        {
            infoFields.UpdateDocument(psdoc);
        }

        /// <summary>
        /// Creating a new PostScript document object
        /// </summary>
        /// <returns>A resource identifier to the newly created postcript document.</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern IntPtr PS_new();

        /// <summary>
        /// <para>Creates a new PostScript document object, with callbacks for memory allocation functions and error handling..</para>
        /// </summary>
        /// <param name="errorhandler">A ErrorHandler delegate to a callback function for handling pslib errors.</param>
        /// <param name="allocproc">A AllocProc delegate to a callback function for handling pslib memory allocation.</param>
        /// <param name="reallocproc">A ReAllocProc delegate to a callback function for handling pslib memory re-allocation.</param>
        /// <param name="freeproc">A FreeProc delegate to a callback function for free pslib allocated memory.</param>
        /// <param name="opaque">A handle that pslib doesn't use.. Like a tag so that the calling program can keep track of a document with a handle other than the psdoc pointer. The value passed here can be retrived for a document at any time via PS_get_opaque(). </param>
        /// <returns>A resource identifier to the newly created postcript document.</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern IntPtr PS_new2([MarshalAs(UnmanagedType.FunctionPtr)] ErrorHandler errorhandler,
                                            [MarshalAs(UnmanagedType.FunctionPtr)] AllocProc allocproc,
                                            [MarshalAs(UnmanagedType.FunctionPtr)] ReAllocProc reallocproc,
                                            [MarshalAs(UnmanagedType.FunctionPtr)] FreeProc freeproc, IntPtr opaque);

        /// <summary>
        /// <para>Creates a new PostScript document object, with callbacks for error handling.</para>
        /// </summary>
        /// <param name="errorhandler">A ErrorHandler delegate to a callback function for handling pslib errors.</param>
        /// <returns>A resource identifier to the newly created postcript document.</returns>
        public static IntPtr PS_new2(ErrorHandler errorhandler)
        {
            return PS_new2(errorhandler, null, null, null, IntPtr.Zero);
        }

        [DllImport("pslib.dll",CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Auto)]
        public static extern int PS_open_fp(IntPtr psdoc, IntPtr fp);

        ///// <summary>
        ///// Not implemented. Do not use. 
        ///// <para>Uses an already open file pointer as the output file.</para>
        ///// </summary>
        ///// <param name="psdoc">A resource identifier to a postcript document.</param>
        ///// <param name="fp"></param>
        ///// <returns></returns>
        //public static int PS_open_fp(IntPtr psdoc, ref SafeFileHandle fp)
        //{
        //    throw new NotImplementedException("The method or operation is not implemented.");
        //}

        /// <summary>
        /// Returns pointer which has been passed to PS_new2()
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <returns>pointer which has been passed to PS_new2</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern IntPtr PS_get_opaque(IntPtr psdoc);

        /// <summary>
        /// <para>Opens a PostScript document for writing. Instead of passing a filename or 
        /// open file handle, you pass a function pointer to a function that the output stream can 
        /// write to. The function should have the following .NET signature: </para>
        /// <para>int WriteProc(IntPtr psdoc, IntPtr data, int size), which should write the block of data 
        /// located at the passed in pointer (of memory size in the size parameter) to the output stream, 
        /// and return the size of block that was written. </para>
        /// <para>If you pass null for the function pointer, the library will use it's internal memory buffer,
        /// and you can access the document via ps_get_buffer at any time.</para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="writeproc">a function pointer to a write procedure</param>
        /// <returns>A resource identifier to the opened "memory file" this can be used anywhere a file id is needed.</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_open_mem(IntPtr psdoc, WriteProcCallBack writeproc);

        /// <summary>
        /// Creates a new file on disk and writes the PostScript document into it. 
        /// The file will be closed when ps_close() is called. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_open_file(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string filename);

        /// <summary>
        /// Closes the PostScript document. 
        /// <para>This function writes the trailer of the PostScript document. It also writes the bookmark tree. ps_close() does not free any resources, which is done by ps_delete(). </para>
        /// <para>This function is also called by ps_delete() if it has not been called before. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_close(IntPtr psdoc);

        /// <summary>
        /// Mainly frees memory used by the document. 
        /// <para>Also closes a file, if it was not closed before with ps_close(). </para>
        /// <para>You should in any case close the file with ps_close() before, 
        /// because ps_close() not just closes the file but also outputs a trailor 
        /// containing PostScript comments like the number of pages in the document 
        /// and adding the bookmark hierarchy. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_delete(IntPtr psdoc);

        /// <summary>
        /// <para>
        /// Starts a new page. Although the parameters width and height imply a different 
        /// page size for each page, this is not possible in PostScript. The first call of 
        /// ps_begin_page() will set the page size for the whole document. Consecutive calls will 
        /// have no effect, except for creating a new page. The situation is different if you 
        /// intent to convert the PostScript document into PDF. This function places pdfmarks 
        /// into the document which can set the size for each page indiviually. The resulting PDF 
        /// document will have different page sizes. </para>
        /// 
        /// <para>
        /// Though PostScript does not know different page sizes, pslib places a bounding box 
        /// for each page into the document. This size is evaluated by some PostScript viewers 
        /// and will have precedence over the BoundingBox in the Header of the document. This 
        /// can lead to unexpected results when you set a BoundingBox whose lower left corner 
        /// is not (0, 0), because the bounding box of the page will always have a lower left 
        /// corner (0, 0) and overwrites the global setting. </para>
        /// 
        /// <para>
        /// Each page is encapsulated into save/restore. This means, that most of the settings 
        /// made on one page will not be retained on the next page. </para>
        /// 
        /// <para>
        /// If there is up to the first call of ps_begin_page() no call of ps_findfont(), then 
        /// the header of the PostScript document will be output and the bounding box will be 
        /// set to the size of the first page. The lower left corner of the bounding box is set 
        /// to (0, 0). If ps_findfont() was called before, then the header has been output already, 
        /// and the document will not have a valid bounding box. In order to prevent this, one should 
        /// call ps_set_info() to set the info field BoundingBox and possibly Orientation before any 
        /// ps_findfont() or ps_begin_page() calls. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="width">The width of the page in pixel, e.g. 596 for A4 format. </param>
        /// <param name="height">The height of the page in pixel, e.g. 842 for A4 format.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_begin_page(IntPtr psdoc, float width, float height);

        /// <summary>
        /// Ends a page which was started with ps_begin_page(). Ending a page will leave the current 
        /// drawing context, which e.g. requires to reload fonts if they were loading within the page, 
        /// and to set many other drawing parameters like the line width, or color.. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_end_page(IntPtr psdoc);

        /// <summary>
        /// Sets one of several parameters which are used by many functions.
        /// Parameters are by definition string values.
        /// <para>Parameters are either one of the special cases listed below, or they are set by PSLib
        /// program code at various times, or they are treated as generic dictionary values.. If the parameter name is not in the dictionary, calling set_parameter will add it to the parameter dictionary.</para>
        /// 	<para>
        /// The PSLib C code has special handling for the following names.</para>
        /// 	<para>FontAFM </para>
        /// 	<para>FontOutline </para>
        /// 	<para>FontProtusion </para>
        /// 	<para>FontEncoding </para>
        /// 	<para>RightMarginKerning </para>
        /// 	<para>LeftMarginKerning </para>
        /// 	<para>SearchPath </para> - attempts to load resources located at path.
        ///     <para>underline </para> - true or false
        ///     <para>overline </para> - true or false
        ///     <para>strikeout </para> - true or false
        ///     <para>warning </para> - true or false
        ///     <para>hyphendict  </para> - attempts to load hyphen dictionary located at path.
        ///     <para>inputencoding </para> - attempts to set the input encoding to specified value.
        /// 	<para> fontencoding </para>
        /// 	<para> The encoding of the currently active font. </para>
        /// 	<para>Some other known values (not specifically handled by ps_set_parameter function):</para>
        /// 	<para> ligaturedisolvechar </para>
        /// 	<para> The character which dissolves a ligature. If your are using a font which contains the
        /// ligature `ff' and `|' is the char to dissolve the ligature, then `f|f' will result
        /// in two `f' instead of the ligature `ff'. </para>
        /// 	<para> imageencoding </para>
        /// 	<para> The encoding used for encoding images. Can be either hex or 85. hex encoding uses
        /// two bytes in the postscript file each byte in the image. 85 stand for Ascii85 encoding. </para>
        /// 	<para> linenumbermode </para>
        /// 	<para> Set to paragraph if lines are numbered within a paragraph or box if they are numbered
        /// within the surrounding box. </para>
        /// 	<para> linebreak </para>
        /// 	<para> Only used if text is output with ps_show_boxed(). If set to true a carriage return
        /// will add a line break. </para>
        /// 	<para> parbreak </para>
        /// 	<para> Only used if text is output with ps_show_boxed(). If set to true a carriage return
        /// will start a new paragraph. </para>
        /// 	<para> hyphenation </para>
        /// 	<para> Only used if text is output with ps_show_boxed(). If set to true the paragraph will
        /// be hyphenated if a hypen dictionary is set and exists. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_set_parameter(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string name,
                                                   [MarshalAs(UnmanagedType.LPStr)] string value);

        /// <summary>
        /// Gets one of several parameters which are used by many functions.
        /// Parameters are by definition string values.
        /// <para>Parameter names can be one of the built-in names listed below, one of the names set by various library functions, or a value set by the user via ps_set_parameter.</para>
        /// <para>
        /// The built-in names specifically handled by ps_get_parameter are:</para>
        /// 	<para>fontname </para>
        /// 	<para>The name of the currently active font or the font whose identifier is passed
        /// in parameter modifier . </para>
        /// 	<para> fontencoding </para>
        /// 	<para> The encoding of the currently active font. </para>
        /// 	<para> dottedversion </para>
        /// 	<para> The version of the underlying pslib library in the format [major].[minor].[subminor]</para>
        /// 	<para> scope </para>
        /// 	<para> The current drawing scope. Can be object, document, null, page, pattern, path, template,
        /// prolog, font, glyph. </para>
        /// <para>Some other known values are:</para>
        /// 	<para>FontAFM </para>
        /// 	<para>FontOutline </para>
        /// 	<para>FontProtusion </para>
        /// 	<para>FontEncoding </para>
        /// 	<para>RightMarginKerning </para>
        /// 	<para>LeftMarginKerning </para>
        /// 	<para>SearchPath </para> - attempts to load resources located at path.
        ///     <para>underline </para> - true or false
        ///     <para>overline </para> - true or false
        ///     <para>strikeout </para> - true or false
        ///     <para>warning </para> - true or false
        ///     <para>inputencoding </para>
        /// 	<para> ligaturedisolvechar </para>
        /// 	<para> The character which dissolves a ligature. If your are using a font which contains the
        /// ligature `ff' and `|' is the char to dissolve the ligature, then `f|f' will result
        /// in two `f' instead of the ligature `ff'. </para>
        /// 	<para> imageencoding </para>
        /// 	<para> The encoding used for encoding images. Can be either hex or 85. hex encoding uses
        /// two bytes in the postscript file each byte in the image. 85 stand for Ascii85 encoding. </para>
        /// 	<para> linenumbermode </para>
        /// 	<para> Set to paragraph if lines are numbered within a paragraph or box if they are numbered
        /// within the surrounding box. </para>
        /// 	<para> linebreak </para>
        /// 	<para> Only used if text is output with ps_show_boxed(). If set to true a carriage return
        /// will add a line break. </para>
        /// 	<para> parbreak </para>
        /// 	<para> Only used if text is output with ps_show_boxed(). If set to true a carriage return
        /// will start a new paragraph. </para>
        /// 	<para> hyphenation </para>
        /// 	<para> Only used if text is output with ps_show_boxed(). If set to true the paragraph will
        /// be hyphenated if a hypen dictionary is set and exists. </para>
        /// 	<para> hyphendict </para>
        /// 	<para> Filename of the dictionary used for hyphenation pattern. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="modifier">An identifier needed if a parameter of a resource is requested, e.g. a parameter about an image, file or font handle. In such a case the handle to the resource is passed.</param>
        /// <returns>A <see cref="String"/>.</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.LPStr)] 
        public static extern string PS_get_parameter(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string name,
                                                     float modifier);

        /// <summary>
        /// Sets one of several values which are used by many functions. 
        /// Parameters are by definition float values. 
        /// 
        /// <para>The name can be one of the following: </para>
        /// 
        /// <para>textrendering </para>
        /// <para>The way how text is shown. </para>
        /// 
        /// <para>textx </para>
        /// <para>The x coordinate for text output. </para>
        /// 
        /// <para>texty </para>
        /// <para>The y coordinate for text output. </para>
        /// 
        /// <para>wordspacing </para>
        /// <para>The distance between words relative to the width of a space. </para>
        /// 
        /// <para>leading </para>
        /// <para>The distance between lines in pixels. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_set_value(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string name, float value);


        /// <summary>
        /// Gets one of several values which were set by ps_set_value(). Values are by definition float values. 
        /// <para>The parameter name can have the following values. </para>
        /// <para>fontsize </para>
        ///<para>The size of the currently active font or the font whose identifier is passed in parameter modifier . </para>
        ///<para>font </para>
        ///<para>The currently active font itself. </para>
        /// 
        ///<para>imagewidth </para>
        ///<para>The width of the image whose id is passed in the parameter modifier . </para>
        /// 
        ///<para>imageheight </para>
        ///<para>The height of the image whose id is passed in the parameter modifier . </para>
        /// 
        ///<para>capheight </para>
        ///<para>The height of a capital M in the currently active font or the font whose identifier is passed in parameter modifier . </para>
        /// 
        ///<para>ascender </para>
        ///<para>The ascender of the currently active font or the font whose identifier is passed in parameter modifier . </para>
        /// 
        ///<para>descender </para>
        ///<para>The descender of the currently active font or the font whose identifier is passed in parameter modifier . </para>
        /// 
        ///<para>italicangle </para>
        ///<para>The italicangle of the currently active font or the font whose identifier is passed in parameter modifier . </para>
        /// 
        ///<para>underlineposition </para>
        ///<para>The underlineposition of the currently active font or the font whose identifier is passed in parameter modifier . </para>
        /// 
        ///<para>underlinethickness </para>
        ///<para>The underlinethickness of the currently active font or the font whose identifier is passed in parameter modifier . </para>
        /// 
        ///<para>textx </para>
        ///<para>The current x-position for text output. </para>
        /// 
        ///<para>texty </para>
        ///<para>The current y-position for text output. </para>
        /// 
        ///<para>textrendering </para>
        ///<para>The current mode for text rendering. </para>
        /// 
        ///<para>textrise </para>
        ///<para>The space by which text is risen above the base line. </para>
        /// 
        ///<para>leading </para>
        ///<para>The distance between text lines in points. </para>
        /// 
        ///<para>wordspacing </para>
        ///<para>The space between words as a multiple of the width of a space char. </para>
        /// 
        ///<para>charspacing </para>
        ///<para>The space between chars. If charspacing is != 0.0 ligatures will always be dissolved. </para>
        /// 
        ///<para>hyphenminchars </para>
        ///<para>Minimum number of chars hyphenated at the end of a word. </para>
        /// 
        ///<para>parindent </para>
        ///<para>Indention of the first n line in a paragraph. </para>
        /// 
        ///<para>numindentlines </para>
        ///<para>Number of line in a paragraph to indent if parindent != 0.0. </para>
        /// 
        ///<para>parskip </para>
        ///<para>Distance between paragraphs. </para>
        /// 
        ///<para>linenumberspace </para>
        ///<para>Overall space in front of each line for the line number. </para>
        /// 
        ///<para>linenumbersep </para>
        ///<para>Space between the line and the line number. </para>
        /// 
        ///<para>major </para>
        ///<para>The major version number of pslib. </para>
        ///
        ///<para>minor </para>
        ///<para>The minor version number of pslib. </para>
        ///
        ///<para>subminor, revision </para>
        ///<para>The subminor version number of pslib. </para>
        /// 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="name">Name of the value. </param>
        /// <param name="modifier">The parameter modifier specifies the handle for which the value is to be retrieved. This can be the id of a font or an image. </param>
        /// <returns>The value or 0</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern float PS_get_value(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string name,
                                                float modifier);

        /// <summary>
        /// List all set values
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_list_values(IntPtr psdoc);

        /// <summary>
        /// List all set parameters
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_list_parameters(IntPtr psdoc);

        /// <summary>
        /// List all resources
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_list_resources(IntPtr psdoc);

        /// <summary>
        /// <para>Set the position for the next text output. 
        /// You may alternatively set the x and y value separately by calling ps_set_value() 
        /// and choosing textx respectively texty as the value name. </para>
        /// 
        /// <para>If you want to output text at a certain position it is more convenient to 
        /// use ps_show_xy() instead of setting the text position and calling ps_show(). </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x">x-coordinate of the new text position. </param>
        /// <param name="y">y-coordinate of the new text position. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_set_text_pos(IntPtr psdoc, float x, float y);

        /// <summary>
        /// Sets the line width for all following drawing operations. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="width">The width of lines in points. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_setlinewidth(IntPtr psdoc, float width);

        /// <summary>
        /// Sets how line ends look like. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="type">The type of line ends. Possible values are PS_LINECAP_BUTT, PS_LINECAP_ROUND, or PS_LINECAP_SQUARED. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_setlinecap(IntPtr psdoc, LineCapType type);


        /// <summary>
        /// Sets how lines are joined. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="type">The way lines are joined. Possible values are PS_LINEJOIN_MITER, PS_LINEJOIN_ROUND, or PS_LINEJOIN_BEVEL.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_setlinejoin(IntPtr psdoc, LineJoinType type);


        /// <summary>
        /// If two lines join in a small angle and the line join is set to PS_LINEJOIN_MITER, 
        /// then the resulting spike will be very long. The miter limit is the maximum ratio 
        /// of the miter length (the length of the spike) and the line width. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="value">The maximum ratio between the miter length and the line width. Larger values (> 10) will result in very long spikes when two lines meet in a small angle. Keep the default unless you know what you are doing.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_setmiterlimit(IntPtr psdoc, float value);

        /// <summary> 
        /// Sets flatness
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="value">The value must be between 0.2 and 1. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_setflat(IntPtr psdoc, float value);

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="mode">Unknown</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_setoverprintmode(IntPtr psdoc, int mode);

        /// <summary>
        /// Unknown
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="smoothness">Unknown</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_setsmoothness(IntPtr psdoc, float smoothness);

        /// <summary>
        /// Sets the length of the black and white portions of a dashed line. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="on">The length of the dash. </param>
        /// <param name="off">The length of the gap between dashes. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_setdash(IntPtr psdoc, float on, float off);

        /// <summary>
        /// Sets the length of the black and white portions of a dashed line.
        /// ps_setpolydash() is used to set more complicated dash patterns. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="arr">arr is a list of length elements alternately for the black and white portion. </param>
        /// <param name="length"></param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        //public static extern void PS_setpolydash(IntPtr psdoc, float[] arr, int length);
        private static extern void PS_setpolydash(IntPtr psdoc, IntPtr arr, int length);

        public static void PS_setpolydash(IntPtr psdoc, float[] arr)
        {
            // allocate unmanaged for float[] arr and int (length of array)
            IntPtr fptr = Marshal.AllocHGlobal(arr.Length*Marshal.SizeOf(typeof (float)));

            // copy the array
            Marshal.Copy(arr, 0, fptr, arr.Length);

            PS_setpolydash(psdoc, fptr, arr.Length);
        }

        /// <summary>
        /// Add a section of a cubic Bézier curve described by the three given control points to the current path. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x1">x-coordinate of first control point. </param>
        /// <param name="y1">y-coordinate of first control point. </param>
        /// <param name="x2">x-coordinate of second control point. </param>
        /// <param name="y2">y-coordinate of second control point. </param>
        /// <param name="x3">x-coordinate of third control point. </param>
        /// <param name="y3">y-coordinate of third control point. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_curveto(IntPtr psdoc, float x1, float y1, float x2, float y2, float x3, float y3);

        /// <summary>
        /// Draws a rectangle with its lower left corner at (x , y ). 
        /// The rectangle starts and ends in its lower left corner. If this function is called 
        /// outside a path it will start a new path. If it is called within a path it will add 
        /// the rectangle as a subpath. If the last drawing operation does not end in the lower 
        /// left corner then there will be a gap in the path. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x">x-coordinate of the lower left corner of the rectangle. </param>
        /// <param name="y">y-coordinate of the lower left corner of the rectangle. </param>
        /// <param name="width">The width of the image. </param>
        /// <param name="height">The height of the image. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_rect(IntPtr psdoc, float x, float y, float width, float height);

        /// <summary>
        /// Draws a circle with its middle point at (x , y ). The circle starts and ends at 
        /// position (x +radius , y ). If this function is called outside a path it will 
        /// start a new path. If it is called within a path it will add the circle as a subpath. 
        /// If the last drawing operation does not end in point (x +radius , y ) then there will 
        /// be a gap in the path. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x">The x-coordinate of the circle's middle point. </param>
        /// <param name="y">The y-coordinate of the circle's middle point. </param>
        /// <param name="radius">The radius of the circle </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_circle(IntPtr psdoc, float x, float y, float radius);

        /// <summary>
        /// Draws a portion of a circle with at middle point at (x , y ). The arc starts at an 
        /// angle of alpha and ends at an angle of beta . It is drawn counterclockwise 
        /// (use ps_arcn() to draw clockwise). The subpath added to the current path starts on 
        /// the arc at angle alpha and ends on the arc at angle beta . 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x">The x-coordinate of the circle's middle point. </param>
        /// <param name="y">The y-coordinate of the circle's middle point. </param>
        /// <param name="radius">The radius of the circle </param>
        /// <param name="alpha">The start angle given in degrees. </param>
        /// <param name="beta">The end angle given in degrees. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_arc(IntPtr psdoc, float x, float y, float radius, float alpha, float beta);

        /// <summary>
        /// Draws a portion of a circle with at middle point at (x , y ). The arc starts at 
        /// an angle of alpha and ends at an angle of beta . It is drawn clockwise 
        /// (use ps_arc() to draw counterclockwise). The subpath added to the current path 
        /// starts on the arc at angle beta and ends on the arc at angle alpha . 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x">The x-coordinate of the circle's middle point. </param>
        /// <param name="y">The y-coordinate of the circle's middle point. </param>
        /// <param name="radius">The radius of the circle </param>
        /// <param name="alpha">The start angle given in degrees. </param>
        /// <param name="beta">The end angle given in degrees. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_arcn(IntPtr psdoc, float x, float y, float radius, float alpha, float beta);

        /// <summary>
        /// Takes the current path and uses it to define the border of a clipping area. 
        /// Everything drawn outside of that area will not be visible. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_clip(IntPtr psdoc);

        /// <summary>
        /// Sets the gray value for all following drawing operations. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="gray">The value must be between 0 (white) and 1 (black). </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_setgray(IntPtr psdoc, float gray);

        /// <summary>
        /// Connects the last point with the first point of a path. 
        /// The resulting path can be used for stroking, filling, clipping, etc.. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_closepath(IntPtr psdoc);

        /// <summary>
        /// Connects the last point with first point of a path and draws the resulting closed line. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_closepath_stroke(IntPtr psdoc);

        /// <summary>
        /// Fills and draws the path constructed with previously called drawing functions like ps_lineto(). 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_fill_stroke(IntPtr psdoc);

        /// <summary>
        /// Draws the path constructed with previously called drawing functions like ps_lineto(). 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_stroke(IntPtr psdoc);

        /// <summary>
        /// Fills the path constructed with previously called drawing functions like ps_lineto(). 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_fill(IntPtr psdoc);

        /// <summary>
        /// Saves the current graphics context, containing colors, translation and rotation settings 
        /// and some more. A saved context can be restored with ps_restore(). 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_save(IntPtr psdoc);

        /// <summary>
        /// Restores a previously saved graphics context. Any call of ps_save() must be accompanied 
        /// by a call to ps_restore(). All coordinate transformations, line style settings, 
        /// color settings, etc. are being restored to the state before the call of ps_save(). 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_restore(IntPtr psdoc);

        /// <summary>
        /// <para>Outputs a text in a given box. The lower left corner of the box is at (left , bottom ). 
        /// Line breaks will be inserted where needed. Multiple spaces are treated as one. 
        /// Tabulators are treated as spaces. </para>
        /// <para>The text will be hyphenated if the parameter hyphenation is set to true and the parameter 
        /// hyphendict contains a valid filename for a hyphenation file. The line spacing is taken from the 
        /// value leading. Paragraphs can be separated by an empty line just like in TeX. If the value 
        /// parindent is set to value > 0.0 then the first n lines will be indented. The number n of lines 
        /// is set by the parameter numindentlines. In order to prevent indenting of the first m paragraphs
        /// set the value parindentskip to a positive number. </para>
        /// <para>The parameter hmode can be "justify", "fulljustify", "right", "left", or "center". 
        /// The difference of "justify" and "fulljustify" just affects the last line of the box. 
        /// In fulljustify mode the last line will be left and right justified unless this is also 
        /// the last line of paragraph. In justify mode it will always be left justified. </para>
        /// <para>Used parameters</para>
        /// <para>
        /// The output of ps_show_boxed() can be configured with several parameters and values which 
        /// must be set with either ps_set_parameter() or ps_set_value(). Beside the parameters and 
        /// values which affect text output, the following parameters and values are evaluated. </para>
        /// 
        /// <para>leading (value) </para>
        /// <para>Distance between baselines of two consecutive lines.</para>
        /// 
        /// <para>linebreak (parameter) </para>
        /// <para>Set to "true" if you want a carriage return to start a new line instead of treating it as a space. Defaults to "false". </para>
        /// 
        /// <para>parbreak (parameter) </para>
        /// <para>Set to "true" if you want a carriage return on a single line to start a new paragraph instead of treating it as a space. Defaults to "true". </para>
        /// 
        /// <para>hyphenation (parameter) </para>
        /// <para>Set to "true" in order to turn hyphenation on. This requires a dictionary to be set with the parameter "hyphendict". Defaults to "false". </para>
        /// 
        /// <para>hyphendict (parameter) </para>
        /// <para>Filename of the dictionary used for hyphenation pattern (see below). </para>
        /// 
        /// <para>hyphenminchar (value) </para>
        /// <para>The number of chars which must at least be left over before or after the hyphen. This implies that only words of at least two times this value will be hyphenated. The default value is three. Setting a value of zero will result in the default value. </para>
        /// 
        /// <para>parindent (value) </para>
        /// <para>Set the amount of space in pixel for indenting the first m lines of a paragraph. m can be set with the value "numindentlines". </para>
        /// 
        /// <para>parskip (value) </para>
        /// <para>Set the amount of extra space in pixel between paragraphs. Defaults to 0 which will result in a normal line distance. </para>
        /// 
        /// <para>numindentlines (value) </para>
        /// <para>Number of lines from the start of the paragraph which will be indented. Defaults to 1. </para>
        /// 
        /// <para>parindentskip (value) </para>
        /// <para>Number of paragraphs in the box whose first lines will not be indented. This defaults to 0. This is useful for paragraphs right after a section heading or text being continued in a second box. In both case one would set this to 1. </para>
        /// 
        /// <para>linenumbermode (parameter) </para>
        /// <para>Set how lines are to be numbered. Possible values are "box" for numbering lines in the whole box or "paragraph" to number lines within each paragraph. </para>
        /// 
        /// <para>linenumberspace (value) </para>
        /// <para>The space for the column left of the numbered line containing the line number. The line number will be right justified into this column. Defaults to 20. </para>
        /// 
        /// <para>linenumbersep (value) </para>
        /// <para>The space between the column with line numbers and the line itself. Defaults to 5. </para>
        /// <para>
        /// Note that there will no box be drawn around the text even if the function name suggests this.
        /// After the box has been drawn you can get the new x and y position with ps_get_value(psdoc,"textx", null) and ps_get_value(psdoc,"texty",null);
        /// </para>
        /// <para>textx points to the end of the last character written by ps_show_boxed and texty points to the baseline of the last line written (which means, if there is e.g. a 'g' in the last line then the lower part's y-coordinates of the g will be lower than the value of texty. I hope you understand what I meant) </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="text">The text to be output into the given box. </param>
        /// <param name="left">x-coordinate of the lower left corner of the box. </param>
        /// <param name="bottom">y-coordinate of the lower left corner of the box. </param>
        /// <param name="width">Width of the box. </param>
        /// <param name="height">Height of the box. </param>
        /// <param name="hmode">The parameter hmode can be "justify", "fulljustify", "right", "left", or "center". The difference of "justify" and "fulljustify" just affects the last line of the box. In fulljustify mode the last line will be left and right justified unless this is also the last line of paragraph. In justify mode it will always be left justified. </param>
        /// <param name="feature"></param>
        /// <returns>Number of characters that could not be written. </returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_show_boxed(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text, float left,
                                               float bottom, float width, float height,
                                               [MarshalAs(UnmanagedType.LPStr)] string hmode,
                                               [MarshalAs(UnmanagedType.LPStr)] string feature);

        /// <summary>
        /// Output a text one line below the last line. 
        /// The line spacing is taken from the value "leading" which must be set with ps_set_value(). 
        /// The actual position of the text is determined by the values "textx" and "texty" which can 
        /// be requested with ps_get_value() 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="text">The text to output. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_continue_text(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text);

        /// <summary>
        /// Output a text one line below the last line with specified length. 
        /// The line spacing is taken from the value "leading" which must be set with ps_set_value(). 
        /// The actual position of the text is determined by the values "textx" and "texty" which can 
        /// be requested with ps_get_value() 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="text">The text to output. </param>
        /// <param name="len">How many characters to output.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_continue_text2(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text, int len);

        /// <summary>
        /// Sets the color for drawing, filling, or both. 
        /// <para>Known Bug: The second parameter is currently not always evaluated. The color is sometimes set for filling and drawing just as if fillstroke were passed. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="type">The parameter type can be both, fill, or fillstroke. </param>
        /// <param name="colorspace">The colorspace should be one of gray, rgb, cmyk, spot, pattern. Depending on the colorspace either only the first, the first three or all parameters will be used. </param>
        /// <param name="c1">Depending on the colorspace this is either the red component (rgb), the cyan component (cmyk), the gray value (gray), the identifier of the spot color or the identifier of the pattern. </param>
        /// <param name="c2">Depending on the colorspace this is either the green component (rgb), the magenta component (cmyk). </param>
        /// <param name="c3">Depending on the colorspace this is either the blue component (rgb), the yellow component (cmyk). </param>
        /// <param name="c4">This must only be set in cmyk colorspace and specifies the black component. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private static extern void PS_setcolor(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string type,
                                               [MarshalAs(UnmanagedType.LPStr)] string colorspace, float c1, float c2,
                                               float c3, float c4);

        /// <summary>
        /// Sets the color for drawing, filling, or both. 
        /// <para>Known Bug: The second parameter is currently not always evaluated. The color is sometimes set for filling and drawing just as if fillstroke were passed. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="scopeType">The parameter scopeType can be both, fill, or fillstroke. </param>
        /// <param name="colorspace">The colorspace should be one of gray, rgb, cmyk, spot, pattern. Depending on the colorspace either only the first, the first three or all parameters will be used. </param>
        /// <param name="c1">Depending on the colorspace this is either the red component (rgb), the cyan component (cmyk), the gray value (gray), the identifier of the spot color or the identifier of the pattern. </param>
        /// <param name="c2">Depending on the colorspace this is either the green component (rgb), the magenta component (cmyk). </param>
        /// <param name="c3">Depending on the colorspace this is either the blue component (rgb), the yellow component (cmyk). </param>
        /// <param name="c4">This must only be set in cmyk colorspace and specifies the black component. </param>
        public static void PS_setcolor(IntPtr psdoc, SetColorScopeType scopeType, ColorSpace colorspace, float c1, float c2, float c3,
                                       float c4)
        {
            PS_setcolor(psdoc, scopeType.ToString(), colorspace.ToString(), c1, c2, c3, c4);
        }


        /// <summary>
        /// Creates a spot color from the current fill color. 
        /// The fill color must be defined in rgb, cmyk or gray colorspace. 
        /// The spot color name can be an arbitrary name. 
        /// A spot color can be set as any color with ps_setcolor(). 
        /// When the document is not printed but displayed by an postscript viewer the given 
        /// color in the specified color space is use. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="name">Name of the spot color, e.g. Pantone 5565. </param>
        /// <param name="reserved"></param>
        /// <returns>The id of the new spot color or 0 in case of an error. </returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_makespotcolor(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string name,
                                                  int reserved);

        /// <summary>
        /// <para>Loads a font for later use. Before text is output with a loaded font it must be set with ps_setfont(). This function needs the adobe font metric file in order to calculate the space used up by the characters. A font which is loaded within a page will only be available on that page. Fonts which are to be used in the complete document have to be loaded before the first call of ps_begin_page(). Calling ps_findfont() between pages will make that font available for all following pages. </para>
        /// <para>The name of the afm file must be fontname .afm. If the font shall be embedded the file fontname .pfb containing the font outline must be present as well. </para>
        /// <para>Calling ps_findfont() before the first page requires to output the postscript header which includes the BoundingBox for the whole document. Usually the BoundingBox is set with the first call of ps_begin_page() which now comes after ps_findfont(). Consequently the BoundingBox has not been set and a warning will be issued when ps_findfont() is called. In order to prevent this situation, one should call ps_set_parameter() to set the BoundingBox before ps_findfont() is called. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="fontname">The name of the font. </param>
        /// <param name="encoding">ps_findfont() will try to load the file passed in the parameter encoding . Encoding files are of the same syntax as those used by dvips(1). They contain a font encoding vector (which is currently not used but must be present) and a list of extra ligatures to extend the list of ligatures derived from the afm file. encoding can be NULL or the empty string if the default encoding (TeXBase1) shall be used. If the encoding is set to builtin then there will be no reencoding and the font specific encoding will be used. This is very useful with symbol fonts. </param>
        /// <param name="embed">If true the font will be embedded into the document. This requires the font outline (.pfb file) to be present. </param>
        /// <returns>Returns the identifier of the font or zero in case of an error. The identifier is a positive number. </returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_findfont(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string fontname,
                                             [MarshalAs(UnmanagedType.LPStr)] string encoding, bool embed);

        // start back here for commenting... 

        /// <summary>
        /// Calculates the width of a string in points if it was output in the given font and font size. This function needs an Adobe font metrics file to calculate the precise width. If kerning is turned on, it will be taken into account. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="text">The text for which the width is to be calculated. </param>
        /// <param name="fontid">The identifier of the font to be used. If not font is specified the current font will be used. </param>
        /// <param name="size">The size of the font. If no size is specified the current size is used. </param>
        /// <returns>The width of the string in points</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern float PS_stringwidth(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text, int fontid,
                                                  float size);

        /// <summary>
        /// Calculates the width of a string in points if it was output in the given font and font size. This function needs an Adobe font metrics file to calculate the precise width. If kerning is turned on, it will be taken into account. This overload allows you to specify the width in characters of the string to measure. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="text">The text for which the width is to be calculated. </param>
        /// <param name="xlen">Size, in characters of the string to measure</param>
        /// <param name="fontid">The identifier of the font to be used. If not font is specified the current font will be used. </param>
        /// <param name="size">The size of the font. If no size is specified the current size is used. </param>
        /// <returns>The width of the string in points</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern float PS_stringwidth2(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text, int xlen,
                                                   int fontid, float size);


        /// <summary>
        /// This pslib API function is similar to ps_stringwidth but returns a float array of dimensions 
        /// containing the width, ascender, and descender of the text. The dimensions array is populated 
        /// as an out variable, the float return value is equal to the width (first member of float array),
        /// and is the same return value you'd get from calling ps_stringwidth;
        /// <para>
        /// Note: In this context, I have replaced the float array with a StringGeometry struct, to be more 
        /// clear what the values mean. This struct is not part of the compatible pslib API. 
        /// </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="text">The text for which the width is to be calculated. </param>
        /// <param name="xlen">Size, in characters of the string to measure</param>
        /// <param name="fontid">The identifier of the font to be used. If not font is specified the current font will be used. </param>
        /// <param name="size">The size of the font. If no size is specified the current size is used. </param>
        /// <param name="dimension">StringGeometry struct containing width, ascender, and descender. Note: in the actual C API function declaration, this is a float[] that is always returned with those three members. I use a struct here to make it more clear.</param>
        /// <returns>The width of the string in points</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private static extern float PS_string_geometry(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text,
                                                       int xlen, int fontid, float size, ref StringGeometry dimension);

        /// <summary>
        /// This function is similar to ps_stringwidth but returns an array of dimensions containing 
        /// the width, ascender, and descender of the text. The dimensions array is populated as an 
        /// out variable, the float return value is equal to the width (first member of float array) 
        /// and is the same return value you'd get from calling ps_stringwidth;
        /// <para>Note: This overload is just for compatibility with pslib API function definiton.. I suggest
        /// using the overload that returns the StringGeometry struct.</para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="text">The text for which the width is to be calculated. </param>
        /// <param name="xlen">Size, in characters of the string to measure</param>
        /// <param name="fontid">The identifier of the font to be used. If not font is specified the current font will be used. </param>
        /// <param name="size">The size of the font. If no size is specified the current size is used. </param>
        /// <param name="dimension">float array containing width [0], ascender [1], and descender [2]. Note: This is only to provide compatibility with the C API function declaration</param>
        /// <returns>The width of the string in points</returns>
        public static float PS_string_geometry(
            IntPtr psdoc, string text, int xlen, int fontid, float size, out float[] dimension)
        {
            StringGeometry stringGeometry = PS_string_geometry(psdoc, text, xlen, fontid, size);

            dimension = new float[] {stringGeometry.width, stringGeometry.ascender, stringGeometry.descender};

            return stringGeometry.width;
        }

        /// <summary>
        /// This function is similar to ps_stringwidth but returns a StringGeometry struct of string dimensions containing the width, ascender, and descender of the text. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="text">The text for which the width is to be calculated. </param>
        /// <param name="xlen">Size, in characters of the string to measure</param>
        /// <param name="fontid">The identifier of the font to be used. If not font is specified the current font will be used. </param>
        /// <param name="size">The size of the font. If no size is specified the current size is used. </param>
        /// <returns>StringGeometry struct containing width, ascender, and descender.</returns>
        public static StringGeometry PS_string_geometry(
            IntPtr psdoc, string text, int xlen, int fontid, float size)
        {
            // I'm not returning the float that is returned by the API function, because the value is redundant. 
            // It's the same as the StringGeometry struct member "width". If you need a function that returns 
            // float, use the other overload that matches the C API.

            //float foo = PS_string_geometry(psdoc, text, xlen, fontid, size, ref stringGeometry);

            StringGeometry stringGeometry = new StringGeometry();
            PS_string_geometry(psdoc, text, xlen, fontid, size, ref stringGeometry);

            return stringGeometry;
        }


        /// <summary>
        /// Unknown. (I assume this is to delete a font that was previously loaded via ps_getfont/findfont. Pass this id, and it unloads it, and invalidates the id). 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="fontid"></param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_deletefont(IntPtr psdoc, int fontid);

        /// <summary>
        /// Sets a font, which has to be loaded before with ps_findfont(). Outputting text without setting a font results in an error. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="fontid">The font identifier as returned by ps_findfont(). </param>
        /// <param name="size">The size of the font. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_setfont(IntPtr psdoc, int fontid, float size);

        /// <summary>
        /// Unknown. (I assume this gets the current font for the document, returning a fontid that can be used later). 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_getfont(IntPtr psdoc);

        /// <summary>
        /// Sets the current point to new coordinates. If this is the first call of ps_moveto()
        /// after a previous path has been ended then it will start a new path. If this function 
        /// is called in the middle of a path it will just set the current point and start a subpath. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x">x-coordinate of the point to move to. </param>
        /// <param name="y">y-coordinate of the point to move to. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_moveto(IntPtr psdoc, float x, float y);

        /// <summary>
        /// Adds a straight line from the current point to the given coordinates to the current path. 
        /// Use ps_moveto() to set the starting point of the line. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x">x-coordinate of the end point of the line. </param>
        /// <param name="y">y-coordinate of the end point of the line. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_lineto(IntPtr psdoc, float x, float y);

        /// <summary>
        /// Sets the rotation of the coordinate system. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x">Angle of rotation in degree. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_rotate(IntPtr psdoc, float x);

        /// <summary>
        /// Sets a new initial point of the coordinate system. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x">x-coordinate of the origin of the translated coordinate system. </param>
        /// <param name="y">y-coordinate of the origin of the translated coordinate system. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_translate(IntPtr psdoc, float x, float y);

        /// <summary>
        /// Sets horizontal and vertical scaling of the coordinate system. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="x">Scaling factor in horizontal direction. </param>
        /// <param name="y">Scaling factor in vertical direction. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_scale(IntPtr psdoc, float x, float y);

        /// <summary>
        /// Returns the current Postscript Document string buffer. This is only useful when the document was opened with 
        /// ps_open_mem(). The buffer is never populated if it's opened with ps_open_file, or ps_open_fp.
        /// This also causes pslib to *clear the buffer*. 
        /// So, if you call get buffer, it actually copies it's current buffer to a temporary location, 
        /// clears it's buffer, and returns the pointer to the temporary location. That means, if you call this, 
        /// you *must* handle the buffer contents yourself, as pslib will no longer have that data in memory. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="size">Size of the returned buffer.</param>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr PS_get_buffer(IntPtr psdoc, ref long size);


        /// <summary>
        /// Returns the current Postscript Document string buffer. This is only useful when the document was opened with 
        /// ps_open_mem(). The buffer is never populated if it's opened with ps_open_file, or ps_open_fp.
        /// This also causes pslib to *clear the buffer*. 
        /// So, if you call ps_get_buffer, it actually copies it's current buffer to a temporary location, 
        /// clears it's buffer, and returns the pointer to the temporary location. That means, if you call this, 
        /// you *must* handle the buffer contents yourself, as pslib will no longer have that data in memory. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <returns>string contents of the Postscript file.</returns>
        public static string PS_get_buffer(IntPtr psdoc)
        {
            long sizeofbuff = 0;

            IntPtr buffPtr = PS_get_buffer(psdoc, ref sizeofbuff);

            return (sizeofbuff > 0)
                       ?
                           Marshal.PtrToStringAnsi(buffPtr, (Int32) sizeofbuff)
                       : string.Empty;
        }

        /// <summary>
        /// <para>Places a hyperlink at the given position pointing to a web page. 
        /// The hyperlink's source position is a rectangle with its lower left corner at (llx , lly ) and 
        /// its upper right corner at (urx , ury ). The rectangle has by default a thin blue border. </para>
        /// <para>The hyperlink will not be visible if the document is printed or viewed but it will show 
        /// up if the document is converted to pdf by either Acrobat Distiller™ or Ghostview. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="llx">The x-coordinate of the lower left corner. </param>
        /// <param name="lly">The y-coordinate of the lower left corner. </param>
        /// <param name="urx">The x-coordinate of the upper right corner. </param>
        /// <param name="ury">The y-coordinate of the upper right corner. </param>
        /// <param name="url">The url of the hyperlink to be opened when clicking on this link, e.g. http://www.php.net. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_add_weblink(IntPtr psdoc, float llx, float lly, float urx, float ury,
                                                 [MarshalAs(UnmanagedType.LPStr)] string url);

        /// <summary>
        /// <para>Places a hyperlink at the given position pointing to a second pdf document. Clicking on the 
        /// link will branch to the document at the given page. The first page in a document has number 1. </para>
        /// <para>The hyperlink's source position is a rectangle with its lower left corner at (llx , lly ) 
        /// and its upper right corner at (urx , ury ). The rectangle has by default a thin blue border. </para>
        /// <para>The hyperlink will not be visible if the document is printed or viewed but it will show up if 
        /// the document is converted to pdf by either Acrobat Distiller™ or Ghostview. </para>
        /// /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="llx">The x-coordinate of the lower left corner. </param>
        /// <param name="lly">The y-coordinate of the lower left corner. </param>
        /// <param name="urx">The x-coordinate of the upper right corner. </param>
        /// <param name="ury">The y-coordinate of the upper right corner. </param>
        /// <param name="filename">The name of the pdf document to be opened when clicking on this link. </param>
        /// <param name="page">The page number of the destination pdf document </param>
        /// <param name="dest">The parameter dest determines how the document is being viewed. It can be fitpage, fitwidth, fitheight, or fitbbox. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_add_pdflink(IntPtr psdoc, float llx, float lly, float urx, float ury,
                                                 [MarshalAs(UnmanagedType.LPStr)] string filename, int page,
                                                 [MarshalAs(UnmanagedType.LPStr)] string dest);

        /// <summary>
        /// <para>Places a hyperlink at the given position pointing to a second pdf document. Clicking on the 
        /// link will branch to the document at the given page. The first page in a document has number 1. </para>
        /// <para>The hyperlink's source position is a rectangle with its lower left corner at (llx , lly ) 
        /// and its upper right corner at (urx , ury ). The rectangle has by default a thin blue border. </para>
        /// <para>The hyperlink will not be visible if the document is printed or viewed but it will show up if 
        /// the document is converted to pdf by either Acrobat Distiller™ or Ghostview. </para>
        /// /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="llx">The x-coordinate of the lower left corner. </param>
        /// <param name="lly">The y-coordinate of the lower left corner. </param>
        /// <param name="urx">The x-coordinate of the upper right corner. </param>
        /// <param name="ury">The y-coordinate of the upper right corner. </param>
        /// <param name="filename">The name of the pdf document to be opened when clicking on this link. </param>
        /// <param name="page">The page number of the destination pdf document </param>
        /// <param name="docViewType">The parameter dest determines how the document is being viewed. It can be fitpage, fitwidth, fitheight, or fitbbox. </param>
        public static void PS_add_pdflink(IntPtr psdoc, float llx, float lly, float urx, float ury, string filename,
                                          int page, DocumentViewType docViewType)
        {
            PS_add_pdflink(psdoc, llx, lly, urx, ury, filename, page, docViewType.ToString());
        }


        /// <summary>
        /// <para>Places a hyperlink at the given position pointing to a page in the same document. Clicking on the 
        /// link will branch to the document at the given page. The first page in a document has number 1. </para>
        /// <para>The hyperlink's source position is a rectangle with its lower left corner at (llx , lly ) 
        /// and its upper right corner at (urx , ury ). The rectangle has by default a thin blue border. </para>
        /// <para>The hyperlink will not be visible if the document is printed or viewed but it will show up if 
        /// the document is converted to pdf by either Acrobat Distiller™ or Ghostview. </para>
        /// /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="llx">The x-coordinate of the lower left corner. </param>
        /// <param name="lly">The y-coordinate of the lower left corner. </param>
        /// <param name="urx">The x-coordinate of the upper right corner. </param>
        /// <param name="ury">The y-coordinate of the upper right corner. </param>        
        /// <param name="page">The page number of the destination pdf document </param>
        /// <param name="dest">The parameter dest determines how the document is being viewed. It can be fitpage, fitwidth, fitheight, or fitbbox. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_add_locallink(IntPtr psdoc, float llx, float lly, float urx, float ury, int page,
                                                   [MarshalAs(UnmanagedType.LPStr)] string dest);

        /// <summary>
        /// <para>Places a hyperlink at the given position pointing to a page in the same document. Clicking on the 
        /// link will branch to the document at the given page. The first page in a document has number 1. </para>
        /// <para>The hyperlink's source position is a rectangle with its lower left corner at (llx , lly ) 
        /// and its upper right corner at (urx , ury ). The rectangle has by default a thin blue border. </para>
        /// <para>The hyperlink will not be visible if the document is printed or viewed but it will show up if 
        /// the document is converted to pdf by either Acrobat Distiller™ or Ghostview. </para>
        /// /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="llx">The x-coordinate of the lower left corner. </param>
        /// <param name="lly">The y-coordinate of the lower left corner. </param>
        /// <param name="urx">The x-coordinate of the upper right corner. </param>
        /// <param name="ury">The y-coordinate of the upper right corner. </param>        
        /// <param name="page">The page number of the destination pdf document </param>
        /// <param name="docViewType">The parameter dest determines how the document is being viewed. It can be fitpage, fitwidth, fitheight, or fitbbox. </param>
        public static void PS_add_locallink(IntPtr psdoc, float llx, float lly, float urx, float ury, int page,
                                            DocumentViewType docViewType)
        {
            PS_add_locallink(psdoc, llx, lly, urx, ury, page, docViewType.ToString());
        }

        /// <summary>
        /// <para>
        /// Places a hyperlink at the given position pointing to a file program which is being 
        /// started when clicked on. The hyperlink's source position is a rectangle with its lower 
        /// left corner at (llx, lly) and its upper right corner at (urx, ury). The rectangle has by 
        /// default a thin blue border. </para>
        /// <para>
        /// The hyperlink will not be visible if the document is printed or viewed but it will 
        /// show up if the document is converted to pdf by either Acrobat Distiller™ or Ghostview. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="llx">The x-coordinate of the lower left corner. </param>
        /// <param name="lly">The y-coordinate of the lower left corner. </param>
        /// <param name="urx">The x-coordinate of the upper right corner. </param>
        /// <param name="ury">The y-coordinate of the upper right corner. </param>
        /// <param name="filename">The path of the program to be started, when the link is clicked on. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_add_launchlink(IntPtr psdoc, float llx, float lly, float urx, float ury,
                                                    [MarshalAs(UnmanagedType.LPStr)] string filename);

        /// <summary>
        /// <para>Adds a bookmark for the current page. Bookmarks usually appear in PDF-Viewers left of the page in a hierarchical tree. Clicking on a bookmark will jump to the given page. </para>
        /// <para>The bookmark has no meaning if the document is printed or viewed, but it will be used if the document is converted to pdf by either Acrobat Distiller™ or Ghostview. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="text">The text used for displaying the bookmark. </param>
        /// <param name="parent">A bookmark previously created by this function which is used as the parent of the new bookmark. </param>
        /// <param name="open">If open is true the bookmark will be shown open by the pdf viewer. </param>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_add_bookmark(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text, int parent,
                                                 bool open);

        /// <summary>
        /// <para>Adds a note at a certain position on the page. Notes are like little rectangular sheets with 
        /// text on it, which can be placed anywhere on a page. They are shown either folded or unfolded. 
        /// If unfolded, the specified icon is used as a placeholder. </para>
        /// <para>The note will not be visible if the document is printed or viewed but it will show 
        /// up if the document is converted to pdf by either Acrobat Distiller™ or Ghostview. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="llx">The x-coordinate of the lower left corner. </param>
        /// <param name="lly">The y-coordinate of the lower left corner. </param>
        /// <param name="urx">The x-coordinate of the upper right corner. </param>
        /// <param name="ury">The y-coordinate of the upper right corner. </param>
        /// <param name="contents">The text of the note. </param>
        /// <param name="title">The title of the note as displayed in the header of the note. </param>
        /// <param name="icon">
        /// The icon shown if the note is folded. This parameter can be set to comment, insert, note, paragraph, newparagraph, key, or help. </param>
        /// <param name="open">If open is true the bookmark will be shown open by the pdf viewer. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_add_note(IntPtr psdoc, float llx, float lly, float urx, float ury,
                                              [MarshalAs(UnmanagedType.LPStr)] string contents,
                                              [MarshalAs(UnmanagedType.LPStr)] string title,
                                              [MarshalAs(UnmanagedType.LPStr)] string icon, bool open);

        /// <summary>
        /// <para>Adds a note at a certain position on the page. Notes are like little rectangular sheets with 
        /// text on it, which can be placed anywhere on a page. They are shown either folded or unfolded. 
        /// If unfolded, the specified icon is used as a placeholder. </para>
        /// <para>The note will not be visible if the document is printed or viewed but it will show 
        /// up if the document is converted to pdf by either Acrobat Distiller™ or Ghostview. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="llx">The x-coordinate of the lower left corner. </param>
        /// <param name="lly">The y-coordinate of the lower left corner. </param>
        /// <param name="urx">The x-coordinate of the upper right corner. </param>
        /// <param name="ury">The y-coordinate of the upper right corner. </param>
        /// <param name="contents">The text of the note. </param>
        /// <param name="title">The title of the note as displayed in the header of the note. </param>
        /// <param name="icon">The icon shown if the note is folded. </param>
        /// <param name="open">If open is true the bookmark will be shown open by the pdf viewer. </param>        
        public static void PS_add_note(IntPtr psdoc, float llx, float lly, float urx, float ury, string contents,
                                       string title, PdfNoteIcon icon, bool open)
        {
            PS_add_note(psdoc, llx, lly, urx, ury, contents, title, icon.ToString(), open);
        }

        /// <summary>
        /// Links added with one of the functions ps_add_weblink(), ps_add_pdflink(), etc. will be displayed with a 
        /// surounded rectangle when the postscript document is converted to pdf and viewed in a pdf viewer. 
        /// This rectangle is not visible in the postscript document. This function sets the appearance and width 
        /// of the border line. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="style">style can be solid or dashed. </param>
        /// <param name="width"></param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_set_border_style(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string style,
                                                      float width);

        /// <summary>
        /// Links added with one of the functions ps_add_weblink(), ps_add_pdflink(), etc. will be displayed with a 
        /// surounded rectangle when the postscript document is converted to pdf and viewed in a pdf viewer. 
        /// This rectangle is not visible in the postscript document. This function sets the appearance and width 
        /// of the border line. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="style">style can be solid or dashed. </param>
        /// <param name="width">The line width of the border. </param>        
        public static void PS_set_border_style(IntPtr psdoc, PdfLinksBorderLineStyle style, float width)
        {
            PS_set_border_style(psdoc, style.ToString(), width);
        }


        /// <summary>
        /// Links added with one of the functions ps_add_weblink(), ps_add_pdflink(), etc. will be displayed 
        /// with a surounded rectangle when the postscript document is converted to pdf and viewed in a pdf 
        /// viewer. This rectangle is not visible in the postscript document. This function sets the color 
        /// of the rectangle's border line. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="red">The red component of the border color. </param>
        /// <param name="green">The green component of the border color. </param>
        /// <param name="blue">The blue component of the border color. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_set_border_color(IntPtr psdoc, float red, float green, float blue);

        /// <summary>
        /// Links added with one of the functions ps_add_weblink(), ps_add_pdflink(), etc. will be displayed 
        /// with a surounded rectangle when the postscript document is converted to pdf and viewed in a pdf 
        /// viewer. This rectangle is not visible in the postscript document. This function sets the length 
        /// of the black and white portion of a dashed border line. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="black">The length of the dash. </param>
        /// <param name="white">The length of the gap between dashes. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_set_border_dash(IntPtr psdoc, float black, float white);

        /// <summary>
        /// Starts a new template. A template is called a form in the postscript language. 
        /// It is created similar to a pattern but used like an image. Templates are often used for drawings 
        /// which are placed several times through out the document, e.g. like a company logo. All drawing 
        /// functions may be used within a template. The template will not be drawn until it is placed by 
        /// ps_place_image(). 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="width">The width of the template in pixel. </param>
        /// <param name="height">The height of the template in pixel. </param>
        /// <returns>A resource identifier (image id) for the newly created template. </returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_begin_template(IntPtr psdoc, float width, float height);

        /// <summary>
        /// Ends a template which was started with ps_begin_template(). Once a template has been ended, it can be used like an image. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_end_template(IntPtr psdoc);

        /// <summary>
        /// Starts a new pattern. A pattern is like a page containing e.g. a drawing which can be 
        /// used for filling areas. It is used like a color by calling ps_setcolor() and setting 
        /// the color space to pattern. Patterns are always Postscript PatternType 1.
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="width">The width of the pattern in pixel. </param>
        /// <param name="height">The height of the pattern in pixel. </param>
        /// <param name="xstep">The distance in pixel of placements of the pattern in horizontal direction. </param>
        /// <param name="ystep">The distance in pixel of placements of the pattern in vertical direction. </param>
        /// <param name="painttype">Indicates the Postscript Pattern PaintType parameter</param>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_begin_pattern(IntPtr psdoc, float width, float height, float xstep, float ystep,
                                                  PostscriptPatternPaintType painttype);


        /// <summary>
        /// Ends a pattern which was started with ps_begin_pattern(). Once a pattern has been ended, it can be used like a color to fill areas. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_end_pattern(IntPtr psdoc);


        /// <summary>
        /// Fills an area with a shading, which has to be created before with ps_shading(). This is an alternative way to creating a pattern from a shading ps_shading_pattern() and using the pattern as the filling color. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="shading">The identifier of a shading previously created with ps_shading(). </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_shfill(IntPtr psdoc, int shading);

        /// <summary>
        /// Creates a shading, which can be used by ps_shfill() or ps_shading_pattern(). 
        /// The color of the shading can be in any color space except for pattern. 
        /// The coordinates x0 , y0 , x1 , y1 are the start and end point of the shading. If the type of shading is radial the two points are the middle points of a starting and ending circle. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="shtype">The type of shading can be either radial or axial. Each shading starts with the current fill color and ends with the given color values passed in the parameters c1 to c4 (see ps_setcolor() for their meaning). </param>
        /// <param name="x0">The x-coordinate of the starting point of the shading. </param>
        /// <param name="y0">The y-coordinate of the starting point of the shading. </param>
        /// <param name="x1">The x-coordinate of the ending point of the shading. </param>
        /// <param name="y1">The y-coordinate of the ending point of the shading.</param>
        /// <param name="c1">Depending on the colorspace this is either the red component (rgb), the cyan component (cmyk), the gray value (gray), the identifier of the spot color or the identifier of the pattern. </param>
        /// <param name="c2">Depending on the colorspace this is either the green component (rgb), the magenta component (cmyk). </param>
        /// <param name="c3">Depending on the colorspace this is either the blue component (rgb), the yellow component (cmyk). </param>
        /// <param name="c4">This must only be set in cmyk colorspace and specifies the black component. </param>
        /// <param name="optlist">Possible values are N (float), extend0 (bool), extend1 (bool), antialias (bool), r0 (float), r1 (float), If the shading is of type radial the optlist must also contain the 
        /// parameters r0 and r1 with the radius of the start and end circle. optlist is in the format [name] [value] or [name] { [valuewithspaces] }</param>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_shading(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string shtype, float x0,
                                            float y0, float x1, float y1, float c1, float c2, float c3, float c4,
                                            [MarshalAs(UnmanagedType.LPStr)] string optlist);


        /// <summary>
        /// Creates a pattern based on a shading, which has to be created before 
        /// with ps_shading(). Shading patterns can be used like regular patterns. 
        /// These are Postscript PatternType 2 PatinType 1. 
        /// 
        /// The optlist param is currently not used at all. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="shading">The identifier of a shading previously created with ps_shading(). </param>
        /// <param name="optlist"></param>
        /// <returns>The identifier of the pattern or in case of an error.</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_shading_pattern(IntPtr psdoc, int shading,
                                                    [MarshalAs(UnmanagedType.LPStr)] string optlist);

        /// <summary>
        /// Creates a pattern based on a shading, which has to be created before 
        /// with ps_shading(). Shading patterns can be used like regular patterns. 
        /// These are Postscript PatternType 2 PaintType 1.         
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="shading">The identifier of a shading previously created with ps_shading(). </param>        
        /// <returns>The identifier of the pattern or 0 in case of an error.</returns>        
        public static int PS_shading_pattern(IntPtr psdoc, int shading)
        {
            return PS_shading_pattern(psdoc, shading, string.Empty);
        }

        /// <summary>
        /// Creates a new graphic state
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="optlist">May not be empty. When the graphics state is created, this is parsed and the values are set on the graphics state object. The function will error is this value is empty.. Possible values are setsmoothness (float), linewidth (float), linecap (int), linejoin (int), flatness (float), miterlimit (float), overprintmode (int)... optlist is in the format [name] [value] or [name] { [valuewithspaces] }</param>
        /// <returns>The identifier of the graphics state or 0 in case of an error.</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_create_gstate(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string optlist);

        /// <summary>
        /// Not implemented. Don't use. 
        /// 
        /// <para>Hyphenates the passed word. ps_hyphenate() evaluates the value hyphenminchars 
        /// (set by ps_set_value()) and the parameter hyphendict (set by ps_set_parameter()). 
        /// hyphendict must be set before calling this function. </para>
        /// <para>This function requires the locale category LC_CTYPE to be set properly. This is done 
        /// when the extension is initialized by using the environment variables. On Unix systems read 
        /// the man page of locale for more information. </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="text">text should not contain any non alpha characters. Possible positions for breaks are returned in an array of interger numbers. Each number is the position of the char in text after which a hyphenation can take place. </param>
        /// <param name="hyphens"></param>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int PS_hyphenate(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text,
                                               IntPtr hyphens);

        public static string PS_hyphenate(IntPtr psdoc, string text)
        {
            return PS_hyphenate(psdoc, text, IntPtr.Zero).ToString();
            //throw new Exception("The method or operation is not implemented.");           
        }


        /// <summary>
        /// Output the glyph at position ord in the font encoding vector of 
        /// the current font. The font encoding for a font can be set 
        /// when loading the font with ps_findfont(). 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="c"></param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_symbol(IntPtr psdoc, char c);

        /// <summary>
        /// Calculates the width of a glyph in points if it was output in 
        /// the given font and font size. This function needs an Adobe font 
        /// metrics file to calculate the precise width. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="c">The position of the glyph in the font encoding vector. </param>
        /// <param name="fontid">Calculates the width of a glyph in points if it was output in the given 
        /// font and font size. This function needs an Adobe font metrics file to calculate the precise width. </param>
        /// <param name="size">The size of the font. If no size is specified the current size is used.</param>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern float PS_symbol_width(IntPtr psdoc, char c, int fontid, float size);

        /// <summary>
        /// Get name of a glyph
        /// Changed signature from C:
        /// input now: char as int and fontid
        /// returns String
        /// 
        /// <para>This matches the python bindings. -thoward</para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="c">The position of the glyph in the font encoding vector. </param>
        /// <param name="fontid">Calculates the width of a glyph in points if it was output in the given 
        /// font and font size. This function needs an Adobe font metrics file to calculate the precise width. </param>
        /// <returns>The name of the symbol</returns>
        public static string PS_symbol_name(IntPtr psdoc, char c, int fontid)
        {
            StringBuilder name = new StringBuilder(255);
            PS_symbol_name(psdoc, c, fontid, name, name.Capacity);

            return name.ToString();
        }


        /// <summary>
        /// Get name of a glyph
        /// Changed signature from C:
        /// input now: char as int and fontid
        /// returns String
        /// <para>This matches the python bindings. -thoward</para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="c">The position of the glyph in the font encoding vector.</param>
        /// <param name="fontid">Calculates the width of a glyph in points if it was output in the given
        /// font and font size. This function needs an Adobe font metrics file to calculate the precise width.</param>
        /// <param name="name">The name of the symbol is output here.</param>
        /// <param name="size">The length of the StringBuilder buffer for the name.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private static extern void PS_symbol_name(IntPtr psdoc, int c, int fontid,
                                                  [MarshalAs(UnmanagedType.LPStr)] StringBuilder name, int size);

        /// <summary>
        /// Includes a file into the output file. Must be called within 'prolog' scope. In the output Postscript code, the file contents are wrapped with "PslibDict begin\n" and "end\n". The file is read into memory all at once, so if it's a big file that can't be read completely into memory, the operation will fail. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="filename">The full path and filename of the file to include.</param>
        /// <returns>Returns non-zero on error.</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_include_file(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string filename);

        /// <summary>
        /// <para>
        /// PostScript supports various kinds of fonts. Type3 fonts are fonts whose 
        /// glyphs are created with a subset of regular PostScript commands. This function 
        /// starts a new Type3 font embedded into the document which must be closed with 
        /// PS_end_font. Each glyph is created with consecutive calls of PS_end_glyph and 
        /// PS_end_glyph. Once the new font is created, it can be used like any other font 
        /// loaded with PS_findfont. The passed floating point numbers are used to set the 
        /// font matrix. a and d are usually set to 0.001. The remaining numbers can be left zero.</para>
        /// <para>
        /// Each call of PS_begin_font must be accompanied by a call to PS_end_pattern(3).</para>
        /// <para>
        /// PS_begin_font has been introduced in version 0.4.0 of pslib.</para>
        /// <para>
        /// Must be called within 'document' scope.
        /// </para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="fontname">The name of the font. </param>
        /// <param name="reserverd">Not used. </param>
        /// <param name="a">Usually 0.001</param>
        /// <param name="b">Usually 0.0</param>
        /// <param name="c">Usually 0.0</param>
        /// <param name="d">Usually 0.001</param>
        /// <param name="e">Usually 0.0</param>
        /// <param name="f">Usually 0.0</param>
        /// <param name="optlist">Not used. </param>
        /// <returns>A font id for the newly created font.</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int PS_begin_font(
            IntPtr psdoc,
            [MarshalAs(UnmanagedType.LPStr)] string fontname,
            int reserverd,
            double a,
            double b,
            double c,
            double d,
            double e,
            double f,
            [MarshalAs(UnmanagedType.LPStr)] string optlist);

        public static int PS_begin_font(
            IntPtr psdoc,
            [MarshalAs(UnmanagedType.LPStr)] string fontname)
        {
            return PS_begin_font(psdoc, fontname, 0, 0.001, 0, 0, 0.001, 0, 0, string.Empty);
        }

        /// <summary>
        ///  ends a font. must be called after a ps_begin_font (after glyphs have all been added)
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_end_font(IntPtr psdoc);

        /// <summary>
        /// <para>Starts a new glyph within a Type3 font. The glyph itself can be created with a subset of the regular drawing functions like PS_lineto(3). The glyphname is abitrary but usually something found in the input encoding vector to be accessible by PS_show(3).</para>
        /// <para>The floating point paramters describe the widht and the lower left and upper right corner of the bounding box surrounding the glyph.</para>
        /// <para>Each call of PS_begin_glyph must be accompanied by a call to PS_end_glyph(3).</para>
        /// <para>PS_begin_glyph has been introduced in version 0.4.0 of pslib.</para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="glyphname">Name of the glyph.</param>
        /// <param name="wx">The width of the glyph.</param>
        /// <param name="llx">The x-coordinate of the lower left corner. </param>
        /// <param name="lly">The y-coordinate of the lower left corner. </param>
        /// <param name="urx">The x-coordinate of the upper right corner. </param>
        /// <param name="ury">The y-coordinate of the upper right corner. </param>        
        /// <returns>true on success, false on failure. </returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern bool PS_begin_glyph(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string glyphname,
                                                 double wx, double llx, double lly, double urx, double ury);

        /// <summary>
        /// Finishes a Type3 font glyph. PS_end_glyph has been introduced in version 0.4.0 of pslib.
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_end_glyph(IntPtr psdoc);

        /// <summary>
        /// Adds a new kerning pair to an already loaded font. This function was mainly 
        /// provided to add kerning information to Type3 fonts created with PS_begin_font(3), 
        /// but it can also be used for fonts loaded with PS_findfont(3).
        /// PS_add_kerning has been introduced in version 0.4.0 of pslib.
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="fontid">The resource identifier for the font. </param>
        /// <param name="glyphname1">The name of the target glyph. </param>
        /// <param name="glyphname2">The name of the comparison glyph. </param>
        /// <param name="kern">The kerning distance between the two glyphs.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_add_kerning(IntPtr psdoc, int fontid,
                                                 [MarshalAs(UnmanagedType.LPStr)] string glyphname1,
                                                 [MarshalAs(UnmanagedType.LPStr)] string glyphname2, int kern);

        /// <summary>
        /// Adds a new ligature to an already loaded font. This function was mainly 
        /// provided to add ligatures to Type3 fonts created with PS_begin_font(3), 
        /// but it can also be used for fonts loaded with PS_findfont(3).
        /// <para>PS_add_ligature has been introduced in version 0.4.0 of pslib.</para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="fontid">The resource identifier for the font. </param>
        /// <param name="glyphname1">The first glyphname.</param>
        /// <param name="glyphname2">The second glyphname.</param>
        /// <param name="glyphname3">The third glyphname.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_add_ligature(IntPtr psdoc, int fontid,
                                                  [MarshalAs(UnmanagedType.LPStr)] string glyphname1,
                                                  [MarshalAs(UnmanagedType.LPStr)] string glyphname2,
                                                  [MarshalAs(UnmanagedType.LPStr)] string glyphname3);

        /// <summary>
        /// Output the glyph with the name glyphname. This function is similar 
        /// to PS_symbol(3) but does not take the current font encoding vector 
        /// into account. It is useful in cases where an abitrary glyph, 
        /// not accessible through the font encoding vector, shall be output.
        /// <para>PS_glyph_show has been introduced in version 0.4.0 of pslib.</para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="name">The name of the glyph.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_glyph_show(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// Returns the list of glyphs names available in the current font or the font with 
        /// the passed fontid. The function allocates memory for the array of glyph names 
        /// which must be freed by the calling application with PS_free_glyph_list(3). The 
        /// list of glyphs has len entries and is of no particular order. 
        /// 
        /// <para>PS_glyph_list has been introduced in version 0.4.0 of pslib.</para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="fontid">The resource identifier for the font. </param>
        /// <param name="charlist"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private static extern IntPtr PS_glyph_list(IntPtr psdoc, int fontid, out IntPtr charlist, out int len);

        public static string[] PS_glyph_list(IntPtr psdoc, int fontid)
        {
            int len;
            IntPtr charlist;

            // The current pointer. 
            IntPtr glyphListPtr = PS_glyph_list(psdoc, fontid, out charlist, out len);

            // The string. 
            //string pstrString = null;

            string[] glyphlist = new string[len];

            // Cycle through the pointers returned. 
            for (int pintIndex = 0; pintIndex < len; pintIndex++)
            {
                // Read the pointer. 
                IntPtr pintCurrent = Marshal.ReadIntPtr(glyphListPtr, pintIndex*Marshal.SizeOf(typeof (IntPtr)));

                // Now read the string. 
                glyphlist[pintIndex] = Marshal.PtrToStringAnsi(pintCurrent);
            }
            PS_free_glyph_list(psdoc, glyphListPtr, len);
            return glyphlist;
        }

        /// <summary>
        /// Frees a glyph_list array from memory in the pslib unmanaged memory space... This shoudl be called immediately after retreiveing the glyph list with ps_glyph_list
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="charlist">Pointer to the returned array.</param>
        /// <param name="len">Length of array.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private static extern void PS_free_glyph_list(IntPtr psdoc, IntPtr charlist, int len);

        /// <summary>
        /// Return width of a glyph
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="glyphname">The name of the glyph. </param>
        /// <param name="fontid">The resource identifier of the font. </param>
        /// <param name="size">The font size. </param>
        /// <returns>The width of the glyph.</returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern float PS_glyph_width(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string glyphname,
                                                  int fontid, float size);

        #region tested

        /// <summary>
        /// Reads an image which is already available in memory. The parameter source is currently not 
        /// evaluated and assumed to be memory. The image data is a sequence of pixels starting in the 
        /// upper left corner and ending in the lower right corner. Each pixel consists of components 
        /// color components, and each component has bpc bits. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="type">The type of the image. Possible values are png, jpeg, tiff, bmp or eps. </param>
        /// <param name="source">Not used. </param>
        /// <param name="data">The image data. </param>
        /// <param name="length">The length of the image data. </param>
        /// <param name="width">The width of the image. </param>
        /// <param name="height">The height of the image. </param>
        /// <param name="components">The number of components for each pixel. This can be 1 (gray scale images), 3 (rgb images), or 4 (cmyk, rgba images). </param>
        /// <param name="bpc">Number of bits per component (quite often 8). </param>
        /// <param name="parameters">Not used. </param>
        /// <returns>Returns identifier of image or zero in case of an error. The identifier is a positive number greater than 0. </returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private static extern int PS_open_image(
            IntPtr psdoc,
            [MarshalAs(UnmanagedType.LPStr)] string type,
            [MarshalAs(UnmanagedType.LPStr)] string source,
            [MarshalAs(UnmanagedType.LPStr)] string data,
            long length,
            int width,
            int height,
            int components,
            int bpc,
            [MarshalAs(UnmanagedType.LPStr)] string parameters);

        /// <summary>
        /// Reads an image which is already available in memory. 
        /// The image data is a sequence of pixels starting in the upper left corner and ending 
        /// in the lower right corner. Each pixel consists of components 
        /// color components, and each component has bpc bits. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="type">The type of the image. Possible values are png, jpeg, or eps. </param>
        /// <param name="data">The image data. </param>
        /// <param name="length">The length of the image data. </param>
        /// <param name="width">The width of the image. </param>
        /// <param name="height">The height of the image. </param>
        /// <param name="components">The number of components for each pixel. This can be 1 (gray scale images), 3 (rgb images), or 4 (cmyk, rgba images). </param>
        /// <param name="bpc">Number of bits per component (quite often 8). </param>        
        /// <returns>Returns identifier of image or zero in case of an error. The identifier is a positive number greater than 0. </returns>
        public static int PS_open_image(
            IntPtr psdoc,
            ImageType type,
            //string source, 
            string data,
            long length,
            int width,
            int height,
            int components,
            int bpc) //, 
            //string parameters)
        {
            return PS_open_image(
                psdoc,
                type.ToString(),
                string.Empty, //source, 
                data,
                length,
                width,
                height,
                components,
                bpc,
                string.Empty //parameters
                );
        }

        /// <summary>
        /// Loads an image for later use.
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="type">The type of the image. Possible values are png, jpeg, tiff, bmp or eps. </param>
        /// <param name="filename">The name of the file containing the image data. </param>
        /// <param name="stringparam">
        /// if image is of type png, you may pass the parameter "masked" or "mask" 
        /// to indicate the relationship between two images. If the stringparam is masked, 
        /// the inparam is the image id of a loaded image that was loaded with the mask param. 
        /// For example, to mask two images, the mask image is loaded first, with the 
        /// stringparam "mask", then the masked image is loaded second, with the 
        /// stringparam "masked" and intparam of the handle to the first image.</param>
        /// <param name="intparam">The id of a mask image when using png masking (see stringparam description)</param>
        /// <returns>Returns identifier of image or zero in case of an error. The identifier is a positive number greater than 0. </returns>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern int PS_open_image_file(IntPtr psdoc,
                                                     [MarshalAs(UnmanagedType.LPStr)] string type,
                                                     [MarshalAs(UnmanagedType.LPStr)] StringBuilder filename,
                                                     [MarshalAs(UnmanagedType.LPStr)] string stringparam,
                                                     int intparam);

        public static int PS_open_image_file(
            IntPtr psdoc, ImageType type, string filename)
        {
            StringBuilder b = new StringBuilder(filename);
            //return PS_open_image_file(psdoc, type.ToString(), filename, string.Empty, 0);
            return PS_open_image_file(psdoc, type.ToString(), b, string.Empty, 0);
        }

        /// <summary>
        /// Loads an image for later use. 
        /// <para>Note: This overload is only usefull for PNG images that are being masked.</para>
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="type">The type of the image. Possible values are png, jpeg, tiff, bmp or eps. </param>
        /// <param name="filename">The name of the file containing the image data. </param>
        /// <param name="maskType">        
        /// If image is of ImageType png, the image can be masked with another image.
        /// You specify the masking relationship between the images using PngMaskType enum.
        /// 
        /// The imageID is the id of a previously loaded image that was loaded as PngMaskType.mask.
        /// 
        /// For example, to mask two images, the mask image is loaded first, as PngMaskType.mask, 
        /// then the masked image is loaded second, as PngMaskType.masked and the imageID as the handle 
        /// to the first image.</param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public static int PS_open_image_file(
            IntPtr psdoc, ImageType type, string filename, PngMaskType maskType, int imageId)
        {
            StringBuilder b = new StringBuilder(filename);
            return PS_open_image_file(psdoc, type.ToString(), b, maskType.ToString(), imageId);
            //return PS_open_image_file(psdoc, type.ToString(), filename, maskType.ToString(), imageId);
        }


        /// <summary>
        /// Places a formerly loaded image on the page. The image can be scaled. If the image shall be rotated as well, you will have to rotate the coordinate system before with ps_rotate(). 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="imageid">The resource identifier of the image as returned by ps_open_image() or ps_open_image_file(). </param>
        /// <param name="x">x-coordinate of the lower left corner of the image. </param>
        /// <param name="y">y-coordinate of the lower left corner of the image. </param>
        /// <param name="scale">The scaling factor for the image. A scale of 1.0 will result in a resolution of 72 dpi, because each pixel is equivalent to 1 point. </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_place_image(IntPtr psdoc, int imageid, float x, float y, float scale);

        /// <summary>
        /// Closes an image and frees its resources. Once an image is closed it cannot be used anymore. 
        /// </summary>
        /// <param name="psdoc">A resource identifier to a postcript document.</param>
        /// <param name="imageid">Resource identifier of the image as returned by ps_open_image() or ps_open_image_file(). </param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_close_image(IntPtr psdoc, int imageid);

        #endregion

        #region tested

        /// <summary>
        /// Writes text starting at a the current position.
        /// </summary>
        /// <param name="psdoc">A psdoc handle.</param>
        /// <param name="text">The text to print</param>        
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_show(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text);

        /// <summary>
        /// Writes text starting at a the current position.
        /// </summary>
        /// <param name="psdoc">A psdoc handle.</param>
        /// <param name="text">The text to print</param>
        /// <param name="xlen">How many characters of the string to print.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_show2(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text, int xlen);

        /// <summary>
        /// Writes text starting at a specified position.
        /// </summary>
        /// <param name="psdoc">A psdoc handle.</param>
        /// <param name="text">The text to print</param>
        /// <param name="x">The x position to print text at.</param>
        /// <param name="y">The y position to print text at.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_show_xy(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text, float x,
                                             float y);

        /// <summary>
        /// Writes text starting at a specified position.
        /// </summary>
        /// <param name="psdoc">A psdoc handle.</param>
        /// <param name="text">The text to print</param>
        /// <param name="xlen">How many characters of the string to print.</param>
        /// <param name="x">The x position to print text at.</param>
        /// <param name="y">The y position to print text at.</param>
        [DllImport("pslib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void PS_show_xy2(IntPtr psdoc, [MarshalAs(UnmanagedType.LPStr)] string text, int xlen,
                                              float x, float y);

        #endregion
    }
}