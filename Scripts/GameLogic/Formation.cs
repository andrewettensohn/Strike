using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Formation : Node2D
{
	public List<Sprite2D> FormationPoints = new List<Sprite2D>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;

		List<Node> childNodes = GetChildren().ToList();
		foreach(Node node in childNodes)
		{
			if(node.Name.ToString().Contains("Pos"))
			{
				Sprite2D posSprite = (Sprite2D)node;
				FormationPoints.Add(posSprite);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
