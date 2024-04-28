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

            // Copy file to backup directory
            var inputFileName = Path.GetFileName(InputFilePath);
            var backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
            WriteLine($"Copying {InputFilePath} to {backupFilePath}");
            File.Copy(InputFilePath, backupFilePath, true);

            // Move to in progress directory
            Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));
            var inProgressFilePath = Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);

            if (File.Exists(inProgressFilePath))
            {
                WriteLine($"ERROR: a file with the name {inProgressFilePath} is already being processed");
                return;
            }
            WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
            File.Move(InputFilePath, inProgressFilePath);

            // Determine type of file
            var fileExtension = Path.GetExtension(InputFilePath);

            switch (fileExtension)
            {
                case ".txt":
                    ProcessTextFile(inProgressFilePath);
                    break;
                default:
                    WriteLine($"{fileExtension} is not a supported file type");
                    break;
            }

            // Move file after processing to complete directory
            var completeDirectoryPath = Path.Combine(rootDirectoryPath, CompleteDirectoryName);
            Directory.CreateDirectory(completeDirectoryPath);
            WriteLine($"Moving {inProgressFilePath} to {completeDirectoryPath}");
            //File.Move(inProgressFilePath, Path.Combine(completeDirectoryPath, inputFileName));

            var completedFileName = $"{Path.GetFileNameWithoutExtension(InputFilePath)}-{Guid.NewGuid()}{fileExtension}";

            //completedFileName = Path.ChangeExtension(completedFileName, ".complete");   

            var completedFilePath = Path.Combine(completeDirectoryPath, completedFileName);

            File.Move(inProgressFilePath, completedFilePath);

            // Delete in progress directory
            var inprogressDirectoryPath = Path.GetDirectoryName(inProgressFilePath);
            Directory.Delete(inprogressDirectoryPath, true);
        }

        private void ProcessTextFile(string inprogressFilePath)
        {
            WriteLine($"Processing text file {inprogressFilePath}");

            // Read in and process
        }
    }
}
