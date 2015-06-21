using System.ComponentModel;

namespace XS.Reoria.Framework.Logging
{
    public static class VerbosityExtension {
        public static string GetDescription(this Verbosity verbosity) {
            var fi = verbosity.GetType().GetField(verbosity.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) {
                return attributes[0].Description;
            } else {
                return verbosity.ToString();
            }
        }
    }
}
