using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Transform _player;
    [SerializeField] private float _cameraSpeed;

    [SerializeField] private GameStateDataScriptableObject _levelData;

    private Vector3 _cameraPosition;

    private int[] _cameraPositions = { -40, -40, -30, -20, -10, 0, 10, 20, 35 };
    private int[] _playerPositionRanges = { -45, -35, -25, -15, -5, 5, 15, 25, 45 };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        float xPos = _player.position.x + _offset.x;
        float yPos = _player.position.y + _offset.y;

        float xLerp = Mathf.Lerp(transform.position.x, xPos, Time.deltaTime * _cameraSpeed);
        float yLerp = Mathf.Lerp(transform.position.y, yPos, Time.deltaTime * _cameraSpeed);

        _cameraPosition = new Vector3(xLerp, transform.position.y, _offset.z);

        GetCameraSnapPosition();

        transform.position = _cameraPosition;
        AudioManager.Instance.transform.position = _player.position;
    }

    private void GetCameraSnapPosition()
    {
        for (int i = 0; i < _playerPositionRanges.Length; i++)
        {
            if (_player.position.y < _playerPositionRanges[i])
            {
                _cameraPosition.y = _cameraPositions[i];
                break;
            }
        }

        // If the player's position is greater than the largest range, set the camera position to the last one.
        if (_player.position.y >= _playerPositionRanges[_playerPositionRanges.Length - 1])
        {
            _cameraPosition.y = _cameraPositions[_cameraPositions.Length - 1];
        }
    }
}
