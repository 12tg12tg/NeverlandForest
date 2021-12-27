using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T> : MonoBehaviour
{
    private Dictionary<T, State<T>> stateList = new Dictionary<T, State<T>>();
    public T curState { get; set; }
    public T preState { get; set; }

    public virtual void Update()
    {
        stateList[curState].Update();
    }

    public virtual void FixedUpdate()
    {
        stateList[curState].FixedUpdate();
    }
    public virtual void LateUpdate()
    {
        stateList[curState].LateUpdate();
    }
    public void AddState(T name, State<T> state)
    {
        state.FSM = this;
        stateList.Add(name, state);
    }
    public void ChangeState(T state)
    {
        if (curState.Equals(state))
            return;

        preState = curState;
        stateList[curState].Release();

        curState = state;
        stateList[state].Init();
    }
    public void SetState(T state)
    {
        preState = state;
        curState = state;
        stateList[state].Init();
    }
}
