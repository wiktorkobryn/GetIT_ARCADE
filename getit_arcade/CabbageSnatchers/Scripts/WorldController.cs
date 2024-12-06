using Godot;
using System;
using System.Collections.Generic;

public partial class WorldController : Node2D
{
	private List<Node2D> maps = new List<Node2D>();

	public override void _Ready()
	{
		foreach(Node2D scene in GetChildren())
			maps.Add(scene);

		Random r = new Random();
		int randomizedScene = r.Next(0, 3);
		maps[randomizedScene].Visible = true;
	}
}
