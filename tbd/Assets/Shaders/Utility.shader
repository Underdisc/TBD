
static const float PI =  3.14159265358979323;
static const float TAU = 6.28318530717958647;

float random(float2 st)
{
	float ret = dot(st, float2(13.9898,78.233));
	ret = sin(ret * 43758.5453123);
	return frac(ret);
}

float2 random2(float2 s)
{
	float s1 = dot(s,float2(127.1,311.7));
	float s2 = dot(s,float2(269.5,183.3));
	float2 ret = frac(sin(float2(s1, s2) * 437598.5453));
	return ret;
}

float lerp(float min, float max, float l)
{
	float ret = min + l * (max - min);
	return ret;
}

float2 lerp_delta(float2 min, float2 delta, float2 param)
{
	float2 ret = min + delta * param;
	return ret;
}

float2 unit_vector(float2 vec)
{
	float len = length(vec);
	return vec * (1.0 / len);
}

float noise(float2 st)
{
	float2 st_i = floor(st);
	float2 st_f = frac(st);

	float a = random(st_i);
	float b = random(st_i + float2(1.0, 0.0));
	float c = random(st_i + float2(0.0, 1.0));
	float d = random(st_i + float2(1.0, 1.0));

	float2 l = st_f * st_f * (3.0-2.0 * st_f);

	float ab = lerp(a, b, l.x);
	float cd = lerp(c, d, l.x);
	float abcd = lerp(ab, cd, l.y);
	return abcd;
}

float vignette(float2 uv, float s_g, float e_g)
{
	float2 loc = (uv - float2(0.5, 0.5)) * 2.0;
	float len = length(loc);
	if(len < s_g)
	{
		return 0.0;
	}
	if(len > e_g)
	{
		return 1.0;
	}
	float dist = e_g - s_g;
	float ret = (len - s_g) / dist;
	return ret;
}