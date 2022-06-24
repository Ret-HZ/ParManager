// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace ParTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CommandLine;
    using CommandLine.Text;
    using Yarhl.FileSystem;

    /// <summary>
    /// Main program.
    /// </summary>
    internal static partial class Program
    {
        private static void Main(string[] args)
        {
            using var parser = new Parser(with => with.HelpWriter = null);
            ParserResult<object> parserResult = parser.ParseArguments<Options.List, Options.Extract, Options.Create, Options.Remove, Options.Add>(args);
            parserResult
                .WithParsed<Options.List>(List)
                .WithParsed<Options.Extract>(Extract)
                .WithParsed<Options.Create>(Create)
                .WithParsed<Options.Remove>(Remove)
                .WithParsed<Options.Add>(Add)
                .WithNotParsed(x =>
                {
                    if (args.Length == 1)
                    {
                        if (File.Exists(args[0]))
                        {
                            var opts = new Options.Extract
                            {
                                ParArchivePath = args[0],
                                OutputDirectory = string.Concat(args[0], ".unpack"),
                                Recursive = false,
                            };

                            Extract(opts);
                            return;
                        }

                        if (Directory.Exists(args[0]))
                        {
                            var opts = new Options.Create
                            {
                                InputDirectory = args[0],
                                ParArchivePath =
                                    args[0].EndsWith(".unpack", StringComparison.InvariantCultureIgnoreCase)
                                        ? args[0].Substring(0, args[0].Length - 7)
                                        : string.Concat(args[0], ".par"),
                                AlternativeMode = false,
                                Compression = 1,
                            };

                            Create(opts);
                            return;
                        }
                    }

                    var helpText = HelpText.AutoBuild(
                        parserResult,
                        h =>
                        {
                            h.AutoHelp = false; // hide --help
                            h.AutoVersion = false; // hide --version
                            return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                        },
                        e => e);

                    Console.WriteLine(helpText);
                });
        }

        private static void WriteHeader()
        {
            Console.WriteLine(CommandLine.Text.HeadingInfo.Default);
            Console.WriteLine(CommandLine.Text.CopyrightInfo.Default);
            Console.WriteLine();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute(
            "Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Ownserhip dispose transferred")]
        private static Node ReadDirectory(string dirPath, string nodeName = "", IEnumerable<string> files = null, IEnumerable<string> directories = null)
        {
            dirPath = Path.GetFullPath(dirPath);

            if (string.IsNullOrEmpty(nodeName))
            {
                nodeName = Path.GetFileName(dirPath);
            }

            Node container = Yarhl.FileSystem.NodeFactory.CreateContainer(nodeName);
            var directoryInfo = new DirectoryInfo(dirPath);
            container.Tags["DirectoryInfo"] = directoryInfo;

            // files and folders that were received as arguments contain names created with
            // Path.GetFileName(), so they need to be combined with dirPath.
            // If the arguments were null, then using FileInfo/Directoryinfo and getting the FullName
            // will return a rooted path, which will then *not* get combined with dirPath.
            if (files is null)
            {
                files = new List<string>(directoryInfo.GetFiles().Select(f => f.FullName));
            }

            foreach (string file in files)
            {
                Node fileNode = Yarhl.FileSystem.NodeFactory.FromFile(Path.Combine(dirPath, file), Yarhl.IO.FileOpenMode.Read);
                container.Add(fileNode);
            }

            if (directories is null)
            {
                directories = new List<string>(directoryInfo.GetDirectories().Select(f => f.FullName));
            }

            foreach (string directory in directories)
            {
                Node directoryNode = ReadDirectory(Path.Combine(dirPath, directory));
                container.Add(directoryNode);
            }

            return container;
        }

        /// <summary>
        /// Semi-equivalent to Path.Join but for .NET standard 2.0
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        private static string PathJoin(string path1, string path2)
        {
            if (path2.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path2 = path2.Substring(1);
            }

            return Path.Combine(path1, path2);
        }
    }
}
