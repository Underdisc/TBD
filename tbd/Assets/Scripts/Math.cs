public class Math
{

	public static float lerp(float min, float max, float lerp_param)
	{
		return min + (max - min) * lerp_param;
	}

	public static float QuadOutLerp(float min, float max, float lerp_param)
	{
		lerp_param = lerp_param * lerp_param;
		return lerp(min, max, lerp_param);
	}

	public static float QuadInLerp(float min, float max, float lerp_param)
	{
		lerp_param = lerp_param - 1.0f;
		lerp_param = -(lerp_param * lerp_param) + 1.0f;
		return lerp(min, max, lerp_param);
	}
}