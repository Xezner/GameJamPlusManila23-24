using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Transform _player;
    [SerializeField] private float _cameraSpeed;

    [SerializeField] private LevelDataScriptableObject _levelData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xPos = _player.position.x + _offset.x;
        float yPos = _player.position.y + _offset.y;

        float xLerp = Mathf.Lerp(transform.position.x, xPos, Time.deltaTime * _cameraSpeed);
        float yLerp = Mathf.Lerp(transform.position.y, yPos, Time.deltaTime * _cameraSpeed);

        transform.position = new Vector3(xLerp, transform.position.y, _offset.z);
    }
}
