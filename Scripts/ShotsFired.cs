using Godot;
using System;

public partial class ShotsFired : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		IncreaseShotFired();
	}

	public void IncreaseShotFired()
	{
		Text = $"Shots Fired: {GameManager.Instance.StatShotsFired}";
	}

}
