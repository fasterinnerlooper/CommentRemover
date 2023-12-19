using CommentRemover;
using Konsole;

IConsole console = new Writer();
var dataReader = new DataReader(console);
foreach (string filename in Directory.GetFiles(".", "*.parquet"))
{
    await dataReader.ProcessFileAsync(filename);
}
