using System;
using System.Collections.Generic;
using System.IO;

namespace slc
{

    public class ArgumentsParser
    {
        public static class ParameterErrors
        {
            public const string InvalidDirectory = "Invalid directory. Directory doesn't exist.";
            public const string InvalidFile = "Invalid file name. File doesn't exist.";
            public const string NoInput = "No input files found.";
            public const string OutputDirectory = "Output directory is already specified.";
        }

        public static void Parse(string[] args, out List<string> files, out bool recursive, out string outputDirectory )
        {
            recursive = false;
            outputDirectory = string.Empty;
            files = new List<string>();

            foreach (var arg in args)
            {
                if (!IsFlag(arg))
                {
                    if (File.Exists(arg))
                        files.Add(arg);
                    else InvalidOption(arg, ParameterErrors.InvalidFile);
                    continue;
                }
                switch (arg[1])
                {
                    case 'o':
                    {
                        if (arg[2] == '=')
                        {
                            var dir = arg.Substring(3);
                            if (Directory.Exists(dir))
                                if (outputDirectory == string.Empty)
                                    outputDirectory = dir;
                                else InvalidOption(arg, ParameterErrors.OutputDirectory);
                            else InvalidOption(arg, ParameterErrors.InvalidDirectory);

                        }
                        else InvalidOption(arg);
                        break;
                    }
                    case 'i':
                    {
                        if (arg[2] == '=')
                        {
                            var dir = arg.Substring(3);
                            if (Directory.Exists(dir))
                            {
                                AddFilesFromDir(dir, files, recursive);
                            }
                            else InvalidOption(arg, ParameterErrors.InvalidDirectory);
                        }
                        else InvalidOption(arg);
                        break;
                    }
                    case 'r':
                    {
                        if (arg == "-r") recursive = true;
                        else InvalidOption(arg);

                        break;
                    }
                    default:
                    {
                        InvalidOption(arg);
                        break;
                    }
                }
            }

            if (files.Count == 0) AddFilesFromDir(Directory.GetCurrentDirectory(), files, recursive);

            if (files.Count == 0) throw new ArgumentsParserException(ParameterErrors.NoInput);
        }

        private static void AddFilesFromDir(string dir, ICollection<string> files, bool recursive)
        {
            foreach (var fileName in Directory.EnumerateFiles(dir, "*", recursive?SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                if (fileName.EndsWith(".s4x") || fileName.EndsWith(".s4j") || fileName.EndsWith(".xsd"))
                {
                    files.Add(fileName);
                }
            }
        }

        private static bool IsFlag(string arg)
        {
            return arg[0] == '-';
        }

        public static void Help()
        {
            Console.WriteLine(
                    "Usage:\n\n\tslc [options] [inputFiles] ...\n\n" +
                    "Options:\n" +
                    " -i=DIR           Input directory with s4x, s4j and xsd files\n" +
                    " -o=DIR           Output directory\n" +
                    " -r               Turns on recursive search of files in the input directories.\n" +
                    " inputFiles       Names, including path, of .s4x, .s4j, .xsd files.\n\n" +
                    "Description:\n\n" +
                    "Compiles a set of files in Syntactik format into XML or JSON.\n" +
                    "Validates generated XML files against XML schema (XSD).\n" +
                    "You can specify one or many input directories or files with \nextensions s4x, s4j and xsd.\n" +
                    "If neither directories nor files are given then compiler takes \nthem from the current directory.\n" +
                    "If s4x files are found then s4j files are ignored.\n\n"
            );
        }

        private static void InvalidOption(string arg, string message = null)
        {
            Console.Error.WriteLine("Invalid command line argument: {0}. {1}", arg, message);
        }
    }
}
