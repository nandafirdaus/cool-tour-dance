using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameNuclex.Data;
using System.IO;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
namespace GameNuclex.IO
{
    public class GameIO
    {
        public static string DIR_PATH = "Content/data/Dance/Play/";
        public static string GALLERY_PATH = "Content/data/Dance/Gallery/";
        public static string LEARN_PATH = "Content/data/Dance/Learn/";
        // Jangan diedit //
        private static Comparison<string> compareStrInt = new Comparison<string>(CompareStrInt);
        public static int CompareStrInt(string input1, string input2) {
            input1 = Path.GetFileNameWithoutExtension(input1);
            input2 = Path.GetFileNameWithoutExtension(input2);
            int a = int.Parse(input1);
            int b = int.Parse(input2);
            return a.CompareTo(b);
        }

        public static DanceData LoadDance(string name)
        {
            name = Path.Combine((DIR_PATH), name);
            Trace.WriteLine(name);
            string[] filePath = Directory.GetFiles(name, "*.csv");

            Array.Sort(filePath, compareStrInt);
            
            DanceData data = new DanceData(name);
            for (int n = 0; n < filePath.Length; n++)
            {
                Trace.WriteLine(filePath[n]);
                data.AddDataFrame(LoadFile(filePath[n]));
            }
            return data;
        }

        public static DanceData LoadLearnDance(string name)
        {
            Trace.WriteLine(name);
            string[] filePath = Directory.GetFiles(name, "*.csv");

            Array.Sort(filePath, compareStrInt);

            DanceData data = new DanceData(name);
            for (int n = 0; n < filePath.Length; n++)
            {
                Trace.WriteLine(filePath[n]);
                data.AddDataFrame(LoadFile(filePath[n]));
            }
            
            return data;
        }

        public static DanceDataFrame LoadFile(string filepath)
        {

            string[] fileString = File.ReadAllLines(filepath);

            string danceName = fileString[0];
            long timestamp = long.Parse(fileString[1]);

            Vector3[] result = new Vector3[20];
            string[] arr;
            string[] delimiters = new string[] { ", " };
            for (int n = 2; n < 22; n++)
            {
                arr = fileString[n].Split(delimiters, StringSplitOptions.None);
                result[n-2] = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]));
            }
            NuclexEnum.PlayerPose pose = (fileString[fileString.Length - 1] == "Default") ? NuclexEnum.PlayerPose.Default : NuclexEnum.PlayerPose.SeatMode;
            DanceDataFrame danceData = new DanceDataFrame(danceName, timestamp, result, pose);

            return danceData;
        }

        /*
         * Format : 
         * Head, 10, 20, 30
         * Wrist, 10, 20, 30
         * dst ..
         */

        public static string[] GetDanceInfo() {
            string[] dirName = Directory.GetFiles(GALLERY_PATH, "INFO.xml", SearchOption.AllDirectories);
            List<Tuple<string, string, string>> danceInfo = new List<Tuple<string,string,string>>(dirName.Length);
            string[] temp = new string[dirName.Length];
            
            for (int nn = 0; nn < dirName.Length; nn++) {
                temp[nn] = new StreamReader(dirName[nn]).ReadToEnd();
            }
            return temp;
        }

        public static string[] GetPlayDanceInfo()
        {
            string[] dirName = Directory.GetFiles(DIR_PATH, "INFO.xml", SearchOption.AllDirectories);
            List<Tuple<string, string, string>> danceInfo = new List<Tuple<string, string, string>>(dirName.Length);
            string[] temp = new string[dirName.Length];

            for (int nn = 0; nn < dirName.Length; nn++)
            {
                temp[nn] = new StreamReader(dirName[nn]).ReadToEnd();
            }
            return temp;
        }

        public static string[] GetLearnDanceInfo()
        {
            string[] dirName = Directory.GetFiles(LEARN_PATH, "INFO.xml", SearchOption.AllDirectories);
            List<Tuple<string, string, string>> danceInfo = new List<Tuple<string, string, string>>(dirName.Length);
            string[] temp = new string[dirName.Length];

            for (int nn = 0; nn < dirName.Length; nn++)
            {
                temp[nn] = new StreamReader(dirName[nn]).ReadToEnd();
            }
            return temp;
        }

        public static List<Tuple<string, Texture2D>> GetLearnDanceItem(string danceName, GraphicsDevice graphics)
        {
            string[] dirName = Directory.GetFiles(LEARN_PATH + danceName, "thumbnail.jpg", SearchOption.AllDirectories);
            List<Tuple<string, Texture2D>> danceInfo = new List<Tuple<string, Texture2D>>(dirName.Length);
            //Texture2D[] temp = new Texture2D[dirName.Length];

            for (int nn = 0; nn < dirName.Length; nn++)
            {
                danceInfo.Add(new Tuple<string,Texture2D>(dirName[nn], Texture2D.FromStream(graphics, 
                    File.OpenRead(dirName[nn]))));
            }
            return danceInfo;
        }
    }
}
