using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver_Controller : MonoBehaviour
{
    GameOver_BackGround go_bg;
    [TooltipAttribute("0 リスタート　１　タイトル")]
    public Texture2D[] before_img = new Texture2D[2];
    public Texture2D[] after_img = new Texture2D[2];


    enum Mode
    {
        Restart = 1,
        Title
    }

    Mode icon_pos;
    Dictionary<Mode, System.Action> modeRun;
    Dictionary<Mode, System.Action> IconMove;
    float old_y;
    // Start is called before the first frame update
    void Start()
    {
        go_bg = gameObject.GetComponent<GameOver_BackGround>();
        icon_pos = Mode.Restart;
        IconMove = new Dictionary<Mode, System.Action>()
        {
            {Mode.Restart,()=>{Screen_1(); } },
            {Mode.Title,()=>{Screen_2(); }  }
        };
        modeRun = new Dictionary<Mode, System.Action>()
        {
            {Mode.Restart,()=>{Restart(); } },
            {Mode.Title,()=>{Title(); }  }
        };
        DontDestroyOnLoad(this);
        IconMove[icon_pos]();
    }

    // Update is called once per frame
    void Update()
    {
        float y = Input.GetAxis("Pad_Vertical");
        if (go_bg.fadeObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || (y > 0 && old_y == 0))
            {
                if (icon_pos > Mode.Restart)
                {
                    icon_pos--;
                    IconMove[icon_pos]();
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || (y < 0 && old_y == 0))
            {
                if (icon_pos < Mode.Title)
                {
                    icon_pos++;
                    IconMove[icon_pos]();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
            {
                modeRun[icon_pos]();
            }
        }
        old_y = y;
    }

    private void Screen_1()
    {
        go_bg.UI[(int)icon_pos].GetComponent<Image>().sprite = Sprite.Create(after_img[0], new Rect(0, 0, after_img[0].width, after_img[0].height), Vector2.zero);
        go_bg.UI[(int)icon_pos + 1].GetComponent<Image>().sprite = Sprite.Create(before_img[1], new Rect(0, 0, before_img[1].width, before_img[1].height), Vector2.zero);
    }

    private void Screen_2()
    {
        go_bg.UI[(int)icon_pos].GetComponent<Image>().sprite = Sprite.Create(after_img[1], new Rect(0, 0, after_img[1].width, after_img[1].height), Vector2.zero);
        go_bg.UI[(int)icon_pos - 1].GetComponent<Image>().sprite = Sprite.Create(before_img[0], new Rect(0, 0, before_img[0].width, before_img[0].height), Vector2.zero);
    }

    //private void IconMove()
    //{
    //    Vector3 pos = go_bg.UI[(int)icon_pos].transform.position;
    //    pos.x -= go_bg.UI[(int)icon_pos].GetComponent<RectTransform>().sizeDelta.x + go_bg.UI[3].GetComponent<RectTransform>().sizeDelta.x;
    //    go_bg.UI[3].transform.position = pos;
    //}

    void Restart()
    {
        Time.timeScale = 1;
        ShowStart.showStart = false;
        var titleName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(titleName);
        go_bg.fadeObject.SetActive(false);

    }

    void Title()
    {
        Time.timeScale = 1;
        ShowStart.showStart = true;
        SceneManager.LoadScene("Start");
        go_bg.fadeObject.SetActive(false);
    }


}
