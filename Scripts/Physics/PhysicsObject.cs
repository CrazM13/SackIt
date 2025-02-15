using Godot;
using System;

public partial class PhysicsObject : CharacterBody2D {

	[Export] private PhysicsMaterial physicsMaterial;
	[Export] private TextureProgressBar decayGuage;

	[Export] private bool decayOnActivate = true;
	[Export] private float decayDelay = 5f;

	private bool alive = false;
	private bool shouldTestAlive = false;
	private float lifetime = 0;

	private Vector2 startPosition;

	public override void _Ready() {
		base._Ready();

		startPosition = this.GlobalPosition;

		if (decayGuage != null) {
			decayGuage.Visible = false;
		}
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (alive) {
			lifetime += (float) delta;

			if (decayGuage != null) {
				decayGuage.Value = 1 - (lifetime / decayDelay);
			}

			if (decayOnActivate && lifetime > decayDelay) {
				QueueFree();
			}
		}
		
		if (shouldTestAlive) {
			if (startPosition.DistanceSquaredTo(this.GlobalPosition) > 32*32) {
				if (!alive) alive = true;
				lifetime = 0;
				startPosition = this.GlobalPosition;
				if (decayGuage != null) decayGuage.Visible = true;
				shouldTestAlive = false;
			}
		}

		

	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);

		this.Velocity += GetGravity() * (float) delta;
		this.Velocity *= 1 - (physicsMaterial.Drag * (float) delta);

		Vector2 oldVelocity = this.Velocity;
		float friction = (1 - physicsMaterial.Drag);
		MoveAndSlide();

		for (int i = 0; i < GetSlideCollisionCount(); i++) {
			KinematicCollision2D collision = GetSlideCollision(i);

			this.Velocity = (oldVelocity.Bounce(collision.GetNormal()) * physicsMaterial.Bounciness) + (oldVelocity * (1 - physicsMaterial.Bounciness));
				

			if (collision.GetCollider() is PhysicsObject physicsObject) {
				physicsObject.Push((oldVelocity - this.Velocity) * friction);
			}

			this.Velocity *= friction;
		}
	}

	public void Push(Vector2 velocity) {
		this.Velocity += velocity;

		shouldTestAlive = true;
	}

}
