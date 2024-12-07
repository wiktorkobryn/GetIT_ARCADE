using Godot;
using System;

public partial class GameOver : Control
{
  public int score = 0;
  private Label scoreLabel;

  private bool active = false;

  public override void _Ready()
  {
    this.SetProcess(false);
    scoreLabel = GetNode<Label>("Score");
    scoreLabel.Text = Convert.ToString(score);
  }

  public override void _Process(double delta)
  {
    if (Input.IsActionJustPressed("ui_cross"))
    {
      GetTree().ChangeSceneToFile("res://CabbageSnatchers/Scenes/Game.tscn");
    }
    else if (Input.IsActionJustPressed("ui_circle"))
    {
      GetTree().ChangeSceneToFile("res://CabbageSnatchers/Scenes/GameMenu.tscn");
    }
  }

  void OnPlayerDeath(int scoreValue)
  {
    scoreLabel.Text = Convert.ToString(scoreValue);
    this.SetProcess(true);
    this.Visible = true;
    GetNode<Timer>("ReturnToMenuTimer").Start();
  }

  public void OnReturnToMenuTimerTimeout()
  {
    GetTree().ChangeSceneToFile("res://CabbageSnatchers/Scenes/GameMenu.tscn");
  }
}
