using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator sam;
    int oHash = Animator.StringToHash("o");

    // Start is called before the first frame update
    void Start()
    {
      sam = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void speak(string text, float time)
    {
        string completeText = text.ToUpper();

        float completeTime = time;

        if (!string.IsNullOrEmpty(completeText))
        {
            foreach (char c in completeText)
            {
                switch (c)
                {
                    case ' ':
                    case '!':
                    case ',':
                    case '.':
                    //   break;
                    case 'A':
                    case 'I':
                    //   break;
                    case 'E':
                        sam.SetTrigger("E");
                        break;
                    case 'U':
                    case 'J':
                    //  break;
                    case 'M':
                    case 'B':
                    case 'P':
                    //  break;
                    case 'F':
                    case 'V':
                    // break;
                    case 'O':
                        sam.SetTrigger("E");
                        break;
                    case 'W':
                    case 'Q':
                    //  break;
                    case 'L':
                    case 'T':
                    case 'H':
                    // break;
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
                        sam.SetTrigger("D");
                        break;
                }
            }
        }
    }
}
