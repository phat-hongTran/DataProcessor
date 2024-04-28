using System;
using System.IO;
using static System.Console;

namespace DataProcessor
{
    class FileProcessor
    {
        private const string BackupDirectoryName = "backup";
        private const string InProgressDirectoryName = "processing";
        private const string CompleteDirectoryName = "complete";
        public string InputFilePath { get; }
        public FileProcessor(string inputFilePath) => InputFilePath = inputFilePath;
        public void Process()
        {
            WriteLine($"Begin process of {InputFilePath}");

            // Check if file exists
            if (!File.Exists(InputFilePath))
            {
                WriteLine($"ERROR: file {InputFilePath} not found");
                return;
            }

            var rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;
            WriteLine($"Root data path is {rootDirectoryPath}");

            // Check if backup directory exists
            var backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);

            //if (!Directory.Exists(backupDirectoryPath))
            //{
                WriteLine($"Attempting to create backup directory {backupDirectoryPath}");
                Directory.CreateDirectory(backupDirectoryPath);
            //}
        }
    }
}
