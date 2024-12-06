using Godot;
using System;
using System.Collections.Generic;

public partial class Hud : Control
{

  private HBoxContainer healthBar;
  private List<TextureRect> healthPoints = new List<TextureRect>();

  public override void _Ready()
  {
    healthBar = GetNode<HBoxContainer>("HealthBarBg/HealthBar");
    foreach (TextureRect healthPoint in healthBar.GetChildren())
    {
      healthPoints.Add(healthPoint);
    }
  }

  void OnPlayerUpdateHealthBarHud(int value)
  {
    for (int i = 0; i < healthPoints.Count; i++)
    {
      healthPoints[i].Visible = value > i;
    }
  }

  void OnPlayerDeath(int score)
  {
    this.Visible = false;
  }
}
