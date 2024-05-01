using System;
using System.IO;
using static System.Console;

namespace DataProcessor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Parsing command line options");

            var directoryToWatch = args[0];

            if (!Directory.Exists(directoryToWatch))
            {
                WriteLine($"ERROR: {directoryToWatch} does not exist");
            }
            else
            {
                WriteLine($"Watching directory {directoryToWatch} for changes");

                using var inputFileWatcher = new FileSystemWatcher(directoryToWatch);

                inputFileWatcher.IncludeSubdirectories = false;
                inputFileWatcher.InternalBufferSize = 32768; // 32 KB
                inputFileWatcher.Filter = "*.*";    // this is default
                inputFileWatcher.NotifyFilter = NotifyFilters.LastWrite;

                inputFileWatcher.Created += FileCreated;
                inputFileWatcher.Changed += FileChanged;
                inputFileWatcher.Deleted += FileDeleted;
                inputFileWatcher.Renamed += FileRenamed;
                inputFileWatcher.Error += WatcherError;

                inputFileWatcher.EnableRaisingEvents = true;

                WriteLine("Press enter to exit");
                ReadLine();
            }
        }

        private static void WatcherError(object sender, ErrorEventArgs e)
        {
            WriteLine($"ERROR: file system watching may no longer be active: {e.GetException()}");
        }

        private static void FileRenamed(object sender, RenamedEventArgs e)
        {
            WriteLine($"File renamed: {e.OldName} to {e.Name}");
        }

        private static void FileDeleted(object sender, FileSystemEventArgs e)
        {
            WriteLine($"File deleted: {e.Name} - type: {e.ChangeType}");
        }

        private static void FileChanged(object sender, FileSystemEventArgs e)
        {
            WriteLine($"File changed: {e.Name} - type: {e.ChangeType}");
        }

        private static void FileCreated(object sender, FileSystemEventArgs e)
        {
            WriteLine($"File created: {e.Name} - type: {e.ChangeType}");
        }

        private static void ProcessSingleFile(string filePath)
        {
            var fileProcessor = new FileProcessor(filePath);
            fileProcessor.Process();
        }

        private static void ProcessDirectory(string directoryPath, string fileType)
        {
            switch (fileType)
            {
                case "TEXT":
                    string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
                    foreach (var textFile in textFiles)
                    {
                        var fileProcessor = new FileProcessor(textFile);
                        fileProcessor.Process();
                    }
                    break;
                default:
                    WriteLine($"ERROR: Unsupported file type {fileType}");
                    break;
            }
        }
    }
}
