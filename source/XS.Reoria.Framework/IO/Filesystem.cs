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
            tmp = tmp.Replace(@"\", this.SeparatorChar);
            tmp = tmp.Replace("/", this.SeparatorChar);

            // Temporarily remove the root path if it is in the path.
            tmp = (!isroot) ? tmp.Replace(this.Root, string.Empty) : tmp;

            // Find out if we are handling a directory or file path.
            bool isdir = (tmp.Substring(tmp.Length - 1, 1) == this.SeparatorChar);

            // Are we cleaning a directory or file path?
            if (isdir)
            {
                // Directory path is simple, just loop through and replace the invalid characters.
                foreach (char c in Path.GetInvalidPathChars())
                {
                    tmp = tmp.Replace(c.ToString(), string.Empty);
                }
            }
            else
            {
                // File path is not simple, start by separating the file and directory paths.
                string dirpath = tmp.Substring(0, tmp.LastIndexOf(this.SeparatorChar) + 1);
                string filepath = tmp.Substring(tmp.LastIndexOf(this.SeparatorChar) + 1);

                // Go and clean the directory path.
                dirpath = this.CleansePath(dirpath, true);

                // Loop through and replace the invalid file path characters.
                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    filepath = filepath.Replace(c.ToString(), string.Empty);
                }

                // Now put the path back together.
                tmp = dirpath + filepath;
            }

            // Are we cleansing the root path, if not append the root path.
            tmp = (!isroot) ? this.Root + tmp : tmp;

            // Return the new path.
            return tmp;
        }
    }
}