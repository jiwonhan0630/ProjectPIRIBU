﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//캐릭터 SM은 오직 캐릭터 State만 요청해야 오류가 없다.
public class CharacterStateMachine : StateMachine {
	protected Character character;
	public CharacterStateMachine(Character _character, States _beginState) {
		character = _character;
		SetState(_beginState);
	}

	public Character GetCharacter() {
		return character;
	}
}

public class CharacterState : State {
	protected Character character;
	//protected UnitCharacter unitCharacter;

	public override void Init() {
		base.Init();
		character = sm.GetChildClass<CharacterStateMachine>().GetCharacter();
		//unitCharacter = character.unit;
	}

}