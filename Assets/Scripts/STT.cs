using System.Collections;
using UnityEngine;

public abstract class STT : MonoBehaviour
{
    public abstract IEnumerator CreateService();
    public abstract bool Active { get; set; }
    public abstract void RecordAgain();
}
