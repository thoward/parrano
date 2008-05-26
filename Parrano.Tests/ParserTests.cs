using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Parrano.Parser;
using System.IO;
using System.Diagnostics;

namespace Parrano.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void ParseTokens()
        {
            IEnumerable<Token> enties = Parser.Parser.GetTokens(new MemoryStream(Encoding.Default.GetBytes(testPS)));

            foreach (Token entity in enties)
            {
                Trace.WriteLine(string.Format("Type: {0}  Pos: {1}  Value: {2}", entity.GetType().Name, entity.Position, entity.Text));
            }
        }

        [Test]
        public void ParseEntites()
        {
            IEnumerable<PSEntity> entities = Parser.Parser.GetEntities(new MemoryStream(Encoding.Default.GetBytes(testPS)));

            DisplayEntities(entities, string.Empty);
        }

        private static void DisplayEntities(IEnumerable<PSEntity> entities, string prefix)
        {
            foreach (PSEntity entity in entities)
            {
                Trace.WriteLine(string.Format(prefix + "Type: {0}  Pos: {1}  Value: {2}", entity.GetType().Name,
                                              entity.Position, entity.UnParse()));

                PSArray array = entity as PSArray;
                if (array != null)
                {
                    //Trace.Write("[");
                    DisplayEntities(array.Values, prefix + "\t");
                    //Trace.Write("]" + Environment.NewLine + prefix);
                }

                PSScopeBlock scopeBlock = entity as PSScopeBlock;
                if (scopeBlock != null)
                {
                    //Trace.Write(Environment.NewLine + prefix + "{" + Environment.NewLine + prefix + "\t");
                    DisplayEntities(scopeBlock.Values, prefix + "\t");
                    //Trace.Write(Environment.NewLine + prefix + "}" + Environment.NewLine + prefix);
                }

                //if (array == null && scopeBlock == null)
                //{
                //    if (entity is PSName)
                //    {
                //        Trace.Write(Environment.NewLine + prefix);
                //    }

                //    Trace.Write(entity.UnParse());

                //    if (entity is PSComment)
                //    {
                //        Trace.Write(Environment.NewLine + prefix);
                //    }
                //    else
                //    {
                //        Trace.Write(" ");
                //    }
                //}
            }
        }

        private const string testPS =
    @"%!PS-Adobe-3.0
%%Creator: pslib 0.4.0
%%Creation-Date: 23/05/2008 04:33 PM
%%PageOrder: Ascend
%%Pages: (atend)
%%BoundingBox: 0 0 596 842
%%Orientation: Portrait
%%DocumentProcessColors: Black
%%DocumentCustomColors: 
%%CMYKCustomColor: 
%%RGBCustomColor: 
%%EndComments
%%BeginProlog
%%BeginResource: definicoes
%%EndResource
%%BeginProcSet
/PslibDict 300 dict def PslibDict begin/N{def}def/B{bind def}N
/TR{translate}N/vsize 11 72 mul N/hsize 8.5 72 mul N/isls false N
/p{show}N/w{0 rmoveto}B/a{moveto}B/l{lineto}B/qs{currentpoint
currentpoint newpath moveto 3 2 roll dup true charpath stroke
stringwidth pop 3 2 roll add exch moveto}B/qf{currentpoint
currentpoint newpath moveto 3 2 roll dup true charpath fill
stringwidth pop 3 2 roll add exch moveto}B/qsf{currentpoint
currentpoint newpath moveto 3 2 roll dup true charpath gsave stroke grestore fill
stringwidth pop 3 2 roll add exch moveto}B/qc{currentpoint
currentpoint newpath moveto 3 2 roll dup true charpath clip
stringwidth pop 3 2 roll add exch moveto}B/qsc{currentpoint
currentpoint initclip newpath moveto 3 2 roll dup true charpath clip stroke
stringwidth pop 3 2 roll add exch moveto}B/qfc{currentpoint
currentpoint initclip newpath moveto 3 2 roll dup true charpath clip fill
stringwidth pop 3 2 roll add exch moveto}B/qfsc{currentpoint
currentpoint initclip newpath moveto 3 2 roll dup true charpath gsave stroke grestore clip fill
stringwidth pop 3 2 roll add exch moveto}B/qi{currentpoint
3 2 roll
stringwidth pop 3 2 roll add exch moveto}B/tr{currentpoint currentpoint 5 4 roll add moveto}B/rt{moveto}B/#copies{1}B
/PslibPageBeginHook{pop pop pop pop pop}B
/PslibPageEndHook{pop}B

