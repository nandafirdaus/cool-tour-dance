using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameNuclex.Data
{
    public class DanceData
    {
        string danceName;
        public List<DanceDataFrame> listOfFrame;
        public DanceData(string name) {
            this.danceName = name;
            listOfFrame = new List<DanceDataFrame>(123);
        }
        public void AddDataFrame(DanceDataFrame frame) {
            listOfFrame.Add(frame);
        }
    }
}
