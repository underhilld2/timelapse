using AForge.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TimeLapse.VidoeLogic;
using NReco.VideoConverter;

namespace TimeLapse
{
    public class Program
    {
        static void Main(string[] args)
        {
            CommandLineArgs.I.parseArgs(args, "height=640;width=480;framerate=5;numDays=2");
            //var tempPath = CommandLineArgs.I.argAsString("tempPath");
            var tempPath = "c:\\temp";
            var sourcePath = CommandLineArgs.I.argAsString("sourcePath");
            var imageType = CommandLineArgs.I.argAsString("imageType");
            var outPath = CommandLineArgs.I.argAsString("outputPath");
            var outputFile = CommandLineArgs.I.argAsString("outputFile");
            int height = (int)CommandLineArgs.I.argAsLong("height");
            int width = (int)CommandLineArgs.I.argAsLong("width");
            int framerate = (int)CommandLineArgs.I.argAsLong("framerate");
            int numDays = (int)CommandLineArgs.I.argAsLong("numDays");

            //Console.WriteLine("Arg tempPath  : '{0}' ", tempPath);
            Console.WriteLine("Arg SourcePath  : '{0}' ", sourcePath);
            Console.WriteLine("Arg imageType  : '{0}' ", imageType);
            Console.WriteLine("Arg OutPath  : '{0}' ", outPath);
            Console.WriteLine("Arg outputFile  : '{0}' ", outputFile);
            Console.WriteLine("Arg height  : '{0}' ", height);
            Console.WriteLine("Arg width  : '{0}' ", width);
            Console.WriteLine("Arg  framerate  : '{0}' ", framerate);
            Console.WriteLine("Arg  numDays  : '{0}' ", numDays);

            if (!string.IsNullOrEmpty(sourcePath) && !string.IsNullOrEmpty(imageType) && !string.IsNullOrEmpty(outPath))
            {
                var startTime = DateTime.UtcNow;
                Console.WriteLine("Start processing: {0}", startTime.ToShortTimeString());
                TimelapseProcessing.CreateTimelapse(sourcePath, imageType, tempPath, width, height, outputFile, numDays);
                File.Copy(Path.Combine(tempPath, $"{outputFile}.mp4"), Path.Combine(outPath, $"{outputFile}.mp4"), true);
                File.Copy(Path.Combine(tempPath, $"{outputFile}.webm"), Path.Combine(outPath, $"{outputFile}.webm"), true);
                File.Delete(Path.Combine(tempPath, $"{outputFile}.mp4"));
                File.Delete(Path.Combine(tempPath, $"{outputFile}.webm"));
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;
                Console.WriteLine("End Time: {0}", endTime.ToShortTimeString());
                Console.WriteLine("End processing: {0}", duration.ToString());
            }
            else
            {
                Console.WriteLine("Invalid parameters");
            }

         }

    }

    class CommandLineArgs
    {
        public static CommandLineArgs I
        {
            get
            {
                return m_instance;
            }
        }

        public string argAsString(string argName)
        {
            if (m_args.ContainsKey(argName))
            {
                return m_args[argName];
            }
            else return "";
        }

        public long argAsLong(string argName)
        {
            if (m_args.ContainsKey(argName))
            {
                return Convert.ToInt64(m_args[argName]);
            }
            else return 0;
        }

        public double argAsDouble(string argName)
        {
            if (m_args.ContainsKey(argName))
            {
                return Convert.ToDouble(m_args[argName]);
            }
            else return 0;
        }

        public void parseArgs(string[] args, string defaultArgs)
        {
            m_args = new Dictionary<string, string>();
            parseDefaults(defaultArgs);

            foreach (string arg in args)
            {
                string[] words = arg.Split('=');
                m_args[words[0]] = words[1];
            }
        }

        private void parseDefaults(string defaultArgs)
        {
            if (defaultArgs == "") return;
            string[] args = defaultArgs.Split(';');

            foreach (string arg in args)
            {
                string[] words = arg.Split('=');
                m_args[words[0]] = words[1];
            }
        }

        private Dictionary<string, string> m_args = null;
        static readonly CommandLineArgs m_instance = new CommandLineArgs();
    }
}
