
// Queue-like workflow for simple AI systems. ReactBase compatible.

// @matnesis
// 2015/06/17 01:44:37 PM


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class React : MonoBehaviour
{
	[Header("Shared Data")]
	public Transform target;

	[Header("Config")]
	public float tick = 0.1f; // Time to rest between ReactBase components evaluation

	[Header("ReactBase Queue")]
	public List<ReactBase> untilCondition; // Main queue to evaluate

	[Header("Info")]
	public string currentReaction = ""; // ReactBase component currently evaluated
	public string lastReactionExecuted = ""; // Last ReactBase executed (Condition == true)

	private Coroutine coUntilConditionExecution; // Main coroutine


	void Start()
	{
		StartCoroutine(Play());
	}


	public IEnumerator Play()
	{
		// Waiting for Start
		yield return new WaitForEndOfFrame();

		// Restart
		Stop();
		coUntilConditionExecution = StartCoroutine(ExecuteUntilCondition());
	}


	public void Stop()
	{
		// Stops the main coroutine
		if (coUntilConditionExecution != null)
			StopCoroutine(coUntilConditionExecution);
	}


	IEnumerator ExecuteUntilCondition()
	{
		bool stopTheRest = false;


		foreach (ReactBase r in untilCondition)
		{
			// +Debug info
			currentReaction = r.GetType().Name;


			// Stop all reactions "mode"
			if (stopTheRest)
			{
				r.Stop();
				continue;
			}


			// Evaluation
			if (r.Condition())
			{
				// +Debug info
				lastReactionExecuted = r.GetType().Name;

				// Coroutine execution
				yield return StartCoroutine(r.Action());

				// Flag to stop the rest of evaluations
				stopTheRest = true;
			}
			else
			{
				// To avoid leftovers when the last action executed is the
				// current reaction to evaluate
				r.Stop();
			}


			// Rest time between evaluation
			yield return new WaitForSeconds(tick);
		}


		// Wait and retry
		yield return new WaitForEndOfFrame();
		coUntilConditionExecution = StartCoroutine(ExecuteUntilCondition());
	}
}