/reencdict 12 dict def /ReEncode { reencdict begin
/newcodesandnames exch def /newfontname exch def /basefontname exch def
/basefontdict basefontname findfont def /newfont basefontdict maxlength dict def
basefontdict { exch dup /FID ne { dup /Encoding eq
{ exch dup length array copy newfont 3 1 roll put }
{ exch newfont 3 1 roll put } ifelse } { pop pop } ifelse } forall
newfont /FontName newfontname put newcodesandnames aload pop
128 1 255 { newfont /Encoding get exch /.notdef put } for
newcodesandnames length 2 idiv { newfont /Encoding get 3 1 roll put } repeat
newfontname newfont definefont pop end } def
end
%%EndProcSet
%%BeginProcSet
%!
% Colour separation.
% Ask dvips to do 4 pages. In bop-hook, cycle
% round CMYK color spaces.
%
% Sebastian Rahtz 30.9.93
% checked 7.1.94
% from Green Book, and Kunkel Graphic Design with PostScript
% (Green Book Listing 9-5, on page 153.)
%
% This work is placed in the public domain
/seppages  0  def 
userdict begin
/Min {% 3 items on stack
2 copy lt { pop }{ exch pop } ifelse
2 copy lt { pop }{ exch pop } ifelse
} def
/SetGray {
 1 exch sub systemdict begin adjustdot setgray end    
} def
/sethsbcolor {systemdict begin
  sethsbcolor currentrgbcolor end
  userdict begin setrgbcolor end}def 

/ToCMYK
% Red book p 305
  {
% subtract each colour from 1
  3 { 1 exch sub 3 1 roll } repeat
% define percent of black undercolor
% find minimum (k)
  3 copy  Min 
% remove undercolor
  blackUCR sub
  dup 0 lt {pop 0} if 
  /percent_UCR exch def 
%
% subtract that from each colour
%
  3 { percent_UCR sub 3 1 roll } repeat 
% work out black itself
  percent_UCR 1.25 mul % 1 exch sub
% stack should now have C M Y K
} def 
%
% crop marks
%
/cX 18 def 
/CM{gsave TR 0 cX neg moveto 0 cX lineto stroke
cX neg 0 moveto cX 0 lineto stroke grestore}def 
%
/bop-hook{cX dup TR
%
% which page are we producing
%
   seppages 1 add 
    /seppages exch def
     seppages 5 eq { /seppages  1  def } if
     seppages 1 eq { 
      /ColourName (CYAN) def 
      CYAN setupcolor    
      /WhichColour 3 def } if 
   seppages 2 eq { 
      /ColourName (MAGENTA) def 
      MAGENTA setupcolor 
     /WhichColour 2 def } if
   seppages 3 eq { 
      /ColourName (YELLOW) def
      YELLOW setupcolor  
      /WhichColour 1 def } if 
   seppages 4 eq { 
      /ColourName (BLACK) def 
      BLACK setupcolor   
      /WhichColour 0 def } if 
%
% crop marks
%
gsave .3 setlinewidth 
3 -7 moveto
/Helvetica findfont 6 scalefont setfont
ColourName show
0 0 CM 
vsize cX 2 mul sub dup hsize cX 2 mul sub dup isls{4 2 roll}if 0 CM 
exch CM 0 
exch CM 
grestore 0 cX -2 mul TR isls
{cX -2 mul 0 TR}if
      } def end
% 
/separations 48 dict def
separations begin
   /cmykprocs [ %def
       % cyan
    { pop pop  pop SetGray  }
       % magenta
    { pop pop exch pop SetGray  }
       % yellow
    { pop 3 1 roll pop pop SetGray  }
       % black
    { 4 1 roll pop pop pop SetGray  }
   ] def
   /rgbprocs [ %def
       % cyan
    { ToCMYK pop pop pop SetGray }
       % magenta
    { ToCMYK pop pop exch pop SetGray }
       % yellow
    { ToCMYK pop 3 1 roll pop pop SetGray }
       % black
    { ToCMYK 4 1 roll pop pop pop SetGray  }
   ] def
   /testprocs [ %def
       % cyan
    { ToCMYK pop pop pop  }
       % magenta
    { ToCMYK pop pop exch pop  }
       % yellow
    { ToCMYK pop 3 1 roll pop pop  }
       % black
    { ToCMYK 4 1 roll pop pop pop   }
   ] def
   /screenangles [ %def
       105  % cyan
       75    % magenta
       0      % yellow
       45    % black
   ] def
