using Godot;
using System;

public partial class Cannon : Node2D {

	[ExportGroup("References")]
	[Export] private TextureProgressBar powerBar;
	[Export] private Node2D rotatingNode;
	[Export] private Node2D nozzel;
	[Export] private PackedScene cannonballPrefab;

	[ExportGroup("Settings")]
	[Export] private float minForce = 0;
	[Export] private float maxForce = 1;
	[Export] private float powerChangeSpeed = 1;
	[ExportSubgroup("Prediction")]
	[Export] private int predictionSteps = 10;

	private float currentPower = -1;
	private int powerChangeDir = 1;
	private bool wasMouseButtonPressed;

	public override void _Ready() {
		base._Ready();

		powerBar.MinValue = minForce;
		powerBar.MaxValue = maxForce;

	}

	public override void _Process(double delta) {
		base._Process(delta);

		bool isMouseButtonPressed = Input.IsMouseButtonPressed(MouseButton.Left);
		if (!isMouseButtonPressed && wasMouseButtonPressed) {
			// TODO Fire

			GameManager.Instance.StatShotsFired++;
			currentPower = -1;
		} else if (isMouseButtonPressed) {
			UpdatePowerBar((float) delta);
		}

		UpdateAiming();

		wasMouseButtonPressed = isMouseButtonPressed;
	}

	private void UpdatePowerBar(float delta) {
		currentPower += delta * powerChangeSpeed * powerChangeDir;

		if (currentPower > maxForce) {
			currentPower = maxForce;
			powerChangeDir = -1;
		} else if (currentPower < minForce) {
			currentPower = minForce;
			powerChangeDir = 1;
		}

		powerBar.Value = currentPower;
	}

	private void UpdateAiming() {
		rotatingNode.LookAt(GetGlobalMousePosition());
	}



}
