using UnityEngine;
using System;

public static class EnumExtensions
{
	public static bool Contains(this Enum value, Enum layer)
	{
		return Convert.ToInt32(value) == (Convert.ToInt32(value) | (1 << Convert.ToInt32(layer)));
	}
}
