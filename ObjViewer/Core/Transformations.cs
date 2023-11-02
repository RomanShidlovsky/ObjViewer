﻿using ObjViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ObjViewer.Core
{
    public static class Transformations
    {
        private static readonly Camera _camera = new Camera();
        public static void TransformFromLocalToViewPort(ObjModel model, ModelParams modelParams)
        {
            Matrix4x4 mvpMatrix = GetMVP(modelParams);
            float w;

            for (int i = 0; i < model.Points.Count; i++)
            {
                model.WorldPoints.Add(Vector4.Transform(model.Points[i], GetWorldMatrix(modelParams)));
                model.Points[i] = Vector4.Transform(model.Points[i], mvpMatrix);
                
                
                w = model.Points[i].W;
                model.Points[i] /= w;

                model.Points[i] = Vector4.Transform(model.Points[i], GetViewPortMatrix(modelParams));
                model.Points[i] = new Vector4(
                    model.Points[i].X,
                    model.Points[i].Y,
                    model.Points[i].Z,
                    w);
            }
            
            TransformNormal(model, modelParams);
        }

        public static Matrix4x4 GetWorldMatrix(ModelParams modelParams)
        {
            return Matrix4x4.CreateScale(modelParams.Scaling) *
                Matrix4x4.CreateFromYawPitchRoll(modelParams.ModelYaw, modelParams.ModelPitch, modelParams.ModelRoll) *
                Matrix4x4.CreateTranslation(modelParams.TranslationX, modelParams.TranslationY, modelParams.TranslationZ);                             
        }

        public static Matrix4x4 GetViewerMatrix(ModelParams modelParams)
        {
            Vector3 eye = new Vector3(modelParams.CameraPositionX, modelParams.CameraPositionY, modelParams.CameraPositionZ);
            Vector3 lookAt = new Vector3(0, 0, 0);
            Vector3 upVector = new Vector3(0, 1, 0);

            _camera.SetCameraView(eye, lookAt, upVector, modelParams.Width, modelParams.Height);
            
            if (modelParams.DeltaX != 0 || modelParams.DeltaY != 0) 
            {
                _camera.UpdateCamera(modelParams.DeltaX, modelParams.DeltaY);
            }

            return _camera.ViewMatrix;
        }
        
        private static void TransformNormal(ObjModel model, ModelParams modelParams)
        {
            for (int i = 0; i < model.Normals.Count; i++)
            {
                model.Normals[i] = Vector3.Normalize(Vector3.TransformNormal(model.Normals[i], GetNormalMatrix(modelParams)));
            }
        }

        public static Matrix4x4 GetPerspectiveProjectionMatrix(ModelParams modelParams)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(modelParams.FieldOfView, modelParams.AspectRatio, modelParams.NearPlaneDistance, modelParams.FarPlaneDistance);
        }

        public static Matrix4x4 GetViewPortMatrix(ModelParams modelParams)
        {
            return new Matrix4x4(modelParams.Width / 2, 0, 0, 0,
                                 0, -modelParams.Height / 2, 0, 0,
                                 0, 0, 1, 0,
                                 modelParams.XMin + (modelParams.Width / 2), modelParams.YMin + (modelParams.Height / 2), 0, 1);
        }

        public static Matrix4x4 GetMVP(ModelParams modelParams)
        {
            return GetWorldMatrix(modelParams) * GetViewerMatrix(modelParams) * GetPerspectiveProjectionMatrix(modelParams);
        }

        public static Matrix4x4 GetNormalMatrix(ModelParams modelParams)
        {
            return Matrix4x4.CreateFromYawPitchRoll(modelParams.ModelYaw, modelParams.ModelPitch,
                modelParams.ModelRoll);
        }
    }
}
