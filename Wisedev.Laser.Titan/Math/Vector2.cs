﻿using Wisedev.Laser.Titan.DataStream;

namespace Wisedev.Laser.Titan.Math;

public class Vector2
{
    public int X;
    public int Y;


    public Vector2()
    {
    }

    public Vector2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Add(Vector2 vector)
    {
        X += vector.X;
        Y += vector.Y;
    }

    public void Multiply(Vector2 vector2)
    {
        X *= vector2.X;
        Y *= vector2.Y;
    }

    public void Substruct(Vector2 vector)
    {
        X -= vector.X;
        Y -= vector.Y;
    }

    public void Set(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int Normalize(int value)
    {
        int len = GetDistance();

        if (len != 0)
        {
            X = X * value / len;
            Y = Y * value / len;
        }

        return len;
    }

    public int GetDistance()
    {
        int length = 0x7FFFFFFF;

        if ((uint)(46340 - X) <= 92680)
        {
            if ((uint)(46340 - Y) <= 92680)
            {
                int lengthX = X * X;
                int lengthY = Y * Y;

                if ((uint)lengthY < (lengthX ^ 0x7FFFFFFFu))
                {
                    length = lengthX + lengthY;
                }
            }
        }

        return (int)System.Math.Sqrt(length);
    }

    /*public int GetAngle()
    {
        return LogicMath.GetAngle(X, Y);
    }*/

    public void Encode(ByteStream stream)
    {
        stream.WriteInt(X);
        stream.WriteInt(Y);
    }

    public void Decode(ByteStream stream)
    {
        X = stream.ReadInt();
        Y = stream.ReadInt();
    }
}
