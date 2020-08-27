using Godot;
using System;

public class BodySnake : Node2D
{
    AnimatedSprite animatedSprite;
    public Node2D nextBody;
    public Node2D prevBody;
    int animFrame = 0;
    bool food = false;
    bool createTail = false;
    private PackedScene bodySnake = (PackedScene)GD.Load("res://snake/BodySnake.tscn");
    int step = 8;
    Vector2 prevPosition;

    public override void _Ready()
    {
        animatedSprite = (AnimatedSprite)GetNode("SnakeElement/AnimatedSprite");
        prevPosition = GlobalPosition;
        Change();
    }

    public void Change()
    {
        if(createTail)
        {
            createTail = false;
            CreateTail();
        }

        animatedSprite.RotationDegrees = 0;
        animatedSprite.FlipH = false;
        animatedSprite.FlipV = false;
        if(prevBody == null)
        { // Tail
            animatedSprite.Play("tail");
            var nextSide = GetSide(Transform.origin, nextBody.Transform.origin);
            nextSide = nextSide + 2 > 3 ? nextSide - 2 : nextSide + 2;
            if(nextSide == 1)
            {
                animatedSprite.RotationDegrees = 90;
            }
            else if(nextSide == 2)
            {
                animatedSprite.FlipH = true;
                animatedSprite.FlipV = true;
            }
            else if(nextSide == 3)
            {
                animatedSprite.RotationDegrees = 270;
            }
        }
        else if(nextBody.Transform.origin.x == prevBody.Transform.origin.x)
        { // Body horizontal
            animatedSprite.Play(food?"full":"body");
            animatedSprite.RotationDegrees = 90;
            animatedSprite.Frame = animFrame;
        }
        else if(nextBody.Transform.origin.y == prevBody.Transform.origin.y)
        { // Body vertical
            animatedSprite.Play(food?"full":"body");
            animatedSprite.Frame = animFrame;
        }
        else
        { // Rotated body
            animatedSprite.Play(food?"full_rotate":"rotate_body");
            var nextSide = GetSide(Transform.origin, nextBody.Transform.origin);
            var prevSide = GetSide(Transform.origin, prevBody.Transform.origin);
            nextSide = nextSide + 2 > 3 ? nextSide - 2 : nextSide + 2;
            prevSide = prevSide + 2 > 3 ? prevSide - 2 : prevSide + 2;
            if(nextSide == 1)
            {
                animatedSprite.RotationDegrees = 90;
            }
            else if(nextSide == 2)
            {
                animatedSprite.FlipH = true;
                animatedSprite.FlipV = true;
            }
            else if(nextSide == 3)
            {
                animatedSprite.RotationDegrees = 270;
            }

            if((nextSide + 1 > 3 ? 0 : nextSide + 1)  == prevSide)
            {
                animatedSprite.FlipV = !animatedSprite.FlipV;
            }
        }

        if(food)
        {
            food = false;
            if(prevBody != null) prevBody.Set("food", true);
            else createTail = true;
        }
        prevPosition = GlobalPosition;
    }

    void CreateTail()
    {
        Node2D body = (Node2D)bodySnake.Instance();
        body.GlobalPosition = prevPosition;
        body.Set("nextBody", this);
        prevBody = body;
        GetTree().Root.GetChild(0).CallDeferred("add_child", body);
        //GetTree().Root.GetChild(0).AddChild(body);
        Node2D current = (Node2D)nextBody;
        while(true)
        {
            var next = current.Get("nextBody");
            if(next == null)
            {
                current.Set("lastBody", body);
                break;
            }
            current = (Node2D) next;
        }
    }

    int GetSide(Vector2 currentPos, Vector2 nextPos)
    {
        if(currentPos.x > nextPos.x)
        {
            return 2;
        }
        else if (currentPos.x < nextPos.x)
        {
            return 0;
        }
        else if (currentPos.y > nextPos.y)
        {
            return 3;
        }
        else
        {
            return 1;
        }
    }

    Vector2 GetSide(int side)
    {
        int posX = 0;
        int posY = 0;
        if(side == 0) posX = -1;
        else if(side == 1) posY = -1;
        else if(side == 2) posX = 1;
        else posY = 1;
        return new Vector2(posX, posY);
    }
}
