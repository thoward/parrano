using System;
using System.Runtime.InteropServices;

namespace Parrano.Api
{

    #region Delegates

    /// <summary>
    /// A delegate which serves as a function pointer for a error handler callback function for use in the PS_new2 API call.
    /// </summary>
    /// <param name="psdoc">A resource identifier to a postcript document.</param>
    /// <param name="level">The integer error level.</param>
    /// <param name="msg">The error message.</param>
    /// <param name="data">A pointer to related data.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ErrorHandler(IntPtr psdoc, int level, string msg, IntPtr data);

    /// <summary>
    /// A delegate which serves as a function pointer for a memory allocation callback function for use in the PS_new2 API call.
    /// </summary>
    /// <param name="psdoc">A resource identifier to a postcript document.</param>
    /// <param name="size">The size of memory block to allocate.</param>
    /// <param name="caller">The caller.</param>
    /// <returns>A pointer to a region of allocated memory.</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr AllocProc(IntPtr psdoc, Int32 size, string caller);

    /// <summary>
    /// A delegate which serves as a function pointer for a memory re-allocation callback function for use in the PS_new2 API call.
    /// </summary>
    /// <param name="psdoc">A resource identifier to a postcript document.</param>
    /// <param name="mem">A pointer to a previously allocated area of memory (the return value from a call to AllocProc).</param>
    /// <param name="size">The size of memory block to allocate.</param>
    /// <param name="caller">The caller.</param>
    /// <returns>A pointer to a region of allocated memory.</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr ReAllocProc(IntPtr psdoc, IntPtr mem, Int32 size, string caller);

    /// <summary>
    /// A delegate which serves as a function pointer for a callback function which frees previously allocated memory for use in the PS_new2 API call.
    /// </summary>
    /// <param name="psdoc">A resource identifier to a postcript document.</param>
    /// <param name="mem">A pointer to a previously allocated area of memory (the return value from a call to AllocProc).</param>        
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FreeProc(IntPtr psdoc, IntPtr mem);

    /// <summary>
    /// WriteProc callback delegate
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int WriteProcCallBack(IntPtr psdoc, IntPtr data, int size);

    #endregion Delegates
}