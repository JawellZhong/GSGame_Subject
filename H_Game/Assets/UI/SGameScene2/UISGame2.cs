using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISGame2 : MonoBehaviour
{

    int mCurLevel;
    int mCurExp;

    Transform mMeinv;       // 美女图片
    Transform mLevelText;   // 等级文字   
    List<Transform> mAllButton;     // 所有按钮
    TweenPosition CurClickTween;    // 当前的点击的物体

    Transform mPlayerMove;   // 玩家出手图片
    Transform mAIMove;       // AI出手图片
    Transform mResult;       // 当前结果图片

    // 剪刀 0 石头 2 布 3
    int mPlayerHit;     // 玩家出手数字
    int mAIHit;         // AI出手数字

    private void Awake()
    {
        // 设置关卡测试
        SetLevel(1);

        Init();
    }

    private void Init()
    {
        mAllButton = new List<Transform>();

        Transform mScissorsButton = transform.Find("Scissors");
        if (mScissorsButton != null)
        {
            UIEventListener.Get(mScissorsButton.gameObject).onClick += OnClickGuess;
            EventDelegate.Add(mScissorsButton.GetComponent<TweenPosition>().onFinished, OnTweenFinish);
            mAllButton.Add(mScissorsButton);
        }

        Transform mStoneButton = transform.Find("Stone");
        if (mStoneButton != null)
        {
            UIEventListener.Get(mStoneButton.gameObject).onClick += OnClickGuess;
            EventDelegate.Add(mStoneButton.GetComponent<TweenPosition>().onFinished, OnTweenFinish);
            mAllButton.Add(mStoneButton);
        }

        Transform mClothButton = transform.Find("Cloth");
        if (mClothButton != null)
        {
            UIEventListener.Get(mClothButton.gameObject).onClick += OnClickGuess;
            EventDelegate.Add(mClothButton.GetComponent<TweenPosition>().onFinished, OnTweenFinish);
            mAllButton.Add(mClothButton);
        }

        mPlayerMove = transform.Find("Player");
        mAIMove = transform.Find("AI");
        mResult = transform.Find("Result");
        mMeinv = transform.Find("Meinv");
        mLevelText = transform.Find("Top/Level");
        SetMeinv();
    }

    /// <summary>
    /// 开始选择等级后的初始化
    /// </summary>
    /// <param name="l"></param>
    public void SetLevel(int l)
    {
        mCurLevel = l;
        mCurExp = 1;
    }

    void SetMeinv()
    {
        string name = string.Format("m2meinv{0}{1}.jpg", mCurLevel, mCurExp);
        mMeinv.GetComponent<UISprite>().spriteName = name;
        mLevelText.GetComponent<UILabel>().text = mCurLevel + "-" + mCurExp;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    /// <summary>
    /// 点击某一个猜拳
    /// </summary>
    /// <param name="go"></param>
    void OnClickGuess(GameObject go)
    {
        // 点击就播放动画移动到中央
        TweenPosition tp = go.GetComponent<TweenPosition>();
        CurClickTween = tp;
        tp.PlayForward();

        // 其他按钮消失
        for (int i = 0; i < mAllButton.Count; i++)
        {
            if (mAllButton[i] != go.transform)
                mAllButton[i].gameObject.SetActive(false);
            else
                mPlayerHit = i; // 记录当前的出拳
        }
    }

    /// <summary>
    /// 动画结束
    /// </summary>
    void OnTweenFinish()
    {
        if (CurClickTween == null) return;

        // 动画播完回到初始位置, 然后隐藏
        CurClickTween.ResetToBeginning();
        CurClickTween.gameObject.SetActive(false);

        // 进行PK
        StartCoroutine(PK());
    }

    /// <summary>
    /// PK
    /// </summary>
    /// <returns></returns>
    IEnumerator PK()
    {
        // Player 图片移动
        ChangedSpriteName(mPlayerMove, CurClickTween.GetComponent<UISprite>().spriteName);

        // AI 图片移动
        string[] tmpAI = new string[] { "jian2", "quan2", "bu2" }; // 图片名字
        mAIHit = Random.Range(0, 3); // 随机出拳
        ChangedSpriteName(mAIMove, tmpAI[mAIHit]);

        // PK结果
        int result = (mPlayerHit - mAIHit + 3) % 3;
        string tName = "";
        switch (result)
        {
            case 0:
                //平局
                tName = "cq_jg_3";
                break;
            case 1:
                //玩家赢
                tName = "cq_jg_1";
                break;
            case 2:
                //AI赢
                tName = "cq_jg_2";
                break;
            default:
                break;
        }
        mResult.gameObject.SetActive(true);
        mResult.GetComponent<UISprite>().spriteName = tName;

        // 重新开始
        yield return new WaitForSeconds(1f);
        ReStart(result);
    }

    /// <summary>
    /// 换图片并移动
    /// </summary>
    /// <param name="t"></param>
    /// <param name="name"></param>
    void ChangedSpriteName(Transform t, string name)
    {
        // 玩家Hit换图片
        t.gameObject.SetActive(true);
        t.GetComponent<UISprite>().spriteName = name;
        TweenPosition tpp = t.GetComponent<TweenPosition>();
        tpp.PlayForward();
    }

    /// <summary>
    /// resule 为结果， 0平局， 1赢， 2输
    /// </summary>
    /// <param name="resule"></param>
    void ReStart(int resule)
    {
        mResult.gameObject.SetActive(false);

        mPlayerMove.GetComponent<TweenPosition>().ResetToBeginning();
        mPlayerMove.gameObject.SetActive(false);

        mAIMove.GetComponent<TweenPosition>().ResetToBeginning();
        mAIMove.gameObject.SetActive(false);

        for (int i = 0; i < mAllButton.Count; i++)
        {
            mAllButton[i].gameObject.SetActive(true);
        }

        // 结果导致背景变化
        if (resule == 1)// 赢， 进阶
        {
            mCurLevel += ++mCurExp > 3 ? 1 : 0;
            if (mCurExp > 3) mCurExp = 1;
        }
        else if (resule == 2) // 输， 退格
        {
            if (mCurExp >= 2)
            {
                mCurExp--;
            }
        }
        SetMeinv();
    }
}