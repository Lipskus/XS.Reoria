using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace XS.Reoria.Framework.Logging
{
    public class Logger {
        #region Static
        public static string DefaultTimestampFormat { get { return @"yyyy-MM-dd\/HH\:mm\:ss.fff"; } }
        public static Logger Global { get; private set; }
        
        static Logger() {
#if DEBUG
            Global = new Logger(Console.OpenStandardOutput(), Verbosity.Debug);
#else
            Global = new Logger(Console.OpenStandardOutput());
#endif
        }
        #endregion Static

        #region Fields/Properties
        private string timestampFormat;
        private ConcurrentDictionary<Stream, bool> streams;
        private ConcurrentDictionary<Stream, StreamWriter> writers;
        private ConcurrentDictionary<Stream, string> fileNames;
        private ConcurrentDictionary<string, Stream> fileStreams;
        private Stream fallbackStream;
        private StreamWriter fallbackWriter;

        public Verbosity Verbosity { get; set; }
        public bool VerbosityLabelEnabled { get; set; }
        public bool TimestampEnabled { get; set; }
        public string TimestampFormat {
            get { return timestampFormat; }
            set {
                try {
                    DateTime.UtcNow.ToString(value);
                    timestampFormat = value;
                } catch (Exception exception) {
                    Debug(exception);
                }
            }
        }

        public IEnumerable<Stream> Streams { get { return streams.Keys; } }
        public IEnumerable<string> FileNames { get { return fileNames.Values; } }
        public IEnumerable<Stream> FileStreams { get { return fileStreams.Values; } }
        public Stream Fallback {
            get { return fallbackStream; }
            set {
                if (fallbackWriter != null) {
                    fallbackWriter.Close();
                }

                fallbackStream = value;
                if (fallbackStream != null) {
                    fallbackWriter = new StreamWriter(fallbackStream);
                }
            }
        }
        #endregion

        #region Constructors
        public Logger(Verbosity? verbosity = null) {
            Verbosity = Verbosity.Default;

            if (verbosity.HasValue) {
                Verbosity = verbosity.Value;
            }

            Fallback = Console.OpenStandardError();
            streams = new ConcurrentDictionary<Stream, bool>();
            writers = new ConcurrentDictionary<Stream, StreamWriter>();
            fileStreams = new ConcurrentDictionary<string, Stream>();
            fileNames = new ConcurrentDictionary<Stream, string>();

            TimestampEnabled = true;
            TimestampFormat = DefaultTimestampFormat;
            VerbosityLabelEnabled = true;
        }

        public Logger(string filename, Verbosity? verbosity = null)
            : this(verbosity) {
            if (!Attach(filename)) {
                Debug("Failed to open file '{0}' for logging (+a).", filename);
            }
        }

        public Logger(Stream stream, Verbosity? verbosity = null)
            : this(verbosity) {
                if (!Attach(stream)) {
                    Debug("Failed to attach stream for logging.");
                }
        }
        #endregion

        #region File Attachment/Detachment
        public bool Attach(string filename) {
            if (filename == null || filename.Trim().Length < 1) {
                return false;
            }

            filename = filename.Trim();

            try {
                if (!fileStreams.ContainsKey(filename)) {
                    var stream = File.Open(filename, FileMode.Append);

                    if (Attach(stream)) {
                        if (fileNames.TryAdd(stream, filename)) {
                            return fileStreams.TryAdd(filename, stream);
                        }
                    }
                } else {
                    Debug("This file has already been attached: '{0}'.", filename);
                }
            } catch (Exception exception) {
                Warning(exception);
            }

            return false;
        }

        public bool Detach(string filename) {
            if (filename == null || filename.Trim().Length < 1) {
                return false;
            }

            filename = filename.Trim();

            Stream stream = null;
            if (fileStreams.TryGetValue(filename, out stream)) {
                return Detach(stream);
            }

            return false;
        }
        #endregion File Attachment/Detachment

        #region Stream Attachment/Detachment
        public bool Attach(Stream stream) {
            if (stream == null) {
                return false;
            }

            try {
                if (!streams.ContainsKey(stream)) {
                    if (streams.TryAdd(stream, true)) {
                        return writers.TryAdd(stream, new StreamWriter(stream));
                    }
                } else {
                    Debug("This stream has already been attached.");
                }
            } catch (Exception exception) {
                Warning(exception);
            }

            return false;
        }

        public bool Detach(Stream stream) {
            if (stream == null) {
                return false;
            }

            Stream otherStream = null;

            string name = "";
            if (fileNames.TryRemove(stream, out name)) {
                fileStreams.TryRemove(name, out otherStream);
            }
            
            StreamWriter writer;
            if (writers.TryRemove(stream, out writer)) {
                writer.Close();
            }

            bool exists = false;
            var result = streams.TryRemove(stream, out exists);

            if (otherStream != stream) {
                Debug("The associated stream to '{0}' did not match this stream.", name);
            }

            return result;
        }
        #endregion Stream Attachment/Detachment

        #region Utility
        private bool TryLog(TextWriter writer, string message) {
            try {
                writer.WriteLine(message);
                writer.Flush();
                return true;
            } catch (Exception exception) {
                System.Diagnostics.Debug.WriteLine(exception);
            }

            return false;
        }
        #endregion

        #region Verbosity Logging
        public bool Log(Verbosity verbosity, object obj) {
            return Log(verbosity, Convert.ToString(obj));
        }

        public bool Log(Verbosity verbosity, string message) {
            var result = false;

            if ((verbosity & Verbosity) == verbosity) {
                var timestamp = "";
                if (TimestampEnabled) {
                    timestamp = string.Format("[{0}]", DateTime.UtcNow.ToString(TimestampFormat));
                }

                var verbosityLabel = "";
                if (VerbosityLabelEnabled) {
                    verbosityLabel = string.Format("[{0}]", verbosity.GetDescription());
                }

                var label = timestamp + verbosityLabel;
                if (label.Trim().Length > 0) {
                    label = label.Trim() + " ";
                }

                var formattedMessage = string.Format("{0}{1}", label, message);
                foreach (var writer in writers.Values) {
                    result |= TryLog(writer, formattedMessage);
                }

                if (writers.Values.Count < 1) {
                    TryLog(fallbackWriter, formattedMessage);
                }
            }

            return result;
        }

        public bool Log(Verbosity verbosity, Exception exception) {
            var result = Log(verbosity, "[Exception Message] {0}", exception.Message);
            result &= Debug("[Exception Source] {0}", exception.Source);
            result &= Verbose("[Exception Stack] {0}", exception.StackTrace);

            return result;
        }

        public bool Log(Verbosity verbosity, string format, params object[] args) {
            if ((verbosity & Verbosity) == verbosity) {
                return Log(verbosity, string.Format(format, args));
            }

            return false;
        }
        #endregion Verbosity Logging

        #region Generic Logging
        public bool Log(object obj) {
            return Log(Verbosity.None, obj);
        }

        public bool Log(string message) {
            return Log(Verbosity.None, message);
        }

        public bool Log(Exception exception) {
            return Log(Verbosity.None, exception);
        }

        public bool Log(string format, params object[] args) {
            return Log(Verbosity.None, format, args);
        }

        public bool Verbose(object obj) {
            return Log(Verbosity.Verbose, obj);
        }

        public bool Verbose(string message) {
            return Log(Verbosity.Verbose, message);
        }

        public bool Verbose(Exception exception) {
            return Log(Verbosity.Verbose, exception.Message);
        }

        public bool Verbose(string format, params object[] args) {
            return Log(Verbosity.Verbose, format, args);
        }

        public bool Debug(object obj) {
            return Log(Verbosity.Debug, obj);
        }

        public bool Debug(string message) {
            return Log(Verbosity.Debug, message);
        }

        public bool Debug(Exception exception) {
            return Log(Verbosity.Debug, exception);
        }

        public bool Debug(string format, params object[] args) {
            return Log(Verbosity.Debug, format, args);
        }

        public bool Info(object obj) {
            return Log(Verbosity.Info, obj);
        }

        public bool Info(string message) {
            return Log(Verbosity.Info, message);
        }

        public bool Info(Exception exception) {
            return Log(Verbosity.Info, exception);
        }

        public bool Info(string format, params object[] args) {
            return Log(Verbosity.Info, format, args);
        }

        public bool Warning(object obj) {
            return Log(Verbosity.Warning, obj);
        }

        public bool Warning(string message) {
            return Log(Verbosity.Warning, message);
        }

        public bool Warning(Exception exception) {
            return Log(Verbosity.Warning, exception.Message);
        }

        public bool Warning(string format, params object[] args) {
            return Log(Verbosity.Warning, format, args);
        }

        public bool Error(object obj) {
            return Log(Verbosity.Error, obj);
        }

        public bool Error(string message) {
            return Log(Verbosity.Error, message);
        }

        public bool Error(Exception exception) {
            return Log(Verbosity.Error, exception.Message);
        }

        public bool Error(string format, params object[] args) {
            return Log(Verbosity.Error, format, args);
        }
        #endregion Generic Logging
    }
}