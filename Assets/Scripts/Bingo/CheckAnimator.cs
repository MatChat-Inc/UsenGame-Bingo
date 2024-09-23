﻿using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class CheckAnimator : MonoBehaviour
{
    const float __default_speed__ = 5;
    const int __cell_height__ = 500;
    public AwardNumObj[] m_Texts;
    public float m_speed = __default_speed__;
    bool m_isAnimate = false;
    bool m_isEaseToStop;
    bool m_rotateStopped = true;
    private bool m_waitingBingo;
    GameDataHandler m_gameData;
    float[] m_progress = new[] {0f, 1f, 2f, 3f};
    Vector3[] m_animPositions = new[] {Vector3.up*__cell_height__*2, Vector3.up*__cell_height__, Vector3.zero, Vector3.down*__cell_height__};

    Coroutine m_easeCoroutine;
    bool m_isNeedResetTextsPos;
    public Action AnimateFinishedCallback;

    void Update()
    {
        if (!m_isAnimate)
        {
            return;
        }

        float t = Time.deltaTime * m_speed;
        for (int i = 0; i < m_Texts.Length; i++)
        {
            m_progress[i] += t;
            // Debug.Log(MoveNextPosition(i));
            m_Texts[i].gameObject.transform.localPosition = MoveNextPosition(i);
        }

        if (m_isNeedResetTextsPos) {
            m_isNeedResetTextsPos = false;
            ResetRotateTextsPos();
        }
    }

    Vector3 MoveNextPosition(int i) {
        int index = Mathf.FloorToInt(m_progress[i]);
        if (index > m_animPositions.Length - 2) {
            m_progress[i] -= index;
            index = 0;
            if (i == 2 && m_isEaseToStop) {
                m_isAnimate = false;
                StartCoroutine(WaitToTriggerNextRound());
                m_gameData.SetCellChecked(int.Parse(m_Texts[1].text)-1);
                AudioManager.Instance.PlayNumberCheckEffect();
                m_isNeedResetTextsPos = true;
                m_rotateStopped = true;
                AnimateFinishedCallback();
            }
            m_Texts[i].text = m_gameData.GetUncheckedNumber().ToString();
            return m_animPositions[index];
        }else {
            return Vector3.Lerp(m_animPositions[index], m_animPositions[index + 1], m_progress[i]-index);
        }
    }

    void ResetRotateTextsPos() {
        for (int i = 0; i < m_Texts.Length; i++)
        {
            m_Texts[i].gameObject.transform.localPosition = m_animPositions[i+1];
        }
    }

    IEnumerator<WaitForSeconds> EaseAnimSpeed() {
        m_waitingBingo = true;
        m_speed = m_speed * 0.3f;
        yield return new WaitForSeconds(1f);
        m_isEaseToStop = true;
    }

    IEnumerator<WaitForEndOfFrame> EaseAnimSpeedOut()
    {
        m_waitingBingo = true;
        float timeInterval = 0f;
        float duration = 2f + AppConfig.Instance.rotateEaseExtraTime;
        while (true)
        {
            var x = timeInterval / duration;
            var y = -x * x + 2 * x;
            m_speed = __default_speed__ * (1 - y);

            yield return new WaitForEndOfFrame();
            if (m_speed < 0.5f)
                break;
            timeInterval += Time.deltaTime;
        }
        m_isEaseToStop = true;
    }

    IEnumerator<WaitForSeconds> WaitToTriggerNextRound() {
        yield return new WaitForSeconds(1);
        m_waitingBingo = false;
        AudioManager.Instance.StopNumberRotateEffect();
    }

    public void Animate(GameDataHandler gameData) {
        m_gameData = gameData;
        if (!m_waitingBingo) {
            if (m_isAnimate) {
                if (AppConfig.Instance.rotateEaseExtraTime == 0)
                    m_easeCoroutine = StartCoroutine(EaseAnimSpeed());
                else
                    m_easeCoroutine = StartCoroutine(EaseAnimSpeedOut());
            } else {
                m_speed = __default_speed__;
                m_rotateStopped = false;
                m_isAnimate = true;
                m_isEaseToStop = false;
                AudioManager.Instance.PlayNumberRotateEffect();
            }
        }
    }

    public void ResetCheckTexts() {
        for (int i = 0; i< m_Texts.Length; i++)
        {
            var textView = m_Texts[i];
            if (i == 1)
            {
                textView.text = "0";
            }else
            {
                textView.text = UnityEngine.Random.Range(0, AppConfig.Instance.MaxCellCount - 1).ToString();
            }
        }
    }

    public bool isAnimteFinished() {
        return m_rotateStopped;
    }

    public bool isAnimating() {
        return m_isAnimate;
    }

    public void ForceStop() {
        StopRotateAnimate();
        if (m_easeCoroutine != null)
            StopCoroutine(m_easeCoroutine);
    }

    public void StopRotateAnimate() {
        m_waitingBingo = false;
        m_isEaseToStop = false;
        m_rotateStopped = true;
        if (m_isAnimate) {
            m_isAnimate = false;
            m_progress = new[] {0f, 1f, 2f, 3f};
            for (int i = 0; i < m_Texts.Length; i++)
            {
                m_Texts[i].gameObject.transform.localPosition = m_animPositions[i];
            }
        }
        AnimateFinishedCallback();
    }

    public void OnThemeTypeChanged() {
        for (int i = 0; i < m_Texts.Length; i++)
        {
            m_Texts[i].text = m_Texts[i].text;
        }
    }

    public void UpdateAwardNumTheme() {
        for (int i = 0; i < m_Texts.Length; i++)
        {
            m_Texts[i].UpdateTextTheme();
        }
    }

}