using Godot;
using System;

public partial class Cannon : CharacterBody2D {

	[ExportGroup("References")]
	[Export] private TextureProgressBar powerBar;
	[Export] private Node2D rotatingNode;
	[Export] private Node2D nozzel;
	[Export] private Line2D prediction;
	[Export] private PackedScene cannonballPrefab;

	[ExportGroup("Settings")]
	[Export] private float minForce = 0;
	[Export] private float maxForce = 1;
	[Export] private float powerChangeSpeed = 1;
	[ExportSubgroup("Prediction")]
	[Export] private PhysicsMaterial predictionPhysics;
	[Export] private int predictionInterval = 10;
	[Export] private int predictionSteps = 10;

	private float currentPower = 0;
	private int powerChangeDir = 1;
	private bool wasMouseButtonPressed;

	public override void _Ready() {
		base._Ready();

		powerBar.MinValue = minForce;
		powerBar.MaxValue = maxForce;

		prediction.Visible = false;

	}

	public override void _Process(double delta) {
		base._Process(delta);

		bool isMouseButtonPressed = Input.IsMouseButtonPressed(MouseButton.Left);
		if (!isMouseButtonPressed && wasMouseButtonPressed) {
			Fire();
		} else if (isMouseButtonPressed) {
			UpdatePowerBar((float) delta);
			ShowPrediction((float) delta);
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

	private void ShowPrediction(float delta) {
		prediction.Visible = true;

		Vector2[] newPoints = new Vector2[predictionSteps];
		Vector2 point = nozzel.GlobalPosition;
		Vector2 aimingDir = Vector2.Right.Rotated(rotatingNode.Rotation);
		Vector2 velocity = aimingDir * currentPower;

		newPoints[0] = point;
		for (int i = 1; i < predictionSteps; i++) {

			for (int _ = 0; _ < predictionInterval; _++) {
				velocity += GetGravity() * delta;
				velocity *= 1 - (predictionPhysics.Drag * delta);
				point += velocity * delta;
			}

			newPoints[i] = point;
		}

		prediction.Points = newPoints;
	}

	private void Fire() {
		prediction.Visible = false;
		GameManager.Instance.StatShotsFired++;

		Node node = cannonballPrefab.Instantiate();

		if (node is PhysicsObject physicsObject) {
			Vector2 aimingDir = Vector2.Right.Rotated(rotatingNode.Rotation);
			Vector2 velocity = aimingDir * currentPower;

			physicsObject.Push(velocity);
			physicsObject.GlobalPosition = nozzel.GlobalPosition;
		}

		GetTree().CurrentScene.AddChild(node);

		currentPower = 0;
	}

}
