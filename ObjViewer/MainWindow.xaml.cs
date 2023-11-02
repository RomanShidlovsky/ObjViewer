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
        private ObjModel model;
        private ObjModel modelMain;
        private int width, height;

        private ModelParams _modelParams;

        /*private Bgra32Bitmap _bitmap;
        private Bresenham _bresenham;
        private FlatShading _flatShading;
        private ObjModel? _clonedModel;*/
        private bool _isDragging = false;
        private bool _cameraMoving = false;
        private Point _startPosition;
        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        public MainWindow()
        {
            InitializeComponent();
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
                    model = ObjParser.Parse(fileLines);

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
                    //WriteableBitmap source = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                    /*_bitmap = new Bgra32Bitmap(source);
                    _bresenham = new Bresenham(_bitmap);*/
                    // LambertLighting lighting = new LambertLighting(GetLightingVectorFromTextBox());
                    //_flatShading = new FlatShading(_bitmap, lighting, GetColorFromTextBox());


                    DrawModel();
                }
            }
        }

        /*private Color GetColorFromTextBox()
        {
            return Color.FromRgb(
                byte.Parse(ColorRTextBox.Text, _cultureInfo),
                byte.Parse(ColorGTextBox.Text, _cultureInfo),
                byte.Parse(ColorBTextBox.Text, _cultureInfo));
        }*/

        private Vector3 GetLightingVectorFromTextBox()
        {
            return 100 * new Vector3(
                int.Parse(LightVectorXTextBox.Text, _cultureInfo),
                int.Parse(LightVectorYTextBox.Text, _cultureInfo),
                int.Parse(LightVectorZTextBox.Text, _cultureInfo));
        }

        private void DrawModel()
        {
            if (model is null)
                return;

            try
            {
                width = (int)GridPicture.ActualWidth;
                height = (int)GridPicture.ActualHeight;
                WriteableBitmap source = new(width, height, 96, 96, PixelFormats.Bgra32, null);
                Bgra32Bitmap bitmap = new(source);

                modelMain = model.Clone() as ObjModel;
                Transformations.TransformFromLocalToViewPort(modelMain, _modelParams);


                Color color = ColorPicker.Color;

                if (BresenhamRadioButton.IsChecked is true)
                {
                    Bresenham bresenham = new(bitmap, modelMain);
                    bresenham.DrawModel(color);
                }
                else if (FlatShadingRadioButton.IsChecked is true)
                {
                    LambertLighting lighting = new LambertLighting(GetLightingVectorFromTextBox());
                    FlatShading shader = new FlatShading(bitmap, lighting, modelMain);
                    shader.DrawModel(color);
                }
                else if (BlinnPhongRadioButton.IsChecked is true)
                {
                    BlinnPhongLighting lighting = new BlinnPhongLighting(GetLightingVectorFromTextBox(),
                        new Vector3(_modelParams.CameraPositionX, _modelParams.CameraPositionY, _modelParams.CameraPositionZ),
                        (float)AmbientIntensitySlider.Value ,
                        (float)DiffuseIntensitySlider.Value,
                        (float)SpecularIntensitySlider.Value,
                        (float)ShineIntensitySlider.Value,
                        new Vector3(color.R / 255f, color.G / 255f, color.B/ 255f),
                        new Vector3(1, 1, 1));

                    BlinnPhongShading shader = new BlinnPhongShading(bitmap, lighting, modelMain);
                    shader.DrawModel(color);
                }

                Picture.Source = bitmap.Source;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            DrawModel();
        }

        private void GridPicture_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (model is null)
                return;
            //_modelParams.ModelYaw += (float)(e.Delta/100 * Math.PI / 180) ;
            _modelParams.Scaling += (float)e.Delta / 1000;
            DrawModel();
        }

        private void GridPicture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (modelMain is null)
                return;

            Point p = e.GetPosition(Picture);

            if (modelMain.IsPointInModelRect(p.X, p.Y))
            {
                _isDragging = true;
                _startPosition = p;
            }
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