using UnityEngine;

public class CharacterInputReceiver : BaseInputReceiver
{
    private static readonly int MAX_NUM_RAYCAST_HITS = 3;

    public Transform characterRoot = null;
    public Character character = null;
    public float raycastDistance = 100;
    public LayerMask raycastLayerMask = new LayerMask();

    private Vector3 _initialPosition = Vector3.zero;
    private RaycastHit2D[] _raycastResults = null;
    private Room _currentRoom = null;

    private void Start()
    {
        _raycastResults = new RaycastHit2D[MAX_NUM_RAYCAST_HITS];
    }

    public override void OnTouchBegan(TouchEventInfo eventInfo)
    {
        if (null != _currentRoom)
        {
            _currentRoom.RemoveCharacter(character);
        }
        _initialPosition = characterRoot.position;
    }

    public override void OnTouchMoved(TouchEventInfo eventInfo)
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventInfo.touchPositionPix);
        newPosition.z = characterRoot.position.z;
        characterRoot.position = newPosition;
    }

    public override void OnTouchEnded(TouchEventInfo eventInfo)
    {
        // Check if there's a room below
        Ray ray = Camera.main.ScreenPointToRay(eventInfo.touchPositionPix);
        int numHits = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, _raycastResults, raycastDistance, raycastLayerMask);
        bool hasFoundRoom = false;
        for (int hitIdx = 0; hitIdx < numHits; ++hitIdx)
        {
            Room room = _raycastResults[hitIdx].collider.GetComponentInParent<Room>();
            if (null != room)
            {
                _currentRoom = room;
                room.AddCharacter(character);
                hasFoundRoom = true;
                break;
            }
        }

        if (!hasFoundRoom)
        {
            characterRoot.position = _initialPosition;
        }
    }
}
