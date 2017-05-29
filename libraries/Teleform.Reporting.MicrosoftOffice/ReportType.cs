using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting
{
    using MicrosoftOffice;
    using Teleform.IO.Compression;

    public enum ReportType
    {
        [Builder(typeof(WordReportBuilder))]
        [ContentType("application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx")]
        [Parameters(null)]
        Word,
        [ContentType("application/x-zip-compressed", ".zip")]
        [Builder(typeof(ArchiveReportBuilder))]
        [Parameters(typeof(WordReportBuilder), typeof(SevenZipArchivator), "~/app_data/zip/")]
        ZipWord,

        [ContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx")]
        [Builder(typeof(BaseExcelBuilder))]
        [Parameters(null)]
        Excel,

        [ContentType("application/octet-stream", ".xlsx")]
        [Builder(typeof(BaseExcelBuilder))]
        [Parameters(null)]
        Table,

        [ContentType("text/csv", ".csv")]
        [Builder(typeof(CsvReportBuilder))]
        [Parameters(null)]
        Csv,

        ZipExcel
    }
}
