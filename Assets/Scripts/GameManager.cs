using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	static GameManager instance;

	public float tickRate = 1f;
	public UnityEvent onTick;

	float nextTick;
	List<State> states;

	private void Awake()
	{
		instance = this;
		states = new List<State>();
	}

	void Start()
	{
		nextTick = Time.time + tickRate;
	}
	
	void Update()
	{
		if (nextTick < Time.time)
		{
			for (int i = 0; i < states.Count; i++)
				states[i].Tick();
			onTick.Invoke();
			nextTick += tickRate;
		}
	}

	public static void RegisterState(State state)
	{
		instance.states.Add(state);
	} 

	public static State GetRandomEnemyState(State self)
	{
		if (self == null)
			return instance.states[Random.Range(0, instance.states.Count)];
		int rnd = Random.Range(0, instance.states.Count-1);
		if (self == instance.states[rnd])
			rnd = instance.states.Count - 1;
		return instance.states[rnd];
	}

	public static void RegisterOnTick(UnityAction act)
	{
		instance.onTick.AddListener(act);
	}
}
