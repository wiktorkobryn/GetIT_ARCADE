using Godot;
using System;

public partial class GameOver : Control
{
  public int score = 0;
  private Label scoreLabel;

  public override void _Ready()
  {
    scoreLabel = GetNode<Label>("Score");
    scoreLabel.Text = Convert.ToString(score);
  }

  void OnPlayerDeath(int scoreValue)
  {
    scoreLabel.Text = Convert.ToString(scoreValue);
    this.Visible = true;
  }
}
