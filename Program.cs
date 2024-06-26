﻿using System;
using System.IO;
using static System.Console;

namespace DataProcessor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Parsing command line options");

            var command = args[0];

            if (command == "--file")
            {
                var filePath = args[1];

                // Check if path is fully qualified
                if (!Path.IsPathFullyQualified(filePath))
                {
                    WriteLine($"ERROR: Path '{filePath}' must be fully qualified");
                    ReadLine();
                    return;
                }
                WriteLine($"Single file {filePath} selected");
                ProcessSingleFile(filePath);
            }
            else if (command == "--dir")
            {
                var directoryPath = args[1];
                var fileType = args[2];
                WriteLine($"Directory {directoryPath} selected for {fileType} files");
                ProcessDirectory(directoryPath, fileType);
            }
            else
            {
                WriteLine("Invalid command line options");
            }
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
