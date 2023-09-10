using Microsoft.Win32;
using ObjViewer.Core;
using ObjViewer.Model;
using System;
using System.Collections.Generic;
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

namespace ObjViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObjModel _model = new ObjModel();
        private ModelParams _modelParams;
        private Bgra32Bitmap _bitmap;
        private Bresenham _bresenham;
        
        public MainWindow()
        {
            InitializeComponent();   
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog= new OpenFileDialog();

            if (openFileDialog.ShowDialog() is not null)
            {
               // if (openFileDialog.FileName)
                _model.LoadObjFromFile(openFileDialog.FileName);

                int width = (int)gridPicture.ActualWidth;
                int height = (int)gridPicture.ActualHeight;

                WriteableBitmap source = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                _bitmap = new Bgra32Bitmap(source);

                _modelParams = new ModelParams(2f, 60, 0, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0,
                    60, (float)width / height, 0.1f, 100f, 0, 0, width, height);

              
                _bresenham = new Bresenham(_model, _bitmap);
                DrawModel();

                picture.Source = _bitmap.Source;
            }
        }

        private void DrawModel()
        {
            Transformations.TransformFromLocalToViewPort(_model, _modelParams);
            _bresenham.DrawModel();
        }

    }
}
