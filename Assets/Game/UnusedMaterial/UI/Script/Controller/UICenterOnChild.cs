//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Ever wanted to be able to auto-center on an object within a draggable panel?
/// Attach this script to the container that has the objects to center on as its children.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Center Scroll View on Child")]
public class UICenterOnChild : MonoBehaviour
{
    /// <summary>
    /// The strength of the spring.
    /// </summary>
    public UIPanel Panel;
    public GameObject center;
    public float distanceX = 0;
    public float Buttonwidth = 0;
    public float distanceY = 0;
    public float Buttonheight = 0;
    public float count = 0;
    public float length = 0;
    public float springStrength = 8f;

    /// <summary>
    /// If set to something above zero, it will be possible to move to the next page after dragging past the specified threshold.
    /// </summary>

    public float nextPageThreshold = 0f;

    /// <summary>
    /// Callback to be triggered when the centering operation completes.
    /// </summary>

    public SpringPanel.OnFinished onFinished;

    UIScrollView mScrollView;
    GameObject mCenteredObject;

    Vector3 panelCenter;

    float gapToIgnore = 0.0f;

    /// <summary>
    /// Game object that the draggable panel is currently centered on.
    /// </summary>

    public GameObject centeredObject { get { return mCenteredObject; } }

    void OnEnable()
    {

    }

    void OnDragFinished()
    {
        if (enabled)
        {
            Recenter();
        }
    }

    /// <summary>
    /// Ensure that the threshold is always positive.
    /// </summary>

    void OnValidate()
    {
        nextPageThreshold = Mathf.Abs(nextPageThreshold);
    }

    /// <summary>
    /// Recenter the draggable list on the center-most child.
    /// </summary>

    public void Recenter(bool inFirstTime = false)
    {
        if (mScrollView == null)
        {
            mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);

            if (mScrollView == null)
            {
                enabled = false;

                return;
            }
            else
            {
                mScrollView.onDragFinished = OnDragFinished;

                if (mScrollView.horizontalScrollBar != null)
                    mScrollView.horizontalScrollBar.onDragFinished = OnDragFinished;

                if (mScrollView.verticalScrollBar != null)
                    mScrollView.verticalScrollBar.onDragFinished = OnDragFinished;
            }
        }

        if (mScrollView.panel == null)
        {
            return;
        }

        // Calculate the panel's center in world coordinates
        Vector3[] corners = mScrollView.panel.worldCorners;

        //知道总屏幕宽度 / 知道世界坐标总屏幕宽度 =每一个世界坐标等于多少单位
        //知道按钮宽度  / 每一个世界坐标等于多少单位=按钮是多少世界坐标单位
        float widthX = (Buttonwidth / (Panel.width / (corners[2].x - corners[0].x))) / 2;
        //得到边框在实际世界坐标里的位子
        float posX = distanceX / (Panel.width / (corners[2].x - corners[0].x));

        //知道总屏幕宽度 / 知道世界坐标总屏幕宽度 =每一个世界坐标等于多少单位
        //知道按钮宽度  / 每一个世界坐标等于多少单位=按钮是多少世界坐标单位
        float heightY = (Buttonheight / (Panel.height / (corners[2].y - corners[0].y))) / 2.0f;
        //得到边框在实际世界坐标里的位子
        float posY = distanceY / (Panel.height / (corners[2].y - corners[0].y));

        switch (mScrollView.movement)
        {
            case UIScrollView.Movement.Horizontal:
                {
                    // the zero condition is for the very first click, we want to keep it at the very first location
                    if ((UICamera.lastTouchPosition - mScrollView.FirstPosition).x <= -gapToIgnore || inFirstTime)
                    {
                        panelCenter = (new Vector3(corners[0].x + widthX + posX, corners[0].y, corners[0].z));
                    }
                    else if ((UICamera.lastTouchPosition - mScrollView.FirstPosition).x >= gapToIgnore)
                    {
                        panelCenter = (new Vector3(corners[2].x - widthX - posX, corners[2].y, corners[2].z));
                    }
                    else
                    {
                        // use old panel center, don't give it a new value
                    }

                    break;
                }

            case UIScrollView.Movement.Vertical:
                {
                    // the zero condition is for the very first click, we want to keep it at the very first location
                    if ((UICamera.lastTouchPosition - mScrollView.FirstPosition).y >= gapToIgnore || inFirstTime)
                    {
                        panelCenter = (new Vector3(corners[2].x, corners[2].y - heightY - posY, corners[2].z));
                    }
                    else if ((UICamera.lastTouchPosition - mScrollView.FirstPosition).y <= -gapToIgnore)
                    {
                        panelCenter = (new Vector3(corners[0].x, corners[0].y + heightY + posY, corners[0].z));
                    }
                    else
                    {
                        // use old panel center, don't give it a new value
                    }

                    break;
                }
        }

