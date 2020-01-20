using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour
{
    public float GameEndDelay = 3.0f;
    public float FadeDuration = 2.0f;
    public GameObject VictoryIndicator;
    public GameObject DefeatIndicator;
    public GameObject ContinuePrompt;
    public GameResult GameResult { get; set; } = GameResult.NYD;

    private bool _resultShown = false;
    private bool _promptShown = false;

    private void Update()
    {
        if (this.GameResult != GameResult.NYD)
        {
            GameEndDelay -= Time.deltaTime;

            if (!this._resultShown)
            {
                if (this.GameResult == GameResult.Victory)
                {
                    // FadeUIImageIn(this.VictoryIndicator);
                    this.VictoryIndicator.SetActive(true);
                    StartCoroutine(new VisualUtils().FadeWithImageTo(this.VictoryIndicator, 1.0f, FadeDuration)); 
                }
                else
                {
                    // FadeUIImageIn(this.DefeatIndicator);
                    this.DefeatIndicator.SetActive(true);
                    StartCoroutine(new VisualUtils().FadeWithImageTo(this.DefeatIndicator, 1.0f, FadeDuration));
                }

                this._resultShown = true;
            }

            if (GameEndDelay < 0)
            {
                if (!this._promptShown)
                {
                    this.ContinuePrompt.SetActive(true);
                    this._promptShown = true;
                }

                if (Input.anyKey)
                {
                    ReturnToMainMenu();
                }
            }
        }
    }

    //public void FadeUIImageIn(GameObject objectToFade)
    //{
    //    Image image = objectToFade.GetComponent<Image>();
    //    Color endColor = image.color;
    //    Color startColor = endColor;
    //    startColor.a = 100;
    //    image.color = startColor;
    //    gameObject.SetActive(true);
    //    objectToFade.GetComponent<Image>().CrossFadeColor(endColor, this.FadeDuration, false, true);
    //}

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
