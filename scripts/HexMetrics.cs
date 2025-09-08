using Godot;
public static class HexMetrics
{

	public const float outerRadius = 1f;
	public const float innerRadius = outerRadius * 0.866025404f;
	public static Vector3[] Corners = {
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius)
	};
	/// <summary>
	/// Конвертирует мировые координаты в кубические 3D координаты
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public static Vector3I AxialFromWorldCoords(Vector3 position)
	{
		float x = position.X / (innerRadius * 2f);
		float y = -x;

		float offset = position.Z / (outerRadius * 3f);
		x -= offset;
		y -= offset;

		int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(y);
		int iZ = Mathf.RoundToInt(-x - y);


		if (iX + iY + iZ != 0)
		{
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(y - iY);
			float dZ = Mathf.Abs(-x - y - iZ);

			if (dX > dY && dX > dZ)
			{
				iX = -iY - iZ;
			}
			else if (dZ > dY)
			{
				iZ = -iX - iY;
			}
		}

		return new(iX, iZ, -iX - iZ);
	}
	/// <summary>
	/// Конвертирует кубические координаты в 2D квадратные
	/// </summary>
	/// <param name="coords"></param>
	/// <returns></returns>
	public static Vector2I AxialToOffset(Vector3I coords)
	{
		return new(coords.X + (int)(coords.Y / 2), coords.Y);
	}
	public static Vector3 RotateVector(Vector3 vector, float degrees)
	{
		return vector.Rotated(Vector3.Down, Mathf.DegToRad(degrees));
	}
	/// <summary>
	/// Get point on border of hexagon with radius of outerPoint by intersecting straight line lying on side of hexagon with radius of innerPoint
	/// </summary>
	/// <param name="outerPoint">Radius of outer hex</param>
	/// <param name="innerPoint">Radius of inner hex</param>
	/// <param name="leftSide">Side of inner hex is counterclockwise from side of outer hex</param>
	/// <param name="rotation">Rotation of point relative to the center of hex</param>
	/// <returns>Coordinates of intersection point</returns>
	public static Vector3 Intersect(float outerPoint, float innerPoint, bool leftSide, int rotation)
	{
		float distance = outerPoint - innerPoint;
		if (leftSide)
		{
			return (Corners[0] * innerPoint + new Vector3(distance / 2, 0, -distance * Mathf.Sqrt(3) / 2)).Rotated(Vector3.Down, Mathf.DegToRad(rotation));
		}
		else
		{
			return (Corners[0] * innerPoint + new Vector3(-distance / 2, 0, -distance * Mathf.Sqrt(3) / 2)).Rotated(Vector3.Down, Mathf.DegToRad(rotation + 60));
		}
	}
}

public enum HexDirection
{
	NE, E, SE, SW, W, NW
}

public enum HexBorderRiverState
{
	NO, OUT, IN
}
public static class HexBorderRiverStateExtensions
{
	public static HexBorderRiverState Opposite(this HexBorderRiverState state)
	{
		switch (state)
		{
			case HexBorderRiverState.OUT:
				return HexBorderRiverState.IN;
			case HexBorderRiverState.IN:
				return HexBorderRiverState.OUT;
			default:
				return HexBorderRiverState.NO;
		}
	}
}

public static class HexDirectionExtensions
{
	public static HexDirection Opposite(this HexDirection direction)
	{
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
}