        // length == count is more for when we have exact amount of buttons and meant for fill the whole screen (or at least parts) for main menus and such
        // if we have whole numbers of buttons we can probably use this too
        // if all of the buttons together cannot fill the screen (not exactly a count and length but close if multiplied) we should use this too, other one is buggy for that purpose

        // for main menu and sub menus, this is different right now. Will investigate this tmr.

        // for main menu, single line
        // the first part doesn't work from 0.00000001 to somewhere in the middle but works in the higher digits once we can flip over
        // the first part definitely works for 2 number equal or in exact multiples
        // the second part definitely works for between 0.000001 and 0.4ish

        // for multi row stuff.....
        // first part doesn't work very well honestly, it works well for the most part but NOT when we are poking out a bit. the < 0.5 stuff
        // first part, it definitely works for gap+width > 0.5f stuff
        // second part, definitely works for the > 0.5f stuff
        // second part, bugs rarely happen but < 0.5 stuff not so good...

        // also test the single row vs multi row stuff.... and main menu single row or scrollpanel single row

        // single line (both main menu and scroll panel) "==" can be handled by the first part, < 0.Xf can be handled by second part, > 0.Xf can be handled by first part?
        // multi line (scroll panel only) "==" can be handled by the first part, < 0.Xf can be handled by second part (with minor end zone bug, possibly exists but un-discoverable in main menu single row but discoverable in scroll panel single roll), > 0.Xf can be handled by first part

        // we can fix all these by fixing the second part's minor bug and find what is 0.Xf
        // we can also fix this by fixing the < 0.Xf part using part one and probably have to find what 0.Xf is

        // Offset this value by the momentum
        Vector3 pickingPoint = panelCenter - mScrollView.currentMomentum * (mScrollView.momentumAmount * 0.1f);
        mScrollView.currentMomentum = Vector3.zero;

        float min = float.MaxValue;
        Transform closest = null;
        Transform trans = transform;
        int index = 0;

        // Determine the closest child
        for (int i = 0, imax = trans.childCount; i < imax; ++i)
        {
            Transform t = trans.GetChild(i);
            float sqrDist = Vector3.SqrMagnitude(t.position - pickingPoint);

            if (sqrDist < min)
            {
                min = sqrDist;
                closest = t;
                index = i;
            }
        }

