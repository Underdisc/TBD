
class Action
{
    // Starts slowly and appproaches quickly.
    public static float QuadOut(float perc)
    {
        return perc * perc;
    }

    // Starts fast and approaches slowly.
    public static float QuadIn(float perc)
    {
        return ((-perc + 1.0f) * (perc - 1.0f)) + 1.0f;
    }

    // Starts slow, moves quickly through the center, and ends slow.
    public static float QuadOutIn(float perc)
    {
        float lerp_param;
        if(perc < 0.5)
        {
            perc *= 2.0f;
            lerp_param = QuadOut(perc);
            lerp_param /= 2.0f;
        }
        else
        {
            perc = perc - 0.5f;
            perc *= 2.0f;
            lerp_param = QuadIn(perc);
            lerp_param = 0.5f + lerp_param / 2.0f;
        }
        return lerp_param;
    }
}