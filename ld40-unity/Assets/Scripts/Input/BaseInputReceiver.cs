using UnityEngine;

public class TouchEventInfo
{
    public Vector3 touchPositionPix;
}

public class BaseInputReceiver : MonoBehaviour
{
    public virtual void OnTouchBegan(TouchEventInfo eventInfo)
    {}

    public virtual void OnTouchMoved(TouchEventInfo eventInfo)
    {}

    public virtual void OnTouchEnded(TouchEventInfo eventInfo)
    {}
}
