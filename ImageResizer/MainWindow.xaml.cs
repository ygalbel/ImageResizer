using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageProcessor;
using ImageProcessor.Imaging;
using Microsoft.Win32;

namespace ImageResizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int maxSize;

        public MainWindow()
        {
            maxSize = Int32.Parse(ConfigurationManager.AppSettings["maxSize"]);
            InitializeComponent();
        }

        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.jpeg)|*.jpeg;*.jpg;*.png|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    try
                    {

                        ResizeFile(filename, maxSize);
                        MessageBox.Show("Finish", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }

            }
        }


        private void ResizeFile(string s, int maxSize)
        {
            byte[] photoBytes = File.ReadAllBytes(s); // change imagePath with a valid image path
            int quality = 70;
            var size = new Size(200, 200);

            using (var inStream = new MemoryStream(photoBytes))
            {
                using (var outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (var imageFactory = new ImageFactory(preserveExifData: true))
                    {

                        long length = new System.IO.FileInfo(s).Length;
                        var currentFact = 1.0;
                        var splitted = s.Split('.');
                        splitted[0] = splitted[0] + "_resized";
                        var newPath = string.Join(".", splitted);


                        while (length > (maxSize * 960))
                        {
                            currentFact -= 0.1;

                            var a = imageFactory.Load(inStream);

                            a
                            .Resize(new System.Drawing.Size((int)(a.Image.Width * currentFact), (int)(a.Image.Height * currentFact)))
                            .Quality(quality)
                            .Save(newPath);

                            length = new System.IO.FileInfo(newPath).Length;
                        }


                    }
                }
            }
        }
    }
}
