using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T>
{
	FSM<T> m_FSM;
	public GameObject gameObject
	{
		get => m_FSM.gameObject;
	}
	public Transform transform
	{
		get => m_FSM.gameObject.transform;
	}
	public FSM<T> FSM { get => m_FSM; set => m_FSM = value; }


	private T m_eState;
	public T state
	{
		get => m_eState;
		set => m_eState = value;
	}

	public abstract void Init();
	public abstract void Update();
	public abstract void FixedUpdate();
	public abstract void LateUpdate();
	public abstract void Release();
}
