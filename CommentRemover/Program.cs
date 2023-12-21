using Konsole;

namespace CommentRemover;

public class Program
{
    public static async Task Main(string[] args)
    {
        IConsole console = new Writer();
        var dataReader = new DataReader(console);
        if (args.Length == 1)
        {
            var filename = Convert.ToString(args[0]);
            await dataReader.ProcessFileAsync(filename);
        }
        else
        {
            foreach (string filename in Directory.GetFiles(".", "*.parquet"))
            {
                await dataReader.ProcessFileAsync(filename);
            }
        }


    }
}
