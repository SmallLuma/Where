﻿using OpenTK.Graphics.ES20;
using System;
using System.Collections.Generic;

namespace Where.Game
{
    public class GameContext : Engine.GameObjectList
    {
        public GameContext(int width, int height)
        {
            Map = MapGen.MapGen.NewMap(width, height);
            //MapGen.MapGen.PaintMap(Map);
            List<MapGen.Point> wallPoints = new List<MapGen.Point>();
            renderer = new Renderer.Renderer3D.Renderer3D();

            OpenTK.Vector2 playerPos = new OpenTK.Vector2();
            MapGen.Point targetPos = new MapGen.Point();
            for (int y = 0; y < Map.Height; ++y)
            {
                for (int x = 0; x < Map.Width; ++x)
                {
                    if (Map.BlockCells[x, y] == MapGen.Block.Wall || Map.BlockCells[x, y] == MapGen.Block.Border)
                    {
                        wallPoints.Add(new MapGen.Point() { X = x, Y = y });
                        Objects.Add(new Wall(new MapGen.Point() { X = x, Y = y }));
                    }
                    else if (Map.BlockCells[x, y] == MapGen.Block.Begin)
                        playerPos = new OpenTK.Vector2(x, y);
                    else if (Map.BlockCells[x, y] == MapGen.Block.Target)
                        targetPos = new MapGen.Point() { X = x, Y = y };
                }
            }

            renderer.SetWallBuffer(wallPoints, targetPos);

            player = new Player(renderer, playerPos);
            Objects.Add(player);
            Objects.Add(new Target(targetPos));

            //TODO:强引用，删除GameContext时内存泄漏。
            Engine.Engine.Window.RenderFrame += OnDraw;

            CurrentGame = new WeakReference(this);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        private void OnDraw(object obj, object arg)
        {
            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            renderer.OnDraw();
        }

        public Renderer.IRenderer Renderer { get => renderer; }
        public Player Player { get => player; }

        private readonly Player player;
        private readonly Renderer.IRenderer renderer;
        public MapGen.Map Map { get; private set; }

        public static WeakReference CurrentGame { get; private set; }
    }
}