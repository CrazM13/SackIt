using Godot;
using System;

[GlobalClass]
public partial class PhysicsMaterial : Resource {

	[Export] public float Drag { get; set; } = 0.2f;
	[Export] public float Bounciness { get; set; } = 0.2f;

}
