using System;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph;

namespace LabWork_06_OpenGL
{
    public partial class MainForm : Form
    {
        private string filePath = "E:\\Study\\ComputerGraphics\\ComputerGraphics\\OpenGL\\blinded_cats_logo_black_opengl.obj";

        private Dictionary<string, ObjectData> objects = new Dictionary<string, ObjectData>();
        private List<Vertex> vertices = new List<Vertex>();
        private List<Vector2> texCoords = new List<Vector2>();
        private float angle = 0.0f;

        public MainForm()
        {
            InitializeComponent();
            parser(filePath);
        }

        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs args)
        {
            var gl = openGLControl.OpenGL;
            float r = 250f / 255f, g = 248f / 255f, b = 239f / 255f;
            gl.ClearColor(r, g, b, 1.0f);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45.0f, (double)openGLControl.Width / (double)openGLControl.Height, 0.01, 100.0);
            gl.LookAt(0, 5, 0, 0, 0, 0, 0, 0, 2);
            ModelRender(gl);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl.OpenGL;
        }

        private void parser(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                string objectName = null;
                ObjectData objectData = null;
                UseMtl mtl = null;

                int starNum = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0)
                        continue;

                    switch (parts[0])
                    {
                        case "o":
                            if (objectData != null && objectName != null)
                            {
                                objects.Add(objectName, objectData);
                            }
                            objectName = parts[1];
                            objectData = new ObjectData();
                            objectData.ObjectName = objectName;
                            objectData.Mtls = new List<UseMtl>();
                            break;
                        case "v":
                            float x = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                            float y = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                            float z = float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);
                            vertices.Add(new Vertex(x, y, z));
                            break;
                        case "vt":
                            float u = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                            float v = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                            texCoords.Add(new Vector2(u, v));
                            break;
                        case "usemtl":
                            if (mtl != null)
                            {
                                objectData.Mtls.Add(mtl);
                            }
                            mtl = new UseMtl();
                            mtl.Polygons = new List<Tuple<int, int, int>>();
                            if (parts[1].StartsWith("color_"))
                            {
                                mtl.UseColor = true;
                                switch (parts[1].Split("_")[1])
                                {
                                    case "black":
                                        mtl.Color = Color.Black;
                                        break;
                                    case "red":
                                        mtl.Color = Color.Red;
                                        break;
                                    case "white":
                                        mtl.Color = Color.White;
                                        break;
                                    case "ears":
                                        mtl.Color = Color.FromArgb(0xFF9A9C);
                                        break;
                                }
                            }
                            break;
                        case "f":
                            string[] v1 = parts[1].Split('/');
                            string[] v2 = parts[2].Split('/');
                            string[] v3 = parts[3].Split('/');
                            mtl?.Polygons.Add(new Tuple<int, int, int>(
                                int.Parse(v1[0]) - 1, int.Parse(v2[0]) - 1, int.Parse(v3[0]) - 1));

                            // Обработка квадратов (четырехугольников)
                            if (parts.Length == 5)
                            {
                                string[] v4 = parts[4].Split('/');
                                mtl?.Polygons.Add(new Tuple<int, int, int>(
                                    int.Parse(v1[0]) - 1, int.Parse(v3[0]) - 1, int.Parse(v4[0]) - 1));
                            }
                            // Обработка полигонов с пятью вершинами (пятиугольников)
                            else if (parts.Length > 5)
                            {
                                for (int i = 3; i < parts.Length - 1; i++)
                                {
                                    string[] vi = parts[i].Split('/');
                                    mtl?.Polygons.Add(new Tuple<int, int, int>(
                                        int.Parse(v1[0]) - 1, int.Parse(vi[0]) - 1, int.Parse(parts[i + 1].Split('/')[0]) - 1));
                                }
                            }
                            break;
                    }
                }
                objectData.Mtls.Add(mtl);
                objects.Add(objectName, objectData);
            }
        }

        private void ModelRender(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            float cameraY = 5.0f - angle * 0.1f; // Анимация отдаления камеры по оси Y

            gl.LookAt(cameraY, 0.0f, 0.0f,  // Положение камеры (над плоскостью XZ)
                      cameraY, -1.0f, 0.0f,  // Камера смотрит на центр координат
                      1.0f, 0.0f, 0.0f); // Направление вниз

            gl.Disable(OpenGL.GL_TEXTURE_2D); // Выключаем текстуры перед отрисовкой моделей

            foreach (var model in objects)
            {
                foreach (var mtl in model.Value.Mtls)
                {
                    if (mtl.UseColor)
                    {
                        gl.Color(mtl.Color);
                    }

                    gl.Begin(OpenGL.GL_TRIANGLES);
                    foreach (var polygon in mtl.Polygons)
                    {
                        // Предварительные вычисления
                        Vertex v1 = vertices[polygon.Item1];
                        Vertex v2 = vertices[polygon.Item2];
                        Vertex v3 = vertices[polygon.Item3];

                        // Отрисовка треугольника
                        gl.Vertex(v1.X, v1.Y, v1.Z);
                        gl.Vertex(v2.X, v2.Y, v2.Z);
                        gl.Vertex(v3.X, v3.Y, v3.Z);
                    }
                    gl.End();
                }
            }
            angle += 1f; // Увеличиваем угол для анимации
        }
    }

    public class ObjectData
    {
        public string ObjectName { get; set; }
        public List<UseMtl> Mtls { get; set; }
    }
    public class UseMtl
    {
        public string TextureFile { get; set; }
        public bool UseColor { get; set; }
        public Color Color { get; set; }
        public List<Tuple<int, int, int>> Polygons { get; set; }

    }
}
