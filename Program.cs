// Function to compute the hash of a file

using System.Security.Cryptography;
using System;
using System.Diagnostics.Metrics;

class Program
{
    static void Main(string[] args)
    {
        // defaults
        string dir = Directory.GetCurrentDirectory();
        string bin = string.Empty;
        bool recursive = false;

        // Loop through the arguments
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "-h":
                case "--help":
                    Console.WriteLine("-h for help, -v for version, -d for direcory, -r for recursive file search, -b to specify the duplicate bin");
                    return;

                case "-v":
                case "--version":
                    Console.WriteLine("v1");
                    return;

                case "-d":
                case "--directory":
                    if (i + 1 < args.Length) // Check if there's a value after the option
                    {
                        dir = args[i + 1];
                        i++; // Skip the next argument because it's the value for -o
                    }
                    else
                    {
                        Console.WriteLine("Error: Missing value for -d option.");
                        return;
                    }
                    break;

                case "-b":
                case "--bin":
                    if (i + 1 < args.Length) // Check if there's a value after the option
                    {
                        bin = args[i + 1];
                        i++; // Skip the next argument because it's the value for -b
                    }
                    else
                    {
                        Console.WriteLine("Error: Missing value for -b option.");
                        return;
                    }
                    break;

                case "-r":
                case "--recursive":
                    recursive = true;
                    break;

                default:
                    Console.WriteLine($"Unknown argument: {args[i]}");
                    return;
            }

            doit(dir, recursive, bin);
        }
    }

    static void doit( string dir, bool recursive , string bin )
    {
        Console.WriteLine("Building file index. ");

        var allFiles = Directory.GetFiles(dir, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        Console.WriteLine($"Found {allFiles.Count()}. ");
        Console.WriteLine("Grouping by size. ");

        // Group files by their size first
        var filesBySize = allFiles.GroupBy(file => new FileInfo(file).Length)
                                  .Where(group => group.Count() > 1) // Only interested in groups with more than one file
                                  .ToList();

        Console.WriteLine($"Found {filesBySize.Count()} groups. ");
        Console.WriteLine("Checking hashes. ");

        // Dictionary to store files that have the same hash
        Dictionary<string, List<string>> duplicatesByHash = new Dictionary<string, List<string>>();

        int progress = 0;

        // Compare files within each size group by their hash
        foreach (var fileGroup in filesBySize)
        {
            if (progress++ % 100 == 0) Console.WriteLine($"{progress} out of {filesBySize.Count()}");

            var fileList = fileGroup.ToList();
            foreach (var file in fileList)
            {
                string fileHash = GetFileHash(file);
                if (fileHash != null)
                {
                    if (!duplicatesByHash.ContainsKey(fileHash))
                    {
                        duplicatesByHash[fileHash] = new List<string>();
                    }
                    duplicatesByHash[fileHash].Add(file);
                }
            }
        }

        // Output the duplicate files
        Console.WriteLine("Duplicate files found:");

        foreach (var hashGroup in duplicatesByHash.Where(g => g.Value.Count > 1))
        {
            Console.WriteLine($"Hash: {hashGroup.Key}");

            int counter = 0;

            foreach (var file in hashGroup.Value)
            {
                Console.WriteLine(file);

                if (counter++ == 0) continue; // skips first

                if (bin != string.Empty)
                {
                    var f = new FileInfo(file);
                    f.MoveTo(bin + "\\" + f.Name);
                    Console.WriteLine ("Moved to bin.");
                }
            }

            Console.WriteLine();
        }
    }


    static string GetFileHash(string filePath)
    {
        try
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error hashing file {filePath}: {ex.Message}");
            return null;
        }
    }
}


