using GMF;
using UnityEngine;

public class InitializationState : IGameState
{
    public void Enter() => Debug.Log("Entering Initialization Menu");
    public void Exit() => Debug.Log("Exiting Initialization Menu");
}

public class LoadingState : IGameState
{
    public void Enter() => Debug.Log("Entering Loading Menu");
    public void Exit() => Debug.Log("Exiting Loading Menu");
}
public class StartingScreenState : IGameState
{
    public void Enter() => Debug.Log("Entering StartingScreen State");
    public void Exit() => Debug.Log("Exiting StartingScreen State");
}

public class MainPlayState : IGameState
{
    public void Enter() => Debug.Log("Entering MainPlay State");
    public void Exit() => Debug.Log("Exiting MainPlay State");
}

public class ShutdownState : IGameState
{
    public void Enter() => Debug.Log("Entering Shutdown State");
    public void Exit() => Debug.Log("Exiting Shutdown State");
}
