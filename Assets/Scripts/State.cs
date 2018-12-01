using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class State : MonoBehaviour {

	[Header("Resources")]
	public int people = 100;
	public int prisoners = 0;
	public float food = 100;
	public float gold = 100;
	public int farmers = 10;
	public int miners = 0;
	public int warriors = 0;
	public float piety = 0.5f;

	[Header("Growth")]
	[SerializeField] float baseFoodMultiplier = 3f;
	[SerializeField] float baseGoldMultiplier = 0.1f;
	[SerializeField] float baseFertility = 10f;
	[SerializeField] float foodFertility = 0.05f;
	[SerializeField] float basePietyDecay = 0.03f;
	[SerializeField] float baseFoodCost = 1f;
	[SerializeField] float baseGoldCost = 1f;
	[NonSerialized] public float foodMultiplier;
	[NonSerialized] public float goldMultiplier;
	[NonSerialized] public float fertility;
	[NonSerialized] public float pietyDecay;
	[NonSerialized] public float foodCost;
	[NonSerialized] public float goldCost;


	[Header("Status")]
	public List<AStateModifier> modifiers;
	[Range(0f,0.1f)] public float autoSacrifice = 0.05f;
	public bool aiAllocate = true;
	[Range(0f, 0.5f)] public float aiMilitary = 0.4f;
	[Range(0f, 0.5f)] public float aiMiners = 0.2f;
	[Range(0, 1000)] public int aiMilitarySize = 200;

	void Start () {
		GameManager.RegisterState(this);
	}

	public void Tick()
	{
		foodMultiplier = baseFoodMultiplier;
		goldMultiplier = baseGoldMultiplier;
		fertility = baseFertility + food * foodFertility;
		pietyDecay = basePietyDecay;
		goldCost = baseGoldCost;
		foodCost = baseFoodCost;
		for (int i = 0; i < modifiers.Count; i++)
			modifiers[i].Apply(this);
		food += farmers * foodMultiplier;
		gold += miners * goldMultiplier;
		piety -= pietyDecay;
		int humans = (prisoners + farmers + miners + warriors + people);
		if (food > humans)
		{
			food -= humans;
			people += (int)fertility;
		}
		if (autoSacrifice > 0)
			AutoSacrifice();
		if (aiAllocate)
			AiAllocate();
	}

	void AutoSacrifice()
	{
		int humans = (prisoners + farmers + miners + warriors + people);
		int sacrifice = (int)(humans * Mathf.Min(autoSacrifice, 1f - piety));
		int delta = Mathf.Min(sacrifice, prisoners);
		int total = delta;
		sacrifice -= delta;
		prisoners -= delta;
		if (sacrifice > 0)
		{
			delta = Mathf.Min(sacrifice, people);
			total += delta;
			sacrifice -= delta;
			people -= delta;
		}
		if (sacrifice > 0)
		{
			delta = Mathf.Min(sacrifice, miners);
			total += delta;
			sacrifice -= delta;
			miners -= delta;
		}
		piety += (float)total / (float)humans;
	}

	void AiAllocate()
	{
		int humans = (prisoners + farmers + miners + warriors + people);
		int delta = Mathf.CeilToInt((humans + fertility - farmers * foodMultiplier) / foodMultiplier);
		if (delta > 0 && food * 4 < humans)
		{
			delta = Mathf.Min(delta, people);
			people -= delta;
			farmers += delta;
		}
		if (people > 0)
		{
			delta = Mathf.FloorToInt((aiMilitary - (float)warriors / (float)humans) * (float)humans);
			if (delta > 0)
			{
				delta = Mathf.Min(delta, people);
				delta = Mathf.Min(delta, Mathf.FloorToInt(gold / goldCost));
				delta = Mathf.Min(delta, Mathf.FloorToInt(food / foodCost));
				people -= delta;
				warriors += delta;
				food -= delta * foodCost;
				gold -= delta * goldCost;
			}
		}
		if (people > 0)
		{
			delta = Mathf.FloorToInt((aiMiners - (float)miners / (float)humans) * (float)humans);
			if (delta > 0)
			{
				delta = Mathf.Min(delta, people);
				people -= delta;
				miners += delta;
			}
		}
	}
}
