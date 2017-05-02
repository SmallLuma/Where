﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Where.Game
{
    public class GameContext:Engine.GameObjectList
    {
        public GameContext(int width,int height)
        {
            var map = MapGen.MapGen.NewMap(width,height);
            //MapGen.MapGen.PaintMap(map);
            List<MapGen.Point> wallPoints = new List<MapGen.Point>();
            renderer = new Renderer.Renderer2D.Renderer2D();
            for (int y = 0;y < map.Height; ++y)
            {
                for (int x = 0; x < map.Width; ++x)
                {
                    if(map.BlockCells[x,y] == MapGen.Block.Wall || map.BlockCells[x, y] == MapGen.Block.Border)
                    {
                        wallPoints.Add(new MapGen.Point() { X = x, Y = y });
                    }
                }
            }

            renderer.SetWallBuffer(wallPoints);
            renderer.SetCamera(1, new OpenTK.Vector2());
            Engine.Engine.Window.RenderFrame += OnDraw;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        void OnDraw(object obj,object arg)
        {
            renderer.OnDraw();
        }

        Renderer.IRenderer renderer;
    }
}
