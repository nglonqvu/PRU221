using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class Observable<T> : UnityEvent<T>
{
    //a list of listeners that are subscribed to the observable
    private List<UnityAction<T>> listeners = new List<UnityAction<T>>();
    public void Subscribe(UnityAction<T> listener)
    {
        listeners.Add(listener);
    }
    public void Unsubscribe(UnityAction<T> listener)
    {
        listeners.Remove(listener);
    }

    public void Notify(T data)
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].Invoke(data);
        }
    }
}

