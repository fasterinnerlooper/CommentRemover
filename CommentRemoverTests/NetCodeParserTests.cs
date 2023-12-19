using CommentRemover;
using System.Text.Json;

namespace CommentRemoverTests
{
    public class NetCodeParserTests
    {
        [Fact]
        public void ShouldRemoveCommentWhenGivenCodeAndComment()
        {
            // Arrange
            var actual = @"using System;
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
}";
            var expected = @"using System;using System.Collections;using System.Linq;using System.Text;namespace HelloWorld{    class Program    {        static void Main(string[] args)        {            Console.WriteLine(""Hello, World!"");        }    }}";
            // Act
            var ret = NetCodeParser.RemoveComments(actual); // This is a comment

            // Assert
            Assert.Equal(expected, ret);
        }

        [Fact]
        public void ShouldRemoveCommentsFromHuggingfaceDataset()
        {
            // Arrange
            var actual = @"namespace DanTup.DartVS.ProjectSystem.Controls { using System; using System.ComponentModel; using System.Drawing; using System.IO; using System.Windows.Forms; using Microsoft.VisualStudio.Shell.Interop; using Package = Microsoft.VisualStudio.Shell.Package; using Url = Microsoft.VisualStudio.Shell.Url; /// <summary> /// Extends a simple text box specialized for browsing to folders. Supports auto-complete and /// a browse button that brings up the folder browse dialog. /// </summary> internal partial class FolderBrowserTextBox : UserControl { private string _rootFolder; // ========================================================================================= // Constructors // ========================================================================================= /// <summary> /// Initializes a new instance of the <see cref=""FolderBrowserTextBox""/> class. /// </summary> public FolderBrowserTextBox() { this.InitializeComponent(); folderTextBox.Enabled = Enabled; browseButton.Enabled = Enabled; } // ========================================================================================= // Events // ========================================================================================= /// <summary> /// Occurs when the text has changed. /// </summary> [Browsable(true)] [EditorBrowsable(EditorBrowsableState.Always)] public new event EventHandler TextChanged { add { base.TextChanged += value; } remove { base.TextChanged -= value; } } [Bindable(true)] [Browsable(true)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] [EditorBrowsable(EditorBrowsableState.Always)] public string RootFolder { get { try { if (!string.IsNullOrEmpty(_rootFolder)) Path.IsPathRooted(_rootFolder); } catch (ArgumentException) { return string.Empty; } return _rootFolder; } set { _rootFolder = value; } } [Bindable(true)] [Browsable(true)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] [EditorBrowsable(EditorBrowsableState.Always)] [DefaultValue(false)] [Description(""When this property is 'true', the folder path will be made relative to RootFolder, when possible."")] public bool MakeRelative { get; set; } // ========================================================================================= // Properties // ========================================================================================= /// <summary> /// Gets or sets the path of the selected folder. /// </summary> [Bindable(true)] [Browsable(true)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] [EditorBrowsable(EditorBrowsableState.Always)] public override string Text { get { return this.folderTextBox.Text; } set { this.folderTextBox.Text = value; } } private string FullPath { get { try { string text = Text ?? string.Empty; if (!string.IsNullOrEmpty(RootFolder)) text = Path.Combine(RootFolder, text); return Path.GetFullPath(text); } catch (ArgumentException) { return string.Empty; } } } // ========================================================================================= // Methods // ========================================================================================= /// <summary> /// Sets the bounds of the control. In this case, we fix the height to the text box's height. /// </summary> /// <param name=""x"">The new x value.</param> /// <param name=""y"">The new y value.</param> /// <param name=""width"">The new width value.</param> /// <param name=""height"">The height value.</param> /// <param name=""specified"">A set of flags indicating which bounds to set.</param> protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) { if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height) { height = this.folderTextBox.Height + 1; } base.SetBoundsCore(x, y, width, height, specified); } /// <summary> /// Brings up the browse folder dialog. /// </summary> /// <param name=""sender"">The browse button.</param> /// <param name=""e"">The <see cref=""EventArgs""/> object that contains the event data.</param> private void OnBrowseButtonClick(object sender, EventArgs e) { // initialize the dialog to the current directory (if it exists) bool overridePersistedInitialDirectory = false; string initialDirectory = null; if (Directory.Exists(FullPath)) { initialDirectory = FullPath; overridePersistedInitialDirectory = true; } IntPtr parentWindow = Handle; Guid persistenceSlot = typeof(FileBrowserTextBox).GUID; IVsUIShell2 shell = (IVsUIShell2)Package.GetGlobalService(typeof(SVsUIShell)); // show the dialog string path = shell.GetDirectoryViaBrowseDialog(parentWindow, persistenceSlot, ""Select folder"", initialDirectory, overridePersistedInitialDirectory); if (path != null) { if (MakeRelative && !string.IsNullOrEmpty(RootFolder)) { string rootFolder = Path.GetFullPath(RootFolder); if (Directory.Exists(rootFolder)) { if (!rootFolder.EndsWith(Path.DirectorySeparatorChar.ToString()) && !rootFolder.EndsWith(Path.AltDirectorySeparatorChar.ToString())) rootFolder = rootFolder + Path.DirectorySeparatorChar; path = new Url(rootFolder).MakeRelative(new Url(path)); } } this.folderTextBox.Text = path; } } /// <summary> /// Raises the <see cref=""TextChanged""/> event. /// </summary> /// <param name=""sender"">The folder text box.</param> /// <param name=""e"">The <see cref=""EventArgs""/> object that contains the event data.</param> private void OnFolderTextBoxTextChanged(object sender, EventArgs e) { UpdateColor(); this.OnTextChanged(EventArgs.Empty); } protected override void OnEnabledChanged(EventArgs e) { folderTextBox.Enabled = Enabled; browseButton.Enabled = Enabled; UpdateColor(); browseButton.Invalidate(); base.OnEnabledChanged(e); } private void UpdateColor() { if (!Enabled) { folderTextBox.BackColor = SystemColors.Control; folderTextBox.ForeColor = SystemColors.GrayText; return; } folderTextBox.ForeColor = SystemColors.ControlText; folderTextBox.BackColor = Directory.Exists(FullPath) ? SystemColors.ControlLightLight : Color.LightSalmon; } } }";

            var expected = @"namespace DanTup.DartVS.ProjectSystem.Controls{    using System;    using System.ComponentModel;    using System.Drawing;    using System.IO;    using System.Windows.Forms;    using Microsoft.VisualStudio.Shell.Interop;    using Package = Microsoft.VisualStudio.Shell.Package;    using Url = Microsoft.VisualStudio.Shell.Url;    internal partial class FolderBrowserTextBox : UserControl    {        private string _rootFolder;        public FolderBrowserTextBox()        {            this.InitializeComponent();            folderTextBox.Enabled = Enabled;            browseButton.Enabled = Enabled;        }        [Browsable(true)]        [EditorBrowsable(EditorBrowsableState.Always)]        public new event EventHandler TextChanged        {            add            {                base.TextChanged += value;            }            remove            {                base.TextChanged -= value;            }        }        [Bindable(true)]        [Browsable(true)]        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]        [EditorBrowsable(EditorBrowsableState.Always)]        public string RootFolder        {            get            {                try                {                    if (!string.IsNullOrEmpty(_rootFolder))                        Path.IsPathRooted(_rootFolder);                }                catch (ArgumentException)                {                    return string.Empty;                }                return _rootFolder;            }            set            {                _rootFolder = value;            }        }        [Bindable(true)]        [Browsable(true)]        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]        [EditorBrowsable(EditorBrowsableState.Always)]        [DefaultValue(false)]        [Description(""When this property is 'true', the folder path will be made relative to RootFolder, when possible."")]        public bool MakeRelative { get; set; }        [Bindable(true)]        [Browsable(true)]        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]        [EditorBrowsable(EditorBrowsableState.Always)]        public override string Text        {            get            {                return this.folderTextBox.Text;            }            set            {                this.folderTextBox.Text = value;            }        }        private string FullPath        {            get            {                try                {                    string text = Text ?? string.Empty;                    if (!string.IsNullOrEmpty(RootFolder))                        text = Path.Combine(RootFolder, text);                    return Path.GetFullPath(text);                }                catch (ArgumentException)                {                    return string.Empty;                }            }        }        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)        {            if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height)            {                height = this.folderTextBox.Height + 1;            }            base.SetBoundsCore(x, y, width, height, specified);        }        private void OnBrowseButtonClick(object sender, EventArgs e)        {            string initialDirectory = null;            if (Directory.Exists(FullPath))            {                initialDirectory = FullPath;                overridePersistedInitialDirectory = true;            }            IntPtr parentWindow = Handle;            Guid persistenceSlot = typeof(FileBrowserTextBox).GUID;            IVsUIShell2 shell = (IVsUIShell2)Package.GetGlobalService(typeof(SVsUIShell));            if (path != null)            {                if (MakeRelative && !string.IsNullOrEmpty(RootFolder))                {                    string rootFolder = Path.GetFullPath(RootFolder);                    if (Directory.Exists(rootFolder))                    {                        if (!rootFolder.EndsWith(Path.DirectorySeparatorChar.ToString()) && !rootFolder.EndsWith(Path.AltDirectorySeparatorChar.ToString()))                            rootFolder = rootFolder + Path.DirectorySeparatorChar;                        path = new Url(rootFolder).MakeRelative(new Url(path));                    }                }                this.folderTextBox.Text = path;            }        }        private void OnFolderTextBoxTextChanged(object sender, EventArgs e)        {            UpdateColor();            this.OnTextChanged(EventArgs.Empty);        }        protected override void OnEnabledChanged(EventArgs e)        {            folderTextBox.Enabled = Enabled;            browseButton.Enabled = Enabled;            UpdateColor();            browseButton.Invalidate();            base.OnEnabledChanged(e);        }        private void UpdateColor()        {            if (!Enabled)            {                folderTextBox.BackColor = SystemColors.Control;                folderTextBox.ForeColor = SystemColors.GrayText;                return;            }            folderTextBox.ForeColor = SystemColors.ControlText;            folderTextBox.BackColor = Directory.Exists(FullPath) ? SystemColors.ControlLightLight : Color.LightSalmon;        }    }}";

            // Act
            var ret = NetCodeParser.RemoveComments(actual);

            // Assert
            Assert.Equal(expected, ret);
        }

        [Fact]
        public static void ShouldReturnAnEmptyStringIfThereIsNotContentToModify()
        {
            // Arrange

            // Act
            var ret = NetCodeParser.RemoveComments(string.Empty);

            // Assert
            Assert.Empty(ret);
        }
    }
}