end  % separations

% setupcolortakes 0, 1, 2, or 3 as its argument,
% for cyan, magenta, yellow, and black.
/CYAN 0 def           /MAGENTA 1 def
/YELLOW 2 def         /BLACK 3 def
/setupcolor{ %def
   userdict begin
       dup separations /cmykprocs get exch get
       /setcmykcolor exch def
       dup separations /rgbprocs get exch get
       /setrgbcolor exch def
       dup separations /testprocs get exch get
       /testrgbcolor exch def
       separations /screenangles get exch get
       currentscreen
           exch pop 3 -1 roll exch
       setscreen
       /setscreen { pop pop pop } def
%
% redefine setgray so that it only shows on the black separation
%
      /setgray {
       WhichColour 0 eq
       {systemdict begin adjustdot setgray end} 
       {pop systemdict begin 1 setgray end}
       ifelse}def 
   end
} bind def

%
% from Kunkel
%
/adjustdot { dup 0 eq { } { dup 1 exch sub .1 mul add} ifelse } def
%
% redefine existing operators
%
% Percent of undercolor removal
/magentaUCR .3 def  
/yellowUCR .07 def  
/blackUCR .4 def 
%
% Correct yellow and magenta
/correctMY {rgb2cym
  1 index yellowUCR mul sub 3 1 roll
  1 index magentaUCR mul sub 3 1 roll
  3 1 roll rgb2cym}def
