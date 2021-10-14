using System;
using System.Collections.Generic;
using System.Text;

namespace FindDuplicates
{
    class DuplicatedFile
    {
        string originalPath;
        string duplicatedPath;

        public DuplicatedFile(string original, string duplicated)
        {
            originalPath = original;
            duplicatedPath = duplicated;
        }

        public void PrintInConsole()
        {
            Console.WriteLine("(original: '" + originalPath + "', duplicate: '" + duplicatedPath + "')");
        }
    }
}
