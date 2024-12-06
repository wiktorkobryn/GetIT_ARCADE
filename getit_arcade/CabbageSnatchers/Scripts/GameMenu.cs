using Godot;
using System;

public partial class GameMenu : Control
{
  public void OnPlayButtonPressed()
  {
	GetTree().ChangeSceneToFile("res://CabbageSnatchers/Scenes/Game.tscn");
  }
}