% 
%(bluely green ) =
%CYAN setupcolor
%.1 .4 .5  testrgbcolor =
%MAGENTA setupcolor
%.1 .4 .5  testrgbcolor =
%YELLOW setupcolor
%.1 .4 .5  testrgbcolor =
%BLACK setupcolor
%.1 .4 .5  testrgbcolor =
%quit
%%EndProcSet
/fontenc-CorkEncoding [
8#000 /grave 8#001 /acute 8#002 /circumflex 8#003 /tilde 8#004 /dieresis 8#005 /hungarumlaut 8#006 /ring 8#007 /caron 
8#010 /breve 8#011 /macron 8#012 /dotaccent 8#013 /cedilla 8#014 /ogonek 8#015 /quotesinglbase 8#016 /guilsinglleft 8#017 /guilsinglright 
8#020 /quotedblleft 8#021 /quotedblright 8#022 /quotedblbase 8#023 /guillemotleft 8#024 /guillemotright 8#025 /endash 8#026 /emdash 8#027 /compwordmark 
8#030 /perthousandzero 8#031 /dotlessi 8#032 /dotlessj 8#033 /ff 8#034 /fi 8#035 /fl 8#036 /ffi 8#037 /ffl 
8#040 /visualspace 8#041 /exclam 8#042 /quotedbl 8#043 /numbersign 8#044 /dollar 8#045 /percent 8#046 /ampersand 8#047 /quoteright 
8#050 /parenleft 8#051 /parenright 8#052 /asterisk 8#053 /plus 8#054 /comma 8#055 /hyphen 8#056 /period 8#057 /slash 
8#060 /zero 8#061 /one 8#062 /two 8#063 /three 8#064 /four 8#065 /five 8#066 /six 8#067 /seven 
8#070 /eight 8#071 /nine 8#072 /colon 8#073 /semicolon 8#074 /less 8#075 /equal 8#076 /greater 8#077 /question 
8#100 /at 8#101 /A 8#102 /B 8#103 /C 8#104 /D 8#105 /E 8#106 /F 8#107 /G 
8#110 /H 8#111 /I 8#112 /J 8#113 /K 8#114 /L 8#115 /M 8#116 /N 8#117 /O 
8#120 /P 8#121 /Q 8#122 /R 8#123 /S 8#124 /T 8#125 /U 8#126 /V 8#127 /W 
8#130 /X 8#131 /Y 8#132 /Z 8#133 /bracketleft 8#134 /backslash 8#135 /bracketright 8#136 /asciicircum 8#137 /underscore 
8#140 /quoteleft 8#141 /a 8#142 /b 8#143 /c 8#144 /d 8#145 /e 8#146 /f 8#147 /g 
8#150 /h 8#151 /i 8#152 /j 8#153 /k 8#154 /l 8#155 /m 8#156 /n 8#157 /o 
8#160 /p 8#161 /q 8#162 /r 8#163 /s 8#164 /t 8#165 /u 8#166 /v 8#167 /w 
8#170 /x 8#171 /y 8#172 /z 8#173 /braceleft 8#174 /bar 8#175 /braceright 8#176 /asciitilde 8#177 /hyphen 
8#200 /Abreve 8#201 /Aogonek 8#202 /Cacute 8#203 /Ccaron 8#204 /Dcaron 8#205 /Ecaron 8#206 /Eogonek 8#207 /Gbreve 
8#210 /Lacute 8#211 /Lcaron 8#212 /Lslash 8#213 /Nacute 8#214 /Ncaron 8#215 /Ng 8#216 /Ohungarumlaut 8#217 /Racute 
8#220 /Rcaron 8#221 /Sacute 8#222 /Scaron 8#223 /Scedilla 8#224 /Tcaron 8#225 /Tcedilla 8#226 /Uhungarumlaut 8#227 /Uring 
8#230 /Ydieresis 8#231 /Zacute 8#232 /Zcaron 8#233 /Zdotaccent 8#234 /IJ 8#235 /Idotaccent 8#236 /dbar 8#237 /section 
8#240 /abreve 8#241 /aogonek 8#242 /cacute 8#243 /ccaron 8#244 /dcaron 8#245 /ecaron 8#246 /eogonek 8#247 /gbreve 
8#250 /lacute 8#251 /lcaron 8#252 /lslash 8#253 /nacute 8#254 /ncaron 8#255 /ng 8#256 /ohungarumlaut 8#257 /racute 
8#260 /rcaron 8#261 /sacute 8#262 /scaron 8#263 /scedilla 8#264 /tquoteright 8#265 /tcedilla 8#266 /uhungarumlaut 8#267 /uring 
8#270 /ydieresis 8#271 /zacute 8#272 /zcaron 8#273 /zdotaccent 8#274 /ij 8#275 /exclamdown 8#276 /questiondown 8#277 /sterling 
8#300 /Agrave 8#301 /Aacute 8#302 /Acircumflex 8#303 /Atilde 8#304 /Adieresis 8#305 /Aring 8#306 /AE 8#307 /Ccedilla 
8#310 /Egrave 8#311 /Eacute 8#312 /Ecircumflex 8#313 /Edieresis 8#314 /Igrave 8#315 /Iacute 8#316 /Icircumflex 8#317 /Idieresis 
8#320 /Eth 8#321 /Ntilde 8#322 /Ograve 8#323 /Oacute 8#324 /Ocircumflex 8#325 /Otilde 8#326 /Odieresis 8#327 /OE 
8#330 /Oslash 8#331 /Ugrave 8#332 /Uacute 8#333 /Ucircumflex 8#334 /Udieresis 8#335 /Yacute 8#336 /Thorn 8#337 /Germandbls 
8#340 /agrave 8#341 /aacute 8#342 /acircumflex 8#343 /atilde 8#344 /adieresis 8#345 /aring 8#346 /ae 8#347 /ccedilla 
8#350 /egrave 8#351 /eacute 8#352 /ecircumflex 8#353 /edieresis 8#354 /igrave 8#355 /iacute 8#356 /icircumflex 8#357 /idieresis 
8#360 /eth 8#361 /ntilde 8#362 /ograve 8#363 /oacute 8#364 /ocircumflex 8#365 /otilde 8#366 /odieresis 8#367 /oe 
8#370 /oslash 8#371 /ugrave 8#372 /uacute 8#373 /ucircumflex 8#374 /udieresis 8#375 /yacute 8#376 /thorn 8#377 /germandbls 
] def
/pdfmark where {pop} {userdict /pdfmark /cleartomark load put} ifelse
[ /Creator (pslib 0.4.0)
  /Creation-Date (23/05/2008 04:33 PM)
/DOCINFO pdfmark ]
%%EndProlog
%%BeginSetup
PslibDict begin
%%EndSetup

%%Page: 1 1
%%PageBoundingBox: 0 0 596 842
%%BeginPageSetup
[ /CropBox [0 0 596.00 842.00] /PAGE pdfmark ]
%%EndPageSetup
save
0 0 596.00 842.00 1 PslibPageBeginHook
restore
save
restore
save
1 PslibPageEndHook
restore
showpage
%%Trailer
end
%%Pages: 1
%%BoundingBox: 0 0 596 842
%%Orientation: Portrait
%%EOF";
    }
}
