using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace PowerBiDiffer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("Args cannot be empty and must define -textconv or -diff");
            }

            var type = args[0];

            if (type.EndsWith("-textconv", StringComparison.OrdinalIgnoreCase))
            {
                if (args.Length < 3)
                {
                    throw new ArgumentException("when using -textconv mode, then 1 file path must be provided for the text conversion");
                }
                App.ConvertToText(args[1]);
            }
            else if (type.EndsWith("-diff", StringComparison.OrdinalIgnoreCase))
            {
                if (args.Length < 3)
                {
                    throw new ArgumentException("when using -diff mode, then 2 filepaths should be provided");
                }

                App.ExecuteComparison(args[1], args[2]);
            }
            else
            {
                throw new Exception($"this is an error");
            }
        }
    }
}