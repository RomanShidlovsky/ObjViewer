using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjViewer.Models
{
    public class ModelParams
    {
        private float _scaling;

        public float Scaling
        {
            get => _scaling;
            set
            {
                if (value > 0.001 && value < 25)
                {
                    _scaling = value;
                }
            }
        }
        
        public float ModelYaw { get; set; }
        public float ModelPitch { get; set; }
        public float ModelRoll { get; set; }
        public float TranslationX { get; set; }
        public float TranslationY { get; set; }
        public float TranslationZ { get; set; }
        public float CameraPositionX { get; set; }
        public float CameraPositionY { get; set; }
        public float CameraPositionZ { get; set; }
        public float CameraYaw { get; set; }
        public float CameraPitch { get; set; }
        public float CameraRoll { get; set; }
        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }
        public float NearPlaneDistance { get; set; }
        public float FarPlaneDistance { get; set; }
        public float XMin { get; set; }
        public float YMin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public float DeltaX { get; set; }
        public float DeltaY { get; set; }

        public ModelParams(float scaling, float modelYaw, float modelPitch, float modelRoll, float translationX,
            float translationY, float translationZ, float cameraPositionX, float cameraPositionY, float cameraPositionZ,
            float cameraYaw, float cameraPitch, float cameraRoll, float fieldOfView, float aspectRatio,
            float nearPlaneDistance,
            float farPlaneDistance, float xMin, float yMin, int width, int height)
        {
            Scaling = scaling;
            ModelYaw = (float)(modelYaw * Math.PI / 180);
            ModelPitch = (float)(modelPitch * Math.PI / 180);
            ModelRoll = (float)(modelRoll * Math.PI / 180);
            TranslationX = translationX;
            TranslationY = translationY;
            TranslationZ = translationZ;
            CameraPositionX = cameraPositionX;
            CameraPositionY = cameraPositionY;
            CameraPositionZ = cameraPositionZ;
            CameraYaw = (float)(cameraYaw * Math.PI / 180);
            CameraPitch = (float)(cameraPitch * Math.PI / 180);
            CameraRoll = (float)(cameraRoll * Math.PI / 180);
            FieldOfView = (float)(fieldOfView * Math.PI / 180);
            AspectRatio = aspectRatio;
            NearPlaneDistance = nearPlaneDistance;
            FarPlaneDistance = farPlaneDistance;
            XMin = xMin;
            YMin = yMin;
            Height = height;
            Width = width;
            DeltaX = 0;
            DeltaY = 0;
        }
    }
}