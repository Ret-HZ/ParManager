// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace ParTool.Options
{
    using System.Collections.Generic;
    using CommandLine;

    /// <summary>
    /// PAR archive extract options.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Class is passed as type parameter.")]
    [Verb("create", HelpText = "Create a Yakuza PAR archive.")]
    internal class Create
    {
        /// <summary>
        /// Gets or sets the input directory.
        /// </summary>
        [Value(0, MetaName = "input", Required = true, HelpText = "Input directory.")]
        public string InputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the PAR archive path.
        /// </summary>
        [Value(1, MetaName = "archive", Required = true, HelpText = "New Yakuza PAR archive path.")]
        public string ParArchivePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the output file will be used in a Yakuza Kenzan, Yakuza 3, 4 or 5.
        /// </summary>
        [Option("alternative-mode", Default = false, HelpText = "Enable it if the PAR is for Yakuza 3, 4, 5 or Kenzan.")]
        public bool AlternativeMode { get; set; }

        /// <summary>
        /// Gets or sets the compression algorithm to use.
        /// </summary>
        [Option('c', "compression", Default = 0x01, HelpText = "SLLZ algorithm.")]
        public int Compression { get; set; }

        /// <summary>
        /// Gets or sets the files to read from the input directory.
        /// </summary>
        [Option("files", HelpText = "Files inside the root of the input directory to be processed.")]
        public IEnumerable<string> InputFiles { get; set; }

        /// <summary>
        /// Gets or sets the folders to read from the input directory.
        /// </summary>
        [Option("folders", HelpText = "Folders inside the root of the input directory to be processed.")]
        public IEnumerable<string> InputFolders { get; set; }
    }
}