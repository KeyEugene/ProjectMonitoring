using System;
using System.Collections.Generic;
using System.Diagnostics;
using Teleform.Reporting;

namespace Teleform.IO.Compression
{
    internal class SevenZipArchivator2 : Archivator
    {
        public enum Command
        {
            Add = 1,
            //Benchmark = 2,
            //Delete = 4,
            //Extract = 8,
            //List = 16,
            //Test = 32,
            //Update = 64,
            //ExtractFullPath = 128
        };
        #region Fields
        public static Dictionary<Command, string> _commands;
        private static string sevenZip = Storage.archivatorPath;
        private static ProcessStartInfo processZip = new ProcessStartInfo();
        #endregion

        #region Properties
        public static Dictionary<Command, string> Commands
        {
            get
            {
                return _commands;
            }
        }
        #endregion

        static SevenZipArchivator2()
        {
            //  initializing Commands dictionary
            if (_commands == null)
            {
                _commands = new Dictionary<Command, string>();

                _commands.Add(Command.Add, @"a");
                //_commands.Add(Command.Benchmark, @"b");
                //_commands.Add(Command.Delete, @"d");
                //_commands.Add(Command.Extract, @"e");
                //_commands.Add(Command.ExtractFullPath, @"x");
                //_commands.Add(Command.List, @"l");
                //_commands.Add(Command.Test, @"t");
                //_commands.Add(Command.Update, @"u");
            }
            processZip.FileName = sevenZip;
        }

        public override void Create(string folder, string archive)
        {
            //processZip.UseShellExecute = false;
            processZip.Arguments = string.Format("{0} \"{1}\" \"{2}\\*\"", Commands[Command.Add], archive, folder);

            //throw new NotSupportedException(string.Format("{0} {1}", processZip.FileName, processZip.Arguments));

            //System.Diagnostics.Trace.WriteLine(string.Format("{0} {1}", processZip.FileName, processZip.Arguments));

            var process = Process.Start(processZip);

            process.CloseMainWindow();
            process.WaitForExit();
        }

    }
}

