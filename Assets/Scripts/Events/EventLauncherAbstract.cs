using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventLauncherAbstract: MonoBehaviour, EventLauncher
{
    [SerializeField] protected GameEvent gameEvent;

    public abstract void Launch();
}

