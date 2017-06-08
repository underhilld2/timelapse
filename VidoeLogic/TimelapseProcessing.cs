using AForge.Video.FFMPEG;
using NReco.VideoConverter;
using System.Drawing;
using System.IO;
using System.Linq;

namespace TimeLapse.VidoeLogic
{
    public class TimelapseProcessing
    {

        public static void CreateTimelapse(string sourcePath, string fileExtension, string outputPath, string outputFile, int width, int height, int framePerSec, int numberOfDays)
        {
            string outputMp4 = $"{outputPath}/{outputFile}.mp4";
            string outputWebm = $"{outputPath}/{outputFile}.webm";
            using (var videoWriter = new VideoFileWriter())
            {
                videoWriter.Open(outputMp4, width, height, framePerSec, VideoCodec.MPEG4);

                DirectoryInfo info = new DirectoryInfo(sourcePath);
                var fileEntries = info.GetFiles(fileExtension, SearchOption.AllDirectories).Where(rec => rec.CreationTime > System.DateTime.Now.AddDays(System.Math.Abs(numberOfDays) * -1)).OrderBy(p => p.CreationTime).ToArray();

                foreach (var fileName in fileEntries)
                {
                    using (Bitmap image = Bitmap.FromFile(fileName.FullName) as Bitmap)
                    {
                        videoWriter.WriteVideoFrame(image);
                    }
                }
                videoWriter.Close();

                var ffMpeg = new FFMpegConverter();
                var settings = new ConvertSettings();
                settings.VideoFrameRate = framePerSec;
                ffMpeg.ConvertMedia(outputMp4, Format.mp4, outputWebm, Format.webm, settings);
            }
        }
    }
}
