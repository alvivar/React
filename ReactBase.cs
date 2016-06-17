
// @matnesis
// 2015/06/17 01:44:37 PM


using System.Collections;
using UnityEngine;

public abstract class ReactBase : MonoBehaviour
{
    public abstract bool Condition();

    public abstract IEnumerator Action();

    public abstract void Stop();
}
