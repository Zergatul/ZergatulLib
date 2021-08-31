using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.FileFormat.Zip
{
    enum CompressionMethod
    {
        NoCompression = 0,
        Deflate = 8
        /*
                 0 - The file is stored (no compression)
        1 - The file is Shrunk
        2 - The file is Reduced with compression factor 1
        3 - The file is Reduced with compression factor 2
        4 - The file is Reduced with compression factor 3
        5 - The file is Reduced with compression factor 4
        6 - The file is Imploded
        7 - Reserved for Tokenizing compression algorithm
        8 - The file is Deflated
        9 - Enhanced Deflating using Deflate64(tm)
       10 - PKWARE Data Compression Library Imploding (old IBM TERSE)
       11 - Reserved by PKWARE
       12 - File is compressed using BZIP2 algorithm
       13 - Reserved by PKWARE
       14 - LZMA
       15 - Reserved by PKWARE
       16 - IBM z/OS CMPSC Compression
       17 - Reserved by PKWARE
       18 - File is compressed using IBM TERSE (new)
       19 - IBM LZ77 z Architecture 
       20 - deprecated (use method 93 for zstd)
       93 - Zstandard (zstd) Compression 
       94 - MP3 Compression 
       95 - XZ Compression 
       96 - JPEG variant
       97 - WavPack compressed data
       98 - PPMd version I, Rev 1
       99 - AE-x encryption marker (see APPENDIX E)
         */
    }
}