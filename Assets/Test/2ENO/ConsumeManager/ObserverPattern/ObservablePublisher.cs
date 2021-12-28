using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObservablePublisher : MonoBehaviour
{
    private readonly ArrayList observerList = new ArrayList();

    protected void Subscribe(Observer observer)
    {
        observerList.Add(observer);
    }

    protected void UnSubscribe(Observer observer)
    {
        observerList.Remove(observer);
    }

    protected void NotifyObservers()
    {
        // var 말고 직접 타입 적어서 형변환 가능
        foreach(Observer observer in observerList)
        {
            observer.Notify(this);
        }
    }
}
