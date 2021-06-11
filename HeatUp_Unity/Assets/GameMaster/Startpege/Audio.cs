using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip audioClip;
    public int interval = 60;
    private AudioSource audioSource;
    private bool oneclick = false;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        StartCoroutine(PlayTitle());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !oneclick || Input.GetKeyDown("joystick button 7") && !oneclick) //マウス左クリック、スペースキーを押した場合
        {
            oneclick = true;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    private IEnumerator PlayTitle()
    {
        while (!oneclick) 
        {
            yield return new WaitForSeconds(interval);
            if (oneclick) 
            {
                yield break;
            }
            audioSource.Play();
        }
        yield break;
    }
}
