using Godot;
public static class HexMetrics
{

	public const float outerRadius = 1f;
	public const float innerRadius = outerRadius * 0.866025404f;
	public static Vector3[] Corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};

	public static Vector2I FromCoords(Vector3 position)
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

		return new(iX, iZ);
	}
	public static Vector2I AxialToOffset(Vector2I coords)
	{
		return new(coords.X + (int)(coords.Y / 2), coords.Y);
	}
}

public enum HexDirection {
	NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions {

    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
}
