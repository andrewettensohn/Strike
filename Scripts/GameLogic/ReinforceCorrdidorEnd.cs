using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ReinforceCorrdidorEnd : Node2D
{
	public List<ReinforcePos> ReinforcePosList = new List<ReinforcePos>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		List<Node> childNodes = GetChildren().ToList();
		foreach(Node node in childNodes)
		{
			if(node.Name.ToString().Contains("Pos"))
			{
				ReinforcePos pos = (ReinforcePos)node;
				ReinforcePosList.Add(pos);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
