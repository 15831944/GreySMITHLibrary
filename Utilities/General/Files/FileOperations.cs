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
                #region Testing two possible solutions
                //possible matches to foldername & number of possible matches
                IEnumerable<string> x = from d in dirlist
                                        where d.Contains(foldernamestosearchfor[0].ToString()) && d.Contains(foldernamestosearchfor[1].ToString())
                                        select d;
                foldermatch = x.ToArray().Length;

                var test = from h in dirlist
                            where foldernamestosearchfor.All(arrayvalue => h.Contains(arrayvalue))
                            select h;
                #endregion
                // the new path should equal the first congruent path found
                try { newpath = x.FirstOrDefault(); }

                // if nothing is found, move further up the directory tree and try again
                catch { Console.WriteLine("The path {0} found no results, moving further up the directory tree to try again...", pathtostartin); }
                if (foldermatch < 1) { pathtostartin = Directory.GetParent(pathtostartin).ToString(); }

                // two items:
                // write an exception handler if the algorithm gets all the way to root with no paths found
                // also, write code that handles the filtering better - you should have to go through the previous 20 lines
                // unless there's something worth finding (put into a separate function)

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
        /// Quickly searches path's directory tree to see if there are any folders which could match the names given
        /// </summary>
        /// <param name="pathtocompareto">lowest portion of directory to start from</param>
        /// <param name="foldernames">folder names to look for</param>
        /// <returns></returns>
        public static bool PossibleMatchFound(string pathtocompareto, params string[] foldernames)
        {
            bool truthvalue = false;
            List<string> alldirectoriesintree = new List<string>();




            return truthvalue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directorytobearchived"></param>
        /// <param name="locationtobeachivedto"></param>
        public static void Archive(string directorytobearchived, string directorytosearchforarchiveparent , string archiveparentfoldername)
        {
            // check within directory structure for foldername
            string archivedirectorypath = FileOperations.FolderFinder(directorytosearchforarchiveparent, directorytobearchived);

            // copy from the latest directory and save to "newfoldername" with current date YEAR-MO-DA format
            string folderwdate = Path.Combine(archiveparentfoldername, TimeUtility.DateFormatter(DateTime.Now));
            if (!Directory.Exists(folderwdate))
            {
                Directory.CreateDirectory(folderwdate);
            }

            // copy all files from directorytobearchived
            // note the files from "foldername" into a list so that you can search the network for them
            List<string> filenames = Directory.GetFiles(directorytobearchived).ToList<string>();

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
