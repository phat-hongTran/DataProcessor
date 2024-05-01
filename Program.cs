using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using static System.Console;

namespace DataProcessor
{
    internal class Program
    {
        private static ConcurrentDictionary<string,string> FileToProcess = new ConcurrentDictionary<string, string>();
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
                using var timer = new Timer(state => ProcessFiles(), null, 0, 1000);

                inputFileWatcher.IncludeSubdirectories = false;
                inputFileWatcher.InternalBufferSize = 32768; // 32 KB
                inputFileWatcher.Filter = "*.*";    // this is default
                inputFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

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

            FileToProcess.TryAdd(e.FullPath,e.FullPath);
        }

        private static void FileCreated(object sender, FileSystemEventArgs e)
        {
            WriteLine($"File created: {e.Name} - type: {e.ChangeType}");

            FileToProcess.TryAdd(e.FullPath, e.FullPath);
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

        private static void ProcessFiles()
        {
            foreach (var file in FileToProcess.Keys)
            {
                if (FileToProcess.TryRemove(file, out _))
                {
                    var fileProcessor = new FileProcessor(file);
                    fileProcessor.Process();
                }
            }
        }   
    }
}
