
// Andrés Villalobos | andresalvivar@gmail.com | @matnesis
// 2015/06/17 01:44:37 PM


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class React : MonoBehaviour
{
	[Header("Info")]
	public string currentReaction = "";
	public string lastReactionExecuted = "";

	[Header("ReactBase Queue")]
	public List<ReactBase> linear;
	public List<ReactBase> untilCondition;

	[Header("Config")]
	public bool autoStart = true;
	public float tick = 0.1f;

	private Coroutine coLinearExecution;
	private Coroutine coUntilConditionExecution;


	void Start()
	{
		if (autoStart)
			Play();
	}


	public void Play()
	{
		Stop();

		coLinearExecution = StartCoroutine(ExecuteLinear());
		coUntilConditionExecution = StartCoroutine(ExecuteUntilCondition());
	}


	public void Stop()
	{
		if (coLinearExecution != null)
			StopCoroutine(coLinearExecution);

		if (coUntilConditionExecution != null)
			StopCoroutine(coUntilConditionExecution);
	}


	IEnumerator ExecuteLinear()
	{
		foreach (ReactBase r in linear)
		{
			yield return new WaitForSeconds(tick);

			if (r.Condition())
				yield return StartCoroutine(r.Action());
		}

		yield return null;

		StartCoroutine(ExecuteLinear());
	}


	IEnumerator ExecuteUntilCondition()
	{
		bool stopTheRest = false;

		foreach (ReactBase r in untilCondition)
		{
			// +Info
			currentReaction = r.GetType().Name;


			// Stop all reactions only
			if (stopTheRest)
			{
				r.Stop();
				continue;
			}


			// Evaluation
			if (r.Condition())
			{
				// +Info
				lastReactionExecuted = r.GetType().Name;

				// Execution
				yield return StartCoroutine(r.Action());

				// No more evaluations until the next execution
				stopTheRest = true;
			}
			else
			{
				// To avoid leftovers when the last action executed is the
				// current else condition #experimental
				r.Stop();
			}


			// Tick
			yield return new WaitForSeconds(tick);
		}


		// Wait and retry
		yield return new WaitForEndOfFrame();
		StartCoroutine(ExecuteUntilCondition());
	}
}
