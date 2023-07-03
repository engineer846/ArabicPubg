using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Common.Utils
{
    public static class FilesUtil
    {
        public enum ModiftyType { Replace, AddBefore, AddAfter, Remove };
        public enum MatchType { Contains, Exact };

        public class Modification
        {
            public List<string> previousLines = new List<string>();
            public List<string> targetLines = new List<string>();
            public List<string> modifiedLines = new List<string>();
            public ModiftyType modificationType = ModiftyType.AddAfter;
            public bool matchAll = false;

            public Modification(List<string> previousLines, List<string> targetLines, List<string> modifiedLines, ModiftyType modificationType, bool matchAll=false)
            {
                this.previousLines = previousLines;
                this.targetLines = targetLines;
                this.modifiedLines = modifiedLines;
                this.modificationType = modificationType;
                this.matchAll = matchAll;
            }
            public Modification(List<string> targetLines, List<string> modifiedLines, ModiftyType modificationType, bool matchAll=false)
            {
                this.targetLines = targetLines;
                this.modifiedLines = modifiedLines;
                this.modificationType = modificationType;
                this.matchAll = matchAll;
            }
        }

        public static void ModifyFileAtPath(string fullHardDrivePath, List<Modification> mods)
        {
            // read all the lines in the target file
            List<string> lines = new List<string>(File.ReadAllLines(fullHardDrivePath).ToList());

            // modify the contents
            int matched = 0;
            int prevLinesIndex = 0;
            int modNumber = 0;
            int targetLine = 0;
            List<string> tmp = new List<string>();
            while (modNumber < mods.Count)
            {
                tmp.Clear();
                tmp.AddRange(lines);
                for (int i = 0; i < lines.Count; i++)
                {
                    if (matched == mods[modNumber].previousLines.Count) // all previous lines matched, continue
                    {
                        if (mods[modNumber].targetLines[targetLine].Trim() == lines[i].Trim()) // find the target line
                        {
                            // Get spaces to match the targetLine
                            Regex match_spaces = new Regex(@"^\s*"); //match all leading spaces
                            Match space_matches = match_spaces.Match(lines[i]);
                            string spaces = "";
                            if (space_matches.Success)
                            {
                                Group g = space_matches.Groups[0];
                                spaces = g.ToString();
                            }

                            switch (mods[modNumber].modificationType)
                            {
                                case ModiftyType.AddAfter:
                                    for (int j = 0; j < mods[modNumber].modifiedLines.Count; j++)
                                    {
                                        if (tmp[i + 1 + j].Trim() != mods[modNumber].modifiedLines[j])
                                            tmp.Insert(i + 1 + j, $"{spaces}{mods[modNumber].modifiedLines[j]}");
                                    }
                                    break;
                                case ModiftyType.AddBefore:
                                    for (int j = 0; j < mods[modNumber].modifiedLines.Count; j++)
                                    {
                                        if (tmp[i - j].Trim() != mods[modNumber].modifiedLines[j])
                                            tmp.Insert(i - j, $"{spaces}{mods[modNumber].modifiedLines[j]}");
                                    }
                                    break;
                                case ModiftyType.Remove:
                                    tmp[i] = $"{spaces}//{tmp[i].Trim()}";
                                    break;
                                case ModiftyType.Replace:
                                    for (int j = 0; j < mods[modNumber].modifiedLines.Count; j++)
                                    {
                                        if (j == 0)
                                            tmp[i] = $"{spaces}{mods[modNumber].modifiedLines[j]}";
                                        else 
                                            tmp.Insert(i + j, $"{spaces}{mods[modNumber].modifiedLines[j]}");
                                    }
                                    break;
                            }
                            if (!mods[modNumber].matchAll) // if matchAll, loop through entire file (only can have 1 target line at a time)
                            {
                                targetLine++;
                                if (targetLine >= mods[modNumber].targetLines.Count)
                                    break;
                            }
                            else // force to rematch previous lines always before doing the mod logic if matching everything in file.
                            {
                                matched = 0;
                            }
                        }
                    }
                    else // match all previous lines before continuing
                    {
                        if (lines[i].Trim() == mods[modNumber].previousLines[prevLinesIndex].Trim())
                        {
                            matched++;
                        }
                        else
                        {
                            matched = 0;
                        }
                    }
                }
                matched = 0;
                prevLinesIndex = 0;
                modNumber++;
                targetLine = 0;

                lines.Clear();
                lines.AddRange(tmp);
            }

            // write modified contents back into the file, overwriting original contents.
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(fullHardDrivePath, false); // will overwrite the entire file
                foreach (string line in lines)
                {
                    writer.WriteLine(line); // write all lines to the same file (includes modifications)
                }
            }
            finally
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Recursively search for a folder with a particular name. Case in-sensitive.
        /// </summary>
        /// <param name="folderName">Name of the folder to return.</param>
        /// <param name="startingDirectory">The directory to start in, if not supplied will default to your projects root directory.</param>
        /// <param name="use_full_path_name">If you want to match the folderName with a full path (use forward slashes "/")</param>
        /// <returns>The full hard drive path to the directory.</returns>
        public static string FindFolderPath(string folderName, string startingDirectory="", bool use_full_path_name = false, string dataPath = "")
        {
            if (string.IsNullOrEmpty(dataPath))
            {
                dataPath = Application.dataPath;
            }

            // Did not supply a directory start in your projects root path
            if (string.IsNullOrEmpty(startingDirectory))
                startingDirectory = dataPath;

            startingDirectory = startingDirectory.Replace("/", $"{Path.DirectorySeparatorChar}");
            // Get a list of directorys at this startingDirectory location
            List<string> dirs = Directory.GetDirectories(startingDirectory).ToList();

            // Loop through directories and try to find the target one
            string cleaned, dir_name_only, found, unv = "";
            foreach(string dir in dirs)
            {
                unv = dir;
                if (!use_full_path_name)
                    cleaned = unv.Replace(dataPath + Path.DirectorySeparatorChar, ""); //Remove the full path to your project
                else
                    cleaned = unv;

                if (SystemInfo.operatingSystem.Contains("Windows"))
                {
                    dir_name_only = cleaned.Replace(startingDirectory, "").Replace($"{Path.DirectorySeparatorChar}", ""); //Only get the directory name
                }
                else
                {
                    dir_name_only = cleaned.Replace(startingDirectory.Replace(dataPath + Path.DirectorySeparatorChar, ""), "").Replace($"{Path.DirectorySeparatorChar}", "");
                }
                if ((!use_full_path_name && dir_name_only.Trim().ToLower() == folderName.Trim().ToLower()) ||
                    (use_full_path_name && cleaned.Trim().ToLower() == folderName.Trim().ToLower()))
                {
                    // Target directory found
                    return unv;
                }

                // Not the target directory, dig further into this directory
                found = FindFolderPath(folderName, unv, use_full_path_name);
                if (!string.IsNullOrEmpty(found))
                {
                    return found;
                }
            }
            
            // Looked through all directories in your project and didn't find what you're after
            return "";
        }

        /// <summary>
        /// Recursively search for a file with a particular name. Case in-sensitive.
        /// </summary>
        /// <param name="folderName">Name of the file to return.</param>
        /// <param name="startingDirectory">The directory to start in, if not supplied will default to your projects root directory.</param>
        /// <param name="use_full_path_name">If you want to match the fileName with a full path (use forward slashes "/")</param>
        /// <returns>The full hard drive path to the file.</returns>
        public static string FindFilePath(string fileName, string startingDirectory="", bool use_full_path_name=false, string dataPath = "")
        {
            if (string.IsNullOrEmpty(dataPath))
            {
                dataPath = Application.dataPath;
            }
            // Did not supply a directory start in your projects root path
            if (string.IsNullOrEmpty(startingDirectory))
                startingDirectory = dataPath;

            startingDirectory = startingDirectory.Replace("/", $"{Path.DirectorySeparatorChar}");
            //startingDirectory = startingDirectory.Replace("\\", "/");

            // Get the list of files in this directory
            List<string> files = Directory.GetFiles(startingDirectory).ToList();
            string file_name_only, cleaned = "";
            foreach (string filePath in files)
            {
                if (!use_full_path_name)
                    cleaned = filePath.Replace(dataPath + Path.DirectorySeparatorChar, "");//.Replace("\\","/");
                else
                    cleaned = filePath;

                if (SystemInfo.operatingSystem.Contains("Windows"))
                {
                    file_name_only = cleaned.Replace(startingDirectory, "").Replace($"{Path.DirectorySeparatorChar}", "");
                }
                else
                {
                    file_name_only = cleaned.Replace(startingDirectory.Replace(dataPath + Path.DirectorySeparatorChar, ""), "").Replace($"{Path.DirectorySeparatorChar}", "");
                }

                if ((!use_full_path_name && file_name_only.Trim().ToLower() == fileName.Trim().ToLower()) ||
                    (use_full_path_name && cleaned.Trim().ToLower() == fileName.Trim().ToLower()))
                {
                    if (Application.platform == RuntimePlatform.LinuxEditor)
                        return $"{Path.DirectorySeparatorChar}" + filePath;//.Replace("\\","/");
                    else
                        return filePath;//.Replace("\\", "/");
                }
            }

            // File wasn't found, recursively search other directories
            // Get a list of directorys at this startingDirectory location
            List<string> dirs = Directory.GetDirectories(startingDirectory).ToList();
            string found = "";
            foreach (string dir in dirs)
            {
                found = FindFilePath(fileName, dir, use_full_path_name, dataPath);
                if (!string.IsNullOrEmpty(found))
                {
                    return found;
                }
            }

            // Looked through all directories in your project and didn't find what you're after
            return "";
        }

        /// <summary>
        /// This will actually physically modify a file according to what you want it to do.
        /// </summary>
        /// <param name="hardDriveFilePath">The full hard drive path to the file to modify</param>
        /// <param name="matchType">If you want to match the target line with just a contains statement or an exact match</param>
        /// <param name="modType">How do you want to add the "additiveLines"</param>
        /// <param name="targetLine">The line to try and match before adding the "additiveLines"</param>
        /// <param name="additiveLines">The lines you want to add to the file</param>
        /// <param name="matchBeforeLines">The lines in the file that must exist prior to the targetLine for modification take place</param>
        /// <param name="matchAfterLines">The lines that must exist after the targetLine before modifications take place</param>
        public static void ModifyFile(string hardDriveFilePath, MatchType matchType, ModiftyType modType, string targetLine, List<string> additiveLines, List<string> matchBeforeLines, List<string> matchAfterLines)
        {
            StreamWriter writer = null;
            try
            {
                string[] lines = File.ReadAllLines(hardDriveFilePath);
                bool beforeLinesMatched = false;
                if (matchBeforeLines.Count < 1) beforeLinesMatched = true;
                List<string> modifiedFile = new List<string>();

                for(int i = 0; i < lines.Length; i++)
                {
                    if (!beforeLinesMatched) // need to match all of these lines before looking for your target line
                    {
                        if (lines[i].Trim() == matchBeforeLines[0].Trim())
                        {
                            matchBeforeLines.RemoveAt(0);
                        }
                        if (matchBeforeLines.Count < 1)
                        {
                            beforeLinesMatched = true;
                        }
                        modifiedFile.Add(lines[i]);
                    }
                    else if ((lines[i].Trim() == targetLine.Trim() && matchType == MatchType.Exact) || 
                        (lines[i].Trim().Contains(targetLine.Trim()) && matchType == MatchType.Contains)) // line matched
                    {
                        // check if all of the after lines will match
                        bool afterMatched = false;
                        if (matchAfterLines.Count < 1) // no lines to match continue to modification
                        {
                            afterMatched = true;
                        }
                        else // need to match all lines after this first target line match
                        {
                            bool completed = true;
                            foreach(string afterLineMatch in matchAfterLines)
                            {
                                if (lines[i].Trim() != afterLineMatch.Trim())
                                {
                                    completed = false;
                                    break;
                                }
                            }
                            afterMatched = completed;
                        }
                        if (afterMatched) // All after lines matched, perform modification
                        {
                            // Get spaces to match the targetLine
                            Regex match_spaces = new Regex(@"^\s*"); //match all leading spaces
                            Match space_matches = match_spaces.Match(lines[i]);
                            string spaces = "";
                            if (space_matches.Success)
                            {
                                Group g = space_matches.Groups[0];
                                spaces = g.ToString();
                            }

                            // perform modification
                            switch (modType)
                            {
                                case ModiftyType.AddAfter:
                                    modifiedFile.Add(lines[i]);
                                    additiveLines.ForEach(x => modifiedFile.Add($"{spaces}{x}"));
                                    break;
                                case ModiftyType.AddBefore:
                                    additiveLines.ForEach(x => modifiedFile.Add($"{spaces}{x}"));
                                    modifiedFile.Add(lines[i]);
                                    break;
                                case ModiftyType.Remove:
                                    modifiedFile.Add($"//{lines[i]}");
                                    break;
                                case ModiftyType.Replace:
                                    modifiedFile.Add($"//{lines[i]}");
                                    additiveLines.ForEach(x => modifiedFile.Add($"{spaces}{x}"));
                                    break;
                            }
                        }
                    }
                    else // not a before line or matched line, just continue
                    {
                        modifiedFile.Add(lines[i]);
                    }
                }

                writer = new StreamWriter(hardDriveFilePath, false); // will overwrite the entire file
                foreach(string line in modifiedFile)
                {
                    writer.WriteLine(line); // write all lines to the same file (includes modifications)
                }
            }
            catch { }
            finally
            {
                if (writer != null) writer.Close();
            }
        }
    
        /// <summary>
        /// This will comment out an entire file
        /// </summary>
        /// <param name="hardDriveFilePath">The file path to comment out.</param>
        public static void CommentOutFile(string hardDriveFilePath)
        {
            StreamWriter writer = null;
            try
            {
                List<string> lines = new List<string>();
                lines.AddRange(File.ReadAllLines(hardDriveFilePath).ToList());
                if (lines[0].Trim() != "/*")
                    lines.Insert(0, "/*");
                if (lines[lines.Count-1].Trim() != "*/")
                    lines.Add("*/");
                
                writer = new StreamWriter(hardDriveFilePath, false); // will overwrite the entire file
                foreach (string line in lines)
                {
                    writer.WriteLine(line); // write all lines to the same file (includes modifications)
                }
            }
            catch { }
            finally
            {
                writer.Close();
            }
        }

        public static string FlattenDirectories(this string targetString)
        {
            string displayPath;
            if (new DirectoryInfo(targetString).Parent != null)
            {
                if (new DirectoryInfo(targetString).Parent.Parent != null)
                {
                    if (new DirectoryInfo(targetString).Parent.Parent.Parent != null)
                    {
                        displayPath = $"...{Path.DirectorySeparatorChar}" + new DirectoryInfo(targetString).Parent.Parent.Name + $"{Path.DirectorySeparatorChar}" + new DirectoryInfo(targetString).Parent.Name + $"{Path.DirectorySeparatorChar}" + Path.GetFileNameWithoutExtension(targetString);
                    }
                    else
                    {
                        displayPath = new DirectoryInfo(targetString).Parent.Parent.Name + $"{Path.DirectorySeparatorChar}" + new DirectoryInfo(targetString).Parent.Name + $"{Path.DirectorySeparatorChar}" + Path.GetFileNameWithoutExtension(targetString);
                    }
                }
                else
                {
                    displayPath = new DirectoryInfo(targetString).Parent.Name + $"{Path.DirectorySeparatorChar}" + new DirectoryInfo(targetString).Parent.Name + $"{Path.DirectorySeparatorChar}" + Path.GetFileNameWithoutExtension(targetString);
                }
            }
            else
            {
                displayPath = Path.GetFileNameWithoutExtension(targetString);
            }
            return displayPath;
        }
    }
}