using Godot;
using System;

public class GameManager {

	#region Singleton
	private static GameManager instance;
	public static GameManager Instance {
		get {
			instance ??= new GameManager();
			
			return instance;
		}
	}

	private GameManager() { /*MT*/ }
	#endregion

	public int StatShotsFired { get; set; } = 0;

}
