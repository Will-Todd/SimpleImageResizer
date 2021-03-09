using ImageProcessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ImageProcessor.Imaging.Formats;
using System.Windows.Forms;
using ImageProcessor.Imaging;

namespace ImpProc
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string inputFolderName = "";
            string outputFolderName = "";

            Console.WriteLine("Simple x4 Image Resizer...");

            Console.WriteLine("Specify Input Folder");

            FolderBrowserDialog b = new FolderBrowserDialog();
            if (b.ShowDialog() == DialogResult.OK)
            {
                inputFolderName = b.SelectedPath;
            }

            Console.WriteLine("Input Folder is: " + inputFolderName);

            string inputRoot = Path.GetFileName(inputFolderName);
            Console.WriteLine("  Input Root is: " + inputRoot);

           Console.WriteLine("Specify Output Folder");

            if (b.ShowDialog() == DialogResult.OK)
            {
                outputFolderName = b.SelectedPath;
            }

            Console.WriteLine("Output Folder is: " + outputFolderName);

            string outputRoot = Path.GetFileName(outputFolderName);
            Console.WriteLine("  Output Root is: " + outputRoot);

            string[] files = Directory.GetFiles(inputFolderName, "*.png", SearchOption.AllDirectories);


            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine();
                Console.WriteLine(files[i]);


                System.Drawing.Image img = System.Drawing.Image.FromFile(files[i]);

                Console.WriteLine("     Original Width: " + img.Width + ", Original Height: " + img.Height);

                int newWidth = RoundTo(img.Width, 4);
                int newHeight = RoundTo(img.Height, 4);

                Console.WriteLine("     New Width: " + newWidth + ", New Height: " + newHeight);

                string[] subs = files[i].Split(new[] { inputRoot }, StringSplitOptions.None);
                string outputDir = outputFolderName + subs[1];

                Console.WriteLine("     Written to " + outputDir);

                Size size = new Size(newWidth, newHeight);

                byte[] photoBytes = File.ReadAllBytes(files[i]);
                using (MemoryStream inStream = new MemoryStream(photoBytes))
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                        using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                        {
                            // Load, resize and save an image.
                            imageFactory.Load(inStream)
                                        .Resize(size)
                                        .Save(outStream);
                        }

                        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(outputDir));
                        FileStream file = new FileStream(outputDir, FileMode.Create, FileAccess.Write);
                        outStream.WriteTo(file);
                        file.Close();
                        outStream.Close();
                    }
                }

            }

            Console.WriteLine("Complete - Resized " + files.Length + " files." );
            Console.ReadLine();
        }


        static int RoundTo(int value, int factor)
        {
            return
                    (int)Math.Round(
                         (value / (double)factor),
                         MidpointRounding.AwayFromZero
                     ) * factor;
        }
    }
}
