using Godot;
using System;

public partial class ToolTipDetails : Panel
{
	private RichTextLabel _titleText;

	private RichTextLabel _statsText;

	private RichTextLabel _descriptionText;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_titleText = GetNode<RichTextLabel>("TitleText");
		_statsText = GetNode<RichTextLabel>("StatsText");
		_descriptionText = GetNode<RichTextLabel>("DescriptionText");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void SetToolTipInfo(ToolTipInfo toolTipInfo)
	{
		_titleText.Text = toolTipInfo.Title;
		_statsText.Text = toolTipInfo.Stats;
		_descriptionText.Text = toolTipInfo.Description;
	}
}
