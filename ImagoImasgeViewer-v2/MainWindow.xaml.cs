using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;

namespace ImagoImasgeViewer_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyCollectionChanged
    {
        // store the name of csv file to read through
        public static string filePath;
        //store the path of the image folder
        public static string fullPath;

        string[] lines;
        public CoreImageModel image = new CoreImageModel();

        public CoreImageModel image2 = new CoreImageModel();

        //Notify UI element of change in the property change
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string image2)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(image2));
        }

        // variable to store image path to preview
        private string _imageToPreview;

        public string imageToPreview
        {
            get
            {
                return _imageToPreview;
            }

            set
            {
                _imageToPreview = value;
                OnPropertyChanged(imageToPreview);
            }
        }


        
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// /Notify the UI element of any change in the list of images
        /// </summary>
        /// <param name="image"></param>
        protected void onCollectionChanged(ObservableCollection<CoreImageModel> image)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public ObservableCollection<CoreImageModel> _imagesList;
        //list to store Model objects
        public ObservableCollection<CoreImageModel> imagesList
        {
            get
            {
                return _imagesList;
            }
            set 
            {
                _imagesList = value;
                onCollectionChanged(imagesList);
            }
        }

        /// <summary>
        /// Fine the full path of the csv file
        /// </summary>
        /// <param name="fileName"></param>
        protected void getFullPath(string fileName)
        {
            if (fileName == null)
                return;
            fullPath = System.IO.Path.GetFullPath(fileName);
        }

        /// <summary>
        /// Main window constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            imagesList = new ObservableCollection<CoreImageModel>();

            DataContext = this;

            //numberOfFiles(fullPath);
        } //end of constructor

        /// <summary>
        /// Handle the closing process of the main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }


        /// <summary>
        /// Read through the csv file and fine the number of images in the file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private uint numberOfFiles(string path)
        {
            lines = File.ReadAllLines(path);
            image.numberOfImages = (uint)lines.Length - 1;

            return image.numberOfImages;
        }

        /// <summary>
        /// Handle click event of the browse button to open the navigation manu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog browser = new OpenFileDialog();
           
            DialogResult result = browser.ShowDialog();

            //DirectoryInfo FolderBrowserDialog = new DirectoryInfo();
            if (result.ToString() == "OK")
            {
                if (imagesList.Count > 0)
                {
                    imagesList.Clear();
                }
                if (System.IO.Path.GetExtension(browser.FileName) == ".csv")
                {
                    //Get the selected file path
                    var fileName = browser.FileName.Split('\\');
                    //Read the csv file and populate the list of images
                    readFromFile(fileName[fileName.Length - 1]);
                    //Set textBox UI element text to name of the selected file
                    txtPath.Text = fileName[fileName.Length - 1];
                    //Show the number of files in the csv file
                    txbnumberOfImages.Text = numberOfFiles(fileName[fileName.Length - 1]).ToString();
                    //set the path to file
                    filePath = txtPath.Text;


                    getFullPath(filePath);
                    image.imagePath = filePath;
                }//end if
                else
                {
                    System.Windows.MessageBox.Show("Please select .csv file");
                }

            }//end if
        }//end btnBrowse_Click




        //*******************************  Image View Tab ****************************************

        /// <summary>
        /// /Helper method to find the selected image in the image list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        private CoreImageModel findImage(ObservableCollection<CoreImageModel> list, string label)
        {
           CoreImageModel image = new CoreImageModel();

           foreach(CoreImageModel im in list)
            {
                if (im.imageLabel.Equals(label))
                {
                        image = im;
                        break;
                }
            }
                return image;
        }

        /// <summary>
        /// Release the mouse capture event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgVe_MouseUp(object sender, MouseButtonEventArgs e)
        {
            imgVe.ReleaseMouseCapture();
        }


        // Points to stoer the starting and origin mouse position in the view
        System.Windows.Point start;
        System.Windows.Point origin;

        /// <summary>
        /// Handle event of pushing mouse left button for panning the image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgVe_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (imgVe.IsMouseCaptured) return;
                imgVe.CaptureMouse();

            start = Mouse.GetPosition(border);
            origin.X = imgVe.RenderTransform.Value.OffsetX;
            origin.Y = imgVe.RenderTransform.Value.OffsetY;

        // Another approach to handle the mouse left button click event
                //imgVe.CaptureMouse();
                //var tt = (TranslateTransform)((TransformGroup)imgVe.RenderTransform).Children.ElementAt(1);
                //start = e.GetPosition(border);
                //origin = new System.Windows.Point(tt.X, tt.Y);
        }

        /// <summary>
        /// Handle mouse moving on selected image for panning purposes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgVe_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!imgVe.IsMouseCaptured) return;
            System.Windows.Point point = Mouse.GetPosition(border);

            Matrix matrix = imgVe.RenderTransform.Value;
            matrix.OffsetX = origin.X + (point.X - start.X);
            matrix.OffsetY = origin.Y + (point.Y - start.Y);

            imgVe.RenderTransform = new MatrixTransform(matrix);

        // Another approach to handle the mouse moving event
                //if (imgVe.IsMouseCaptured)
                //{
                //    var tt = (TranslateTransform)((TransformGroup)imgVe.RenderTransform).Children.ElementAt(1);
                //    Vector v = start - e.GetPosition(border);

                //    tt.X = origin.X - v.X;
                //    tt.Y = origin.Y - v.Y;
                //}
        }

        /// <summary>
        /// Handle zooming event using mouse wheel event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void imgVe_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            System.Windows.Point point = Mouse.GetPosition(border);

            Matrix matrix = border.RenderTransform.Value;

            if (e.Delta > 0)
            {
                matrix.ScaleAtPrepend(1.15, 1.15, point.X, point.Y);
            }

            else
            {
                matrix.ScaleAtPrepend(1 / 1.15, 1 / 1.15, point.X, point.Y);
            }

            border.RenderTransform = new MatrixTransform(matrix);
        }

        /// <summary>
        /// Handle the selected image from list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (CoreImageModel)((System.Windows.Controls.ListView)sender).SelectedItem;

            CoreImageModel image = new CoreImageModel();

            var splitToGetLabel = item.imagePath.Split('\\');

            image.imageLabel = splitToGetLabel[splitToGetLabel.Length - 1];

            image = findImage(imagesList, image.imageLabel);

            if (image != null)
            {
                txtImagePreview.Text = image.imageLabel;

                imageToPreview = image.imagePath;
                BitmapImage bit = new BitmapImage();
                bit.BeginInit();
                string fullPath = System.IO.Path.GetFullPath(imageToPreview);
                bit.UriSource = new Uri(fullPath);
                bit.EndInit();
                imgVe.Source = bit;
            }
            else
            {
                System.Windows.MessageBox.Show("This Emage does not exist!");
            }
        }

        /// <summary>
        /// Read from csv file and populate the list of images in imagesList
        /// </summary>
        /// <param name="path"></param>
        private void readFromFile(string path)
        {
            try
            {
                StreamReader reader = new StreamReader(File.OpenRead(path));

                string line;

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    var linesSplit = line.Split(',');

                    if (!linesSplit[0].Equals("workspace"))
                    {
                        CoreImageModel imageObj = new CoreImageModel();

                        imageObj.imagePath = linesSplit[linesSplit.Length - 1];

                        var splitToGetLabel = linesSplit[linesSplit.Length - 1].Split('\\');

                        imageObj.imageLabel = splitToGetLabel[splitToGetLabel.Length - 1];

                        string pathToCreateImage = "Images\\" + imageObj.imagePath.Substring(2);
                        imageObj.imagePath = pathToCreateImage;

                        System.Drawing.Image newImage = new Bitmap(imageObj.imagePath, true);

                        imageObj.imageShow = newImage;

                        imagesList.Add(imageObj);
                    }
                }
                reader.Close();
            }
            catch (IOException e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }


    }//end class
}//end namespace

