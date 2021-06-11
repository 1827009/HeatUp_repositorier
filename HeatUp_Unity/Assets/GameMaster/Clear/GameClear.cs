using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    [TooltipAttribute("指定するキャラクター")]
    public Character character;

    [TooltipAttribute("次のステージパス")]
    public string nextStagePas;

    [TooltipAttribute("何秒後にフェイドアウトがスタートするか")]
    public float fadeDelay = 1;

    [TooltipAttribute("フェイドアウトのスピード")]
    public float speed = 1;

    private bool start = false;
    private Image image;
    private Fade[] fades;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        fades = new Fade[2];
        fades[0] = transform.GetChild(0).GetComponent<Fade>();
        fades[1] = transform.GetChild(1).GetComponent<Fade>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!character)
        {
            return;
        }
        if (character.hp <= 0 && !start) 
        {
            Clear();
        }
    }

    public void Clear()
    {
        start = true;
        foreach(Fade f in fades)
        {
            f.gameObject.SetActive(true);
        }
        Invoke("FadeStart", fadeDelay);
    }

    private void FadeStart()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float alpha = 0;
        while (alpha < 1) 
        {
            alpha += Time.deltaTime * speed;
            Color color = image.color;
            color.a = alpha;
            image.color = color;
            yield return null;
        }
        SceneManager.LoadScene(nextStagePas);
        yield break;
    }
}
