using UnityEngine;

public abstract class AStateModifier : ScriptableObject
{
	public abstract void Apply(State state);
}