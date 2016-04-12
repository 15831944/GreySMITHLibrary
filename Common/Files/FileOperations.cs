using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GreySMITH.Common.General.Time;

namespace GreySMITH.Common.Files
{
    public static class FileOperations
    {

        /// <summary>
        /// Saves files from a certain folder over to another folder
        /// </summary>
        /// <param name="foldernames">Folder to copy from</param>
        /// <param name="newfoldername">Folder to copy to</param>
        /// <param name="pathtocompareto">path to compare use as an example</param>
        public static void FolderCopy(string pathtocompareto, string newfoldername, string[] locationstosearch, params string[] foldernames)
        {
            string correctdirectory = FolderFinder((Path.GetDirectoryName(pathtocompareto).ToString()), foldernames);

            // note the files from "foldername" into a list so that you can search the network for them
            List<string> filenames = Directory.GetFiles(correctdirectory).ToList<string>();

            // copy from the latest directory and save to "newfoldername" with current date YEAR-MO-DA format
            string folderwdate = Path.Combine(newfoldername, TimeUtility.DateFormatter(DateTime.Now));
            if (!Directory.Exists(folderwdate))
            {
                Directory.CreateDirectory(folderwdate);
            }

            // find the latest files on the network and give the name of the latest location for each of them
            foreach (string file_fullpath in filenames)
            {
                string locationlatest = FindLatest(locationstosearch, file_fullpath);

                try
                {
                    File.Copy(file_fullpath, Path.Combine(folderwdate, Path.GetFileName(file_fullpath)), true);
                }

                catch (Exception e)
                {
                    Console.WriteLine("The following error occured during the program, " + e.StackTrace.ToString());
                }
            }
        }

        /// <summary>
        /// Method finds the appropiate directory amongst a series of possible folders in the path given
        /// </summary>
        /// <param name="pathtostartin">Path that will be searched</param>
        /// <param name="foldernamestosearchfor">Folder the method should search for</param>
        /// <returns>The full path of the folder with foldername in its list</returns>
        public static string FolderFinder(string pathtostartin, params string[] foldernamestosearchfor)
        {
            // variables for amounts of folders which match folder name in the current directory tree
            int foldermatch = 0;
            string pathoffoundfolder = null;
            List<string> dirlist = new List<string>();
            List<string> allfoldersdirectorymustcontain = foldernamestosearchfor.ToList();
            IEnumerable<string> possiblepaths = null;

            // recursively finds the first directory which matches the name
            while (foldermatch < 1)
            {
                // list all possible directories which match the current string
                foreach (string s in foldernamestosearchfor)
                {
                    // list of directories and sub-directories
                    dirlist.AddRange(Directory.EnumerateDirectories(pathtostartin, s, SearchOption.AllDirectories).ToList());
                }

                // if there are duplicate directories which match the conditions
                if (dirlist.Count > 1)
                {
                    // rule out the matches by including only those with ALL names in the directory
                    possiblepaths = from directory in dirlist
                                    where allfoldersdirectorymustcontain.All(directory.Contains)
                                    select directory;
                    foldermatch = possiblepaths.ToArray().Length;
                }

                // if there are any paths that meet requirements
                if (foldermatch > 0)
                {
                    try
                    {
                        // the new path should equal the first equivalent path found
                        pathoffoundfolder = possiblepaths.FirstOrDefault();
                    }
                    
                    // shouldn't happen especially if you get this far
                    catch (ArgumentNullException ane)
                    {
                        Console.WriteLine("No paths were found." + ane.StackTrace);
                    }
                }

                // if nothing is found, move further up the directory tree and try again
                Console.WriteLine("The path {0} found no results, moving further up the directory tree to try again...", pathtostartin);

                // resets the path to the parent and tries again
                if (foldermatch < 1) 
                { 
                    // moves up one directory
                    pathtostartin = Directory.GetParent(pathtostartin).ToString();

                    // if parent is the root - algorithm exits
                    if(pathtostartin == Directory.GetDirectoryRoot(pathtostartin))
                    {
                        throw new Exception("Algorithm has already reached its root with no results. Match could not be found");
                    }
                }
            }

            return pathoffoundfolder;
        }

        /// <summary>
        /// finds the latest instance of a file among various possible locations
        /// </summary>
        /// <param name="networklocations">an array of strings that contains the locations it should search</param>
        /// <param name="fullfilepath">the full path of the file that you are searching for</param>
        public static string FindLatest(string[] networklocations, string fullfilepath)
        {
            // set the Date to the lowest value possible & use the original file root as the base value
            DateTime filedt = DateTime.MinValue;
            string root = Path.GetPathRoot(fullfilepath);
            string loc_latest = root;

            string filename = Path.GetFileName(fullfilepath);
            string filelatest = filename;

            try
            {
                // search through the various locations to see which has the latet version of the file
                foreach (string loc in networklocations)
                {
                    // take away the root location and replace with the various possible locations it could be
                    string temppath = fullfilepath.Remove(0, root.Length);
                    temppath = Path.Combine(loc, temppath);

                    // if "loc" + "path" exists
                    if (Directory.Exists(temppath))
                    {
                        // temp variable for possible latest file
                        string possfile = Path.Combine(temppath, filename);
                        if (File.Exists(possfile))
                        {
                            // see if the file in this location is a later version
                            DateTime tempfiledt = File.GetLastWriteTime(loc);

                            //if tempfile is newer than filedt, filedt should equal it and record the location
                            if (tempfiledt.CompareTo(filedt) > 0)
                            {
                                filedt = tempfiledt;
                                loc_latest = loc;
                                filelatest = possfile;
                            }
                        }
                    }

                    else
                        throw new Exception("File location doesn't exist");
                }
            }

            catch
            { }
            return filelatest;
        }

        /// <summary>
        /// Archives the requested folder to location selected
        /// </summary>
        /// <param name="foldertobearchived"></param>
        /// <param name="directorytosearchforarchiveparent">Location from which search should begin</param>
        /// <param name="archiveparentfoldername">Folder where archive should be saved</param>
        public static void Archive(string foldertobearchived, string directorytosearchforarchiveparent , string archiveparentfoldername)
        {
            // gets path of folder which should be archived
            string archivedirectorypath = FolderFinder(directorytosearchforarchiveparent, foldertobearchived);

            // gets the path the folder under which the archive should be placed
            string archiveparent = FolderFinder(directorytosearchforarchiveparent, archiveparentfoldername);

            // gets the future name of archive folder; includes today's date info
            string folderwdate = Path.Combine(archiveparent, TimeUtility.DateFormatter(DateTime.Now, false));

            // copies all files + subdirectories to archive location
            DirectoryCopy(archivedirectorypath, folderwdate, true);
        }

        /// <summary>
        /// Archives a folder in it's current location
        /// </summary>
        /// <param name="objecttobearchived">File or folder to be archived</param>
        /// <param name="usehour">value to decide whether to use the current hour in the archive folder name</param>
        public static void Archive(string objecttobearchived, bool usehour)
        {
            // string for newfolder with date and time
            string folderwdateandtime = Path.Combine(objecttobearchived, TimeUtility.DateFormatter(DateTime.Now, true));

            // copies the contents of the folder in question to the dated folder, as well as any items within it
            DirectoryCopy(objecttobearchived, folderwdateandtime, true);
        }

        /// <summary>
        /// Copies all files and, optionally, subdirectories to another location
        /// </summary>
        /// <param name="sourceDirName">folder which should be copied</param>
        /// <param name="destDirName">location to copy to; will be created if it doesn't already exist</param>
        /// <param name="copySubDirs">option to copy sub-directories</param>
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copies them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
