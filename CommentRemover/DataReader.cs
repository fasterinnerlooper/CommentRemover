using Konsole;
using Parquet;
using Parquet.Rows;
using System.Collections.Concurrent;
using System.Text.Encodings.Web;

namespace CommentRemover
{
    public class DataReader(IConsole console)
    {
        public IConsole Console { get; } = console;

        public async Task ProcessFileAsync(string filename)
        {
            var tempFile = Path.GetRandomFileName();

            using (var filestream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.Write))
            using (var reader = await ParquetReader.CreateAsync(filestream, new ParquetOptions { TreatByteArrayAsString = true }))
            using (var tempStream = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (var tempWriter = await ParquetWriter.CreateAsync(reader.Schema, tempStream))
            {
                this.Console.WriteLine($"Reading file {filename}");
                var tasks = new List<Task<Table>>();
                var progressBars = new ConcurrentBag<IProgressBar>();
                var fileProgressBar = new ProgressBar(this.Console, reader.RowGroupCount - 1);
                for (int i = 0; i < reader.RowGroupCount; i++)
                {
                    fileProgressBar.Refresh(i, $"Reading row group {i + 1}");
                    var table = await reader.ReadAsTableAsync(rowGroupIndex: i);
                    tasks.Add(Task.Factory.StartNew((obj) =>
                                        {
                                            var taskData = obj as MyTaskData;
                                            var table = taskData.Table;
                                            var i = taskData.Index;
                                            var bar = new ProgressBar(this.Console, table.Count);
                                            progressBars.Add(bar);
                                            foreach (var row in table)
                                            {
                                                bar.Refresh(table.IndexOf(row) + 1, $"Processing row {table.IndexOf(row) + 1} of {table.Count}");
                                                row[0] = HtmlEncoder.Default.Encode(NetCodeParser.RemoveComments(row.GetString(0)));
                                                row[1] = HtmlEncoder.Default.Encode(NetCodeParser.RemoveComments(row.GetString(1)));
                                            }
                                            return table;
                                        }, new MyTaskData { Table = table, Index = i }));

                }
                var result = await Task.WhenAll(tasks);
                foreach (var table in result)
                {
                    using var groupwriter = tempWriter.CreateRowGroup();
                    await groupwriter.WriteAsync(table);
                }
            }
            File.Move(tempFile, filename, true);
            this.Console.WriteLine("Successfully updated parquet file");
        }
    }
    public class MyTaskData
    {
        public required Table Table { get; set; }
        public int Index { get; set; }
    }
}
