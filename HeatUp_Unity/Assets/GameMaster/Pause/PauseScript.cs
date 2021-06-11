using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    //　ポーズした時に表示するUI
    [SerializeField, TooltipAttribute("PauseScrean")]
    private GameObject pauseUI;

    [SerializeField, TooltipAttribute("StartCameraScript")]
    private startCamera startCamera;

    [SerializeField, TooltipAttribute("剣ないUI 0:コンティニュー　1:リスタート　2:タイトル")]
    Texture2D[] before_img = new Texture2D[3];
    [SerializeField, TooltipAttribute("剣あるUI")]
    Texture2D[] after_img = new Texture2D[3];

    private bool enablePouse;

    [SerializeField, TooltipAttribute("0:コンティニュー　1:リスタート　2:タイトル")]
    GameObject[] UI = new GameObject[3];
    float old_y;
    enum Mode
    {
        Continue,
        Restart,
        Title
    }
    Mode icon_pos;

    Dictionary<Mode, System.Action> modeRun;

    private void Start()
    {
        icon_pos = Mode.Continue;

        modeRun = new Dictionary<Mode, System.Action>()
        {
            {Mode.Continue,()=>{pauseUI.SetActive(!pauseUI.activeSelf); Time.timeScale = 1; icon_pos = Mode.Continue; } },
            {Mode.Restart ,()=> {ReStart(); /*Debug.Log("Restert")*/;} },
            {Mode.Title ,()=>{Debug.Log("Title"); GotoTitle(); } }
        };

    }

    private void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0.5f) { return; }
        if (startCamera)
        {
            enablePouse = startCamera.enablePause;
        }
        else
        {
            enablePouse = true;
        }
        if (!enablePouse)
        {
            return;
        }

        float y = Input.GetAxis("Pad_Vertical");

        if (Input.GetKeyDown("q") || Input.GetKeyDown("joystick button 7"))
        {
            //　ポーズUIのアクティブ、非アクティブを切り替え
            pauseUI.SetActive(!pauseUI.activeSelf);

            //　ポーズUIが表示されてる時は停止
            if (pauseUI.activeSelf)
            {
                Time.timeScale = 0f;
                IconMove(1, 0);
                //　ポーズUIが表示されてなければ通常通り進行
            }
            else
            {
                icon_pos = Mode.Continue;
                Time.timeScale = 1f;
            }
        }

        if (pauseUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || (y > 0 && old_y == 0))
            {
                if (icon_pos > Mode.Continue)
                {
                    int old = (int)icon_pos;
                    icon_pos--;
                    IconMove(old, (int)icon_pos);
                }

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || (y < 0 && old_y == 0))
            {
                if (icon_pos < Mode.Title)
                {
                    int old = (int)icon_pos;
                    icon_pos++;
                    IconMove(old, (int)icon_pos);
                }
            }
            else if (Input.GetButtonDown("Submit"))
            {
                modeRun[icon_pos]();
            }
        }

        old_y = y;
    }

    void IconMove(int before, int after)
    {
        UI[after].GetComponent<Image>().sprite = Sprite.Create(after_img[after], new Rect(0, 0, after_img[after].width, after_img[after].height), Vector2.zero);
        UI[before].GetComponent<Image>().sprite = Sprite.Create(before_img[before], new Rect(0, 0, before_img[before].width, before_img[before].height), Vector2.zero);
    }


    void ReStart()
    {
        ShowStart.showStart = false;
        Time.timeScale = 1f;
        string titleName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(titleName);
    }

    void GotoTitle()
    {
        ShowStart.showStart = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start");
    }
}
