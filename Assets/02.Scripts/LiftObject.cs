﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType {
	None, SetTargetPos, SetVelocity, AddVelocity
}

/// <summary>
/// 게임내의 모든 움직이는 클래스 (캐릭터나 플랫폼)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class LiftObject : MonoBehaviour {
	[HideInInspector] public List<LiftObject> childs;//옮길 대상
	[HideInInspector] public Collider2D coll;
	[HideInInspector] public Rigidbody2D rigid;

	[HideInInspector]
	public Vector3
		prePos, currPos, targetPos, standardPos, deltaPos,
		preScale, currScale, standardScale,
		preAngle, currAngle, standardAngle,
		preNormal, currNormal, standardNormal;

	[HideInInspector]
	public float fixedUpdatePerSec;


	protected virtual void Awake() {

		childs = new List<LiftObject>();
		if (!coll) coll = GetComponent<Collider2D>();
		if (!rigid) rigid = GetComponent<Rigidbody2D>();

		UpdateCurrTransforms();
		UpdatePreTransforms();
		standardPos = currPos;
		standardScale = currScale;
		standardAngle = currAngle;
		standardNormal = currNormal;

		fixedUpdatePerSec = 1 / Time.fixedDeltaTime;
	}


	private void FixedUpdate() {

		UpdatePreTransforms();
		UpdateTransform();
		UpdateCurrTransforms();

		deltaPos = currPos - prePos;

		//자신에게 붙은 객체 위치 변경
		foreach (LiftObject child in childs) {
			//child.SetMovement(MovementType.AddVelocity, deltaPos);
			child.SetMovement(MovementType.SetTargetPos, GetLiftPosition(child));
		}
	}

	public Vector3 GetLiftPosition(LiftObject child) {
		return deltaPos + child.transform.position;
		//return (Vector3)GetVelocity() + charPos;
	} 


	//사용자 지정 이동함수 리스트
	struct MovementInput {
		public MovementType type;
		public Vector2 vector;
		public MovementInput(MovementType _type, Vector2 _vec) {
			type = _type;
			vector = _vec;
		}
	}
	private List<MovementInput> movementInputs = new List<MovementInput>();

	protected virtual void UpdateTransform() {
		if (movementInputs.Count == 0) {
			Utility.DrawDir(currPos, currNormal, Color.blue);
			return;
		}

		Vector2 movePos = transform.position;
		Vector2 totalVel = Vector2.zero;
		Vector2 targetVel = rigid.velocity;

		foreach (MovementInput input in movementInputs) {
			switch (input.type) {
				case MovementType.None:
					break;
				//Transform.Position = TargetPosition과 같은 역할.
				case MovementType.SetTargetPos:
					movePos = input.vector;
					break;
				//Rigidbody.Velocity += (초당 이동위치)와 같은 역할.
				case MovementType.AddVelocity:
					totalVel += input.vector;
					break;
				//Rigidbody.Velocity = (초당 이동위치)와 같은 역할.
				case MovementType.SetVelocity:
					targetVel = input.vector;
					break;
			}
		}
		movementInputs.Clear();

		Vector2 addVel = totalVel + (targetVel - rigid.velocity);
		//MovePos
		if (movePos != (Vector2)transform.position) {
			rigid.MovePosition(movePos + addVel*Time.fixedDeltaTime);
		}
		
		rigid.AddForce(
			addVel
			* fixedUpdatePerSec
			* (1 + Time.fixedDeltaTime * rigid.drag)
			* rigid.mass//Drag,Mass 무시
		);
		
	}

	//매프레임마다 LiftObject를 움직이는 방식은 사용자 지정
	public void SetMovement(MovementType inputType, Vector2 inputVec) {
		movementInputs.Add(new MovementInput(inputType, inputVec));
	}
	public Vector2 GetVelocity() {
		return rigid.velocity;
	}



	//1프레임 전 트랜스폼 업데이트
	protected virtual void UpdatePreTransforms() {
		prePos = currPos;
		preScale = currScale;
		preAngle = currAngle;
		preNormal = currNormal;
	}

	//현재프레임 트랜스폼 업데이트
	protected virtual void UpdateCurrTransforms() {
		currPos = transform.position;
		currAngle = transform.eulerAngles;
		currScale = transform.localScale;
		currNormal = transform.up;
	}

	public void AddChild(LiftObject child) {
		childs.Add(child);
	}

	public void RemoveChild(LiftObject child) {
		childs.Remove(child);
	}

	public void Draw() {
		Debug.DrawLine(currPos, currPos - (Vector3)GetVelocity(), Color.green, 1f);
		Debug.DrawLine(currPos, currPos + (Vector3)GetVelocity(), Color.red);
	}
}
