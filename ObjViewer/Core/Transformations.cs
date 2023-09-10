using ObjViewer.Model;
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
        public static void TransformFromLocalToViewPort(ObjModel model, ModelParams modelParams)
        {
            Matrix4x4 mvpMatrix = GetMVP(modelParams);
            float w;

            for (int i = 0; i < model.VertexList.Count; i++)
            {
                model.VertexList[i].Coordinates = Vector4.Transform(model.VertexList[i].Coordinates, mvpMatrix);

                w = model.VertexList[i].Coordinates.W;
                model.VertexList[i].Coordinates /= w;

                model.VertexList[i].Coordinates = Vector4.Transform(model.VertexList[i].Coordinates, GetViewPortMatrix(modelParams));
                model.VertexList[i].Coordinates = new Vector4(
                    model.VertexList[i].Coordinates.X,
                    model.VertexList[i].Coordinates.Y,
                    model.VertexList[i].Coordinates.Z,
                    w);
            }
        }

        private static Matrix4x4 GetWorldMatrix(ModelParams modelParams)
        {
            return Matrix4x4.CreateScale(modelParams.Scaling) *
                Matrix4x4.CreateFromYawPitchRoll(modelParams.ModelYaw, modelParams.ModelPitch, modelParams.ModelRoll) *
                Matrix4x4.CreateTranslation(modelParams.TranslationX, modelParams.TranslationY, modelParams.TranslationZ);                             
        }

        private static Matrix4x4 GetViewerMatrix(ModelParams modelParams)
        {
            bool success = Matrix4x4.Invert(Matrix4x4.CreateFromYawPitchRoll(modelParams.CameraYaw, modelParams.CameraPitch, modelParams.CameraRoll) *
                Matrix4x4.CreateTranslation(modelParams.CameraPositionX, modelParams.CameraPositionY, modelParams.CameraPositionZ), out Matrix4x4 viewMatrix);

            if (success)
            {
                return viewMatrix;
            }
            else
            {
                throw new Exception("Can't invert matrix");
            }
        }

        private static Matrix4x4 GetPerspectiveProjectionMatrix(ModelParams modelParams)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(modelParams.FieldOfView, modelParams.AspectRatio, modelParams.NearPlaneDistance, modelParams.FarPlaneDistance);
        }

        private static Matrix4x4 GetViewPortMatrix(ModelParams modelParams)
        {
            return new Matrix4x4(modelParams.Width / 2, 0, 0, 0,
                                 0, -modelParams.Height / 2, 0, 0,
                                 0, 0, 1, 0,
                                 modelParams.XMin + (modelParams.Width / 2), modelParams.YMin + (modelParams.Height / 2), 0, 1);
        }

        private static Matrix4x4 GetMVP(ModelParams modelParams)
        {
            return GetWorldMatrix(modelParams) * GetViewerMatrix(modelParams) * GetPerspectiveProjectionMatrix(modelParams);
        }
    }
}
