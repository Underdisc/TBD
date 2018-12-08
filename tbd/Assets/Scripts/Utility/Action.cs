public class ActionOperation
{
    // Public Static definitions for different action types.
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

/*

// This is not possible because C# does not allow you to perform operations
// on generic types. You would need to do C++ template instantiation by hand,
// which really isn't worth it. There might be another workaround to this, but
// I do not know what that is at the moment.

public enum UpdateType {Unscale, Scale}
public enum ActionType {Linear, QuadOut, QuadIn, QuadOutIn}

public class Action<T>
{
    private T start;
    private T end;
    private T delta;
    private float timeTotal;
    private float timeElapsed;
    bool done;

    private delegate void VoidVoidDelegate();
    private delegate float FloatVoidDelegate();
    private VoidVoidDelegate UpdateTime;
    private FloatVoidDelegate ActionPerc;

    public Action(T start, 
                  T end, 
                  float time, 
                  UpdateType updateType = UpdateType.Scale,
                  ActionType actionType = ActionType.Linear)
    {
        this.start = start;
        this.end = end;
        timeTotal = time;
        timeElapsed = 0.0f;
        done = false;

        switch(updateType)
        {
        case UpdateType.Scale:
            UpdateTime = UpdateScaledDeltaTime;
            break;
        case UpdateType.Unscale:
            UpdateTime = UpdateUnscaledDeltaTime;
            break;
        }

        switch(actionType)
        {
        case ActionType.Linear:
            ActionPerc = Linear;
            break;
        case ActionType.QuadOut:
            ActionPerc = QuadOut;
            break;
        case ActionType.QuadIn:
            ActionPerc = QuadIn;
            break;
        case ActionType.QuadOutIn:
            ActionPerc = QuadOutIn;
            break;
        }
    }

    float Linear()
    {
        float perc = timeElapsed / timeTotal;
        return perc;
    }
    
    float QuadOut()
    {
        float perc = timeElapsed / timeTotal;
        perc = Action.QuadOut(perc);
        return perc;
    }

    float QuadIn()
    {
        float perc = timeElapsed / timeTotal;
        perc = Action.QuadIn(perc);
        return perc;
    }

    float QuadOutIn()
    {
        float perc = timeElapsed / timeTotal;
        perc = Action.QuadOutIn(perc);
        return perc;
    }

    void UpdateScaledDeltaTime()
    {
        timeElapsed += Time.deltaTime;
    }

    void UpdateUnscaledDeltaTime()
    {
        timeElapsed += Time.unscaledDeltaTime;
    }

    void Update()
    {
        UpdateTime();
        if(timeElapsed > timeTotal)
        {
            done = true;
            return;
        }
        float perc = ActionPerc();
    }

    bool IsDone()
    {
        return done;
    }
}

public class ActionManager<T>
{
    // Add Action
    // Add ActionSequence
}

*/
