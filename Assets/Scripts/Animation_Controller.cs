using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Controller : MonoBehaviour
{
    private Animator _sam;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _sam = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void Animate(string text, AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
        StartCoroutine(StringToTrigger(text, clip.length));
    }

    private IEnumerator StringToTrigger(string text, float length)
    {
        string completeText = text.ToUpper();
        //completeText = completeText.Trim(new Char[] { ' ', '*', '.', '!' });
   
        float time = (length / completeText.Length);
        Debug.Log(time);

        if (!string.IsNullOrEmpty(completeText))
        {
            foreach (char c in completeText)
            {
                switch (c)
                {
                    case ' ':
                    case '*':
                    case '.':
                    case '!':
                        yield return AddToQueue("IdleOZ", time);
                        break;
                    case 'A':
                    case 'I':
                        yield return AddToQueue("A", time);
                        break;
                    case 'E':
                        yield return AddToQueue("E", time);
                        break;
                    case 'U':
                    case 'J':
                        yield return AddToQueue("U", time);
                        break;
                    case 'M':
                    case 'B':
                    case 'P':
                        yield return AddToQueue("M", time);
                        break;
                    case 'F':
                    case 'V':
                        yield return AddToQueue("V", time);
                        break;
                    case 'O':
                        yield return AddToQueue("O", time);
                        break;
                    case 'W':
                    case 'Q':
                        yield return AddToQueue("W", time);
                        break;
                    case 'L':
                    case 'T':
                    case 'H':
                        yield return AddToQueue("L", time);
                        break;
                    case 'C':
                    case 'D':
                    case 'G':
                    case 'K':
                    case 'N':
                    case 'R':
                    case 'S':
                    case 'X':
                    case 'Y':
                    case 'Z':
                        yield return AddToQueue("D", time);
                        break;
                }
            }
        }
    }

    private IEnumerator AddToQueue(string s, float time)
    {
        _sam.SetTrigger(Animator.StringToHash(s));
        yield return new WaitForSeconds(time);
    }
}
