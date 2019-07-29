﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Engine;
using Monofoxe.Engine.Cameras;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using PSH.Physics;
using PSH.Physics.Collisions;
using PSH.Physics.Collisions.Colliders;
using PSH.Test;
using Monofoxe.Engine.Utils;

namespace PSH
{
	public class GameController : Entity
	{
		Camera cam = new Camera(1200, 800);

		public GameController() : base(SceneMgr.GetScene("default")["default"])
		{
			GameMgr.MaxGameSpeed = 60;
			GameMgr.MinGameSpeed = 60; // Fixing framerate on 60.
			GameMgr.FixedUpdateRate = 1.0 / 60.0;

			cam.BackgroundColor = new Color(212, 188, 229);
			cam.Zoom = 1;//0.5f;
			GameMgr.WindowManager.CanvasSize = new Vector2(1200, 800);
			GameMgr.WindowManager.Window.AllowUserResizing = false;
			GameMgr.WindowManager.ApplyChanges();
			GameMgr.WindowManager.CenterWindow();
			GameMgr.WindowManager.CanvasMode = CanvasMode.Fill;
			
			GraphicsMgr.Sampler = SamplerState.PointClamp;
			
			IntersectionSystem.Init();

			CircleShape.CircleVerticesCount = 16;
			
		}

		public override void Update()
		{
			if (Input.CheckButtonPress(Buttons.B) || Input.CheckButton(Buttons.M))
			{
				var p = new Body(Layer, cam.GetRelativeMousePosition());
				var phy = p.GetComponent<CPhysics>();
				phy.DirectionalElasticity = Vector2.UnitY;
			}
			if (Input.CheckButtonPress(Buttons.N))
			{
				var e = new Entity(Layer, "wall");

				var size = new Vector2(100, 500);

				if (Input.CheckButton(Buttons.LeftShift))
				{
					size = new Vector2(500, 100);
				}

				var collider = new RectangleCollider(cam.GetRelativeMousePosition(), size);

				e.AddComponent(new CPosition(collider.Position));
				var phy = new CPhysics(0);
				phy.Collider = collider;
				e.AddComponent(phy);
			}


			GameMgr.WindowManager.WindowTitle = "fps: " + GameMgr.Fps
				+ ", iterations: " + SPhysics._iterations
				+ ", time: " + SPhysics._stopwatch.ElapsedTicks
				+ ", bodies: " + Scene.GetEntityListByComponent<CPhysics>().Count;



			System.Collections.Generic.List<int> bodies = new System.Collections.Generic.List<int>();


			// Unoptimized.
			for(var i = 0; i < bodies.Count; i += 1)
			{
				for (var k = 0; k < bodies.Count; k += 1)
				{
					ResolveCollisions(bodies[i], bodies[k]);
				}
			}

			// Optimized.
			for (var i = 0; i < bodies.Count - 1; i += 1)
			{
				for (var k = i; k < bodies.Count; k += 1)
				{
					ResolveCollisions(bodies[i], bodies[k]);
				}
			}


		}
		void ResolveCollisions(int body1, int body2) {}

		public override void Draw()
		{
			var c = new CircleCollider(Input.MousePosition, 50);

			GraphicsMgr.CurrentColor = Color.Orange;

			c.Draw(true);
			
			var collided = SPhysics.GetAllCollisions(c, null);

			GraphicsMgr.CurrentColor = Color.Blue;
			
			foreach(var cc in collided)
			{
				LineShape.Draw(c.Position, cc.Collider.Position);
			}
			
			GraphicsMgr.CurrentColor = Color.Red * 0.3f;
			//SPhysics.Grid.Draw();

		}

	}
}