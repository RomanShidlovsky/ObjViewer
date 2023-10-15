using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ObjViewer.Core;
using ObjViewer.Core.Lightings;
using ObjViewer.Core.Shaders;
using ObjViewer.Models;
using ObjViewer.Parser;

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
        private FlatShading _flatShading;
        private ObjModel? _clonedModel;
        private bool _isDragging = false;
        private bool _cameraMoving = false;
        private Point _startPosition;
        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        public MainWindow()
        {
            InitializeComponent();
            _model = null;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            const string objFilter = "Obj files (*.obj) | *.obj";
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = objFilter
            };

            if (openFileDialog.ShowDialog() is not null)
            {
                if (openFileDialog.FileName != "")
                {
                    string[] fileLines = File.ReadAllLines(openFileDialog.FileName, Encoding.UTF8);
                    _model = ObjParser.Parse(fileLines);
                    
                    

                    int width = (int)GridPicture.ActualWidth;
                    int height = (int)GridPicture.ActualHeight;
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
                    LambertLighting lighting = new LambertLighting(GetLightingVectorFromTextBox());
                    //_flatShading = new FlatShading(_bitmap, lighting, GetColorFromTextBox());
                    

                    DrawModel();
                }
            }
        }

        private Color GetColorFromTextBox()
        {
            return Color.FromRgb(
                byte.Parse(ColorRTextBox.Text, _cultureInfo),
                byte.Parse(ColorGTextBox.Text, _cultureInfo),
                byte.Parse(ColorBTextBox.Text, _cultureInfo));
        }

        private Vector3 GetLightingVectorFromTextBox()
        {
            return new Vector3(
                int.Parse(LightVectorXTextBox.Text, _cultureInfo),
                int.Parse(LightVectorYTextBox.Text, _cultureInfo),
                int.Parse(LightVectorZTextBox.Text, _cultureInfo));
        }

        private void DrawModel()
        {
            if (_model is null) 
                return;

            try
            {
                _bitmap.Clear(Color.FromRgb(0, 0, 0));
                _clonedModel = (ObjModel)_model.Clone();

                Transformations.TransformFromLocalToViewPort(_clonedModel, _modelParams);
                

                Color color = GetColorFromTextBox();
                

                if (BresenhamRadioButton.IsChecked is true)
                {
                    
                    _bresenham.DrawModel(_clonedModel, color);
                }
                else if (FlatShadingRadioButton.IsChecked is true)
                {
                    LambertLighting lighting = new LambertLighting(GetLightingVectorFromTextBox());
                    _flatShading.Lighting = lighting;
                    _flatShading.DrawModel(_clonedModel, color);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
            
            
                
           
            Picture.Source = _bitmap.Source;
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            DrawModel();
        }

        private void GridPicture_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_model is null) 
                return;
            //_modelParams.ModelYaw += (float)(e.Delta/100 * Math.PI / 180) ;
            _modelParams.Scaling += (float)e.Delta / 1000;
            DrawModel();
        }

        private void GridPicture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_clonedModel == null) 
                return;
            
            Point p = e.GetPosition(Picture);
            
            /*if (_clonedModel.IsPointInObjectRect(p.X, p.Y))
            {
                _isDragging = true;
                _startPosition = p;
            }*/
        }

        private void GridPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPosition = e.GetPosition(Picture);
                double dx = currentPosition.X - _startPosition.X;
                double dy = -(currentPosition.Y - _startPosition.Y);

                _modelParams.TranslationX += (float)dx / 100;
                _modelParams.TranslationY += (float)dy / 100;

                _startPosition = currentPosition;
                DrawModel();
            }
            else if (_cameraMoving)
            {
                Point currentPosition = e.GetPosition(Picture);
                double dx = currentPosition.X - _startPosition.X;
                double dy = -(currentPosition.Y - _startPosition.Y);

                _modelParams.DeltaX += (float)dx;
                _modelParams.DeltaY += (float)dy;

                _startPosition = currentPosition;
                DrawModel();
            }
        }

        private void GridPicture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
        }

        private void GridPicture_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_clonedModel == null) 
                return;
            
            Point p = e.GetPosition(Picture);

            _cameraMoving = true;
            _startPosition = p;
        }

        private void GridPicture_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _cameraMoving = false;
        }

        private void GridPicture_MouseLeave(object sender, MouseEventArgs e)
        {
            _cameraMoving = false;
            _isDragging = false;
        }
    }
}
