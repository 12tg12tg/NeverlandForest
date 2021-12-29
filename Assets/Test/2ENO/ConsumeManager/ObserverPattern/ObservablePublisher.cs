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
        // var ���� ���� Ÿ�� ��� ����ȯ ����
        foreach(Observer observer in observerList)
        {
            observer.Notify(this);
        }
    }
}
