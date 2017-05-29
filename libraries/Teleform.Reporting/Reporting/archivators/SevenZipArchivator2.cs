using System;
using System.Collections.Generic;
using Trace = System.Diagnostics.Trace;
using Process = System.Diagnostics.Process;
using Teleform.Reporting;

namespace Teleform.IO.Compression
{
    // Add a | Benchmark b | Delete d | Extract e | ExtractFullPath x | List l | Test t | Update u

    public class SevenZipArchivator : Archivator
    {
        public override void Create(string folder, string archive)
        {
            using (var process = new Process())
            {

                process.StartInfo.FileName = Storage.archivatorPath;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Arguments = string.Format("a \"{0}\" \"{1}\\*\"", archive, folder);

                Trace.WriteLine(string.Format("{0} {1}",
                    process.StartInfo.FileName,
                    process.StartInfo.Arguments));

                process.Start();
                process.WaitForExit();
            }
        }
    }
}

