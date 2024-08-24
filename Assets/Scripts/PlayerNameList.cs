using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameList : MonoBehaviour
{
    public Animator _animPlayerNameList;
    [SerializeField] private GameObject _overlay;
    [SerializeField] private GameObject _openBtn;
    [SerializeField] private GameObject _playerDetOut;
    [SerializeField] private GameObject _playerCompOut;
    [SerializeField] private GameObject _locatinbtn;
    [SerializeField] private GameObject _apps;
    [SerializeField] private GameObject _RoomPanel;
    [SerializeField] private GameObject _SettingsPanel;


    void Start()
    {
        _SettingsPanel.SetActive(false);
        _RoomPanel.SetActive(false);
        _overlay.SetActive(false);
        _playerCompOut.SetActive(false);
        _playerDetOut.SetActive(false); 
        _overlay.GetComponent<Button>().onClick.AddListener(closePlayerListNamePanel);
    }
    
    public void openPlayerListNamePanel()   
    {
       
            transform.Translate(Vector3.right * 3);
            _animPlayerNameList.SetBool("PlayerNamePanel", true);
            FindObjectOfType<MovingBlock>().MoveRight();
            foreach (var block in FindObjectsOfType<MovingBlock>())
            {
                block.MoveRight();
            }
        _openBtn.SetActive(false); 
        _overlay.SetActive(true);
        _playerDetOut.SetActive(false);
        _playerCompOut.SetActive(false);
       
    }
    public void closePlayerListNamePanel()
    {
        _animPlayerNameList.SetBool("PlayerNamePanel", false);
        FindObjectOfType<MovingBlock>().MoveLeft();

        foreach (var block in FindObjectsOfType<MovingBlock>())
        {
            block.MoveLeft();
        }
        _openBtn.SetActive(true);
        _overlay.SetActive(false);
        _playerDetOut.SetActive(true);
        _playerCompOut.SetActive(true);
    }
    public void ToggleRoomPanel()
    {
        if (_RoomPanel.activeSelf)
        {
            _RoomPanel.SetActive(false);
        }
        else
        {
            _RoomPanel.SetActive(true);
        }
    }
    public void OpenSettings()
    {
        _SettingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        _SettingsPanel.SetActive(false);
    }
    
}
