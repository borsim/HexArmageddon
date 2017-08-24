using System;

using UnityEngine;

public struct GridPoint
{
	public readonly int u;
	public readonly int v;

	public GridPoint(int uu, int vv)
	{
		u = uu;
		v = vv;
	}

	public override string ToString()
	{
		return '(' + u.ToString() + " ," + v.ToString() + ')';
	}

	// sqrt(3)/2 from wolframAlpha
	private const float sqrt3p2 = 0.866025403784438646763723170752936183471402626905190314027f;

	public Vector2 RawPlainPoint() {
		return new Vector2 ((float)u + ((float)v / 2.0f), (float)v * sqrt3p2);
	}

	public GridPoint[] AdjacentPoints()
	{
		GridPoint[] res = new GridPoint[6];
		res[0] = new GridPoint(u - 1, v);
		res[1] = new GridPoint(u, v - 1);
		res[2] = new GridPoint(u + 1, v - 1);
		res[3] = new GridPoint(u + 1, v);
		res[4] = new GridPoint(u, v + 1);
		res[5] = new GridPoint(u - 1, v + 1);
		return res;
	}

	public GridPoint[] AdjacentPointsCircular()
	{
		GridPoint[] res = new GridPoint[11];
		res[0] = new GridPoint(u - 1, v);
		res[1] = new GridPoint(u, v - 1);
		res[2] = new GridPoint(u + 1, v - 1);
		res[3] = new GridPoint(u + 1, v);
		res[4] = new GridPoint(u, v + 1);
		res[5] = new GridPoint(u - 1, v + 1);
		res[6] = new GridPoint(u - 1, v);
		res[7] = new GridPoint(u, v - 1);
		res[8] = new GridPoint(u + 1, v - 1);
		res[9] = new GridPoint(u + 1, v);
		res[10] = new GridPoint(u, v + 1);
		return res;
	}

	public bool isAdjacentTo(GridPoint b) {
		if (u == b.u - 1)
		{
			return (v == b.v) || (v == b.v + 1);
		}
		if (u == b.u)
		{
			return (v == b.v - 1) || (v == b.v + 1);
		}
		if (u == b.u + 1)
		{
			return (v == b.v - 1) || (v == b.v);
		}
		return false;
	}

	public static GridPoint operator + (GridPoint a, GridPoint b) {
		return new GridPoint(a.u + b.u, a.v + b.v);
	}

	public static GridPoint operator - (GridPoint a, GridPoint b) {
		return new GridPoint(a.u - b.u, a.v - b.v);
	}

	public override bool Equals (object obj)
	{
		if (obj is GridPoint) {
			GridPoint ob = (GridPoint)obj;
			return ob.u == u && ob.v == v;
		}
		return false;
	}

	public override int GetHashCode ()
	{
		return u + 1024 * v;
	}

}


