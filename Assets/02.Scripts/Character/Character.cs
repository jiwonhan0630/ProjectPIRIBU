﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public T GetChildClass<T>() where T : Character {
		return (T)this;
	}

	[HideInInspector]
	public Unit unit;
	public StateMachine stateMachine;
	public Animator animator;

}
