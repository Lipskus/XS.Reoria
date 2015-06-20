using System;

namespace XS.Reoria.Framework.IO
{
    public class ExtractedDirectoryPath : IDisposable
    {
        public string Directory { get; set; }
        public string File { get; set; }

        public string FullPath
        {
            get
            {
                // Combine and return the two paths.
                return this.Directory + this.File;
            }
        }

        public ExtractedDirectoryPath()
        {
            // Initialize the variables.
            this.Directory = string.Empty;
            this.File = string.Empty;
        }

        public void Dispose()
        {
            // Dispose of the variables.
            this.Directory = null;
            this.File = null;
        }
    }
}