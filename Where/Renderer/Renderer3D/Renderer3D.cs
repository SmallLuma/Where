﻿using MapGen;
using OpenTK;
using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;
using Where.Renderer.Lower;

namespace Where.Renderer.Renderer3D
{
    public class Renderer3D : IRenderer
    {
        public Renderer3D()
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 1.7F, Engine.Engine.Window.Height / ((float)Engine.Engine.Window.Width), 0.1F, 1000.0F);

            objectDrawLocs.Vertex = objectDraw.GetAttributionLocation("Vertex");
            objectDrawLocs.Normal = objectDraw.GetUniformLocation("Normal");
            objectDrawLocs.Camera = objectDraw.GetUniformLocation("Camera");
            objectDrawLocs.TexCoord = objectDraw.GetAttributionLocation("TexCoordInput");
            objectDrawLocs.EyePos = objectDraw.GetUniformLocation("EyePos");
            objectDrawLocs.Time = objectDraw.GetUniformLocation("Time");
            objectDrawLocs.TBNMatrix = objectDraw.GetUniformLocation("TBNMatrix");
            objectDrawLocs.SkyColor = objectDraw.GetUniformLocation("SkyColor");
            objectDrawLocs.SunColor = objectDraw.GetUniformLocation("SunColor");
            objectDrawLocs.PlayerLight = objectDraw.GetUniformLocation("PlayerLight");
            objectDrawLocs.SunLightPos = objectDraw.GetUniformLocation("SunLightPos");

            objectDraw.EnableAttribute(objectDrawLocs.Vertex);
            objectDraw.EnableAttribute(objectDrawLocs.TexCoord);
            objectDraw.Use();
            objectDraw.SetUniform("Surface", 0);
            objectDraw.SetUniform("Height", 1);
            objectDraw.SetUniform("NormalMap", 2);
            objectDraw.SetUniform("Cloud", 3);

            GL.UseProgram(0);

            cloudNoise.BindTo0AndLoadImage("Cloud");

            wallMateria = new Materia("wall");
            earthMateria = new Materia("grass");
        }

        public void OnDraw()
        {
            time += 1;
            GL.Viewport(0, 0, Engine.Engine.Window.Width, Engine.Engine.Window.Height);

            GL.Enable(EnableCap.DepthTest);

            cloudNoise.Bind(3);
            sky.OnDraw(dayNight);

            objectDraw.Use();
            objectDraw.SetUniform(objectDrawLocs.Time, time);

            earthMateria.Bind();
            earth.OnDraw(objectDraw, objectDrawLocs);

            //此处绘制墙体
            wallMateria.Bind();
            wall.OnDraw(objectDraw, objectDrawLocs);

            GL.Disable(EnableCap.DepthTest);

            GL.Viewport(0, 0, Engine.Engine.Window.Width / 4, Engine.Engine.Window.Height / 4);
            renderer2d.OnDraw();
        }

        public void SetCamera(float angle, float pov, Vector2 pos)
        {
            dayNight.Update();
            renderer2d.SetCamera(angle, pov, pos);

            var eyePos = new Vector3(-21.0F * pos.X, -20.0F, 21.0F * pos.Y);
            Camera = Matrix4.CreateTranslation(eyePos) * Matrix4.CreateRotationY((float)((angle + 180) * Math.PI / 180));

            Matrix4 camera = Camera * Projection;

            sky.SetPos(pos, this);

            objectDraw.Use();
            objectDraw.SetUniform(objectDrawLocs.Camera, ref camera);
            eyePos = new Vector3(21.0F * pos.X, 20.0F, -21.0F * pos.Y);
            objectDraw.SetUniform(objectDrawLocs.EyePos, eyePos);

            objectDraw.SetUniform(objectDrawLocs.SkyColor, dayNight.SkyColorA);
            objectDraw.SetUniform(objectDrawLocs.SunColor, dayNight.SkyColorB);
            objectDraw.SetUniform(objectDrawLocs.PlayerLight, dayNight.PlayerLight);
            objectDraw.SetUniform(objectDrawLocs.SunLightPos, dayNight.SunLightPos);
        }

        public void SetWallBuffer(List<Point> wallPoints, Point targetPoint)
        {
            renderer2d.SetWallBuffer(wallPoints, targetPoint);
            wall = new Wall(wallPoints);
        }

        public static Matrix3 GetTBNMatrix(Vector3 N, Vector3 T)
        {
            Vector3 B = OpenTK.Vector3.Cross(N, T);
            return new Matrix3(T, B, N);
        }

        public Matrix4 Projection { get; private set; }
        public Matrix4 Camera { get; private set; }

        public AfterEffect.AfterEffectSystem AfterEffects { get; private set; } = new AfterEffect.AfterEffectSystem();

        private Renderer2D.Renderer2D renderer2d = new Renderer2D.Renderer2D();
        private Earth earth = new Earth();
        private GLShader objectDraw = new GLShader("VertexShader", "3D_ObjectDraw");

        public struct ObjectDrawShaderLocs
        {
            public int
                Vertex,
                Normal,
                Camera,
                TexCoord,
                EyePos,
                TBNMatrix,
                SkyColor,
                SunColor,
                PlayerLight,
                SunLightPos,
                Time;
        }

        private ObjectDrawShaderLocs objectDrawLocs;

        private Wall wall;
        private float time = 0;

        private GLTexture cloudNoise = new GLTexture();
        private readonly Materia wallMateria, earthMateria;

        private SkyBox sky = new SkyBox();

        private DayNight dayNight = new DayNight();
    }
}