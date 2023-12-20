using Konsole;

namespace CommentRemover;

public class Program
{
    public static async Task Main(string[] args)
    {
        IConsole console = new Writer();
        var dataReader = new DataReader(console);
        foreach (string filename in Directory.GetFiles(".", "*.parquet"))
        {
            await dataReader.ProcessFileAsync(filename);
        }

    }
}
