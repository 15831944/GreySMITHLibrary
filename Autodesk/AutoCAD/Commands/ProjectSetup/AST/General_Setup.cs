using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace General_Setup
{
    class Setup_Methods
    {
        /// <summary>
        /// Creates a series of paths if they don't already exist;
        /// can be input as an string of arrays or as a series of strings
        /// </summary>
        /// <param name="paths_to_be_created"> The path the user
        /// would like to check for existence; will create if it 
        /// doesn't already exist</param>
        public static void Directory_Creator(params string[] paths_to_be_created)
        {
            //Creates the directory if it doesn't already exist
            if (!Directory.Exists(Path.Combine(paths_to_be_created)))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(paths_to_be_created));
            }
        }
    }
    /// <summary>
    /// Creates a Dictionary which will hold a series of Lists
    /// and assign a numeric value with which to identify them
    /// </summary>
    public class DictOfLists : Dictionary<int, List<string>>
    {
        public string FileName;
        public string FilePath;
        public int IdentityNumbers;
        public static int TotalNumIdens;

        public static List<string> FillNewList(params string[] seriesofparameters)
        {
            // pushes all the info into a list which will
            // later be pushed into a dictionary
            List<string> infolist = new List<string>();
            infolist.AddRange(seriesofparameters);

            return infolist;
        }

        /// <summary>
        /// Version of DictOfLists designed to capture the filename and path
        /// of a specific set of files
        /// </summary>
        /// <param name="filename">Name of the file without the path</param>
        /// <param name="filepath">Full path of the file, including name</param>
        public DictOfLists(string filename, string filepath)
        {
            //adds the parameters info to the properties of the object
            //increments the total number of files
            //gives the file an identifying number
            FileName = filename;
            FilePath = filepath;
            TotalNumIdens++;
            IdentityNumbers = TotalNumIdens;
        }

        /// <summary>
        /// Version of DictOfLists designed to capture any number of 
        /// parameters, in the case the number is not known upon creation
        /// </summary>
        /// <param name="infoseries">array of strings to be added as info</param>
        public DictOfLists(params string[] infoseries)
        {
            IdentityNumbers++;
            TotalNumIdens++;          
        }
    }
}

 