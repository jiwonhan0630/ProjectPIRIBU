﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : SingleTon<SceneChanger>
{

    public Image loadingBarImage;
    public CanvasGroup canvasGroup;
    public Image whitePanel;

    private float fadeTimer;
    private float currentFadeSpeed;
    private float currentLoadingFadeSpeed;

    private float loadingTimer;

    public string loadSceneName;

    /// <summary>
    /// 씬 로딩 중인가?
    /// </summary>
    public bool isLoading;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Init()
    {
        base.Init();
        currentFadeSpeed = 1f / 2f;
        currentLoadingFadeSpeed = 1f / 2f;
        loadingBarImage.fillAmount = 0f;
    }

    private void Start()
    {
    }


    public void LoadScene(string _sceneName)
    {
        isLoading = true;
        //gameObject.SetActive(true);

        SceneManager.sceneLoaded += LoadSceneEnd;
        loadSceneName = _sceneName;

        StartCoroutine(LoadSceneAsync(_sceneName, Color.black));

    }
    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {

        if (scene.name == loadSceneName)

        {
            StartCoroutine(FadeAlphaCanvasGroup(1f, 0f));

            SceneManager.sceneLoaded -= LoadSceneEnd;
            if (loadSceneName == "HomeScene")
            {
                UIManager.Instance.SetActiveTrueOnlyThisCanvasObject("MainMenuCanvas");

            }
            else if (loadSceneName == "InGameScene")
            {
                UIManager.Instance.SetActiveTrueOnlyThisCanvasObject("HUDCanvas");
            }
            isLoading = false;
        }

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_sceneName">로드하고 싶은 씬의 이름~</param>
    /// <param name="_fadeColor">어떤 색의 배경으로 페이드 인/페이드 아웃 할 것인가?</param>
    /// <returns></returns>
    private IEnumerator LoadSceneAsync(string _sceneName, Color _fadeColor)
    {
        whitePanel.gameObject.SetActive(true);
        loadingBarImage.gameObject.SetActive(true);
        whitePanel.color = _fadeColor;
        loadingBarImage.fillAmount = 0f;
        loadingTimer = 0f;

        yield return StartCoroutine(FadeAlpha(0f, 1f));


        AsyncOperation loadOp = SceneManager.LoadSceneAsync(_sceneName);
        loadOp.allowSceneActivation = false;

        while (loadOp.isDone==false)
        {
            yield return YieldInstructionCache.WaitForEndOfFrame;
            loadingTimer += Time.unscaledDeltaTime;

            if (loadOp.progress<0.9f)
            {
                loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, loadOp.progress, loadingTimer);
            }
            else
            {
                loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, 1f, loadingTimer);
                if (loadingBarImage.fillAmount==1f)
                {
                    yield return new WaitForSecondsRealtime(1f);
                    loadOp.allowSceneActivation = true;
                    yield break;
                }
            }

        }
        
    }

    private IEnumerator FadeAlphaCanvasGroup(float _startAlphaValue,float _endAlphaValue)
    {
        canvasGroup.alpha =_startAlphaValue;
        float _tempAlpha = _startAlphaValue;
        fadeTimer = 0f;
        while (canvasGroup.alpha!= _endAlphaValue)
        {
            fadeTimer += Time.deltaTime * currentFadeSpeed;
            _tempAlpha = Mathf.Lerp(_startAlphaValue, _endAlphaValue, fadeTimer);
            canvasGroup.alpha = _tempAlpha;

            yield return YieldInstructionCache.WaitForEndOfFrame;

            if (Mathf.Abs(canvasGroup.alpha - _endAlphaValue) <= 0.005f)
            {
                canvasGroup.alpha = _endAlphaValue;
            }
        }


        if (_endAlphaValue == 0f)
        {
            whitePanel.gameObject.SetActive(false);
            //   StartCoroutine(FadeAlphaLoadingBar(1f, 0f));
            loadingBarImage.gameObject.SetActive(false);
        }
    }
    private IEnumerator FadeAlpha(float _startAlphaValue, float _endAlphaValue)
    {
        whitePanel.canvasRenderer.SetAlpha(_startAlphaValue);
        float _tempAlpha = _startAlphaValue;
        fadeTimer = 0f;
        while (whitePanel.canvasRenderer.GetAlpha()!=_endAlphaValue) 
        {
            fadeTimer += Time.deltaTime * currentFadeSpeed;
            _tempAlpha = Mathf.Lerp(_startAlphaValue, _endAlphaValue, fadeTimer);
            whitePanel.canvasRenderer.SetAlpha(_tempAlpha);

            yield return YieldInstructionCache.WaitForEndOfFrame;

            if (Mathf.Abs(whitePanel.canvasRenderer.GetAlpha()-_endAlphaValue)<=0.005f)
            {
                whitePanel.canvasRenderer.SetAlpha(_endAlphaValue);
            }
        }


        if (_endAlphaValue ==0f)
        {
            whitePanel.gameObject.SetActive(false);
         //   StartCoroutine(FadeAlphaLoadingBar(1f, 0f));
            loadingBarImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeAlphaLoadingBar(float _startAlphaValue, float _endAlphaValue)
    {

        if (_startAlphaValue==1f)
        {
            loadingBarImage.fillAmount = 1f;
        }
        loadingBarImage.canvasRenderer.SetAlpha(_startAlphaValue);
        float _tempAlpha = _startAlphaValue;
        fadeTimer = 0f;
        while (loadingBarImage.canvasRenderer.GetAlpha() != _endAlphaValue)
        {
            fadeTimer += Time.deltaTime * currentLoadingFadeSpeed;
            _tempAlpha = Mathf.Lerp(_startAlphaValue, _endAlphaValue, fadeTimer);
            loadingBarImage.canvasRenderer.SetAlpha(_tempAlpha);

            yield return YieldInstructionCache.WaitForEndOfFrame;

            if (Mathf.Abs(loadingBarImage.canvasRenderer.GetAlpha() - _endAlphaValue) <= 0.005f)
            {
                loadingBarImage.canvasRenderer.SetAlpha(_endAlphaValue);
            }
        }


        if (_endAlphaValue == 0f)
        {
            loadingBarImage.gameObject.SetActive(false);
        }
    }

}