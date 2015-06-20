using System.IO;
using System.Windows.Forms;

namespace XS.Reoria.Framework.IO
{
    public class FileSystem
    {
        public string SeparatorChar { get; private set; }
        public string Root { get; private set; }

        public FileSystem()
        {
            this.SeparatorChar = Path.DirectorySeparatorChar.ToString();
            this.Root = this.CleansePath(Application.StartupPath + this.SeparatorChar, true);
        }

        public string CleansePath(string path)
        {
            // Forward to the hidden override.
            return this.CleansePath(path, false);
        }

        private string CleansePath(string path, bool isroot)
        {
            // Create a temporary instance of the input path.
            string tmp = path;

            // Replace / and \ with the appropriate seperator character.
            tmp = tmp.Replace("\\", this.SeparatorChar);
            tmp = tmp.Replace("/", this.SeparatorChar);

            // Are we cleansing the root path, if not append the root path.
            tmp = (!isroot) ? this.Root + tmp : tmp;

            // Return the new path.
            return tmp;
        }
    }
}