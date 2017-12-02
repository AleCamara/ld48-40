using UnityEngine;

public class InputService : MonoBehaviour
{
    private static readonly int MAX_NUM_RAYCAST_HITS = 3;

    public float raycastDistance = 100;
    public LayerMask raycastLayerMask = new LayerMask();

    private BaseInputReceiver _currentReceiver = null;
    private RaycastHit2D[] _raycastResults = null;

    private void Start()
    {
        _raycastResults = new RaycastHit2D[MAX_NUM_RAYCAST_HITS];
    }

    private void Update()
    {
        TouchPhase touchPhase = TouchPhase.Moved;
        if (Input.GetMouseButtonDown(0))
        {
            touchPhase = TouchPhase.Began;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchPhase = TouchPhase.Ended;
        }
        Vector3 touchPosition = Input.mousePosition;

        if (TouchPhase.Began == touchPhase)
        {
            // Check if selecting anything that is a input receiver
            // If so, redirect all input to that input receiver
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            int numHits = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, _raycastResults, raycastDistance, raycastLayerMask);
            for (int hitIdx = 0; hitIdx < numHits; ++hitIdx)
            {
                RaycastHit2D hit = _raycastResults[hitIdx];
                BaseInputReceiver inputReceiver = hit.collider.GetComponent<BaseInputReceiver>();
                if (null != inputReceiver)
                {
                    _currentReceiver = inputReceiver;
                    break;
                }
            }
        }

        if (null == _currentReceiver)
        {
            return;
        }

        // Gather mouse position and give it to the current input receiver
        TouchEventInfo eventInfo = new TouchEventInfo()
        {
            touchPositionPix = touchPosition
        };
        switch (touchPhase)
        {
            case TouchPhase.Began:
                _currentReceiver.OnTouchBegan(eventInfo);
                break;

            case TouchPhase.Moved:
                _currentReceiver.OnTouchMoved(eventInfo);
                break;

            case TouchPhase.Ended:
                _currentReceiver.OnTouchEnded(eventInfo);
                _currentReceiver = null;
                break;
        }
    }
}
