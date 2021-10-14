using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FindDuplicates
{
    class Program
    {
        static void Main(string[] args)
        {
            //string root = @"C:\Users\Sergio\Source\Repos\FindDuplicates"; //I used a smaller root to test the program, since the whole drive has a lot of files
            string root = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            string searchPattern = "*";

            List<FileInfo> allFiles = GetAllFiles(root, searchPattern);
            List<FileInfo> filesToCompare = RemoveUniqueFiles_Hash(allFiles);
            List<FileInfo> alreadyCompared = new List<FileInfo>();
            List<DuplicatedFile> duplicatedFiles = new List<DuplicatedFile>();

            foreach (FileInfo file in filesToCompare)
            {
                if (alreadyCompared.Contains(file))
                {
                    continue;
                }

                CompareFilesWithSameLength(file, filesToCompare.Where(f => f.Length == file.Length && f != file && !alreadyCompared.Contains(f)).ToList(), duplicatedFiles, alreadyCompared);
            }

            PrintDuplicatedFilesInConsole(duplicatedFiles);
        }

        /// <summary>This method gets all the files from a certain root and its subfolders following a search pattern.</summary>
        /// <param name="root">the main root.</param>
        /// <param name="searchPattern">the search pattern used.</param>
        /// <returns>A list of FileInfo with all the information from the files.</returns>
        public static List<FileInfo> GetAllFiles(string root, string searchPattern)
        {
            List<FileInfo> fileList = new List<FileInfo>();
            Stack<string> pending = new Stack<string>();
            
            pending.Push(root);
            while (pending.Count != 0)
            {
                var path = pending.Pop();
                string[] next = null;
                try
                {
                    next = Directory.GetFiles(path, searchPattern);
                }
                catch { }

                if (next != null && next.Length != 0)
                {
                    foreach (var file in next)
                    {
                        fileList.Add(new FileInfo(file));
                    }
                }
                    
                try
                {
                    next = Directory.GetDirectories(path);
                    foreach (var subdir in next)
                    {
                        pending.Push(subdir);
                    }
                }
                catch { }
            }

            return fileList;
        }

        /// <summary>This method removes the unique files from the list. It assumes that if a file has a unique lenght, it is unique.</summary>
        /// <param name="fileList">the file list to reduce.</param>
        /// <returns>A new list of FileInfo with all the files to compare.</returns>
        public static List<FileInfo> RemoveUniqueFiles_Hash(List<FileInfo> fileList)
        {
            List<long> lengthsToRemove = new List<long>();
            Hashtable HLengths = new Hashtable();

            foreach (FileInfo fileInfo in fileList)
            {
                try
                {
                    if (!HLengths.Contains(fileInfo.Length))
                        HLengths.Add(fileInfo.Length, 1);
                    else
                        HLengths[fileInfo.Length] = (int)HLengths[fileInfo.Length] + 1;
                }
                catch { }
            }

            foreach (DictionaryEntry hash in HLengths)
            {
                if ((int)hash.Value == 1)
                {
                    lengthsToRemove.Add((long)hash.Key);
                }
            }

            List<FileInfo> newFileList = new List<FileInfo>();
            foreach (FileInfo fileInfo in fileList)
            {
                try
                {
                    if (!lengthsToRemove.Contains(fileInfo.Length))
                    {
                        newFileList.Add(fileInfo);
                    }
                }
                catch { }
            }

            return newFileList;
        }

        /// <summary>This method compares a file with a list of files to see if any of them has the same content.</summary>
        /// <param name="fileToCompare">the file to compare.</param>
        /// <param name="fileList">the file list to compare with.</param>
        /// <param name="duplicatedFiles">the list of duplicated files and its originals.</param>
        /// <param name="alreadyCompared">the list of already compared files.</param>
        public static void CompareFilesWithSameLength(FileInfo fileToCompare, List<FileInfo> fileList, List<DuplicatedFile> duplicatedFiles, List<FileInfo> alreadyCompared) 
        {
            alreadyCompared.Add(fileToCompare);

            foreach (FileInfo file in fileList)
            {
                try
                {
                    if (AreFileContentsEqual(fileToCompare.FullName, file.FullName))
                    {
                        alreadyCompared.Add(file);

                        string originalPath;
                        string duplicatedPath;

                        if (file.CreationTime < fileToCompare.CreationTime)
                        {
                            originalPath = file.FullName;
                            duplicatedPath = fileToCompare.FullName;
                        }
                        else
                        {
                            originalPath = fileToCompare.FullName;
                            duplicatedPath = file.FullName;
                        }

                        duplicatedFiles.Add(new DuplicatedFile(originalPath, duplicatedPath));
                    }
                } catch { }
            }
        }

        public static bool AreFileContentsEqual(String path1, String path2) =>
          File.ReadAllBytes(path1).SequenceEqual(File.ReadAllBytes(path2));

        public static void PrintDuplicatedFilesInConsole(List<DuplicatedFile> duplicatedFiles)
        {
            foreach (DuplicatedFile dup in duplicatedFiles)
            {
                dup.PrintInConsole();
            }
        }

        #region DiscardedMethods
        public static List<FileInfo> RemoveUniqueFiles_Linq(List<FileInfo> fileList)
        {
            fileList.RemoveAll(f => !fileList.Where(l => l.FullName != f.FullName).Select(l => l.Length).ToList().Contains(f.Length));
            return fileList;
        }

        public static void GetFiles()
        {
            Directory.GetFiles(@"C:\", "*", SearchOption.AllDirectories);
        }
        #endregion
    }
}
