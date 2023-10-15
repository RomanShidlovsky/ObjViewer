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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ObjViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObjModel? _model;
        private ModelParams _modelParams;
        private Bgra32Bitmap _bitmap;
        private Bresenham _bresenham;
        private ObjModel? _clonedModel;
        private bool _isDragging = false;
        private bool _cameraMoving = false;
        private Point _startPosition;

        public MainWindow()
        {
            InitializeComponent();
            _model = null;
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            string ObjFilter = "Obj files (*.obj) | *.obj";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = ObjFilter;

            if (openFileDialog.ShowDialog() is not null)
            {
                if (openFileDialog.FileName != "")
                {
                    _model = new ObjModel();
                    _model.LoadObjFromFile(openFileDialog.FileName);

                    int width = (int)gridPicture.ActualWidth;
                    int height = (int)gridPicture.ActualHeight;
                    _modelParams = new ModelParams(
                                scaling: 0.1f,
                                modelYaw: 0,
                                modelPitch: 0,
                                modelRoll: 0,
                                translationX: 0,
                                translationY: 0,
                                translationZ: 0,
                                cameraPositionX: 0,
                                cameraPositionY: 0,
                                cameraPositionZ: 10,
                                cameraYaw: 0,
                                cameraPitch: 0,
                                cameraRoll: 0,
                                fieldOfView: 60,
                                aspectRatio: (float)width / height,
                                nearPlaneDistance: 0.1f,
                                farPlaneDistance: 100f,
                                xMin: 0,
                                yMin: 0,
                                width: width,
                                height: height);
                    WriteableBitmap source = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                    _bitmap = new Bgra32Bitmap(source);
                    _bresenham = new Bresenham(_bitmap);

                    DrawModel();
                }
            }
        }

        private void DrawModel()
        {
            if (_model is not null)
            {
                _bitmap.Clear(Color.FromRgb(0, 0, 0));

                _clonedModel = (ObjModel)_model.Clone();

                Transformations.TransformFromLocalToViewPort(_clonedModel, _modelParams);
                _clonedModel.UpdateSize();
                
                _bresenham.DrawModel(_clonedModel);

                picture.Source = _bitmap.Source;
            }
        }

        private void drawButton_Click(object sender, RoutedEventArgs e)
        {
            DrawModel();
        }

        private void gridPicture_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_model is not null)
            {
                //_modelParams.ModelYaw += (float)(e.Delta/100 * Math.PI / 180) ;
                _modelParams.Scaling += (float)e.Delta / 1000;
                DrawModel();
            }
        }

        private void gridPicture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_clonedModel != null)
            {
                Point p = e.GetPosition(picture);
                if (_clonedModel.IsPointInObjectRect(p.X, p.Y))
                {
                    _isDragging = true;
                    _startPosition = p;
                }
            }
        }

        private void gridPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPosition = e.GetPosition(picture);
                double dx = currentPosition.X - _startPosition.X;
                double dy = -(currentPosition.Y - _startPosition.Y);

                _modelParams.TranslationX += (float)dx / 100;
                _modelParams.TranslationY += (float)dy / 100;

                _startPosition = currentPosition;
                DrawModel();
            }
            else if (_cameraMoving)
            {
                Point currentPosition = e.GetPosition(picture);
                double dx = currentPosition.X - _startPosition.X;
                double dy = -(currentPosition.Y - _startPosition.Y);

                _modelParams.DeltaX += (float)dx;
                _modelParams.DeltaY += (float)dy;

                _startPosition = currentPosition;
                DrawModel();
            }
        }

        private void gridPicture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
        }

        private void gridPicture_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_clonedModel != null)
            {
                Point p = e.GetPosition(picture);

                _cameraMoving = true;
                _startPosition = p;
            }
        }

        private void gridPicture_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _cameraMoving = false;
        }

        private void gridPicture_MouseLeave(object sender, MouseEventArgs e)
        {
            _cameraMoving = false;
            _isDragging = false;
        }
    }
}
