using Godot;
using System;
using System.Collections.Generic;

public class Game
{
	public string name, description, mainScenePath;
	public Texture2D selectedPanelTex, leftPanelTex, rightPanelTex;
	private string partialPath = "res://UI/Textures/MenuGameButtons/GameButton";

	public Game(string name, string mainScenePath, string description = "---")
	{
		this.name = name;
		this.mainScenePath = mainScenePath;
		this.description = description;

		selectedPanelTex = (Texture2D)GD.Load(partialPath + "Middle" + name + ".png");
		leftPanelTex = (Texture2D)GD.Load(partialPath + "Left" + name + ".png");
		rightPanelTex = (Texture2D)GD.Load(partialPath + "Right" + name + ".png");
	}
}

public partial class Menu : Node
{	
	[Export] Sprite2D panelGameSelected, panelGameLeft, panelGameRight;
	[Export] Label SelectedGameDescriptionLbl;
	private List<Game> games = new List<Game>();
	private int selectedGame = 0;
	private Random random = new Random();

	public override void _Ready()
	{
		// randomizer setup - has to be first
		games.Add(new Game("Random", "", "Select a random game!"));

		// available games list setup
		games.Add(new Game("TmpA", "res://ExampleModule/Scenes/ExampleScene.tscn", "A simple exmple game!"));
		games.Add(new Game("TmpB", "", "A game that gives an error!?"));

		SelectRandomGame();
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("ui_left"))
			MovePanelLeft();
		else if(Input.IsActionJustPressed("ui_right"))
			MovePanelRight();
		else if(Input.IsActionJustPressed("ui_triangle"))
			LaunchSelectedGame();
	}

	private void SelectRandomGame()
	{
		selectedGame = random.Next(1, games.Count);
		ReloadGamePanels();
	}

	private void LaunchSelectedGame()
	{
		if(selectedGame != 0)
			GetTree().ChangeSceneToFile(games[selectedGame].mainScenePath);
		else
			SelectRandomGame();
	}

	private void MovePanelRight()
	{
		selectedGame++;
		if(selectedGame >= games.Count)
			selectedGame = 0;
		
		// play animations right
		//

		ReloadGamePanels();
	}

	private void MovePanelLeft()
	{
		selectedGame--;
		if(selectedGame < 0)
			selectedGame = games.Count - 1;
		
		// play animations left
		//

		ReloadGamePanels();
	}

	private void ReloadGamePanels()
	{
		int previousGame = selectedGame == 0 ? games.Count - 1 : selectedGame - 1;
		int nextGame = selectedGame == games.Count - 1 ? 0 : selectedGame + 1;

		panelGameSelected.Texture = games[selectedGame].selectedPanelTex;
		panelGameLeft.Texture = games[previousGame].leftPanelTex;
		panelGameRight.Texture = games[nextGame].rightPanelTex;

		SelectedGameDescriptionLbl.Text = games[selectedGame].description;
	}

	
}
