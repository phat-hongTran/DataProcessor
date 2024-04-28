using System;
using System.IO;
using static System.Console;

namespace DataProcessor
{
    class FileProcessor
    {
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
        }
    }
}
