using CommentRemover;
using Parquet.Schema;
using Parquet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parquet.Data;
using Moq;
using Konsole;

namespace CommentRemoverTests
{
    public class DataReaderTests : IAsyncLifetime
    {
        readonly string filename = Path.GetRandomFileName();
        private static readonly string[] data = [@"using System;
using System.Collections;
using System.Linq;
using System.Text;

/*
 * Summary
 */
namespace HelloWorld
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        // Single line comment
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!""); //This is a comment
        }
    }
}", "namespace DanTup.DartVS.ProjectSystem.Controls { using System; using System.ComponentModel; using System.Drawing; using System.IO; using System.Windows.Forms; using Microsoft.VisualStudio.Shell.Interop; using Package = Microsoft.VisualStudio.Shell.Package; using Url = Microsoft.VisualStudio.Shell.Url; /// <summary> /// Extends a simple text box specialized for browsing to folders. Supports auto-complete and /// a browse button that brings up the folder browse dialog. /// </summary> internal partial class FolderBrowserTextBox : UserControl { private string _rootFolder; // ========================================================================================= // Constructors // ========================================================================================= /// <summary> /// Initializes a new instance of the <see cref="];

        public Task DisposeAsync()
        {
            File.Delete(filename);
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            // create file schema
            var schema = new ParquetSchema(
                new DataField<string>("context"),
                new DataField<string>("gt"));

            //create data columns with schema metadata and the data you need
            var idColumn = new DataColumn(
               schema.DataFields[0],
               data);

            var cityColumn = new DataColumn(
               schema.DataFields[1],
               new string[] { string.Empty, string.Empty });

            using Stream fileStream = File.OpenWrite(filename);
            using ParquetWriter parquetWriter = await ParquetWriter.CreateAsync(schema, fileStream);
            for (int i = 0; i < 2; i++)
            {
                using ParquetRowGroupWriter groupWriter = parquetWriter.CreateRowGroup();
                await groupWriter.WriteColumnAsync(idColumn);
                await groupWriter.WriteColumnAsync(cityColumn);
            }
        }

        [Fact]
        public async Task ShouldUpdateFileSuccessfully()
        {
            var sut = new DataReader(new MockConsole());
            await sut.ProcessFileAsync(filename);
        }
    }
}
