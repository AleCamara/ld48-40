using UnityEngine;

public class CharacterInputReceiver : BaseInputReceiver
{
    public Transform characterRoot = null;

    public override void OnTouchBegan(TouchEventInfo eventInfo)
    { }

    public override void OnTouchMoved(TouchEventInfo eventInfo)
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventInfo.touchPositionPix);
        newPosition.z = characterRoot.position.z;
        characterRoot.position = newPosition;
    }

    public override void OnTouchEnded(TouchEventInfo eventInfo)
    { }
}
