using Godot;
using System;

public partial class GameMenu : Control
{
  public override void _Process(double delta)
  {
	if (Input.IsActionJustPressed("ui_triangle"))
	{
	  GetTree().ChangeSceneToFile("res://CabbageSnatchers/Scenes/Game.tscn");
	}
  }
}
