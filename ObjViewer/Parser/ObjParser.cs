using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using ObjViewer.Models;

namespace ObjViewer.Parser;

public static class ObjParser
    {
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private static readonly StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;

        private static List<Vector4> points;
        private static List<List<Vector3>> faces;
        private static List<Vector3> textures;
        private static List<Vector3> normals;

        private static void InitLists()
        {
            points = new List<Vector4>();
            faces = new List<List<Vector3>>();
            textures = new List<Vector3>();
            normals = new List<Vector3>();
        }

        public static ObjModel Parse(string[] fileLines)
        {
            if (fileLines is null)
            {
                throw new ArgumentNullException(nameof(fileLines));
            }

            InitLists();
            foreach (string line in fileLines)
            {
                FillLists(line);
            }

            return new ObjModel(points, faces, textures, normals, TriangulateFaces(faces));
        }

        private static void FillLists(string line)
        {
            if (line.StartsWith("v ", comparison))
            {
                points.Add(ParsePoint(line));
            }
            else if (line.StartsWith("vt ", comparison))
            {
                textures.Add(ParseTexture(line));
            }
            else if (line.StartsWith("vn ", comparison))
            {
                normals.Add(ParseNormal(line));
            }
            else if (line.StartsWith("f ", comparison))
            {
                faces.Add(ParseFace(line));
            }
        }

        private static Vector4 ParsePoint(string line)
        {
            string[] values = SplitString(new char[] { ' ' }, line);
            return new Vector4(float.Parse(values[1], culture), float.Parse(values[2], culture), float.Parse(values[3], culture), values.Length > 4 ? float.Parse(values[4], culture) : 1);
        }

        private static Vector3 ParseTexture(string line)
        {
            float w = 0, v = 0;
            string[] values = SplitString(new char[] { ' ' }, line);
            if (values.Length > 2)
            {
                v = float.Parse(values[2], culture);

                if (values.Length > 3)
                {
                    w = float.Parse(values[3], culture);
                }
            }

            return new Vector3(float.Parse(values[1], culture), v, w);
        }

        private static Vector3 ParseNormal(string line)
        {
            string[] values = SplitString(new char[] { ' ' }, line);
            return new Vector3(float.Parse(values[1], culture), float.Parse(values[2], culture), float.Parse(values[3], culture));
        }

        private static List<Vector3> ParseFace(string line)
        {
            string[] values = SplitString(new char[] { ' ' }, line);
            List<Vector3> tempPoints = new();
            for (int i = 1; i < values.Length; i++)
            {
                string[] parameters = SplitString(new char[] { ' ', '/' }, values[i]);
                Vector3 v = new(float.Parse(parameters[0], culture) - 1, float.Parse(parameters[1], culture) - 1, float.Parse(parameters[2], culture) - 1);
                tempPoints.Add(v);
            }

            return tempPoints;
        }

        private static string[] SplitString(char[] separators, string str)
        {
            return str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }
        
        private static List<List<Vector3>> TriangulateFaces(List<List<Vector3>> faces)
        {
            List<List<Vector3>> triangles = new List<List<Vector3>>();

            foreach (var polygon in faces)
            {
                if (polygon.Count < 3)
                {
                    Console.WriteLine("Polygon must have at least 3 vertices.");
                    return triangles;
                }
                
                for (int i = 1; i < polygon.Count - 1; i++)
                {
                    List<Vector3> triangle = new List<Vector3> { polygon[0], polygon[i], polygon[i + 1] };
                    triangles.Add(triangle);
                }
            }
            
            return triangles;
        }
    }