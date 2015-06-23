using System;
using System.IO;
using System.Windows.Forms;
using WinApp = System.Windows.Forms.Application;

namespace XS.Reoria.Framework.IO
{
    public class FileSystem
    {
        public string SeparatorChar { get; private set; }
        public string Root { get; private set; }

        public FileSystem()
        {
            this.SeparatorChar = Path.DirectorySeparatorChar.ToString();
            this.Root = this.CleanPath(WinApp.StartupPath + this.SeparatorChar);
        }

        public string RelativePath(string path)
        {
            // Append the root path to the cleaned relative path.
            return this.Root + this.CleanPath(path);
        }
        
        public string AbsolutePath(string path)
        {
            // Return the cleaned absolute the cleaned path.
            return this.CleanPath(path);
        }

        private string CleanPath(string path)
        {
            // Create a temporary instance of the input path.
            string tmp = path;

            // Replace / and \ with the appropriate seperator character.
            tmp = tmp.Replace("\\", this.SeparatorChar);
            tmp = tmp.Replace(@"\", this.SeparatorChar);
            tmp = tmp.Replace("/", this.SeparatorChar);

            // Are we cleaning a directory or file path?
            if (this.IsDirectoryPath(tmp))
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
                using (ExtractedDirectoryPath p = this.ExtractDirectoryPath(tmp))
                {
                    // Go and clean the directory path.
                    p.Directory = this.CleanPath(p.Directory);

                    // Loop through and replace the invalid file path characters.
                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        p.File = p.File.Replace(c.ToString(), string.Empty);
                    }

                    // Now put the path back together.
                    tmp = p.FullPath;
                }
            }

            return tmp;
        }

        public bool IsDirectoryPath(string path)
        {
            // Check if the last character is the seperator character.
            return (path.Substring(path.Length - 1, 1) == this.SeparatorChar);
        }

        public ExtractedDirectoryPath ExtractDirectoryPath(string path)
        {
            // Create the return value.
            ExtractedDirectoryPath retval = new ExtractedDirectoryPath();

            // Split the strings.
            retval.Directory = path.Substring(0, path.LastIndexOf(this.SeparatorChar) + 1);
            retval.File = path.Substring(path.LastIndexOf(this.SeparatorChar) + 1);

            // Return the extracted paths.
            return retval;
        }
    }
}