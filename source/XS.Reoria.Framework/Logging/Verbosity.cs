using System.ComponentModel;

namespace XS.Reoria.Framework.Logging
{
    public enum Verbosity : ulong {
        [Description("None")]
        None = 0x0,
        
        [Description("Error")]
        Error = 0x1,
        
        [Description("Warning")]
        Warning = 0x3,
        
        [Description("Info")]
        Info = 0x7,
        
        [Description("Debug")]
        Debug = 0xF,
        
        [Description("Verbose")]
        Verbose = 0x1F,
        
        [Description("All")]
        All = 0xFFFFFFFFFFFFFFFFul,

        /* Aliases */
        [Description("Info")]
        Default = Info,
        
        [Description("Error")]
        Production = Error,
        
        [Description("Verbose")]
        Developer = Verbose
    }
}
