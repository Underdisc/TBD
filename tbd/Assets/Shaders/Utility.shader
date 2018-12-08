
static const float PI =  3.14159265358979323;
static const float TAU = 6.28318530717958647;
static const float EPSILON_5 = 0.00001;

float eq(float a, float b)
{
	return 1.0 - abs(sign(a - b));
}

float neq(float a, float b)
{
	return abs(sign(a - b));
}

float gt(float a, float b)
{
	return max(sign(a - b), 0.0);
}

float lt(float a, float b)
{
	return max(sign(b - a), 0.0);
}

float gteq(float a, float b)
{
	return max(eq(a, b), gt(a, b));
}

float lteq(float a, float b)
{
	return max(eq(a, b), lt(a, b));
}

float invert(float value, float min, float max)
{
	float max_dist = max - value;
	return min + max_dist;
}

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

float gradient(float minimum, float maximum, float p)
{
	float delta = maximum - minimum;
	float p_delta = p - minimum;
	float gradient = gt(p, minimum);
	gradient = min(gradient * p_delta / delta, 1.0);
	return gradient;
}

float apply_gradient(float grad, float v)
{
	float applied = gt(grad, 0.0) * v * grad;
	applied = min(applied + grad, 1.0);
	return applied;
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

float toon(float value, int toon_factor)
{
	return floor(value * toon_factor) / toon_factor;
}

// Project a vector, v, onto a plane with normal, n.
float3 project_onto_plane(float3 v, float3 n)
{
	float v_dot_n = dot(v, n);
	float n_mag = length(n);
	float a = v_dot_n / (n_mag * n_mag);
	float3 proj = v - a * n;
	return proj;
}