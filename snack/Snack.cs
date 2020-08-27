using Godot;
using System;

public class Snack : Area2D
{
    public override void _Ready()
    {
        SetMeta("type", "snack");
        RandomNumberGenerator random = new RandomNumberGenerator();
        random.Randomize();
        ((AnimatedSprite)GetNode("AnimatedSprite")).Frame = random.RandiRange(0, 3);
    }
}
