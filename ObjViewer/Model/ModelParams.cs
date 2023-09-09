using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjViewer.Model
{
    public class ModelParams
    {
        public float Scaling { get; set; }
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
        public int XMin { get; set; }
        public int YMin { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ModelParams(float scaling, float modelYaw, float modelPitch, float modelRoll, float translationX,
               float translationY, float translationZ, float cameraPositionX, float cameraPositionY, float cameraPositionZ,
               float cameraYaw, float cameraPitch, float cameraRoll, float fieldOfView, float aspectRatio, float nearPlaneDistance,
               float farPlaneDistance, int xMin, int yMin, int width, int height)
        {
            Scaling = scaling;
            ModelYaw = modelYaw;
            ModelPitch = modelPitch;
            ModelRoll = modelRoll;
            TranslationX = translationX;
            TranslationY = translationY;
            TranslationZ = translationZ;
            CameraPositionX = cameraPositionX;
            CameraPositionY = cameraPositionY;
            CameraPositionZ = cameraPositionZ;
            CameraYaw = cameraYaw;
            CameraPitch = cameraPitch;
            CameraRoll = cameraRoll;
            FieldOfView = fieldOfView;
            AspectRatio = aspectRatio;
            NearPlaneDistance = nearPlaneDistance;
            FarPlaneDistance = farPlaneDistance;
            XMin = xMin;
            YMin = yMin;
            Height = height;
            Width = width;
        }
    }
}
