using Godot;
using System;

public partial class PauseMenu : Control
{	
	private string menuScenePath = "res://UI/Scenes/MainMenu.tscn";

	public override void _Ready()
	{
		this.Visible = false;

		// unlocking user input on pause
		SetProcess(true);
		SetProcessUnhandledInput(true);
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("ui_pause"))
			PauseGameToggle();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (GetTree().Paused && @event.IsActionPressed("ui_cross"))
			PauseGameToggle();
		else if (GetTree().Paused && @event.IsActionPressed("ui_circle"))
			GetTree().ChangeSceneToFile(menuScenePath);
	}

	public void PauseGameToggle()
	{
		// does not listen to inputs
		GetTree().Paused = !GetTree().Paused;
		this.Visible = !this.Visible;
	}
}
