using Godot;
using System;

public class PlayerSnake : Node2D
{
    AnimatedSprite animatedSprite;
    public Node2D nextBody = null;
    [Export]
    int currentRotation = 0; // 0 - L, 1 - U, 2 - R, 3 - D
    int nextRotation = 0; // 0 - L, 1 - U, 2 - R, 3 - D
    [Export]
    int tale = 2;
    [Export]
    int step = 8;
    Node timer;
    Area2D area2D;
    private PackedScene bodySnake = (PackedScene)GD.Load("res://snake/BodySnake.tscn");
    Node2D lastBody;
    int animFrame = 0;
    bool food = false;

    public override void _Ready()
    {
        timer = GetTree().Root.FindNode("Timer", true, false);
        timer.Connect("timeout", this, nameof(Move));
        area2D = (Area2D)GetNode("SnakeElement");
        area2D.Connect("area_entered", this, nameof(Collision));
        nextRotation = currentRotation;
        animatedSprite = (AnimatedSprite)GetNode("SnakeElement/AnimatedSprite");
        for(int i = 0; i < tale; i++)
            CreateTail();
    }

    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("ui_left"))
        {
            if(currentRotation != 2)
            {
                nextRotation = 0;
                var spaceState = GetWorld2d().DirectSpaceState;
                var result = spaceState.IntersectRay(Transform.origin, Transform.origin + new Vector2(-step, 0));
                if (result.Count > 0)
                    GD.Print("Hit at point: ", result["position"]);
            }
        }
        else if(Input.IsActionJustPressed("ui_right"))
        {
            if(currentRotation != 0)
            {
                nextRotation = 2;
                var spaceState = GetWorld2d().DirectSpaceState;
                var result = spaceState.IntersectRay(Transform.origin, Transform.origin + new Vector2(-step, 0));
                if (result.Count > 0)
                    GD.Print("Hit at point: ", result["position"]);
            }
        }
        else if(Input.IsActionJustPressed("ui_up"))
        {
            if(currentRotation != 3)
            {
                nextRotation = 1;
                var spaceState = GetWorld2d().DirectSpaceState;
                var result = spaceState.IntersectRay(Transform.origin, Transform.origin + new Vector2(-step, 0));
                if (result.Count > 0)
                    GD.Print("Hit at point: ", result["position"]);
            }
        }
        else if(Input.IsActionJustPressed("ui_down"))
        {
            if(currentRotation != 1)
            {
                nextRotation = 3;
                var spaceState = GetWorld2d().DirectSpaceState;
                var result = spaceState.IntersectRay(Transform.origin, Transform.origin + new Vector2(-step, 0));
                if (result.Count > 0)
                    GD.Print("Hit at point: ", result["position"]);
            }
        }
        
        if (Input.IsActionJustPressed("ui_select"))
        {
            CreateTail();
        }

        // Animation

        if(nextRotation == currentRotation)
        {
            animatedSprite.Play("head");
        }
        else if(nextRotation != currentRotation)
        {
            if((nextRotation + 1 > 3 ? 0 : nextRotation + 1) == currentRotation)
                animatedSprite.Play("rotate_left");
            else animatedSprite.Play("rotate_right");
        }
    }

    public void Move()
    {
        MoveTail();
        currentRotation = nextRotation;
        GlobalPosition = Transform.origin + GetSide(nextRotation) * step;

        if(currentRotation == 0)
        {
            animatedSprite.FlipH = false;
            animatedSprite.FlipV = false;
            animatedSprite.RotationDegrees = 0;
        }
        else if(currentRotation == 1)
        {
            animatedSprite.FlipH = false;
            animatedSprite.FlipV = false;
            animatedSprite.RotationDegrees = 90;
        }
        else if(currentRotation == 2)
        {
            animatedSprite.FlipH = true;
            animatedSprite.FlipV = true;
            animatedSprite.RotationDegrees = 0;
        }
        else
        {
            animatedSprite.FlipH = false;
            animatedSprite.FlipV = false;
            animatedSprite.RotationDegrees = 270;
        }
        ChangeTail();
    }

    void CreateTail()
    {
        Node2D body = (Node2D)bodySnake.Instance();
        Vector2 pos = (
            lastBody == null ?
                Transform.origin + GetSide((nextRotation + 2) > 3 ? nextRotation -2 : nextRotation + 2) * step :
                lastBody.Transform.origin + GetSide(lastBody.Transform.origin, ((Node2D)lastBody.Get("nextBody")).Transform.origin) * step
        );
        body.GlobalPosition = pos;
        body.Set("nextBody", lastBody == null ? this : lastBody);
        GetTree().Root.GetChild(0).CallDeferred("add_child", body);
        //GetTree().Root.GetChild(0).AddChild(body);
        if(lastBody != null)lastBody.Set("prevBody", body);
        lastBody = body;
    }

    void MoveTail()
    {
        Node2D currentBody = lastBody;
        while(currentBody != this)
        {
            Node2D nextBody = (Node2D)currentBody.Get("nextBody");
            if(nextBody == this && food)
            {
                food = false;
                currentBody.Set("food", true);
            }
            currentBody.GlobalPosition = nextBody.Transform.origin;
            currentBody = nextBody;
        }
    }

    void ChangeTail()
    {
        int animFrameCurrent = animFrame;
        Node2D currentBody = lastBody;
        while(currentBody != this)
        {
            Node2D nextBody = (Node2D)currentBody.Get("nextBody");
            animFrameCurrent = (animFrameCurrent + 1 > 3) ? 0 : animFrameCurrent + 1;
            currentBody.Set("animFrame", animFrameCurrent);
            currentBody.Call("Change");
            currentBody = nextBody;
        }
        animFrame = (animFrame + 1 > 3) ? 0 : animFrame + 1;
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

    Vector2 GetSide(Vector2 currentPos, Vector2 nextPos)
    {
        if(currentPos.x > nextPos.x)
        {
            return GetSide(2);
        }
        else if (currentPos.x < nextPos.x)
        {
            return GetSide(0);
        }
        else if (currentPos.y > nextPos.y)
        {
            return GetSide(3);
        }
        else
        {
            return GetSide(1);
        }
    }

    public void Collision(Area2D area)
    {
        if(area.GetMeta("type") != null)
        {
            if(area.GetMeta("type").ToString() == "snack")
            {
                area.QueueFree();
                food = true;
            }
            GD.Print("asdasdasdasd " + area.Name + area.GetMeta("type"));
        }
    }
}
