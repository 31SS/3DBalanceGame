using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private GameObject playerOrigin;
    [SerializeField] private GameObject CPUOrigin;
    [SerializeField] private List<GameObject> respawnPosition;
    [SerializeField] private GameObject startPosition;

    private GameObject _player;
    private PlayerManager _playerManager;

    private void Start()
    {
        // _player = Instantiate(playerOrigin, respawnPosition[0].transform.position, Quaternion.identity);
        _player = Instantiate(playerOrigin, startPosition.transform.position, Quaternion.identity);
        _playerManager = _player.GetComponent<PlayerManager>();
        
    }

    void Update()
    {
        // Debug.Log(_playerManager.StateProcessor.State.Value.GetStateName());
        if (_playerManager.StateProcessor.State.Value.GetStateName() == "State:Die")
        {
            _player = Instantiate(playerOrigin, startPosition.transform.position, Quaternion.identity);
            _playerManager = _player.GetComponent<PlayerManager>();
        }
    }
}
