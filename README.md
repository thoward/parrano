# Parrano

**Parrano** is a .NET wrapper for the [PSlib](http://pslib.sourceforge.net) C API. [PSlib](http://pslib.sourceforge.net) is a popular C library for generating multi page PostScript documents. There are functions for drawing lines, arcs, rectangles, curves, etc. [PSlib](http://pslib.sourceforge.net) also provides very sophisticated functions for text output including hyphenation and kerning, as well as the ability to define custom fonts and encodings.

The [PSlib](http://pslib.sourceforge.net) version that this wrapper is based on is v0.4.0. Please check the [PSlib home page](http://pslib.sourceforge.net) to obtain the latest version. 

Parrano also includes a simple PostScript parser.

A note about licensing:
Although the Parrano wrapper code is offered under the MIT license (completely unrestricted), [PSlib](http://pslib.sourceforge.net) is licensed under the GPL and LGPL licenses. This may prevent use of PSlib within commercial applications without a commercial license. Please [contact the PSlib project owners](http://pslib.sourceforge.net/contact.php) to inquire about commercial use. 

However, once you have determined the validity of your [PSlib](http://pslib.sourceforge.net) license, you may use the Paranno wrapper code to access it from your commercial application with no licensing concerns.

## Current Status

Parrano is not yet to a first release. 

If you are interested in working on the code, feel free to get the latest version, make your changes, and send a patch in. If you want to do a lot of work on the project, drop a line to thoward37@gmail.com, and I'll set you up with commit access to the repository. A quick note indicating what you want to work on would be helpful, so we don't step each other's toes.

## Outstanding Issues

* Trying to decide how to handle the `PS_new_2()` API declaration. It has the ability to pass function pointers for all memory allocation for a particular `psdoc`, which seems like a marshaling nightmare, as well as a source of a lot of potential memory consistency problems. I've stubbed out the declaration and delegates for the function pointers, but I'm hesitant to actually give public access to this overload without a higher-level managed object that will interact against it.  

## To Do

  * Write unit tests for all the core API calls in `Parrano.Tests`. 
  * Port all code in the PSlib examples library in `Parrano.Examples` project.
  * Compile PSlib v0.4.1 with image support for Win32. Current development is being done using a downloaded binary of v0.4.0 without the gif/tiff/etc support. The only image type that works with this library is `eps`, `png`, and `jpeg`. Until I create a correct DLL, I won't be able to test the image functionality.
  * Create managed OO layer on top of core API calls, to abstract the procedural nature of the library. 
  * Make sure all extern declarations constrained with .NET enums as parameters also have original compatibility API declaration publicly available, to allow for easier porting.
  * Update enums to have proper Pascal casing, and instead of calling .ToString(), apply `Description` attributes, and call `DescriptionAttribute.GetCustomAttribute` to read the string description which will be passed to the API extern call. This will allow for better maintainability of code if the strings PSlib expects change. 

