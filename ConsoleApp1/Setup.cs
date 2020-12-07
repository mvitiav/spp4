using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class Setup
    {
        public int paralelFilesLoaded { get; }
        public int paralelTasksProcessed { get; }
        public int paralelFilesWriten { get; }
        public string outputPath { get; }
        public List<string> inputPath { get; }

        public Setup(int paralelFilesLoaded, int paralelTasksProcessed, int paralelFilesWriten,string outputPath)
        {
            this.paralelFilesLoaded = paralelFilesLoaded;
            this.paralelTasksProcessed = paralelTasksProcessed;
            this.paralelFilesWriten = paralelFilesWriten;
            this.outputPath = outputPath;
            this.inputPath = new List<string>();
        }


    }
}
