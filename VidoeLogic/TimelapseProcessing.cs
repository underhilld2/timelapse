using AForge.Video.FFMPEG;
using NReco.VideoConverter;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types

namespace TimeLapse.VidoeLogic
{
    public class TimelapseProcessing
    {

        public static void CreateTimelapse(string sourcePath, string fileExtension, string outputPath, int outputWidth, int outputHeight, string outputFile, int numberOfDays)
        {
            string outputMp4 = $"{outputPath}/{outputFile}.mp4";
            string outputWebm = $"{outputPath}/{outputFile}.webm";
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            var fileEntries = info.GetFiles(fileExtension, SearchOption.AllDirectories).Where(rec => rec.CreationTime > System.DateTime.Now.AddDays(System.Math.Abs(numberOfDays) * -1)).OrderBy(p => p.CreationTime).ToArray();
            Console.WriteLine($"Found {fileEntries.Count()} images to process");

            int currentCounter = 0;
            if (fileEntries.Count() > 0)
            {
                int rate = fileEntries.Count() / 120;
                if (rate < 5)
                {
                    rate = 5;
                }

                using (var videoWriter = new VideoFileWriter())
                {
                    videoWriter.Open(outputMp4, outputWidth, outputHeight, rate, VideoCodec.MPEG4);


                    foreach (var fileName in fileEntries)
                    {
                        using (Bitmap image = Bitmap.FromFile(fileName.FullName) as Bitmap)
                        {
                            Bitmap resized = new Bitmap(image, new Size(outputWidth, outputHeight));
                            videoWriter.WriteVideoFrame(resized);
                        }
                        currentCounter++;
                        if (currentCounter % 5 == 0)
                        {
                            Console.WriteLine($"Writing image {currentCounter} of {fileEntries.Count()}");
                        }
                    }
                    videoWriter.Close();

                    var ffMpeg = new FFMpegConverter();
                    var settings = new ConvertSettings();
                    settings.VideoFrameRate = rate;
                    ffMpeg.ConvertMedia(outputMp4, Format.mp4, outputWebm, Format.webm, settings);
                }

                // upload to blob storage
                
            }
        }
    }
}
