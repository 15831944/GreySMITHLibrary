using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using GreySMITH.Utilities.General.Time;

namespace GreySMITH.Utilities.General.Files
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

            // find the latest files and give the name of the latest location for each of them
            foreach (string file_fullpath in filenames)
            {
                string locationlatest = FindLatest(locationstosearch, file_fullpath);

                // copy from the latest directory and save to "newfoldername" with current date YEAR-MO-DA format
                string folderwdate = Path.Combine(newfoldername, TimeUtility.DateFormatter(DateTime.Now));
                if (!Directory.Exists(folderwdate))
                {
                    Directory.CreateDirectory(folderwdate);
                }

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
            // as well as the current directory
            int foldermatch = 0;
            string newpath = null;
            // if there are any folders in the directory tree which could match the path,
            // check the path's tree, otherwise, just fail out
            //if(PossibleMatchFound(pathtocompareto, foldernames))
            #region Directory Searching Algorithm
            // try to find folders which match foldername in the current directory and sub-directories
            // if none match move up the directory tree and search its sub-directories until one is found
            while (foldermatch < 1)
            {
                // placeholder list
                List<string> dirlist = new List<string>();

                // list all possible directories in the current string
                foreach (string s in foldernamestosearchfor)
                {
                    try
                    {
                        // list of directories and sub-directories
                        dirlist.AddRange(Directory.EnumerateDirectories(pathtostartin, s, SearchOption.AllDirectories).ToList());
                    }

                    catch { }
                }
                IEnumerable<string> x = null;

                if (dirlist.Count > 1)
                {
                    //possible matches to foldername & number of possible matches
                    x = from d in dirlist
                        where d.Contains(foldernamestosearchfor[0].ToString()) && d.Contains(foldernamestosearchfor[1].ToString())
                        select d;
                    foldermatch = x.ToArray().Length;

                    // the new path should equal the first congruent path found
                    try { newpath = x.FirstOrDefault(); }

                    // if nothing is found, move further up the directory tree and try again
                    catch { Console.WriteLine("The path {0} found no results, moving further up the directory tree to try again...", pathtostartin); }
                }

                else
                {
                    try
                    {
                        newpath = dirlist.FirstOrDefault();
                        foldermatch = 1;
                    }

                    catch { Console.WriteLine("The path {0} found no results, moving further up the directory tree to try again...", pathtostartin); }
                }

                if (foldermatch < 1) { pathtostartin = Directory.GetParent(pathtostartin).ToString(); }
            }
            #endregion
            return newpath;
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
                    temppath = Path.Combine("\\", loc, temppath);

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
            // check within directory structure for foldername
            string archivedirectorypath = FileOperations.FolderFinder(directorytosearchforarchiveparent, foldertobearchived);

            string archiveparent = FileOperations.FolderFinder(directorytosearchforarchiveparent, archiveparentfoldername);

            // copy from the latest directory and save to "newfoldername" with current date YEAR-MO-DA format
            string folderwdate = Path.Combine(archiveparent, TimeUtility.DateFormatter(DateTime.Now));
            if (!Directory.Exists(folderwdate))
            {
                Directory.CreateDirectory(folderwdate);
            }

            // copy all files from directorytobearchived
            // note the files from "foldername" into a list so that you can search the network for them
            List<string> foldernames = Directory.GetDirectories(archivedirectorypath).ToList<string>();
            List<string> filenames = new List<string>();

            // if the directory has any sub-directories, grab those files and add them to the list
            if(foldernames.Count > 0)
            {
                // add a while loop to look further in the folders until there are no further directories

                foreach(string folder in foldernames)
                {
                    filenames.AddRange(Directory.GetFiles(folder).ToList<string>());
                }
            }

            // finally add any files left in the main directory
            filenames.AddRange(Directory.GetFiles(archivedirectorypath).ToList<string>());

            foreach (string file_fullpath in filenames)
            {
                try
                {
                    File.Copy(file_fullpath, Path.Combine(folderwdate, Path.GetFileName(file_fullpath)), true);
                }

                catch (Exception e)
                {
                    Console.WriteLine("The following error occured during the program: \n" + e.StackTrace.ToString());
                }
            }
        }
    }
}
