using System.Collections;
using UnityEngine;

public abstract class Assistant : MonoBehaviour
{
    public abstract IEnumerator CreateService();
    public abstract void AnswerRequest(string spokenText);
}