        switch (mScrollView.movement)
        {
            case UIScrollView.Movement.Horizontal:
                {
                    // check if the closest child checks out
                    Vector3 LeftMostCenterPosition = new Vector3(corners[0].x + widthX + posX, corners[0].y, corners[0].z);
                    Vector3 RiteMostCenterPosition = new Vector3(corners[2].x - widthX - posX, corners[2].y, corners[2].z);

                    Vector3 DistanceBetweenCurrentCenterAndClosest = panelCenter - trans.GetChild(index).position;
                    Vector3 LeftMostObjectAfterPosition = trans.GetChild(0).position + DistanceBetweenCurrentCenterAndClosest;
                    Vector3 RiteMostObjectAfterPosition = trans.GetChild(trans.childCount - 1).position + DistanceBetweenCurrentCenterAndClosest;

                    // wait, it is totally kool to be more left than the leftest position...

                    // skewed left, really no good
                    if (LeftMostObjectAfterPosition.x < LeftMostCenterPosition.x && RiteMostObjectAfterPosition.x < RiteMostCenterPosition.x)
                    {
                        // anchor to the right, or wherever
                        panelCenter = new Vector3(corners[2].x - widthX - posX, corners[2].y, corners[2].z);
                        closest = trans.GetChild(trans.childCount - 1);
                        index = trans.childCount - 1;
                    }

                    // skewed right, no good
                    if (LeftMostObjectAfterPosition.x > LeftMostCenterPosition.x && RiteMostObjectAfterPosition.x > RiteMostCenterPosition.x)
                    {
                        // anchor to the left, or wherever
                        panelCenter = new Vector3(corners[0].x + widthX + posX, corners[0].y, corners[0].z);
                        closest = trans.GetChild(0);
                        index = 0;
                    }

                    // I mean we don't ever get to go out of bounds anymore but it also kinda depends on which way we are swiping....
                    // This is kinda the idea and we will figure it out
                    // Which ever one that is closer to its own -est center position?

                    break;
                }
            case UIScrollView.Movement.Vertical:
                {
                    // check if the closest child checks out
                    Vector3 TopMostCenterPosition = new Vector3(corners[2].x, corners[2].y - heightY - posY, corners[2].z);
                    Vector3 BotMostCenterPosition = new Vector3(corners[0].x, corners[0].y + heightY + posY, corners[0].z);

                    Vector3 DistanceBetweenCurrentCenterAndClosest = panelCenter - trans.GetChild(index).position;
                    Vector3 TopMostObjectAfterPosition = trans.GetChild(0).position + DistanceBetweenCurrentCenterAndClosest;
                    Vector3 BotMostObjectAfterPosition = trans.GetChild(trans.childCount - 1).position + DistanceBetweenCurrentCenterAndClosest;

                    // skewed left, really no good
                    if (TopMostObjectAfterPosition.y > TopMostCenterPosition.y && BotMostObjectAfterPosition.y > BotMostCenterPosition.y)
                    {
                        // anchor to the left, or wherever
                        panelCenter = new Vector3(corners[0].x, corners[0].y + heightY + posY, corners[0].z);
                        closest = trans.GetChild(trans.childCount - 1);
                        index = trans.childCount - 1;
                    }

                    // skewed right, no good
                    if (TopMostObjectAfterPosition.y < TopMostCenterPosition.y && BotMostObjectAfterPosition.y < BotMostCenterPosition.y)
                    {
                        // anchor to the right, or wherever
                        panelCenter = new Vector3(corners[2].x, corners[2].y - heightY - posY, corners[2].z);
                        closest = trans.GetChild(0);
                        index = 0;
                    }

                    // I mean we don't ever get to go out of bounds anymore but it also kinda depends on which way we are swiping....
                    // This is kinda the idea and we will figure it out
                    // Which ever one that is closer to its own -est center position?

                    break;
                }
        }

        // If we have a touch in progress and the next page threshold set
        if (nextPageThreshold > 0f && UICamera.currentTouch != null)
        {
            // If we're still on the same object
            if (mCenteredObject != null && mCenteredObject.transform == trans.GetChild(index))
            {
                Vector2 totalDelta = UICamera.currentTouch.totalDelta;

                float delta = 0f;

                switch (mScrollView.movement)
                {
                    case UIScrollView.Movement.Horizontal:
                        {
                            delta = totalDelta.x;
                            break;
                        }
                    case UIScrollView.Movement.Vertical:
                        {
                            delta = totalDelta.y;
                            break;
                        }
                    default:
                        {
                            delta = totalDelta.magnitude;
                            break;
                        }
                }
            }
        }

        CenterOn(closest, panelCenter);
    }

    /// <summary>
    /// Center the panel on the specified target.
    /// </summary>

    void CenterOn(Transform target, Vector3 panelCenter)
    {
        if (target != null && mScrollView != null && mScrollView.panel != null)
        {
            Transform panelTrans = mScrollView.panel.cachedTransform;
            mCenteredObject = target.gameObject;

            // Figure out the difference between the chosen child and the panel's center in local coordinates
            Vector3 cp = panelTrans.InverseTransformPoint(target.position);
            Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
            Vector3 localOffset = cp - cc;

            // Offset shouldn't occur if blocked
            if (!mScrollView.canMoveHorizontally) localOffset.x = 0f;
            if (!mScrollView.canMoveVertically) localOffset.y = 0f;
            localOffset.z = 0f;

            // Spring the panel to this calculated position
            SpringPanel.Begin(mScrollView.panel.cachedGameObject,
                panelTrans.localPosition - localOffset, springStrength).onFinished = onFinished;
        }
        else mCenteredObject = null;
    }

    /// <summary>
    /// Center the panel on the specified target.
    /// </summary>

    public void CenterOn(Transform target)
    {
        if (mScrollView != null && mScrollView.panel != null)
        {
            Vector3[] corners = mScrollView.panel.worldCorners;
            Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;
            CenterOn(target, panelCenter);
        }
    }
}
