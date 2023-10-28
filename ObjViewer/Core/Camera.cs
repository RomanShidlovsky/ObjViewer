using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ObjViewer.Core
{
    public class Camera
    {
        public Matrix4x4 ViewMatrix { get; private set; }
        public Vector3 Eye { get; private set; }
        public Vector3 LookAt { get; private set; }
        public Vector3 UpVector { get; private set; }
        private int _viewPortWidth;
        private int _viewPortHeight;
        
        public Camera() {}

        public Camera(Vector3 eye, Vector3 lookAt, Vector3 upVector, int viewPortWidth, int viewPortHeight)
        {
            Eye = eye;
            LookAt = lookAt;
            UpVector = upVector;
            _viewPortWidth = viewPortWidth;
            _viewPortHeight = viewPortHeight;
            UpdateViewMatrix();
        }

        public Vector3 GetViewDir() => -new Vector3(ViewMatrix.M31, ViewMatrix.M32, ViewMatrix.M33);

        public Vector3 GetRightVector() => new Vector3(ViewMatrix.M11, ViewMatrix.M12, ViewMatrix.M13);

        public void SetCameraView(Vector3 eye, Vector3 lookAt, Vector3 up)
        {
            Eye = eye;
            LookAt = lookAt;
            UpVector = up;
            UpdateViewMatrix();
        }
        
        public void SetCameraView(Vector3 eye, Vector3 lookAt, Vector3 up, int viewPortWidth, int viewPortHeight)
        {
            Eye = eye;
            LookAt = lookAt;
            UpVector = up;
            _viewPortWidth = viewPortWidth;
            _viewPortHeight = viewPortHeight;
            UpdateViewMatrix();
        }

        public void UpdateViewMatrix()
        {
            ViewMatrix = Matrix4x4.CreateLookAt(Eye, LookAt, UpVector);
        }

        public void UpdateCamera(float deltaX, float deltaY)
        {
            Vector3 position = Eye;
            Vector3 pivot = LookAt;

            float deltaAngleX = (float)(2 * Math.PI / _viewPortWidth);
            float deltaAngleY = (float)(Math.PI / _viewPortHeight);

            float xAngle = deltaX * deltaAngleX;
            float yAngle = deltaY * deltaAngleY;

            float cosAngle = Vector3.Dot(GetViewDir(), UpVector);
            if (cosAngle * Math.Sign(yAngle) > 0.99f)
            {
                yAngle = 0;
            }

            /*Matrix4x4 rotationMatrixX = Matrix4x4.CreateRotationX(xAngle, LookAt);
            position = Vector4.Transform((position - pivot), rotationMatrixX) + pivot;
        
            Matrix4x4 rotationMatrixY = Matrix4x4.CreateRotationY(yAngle, LookAt);
            Vector4 finalPosition = Vector4.Transform((position - pivot), rotationMatrixY) + pivot;*/

            Vector3 zAxis = position - pivot;
            Vector3 yAxis = Vector3.Normalize(Vector3.Cross(UpVector, zAxis));

            Matrix4x4 rotationMatrixX = Matrix4x4.CreateFromAxisAngle(UpVector, xAngle);
            Matrix4x4 rotationMatrixY = Matrix4x4.CreateFromAxisAngle(yAxis, yAngle);

            Matrix4x4 rotationMatrix = rotationMatrixX * rotationMatrixY;

            Vector4 newPosition = Vector4.Transform(new Vector4(zAxis, 1.0f), rotationMatrix);
            Vector3 finalPosition = pivot + new Vector3(newPosition.X, newPosition.Y, newPosition.Z);

            SetCameraView(finalPosition, LookAt, UpVector);
            //SetCameraView(new Vector3(finalPosition.X, finalPosition.Y, finalPosition.Z), LookAt, UpVector);
        }

        public static Vector3 GetViewDir(Matrix4x4 viewMatrix)
        {
            return -new Vector3(viewMatrix.M31, viewMatrix.M32, viewMatrix.M33);
        }
    }
}
