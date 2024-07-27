using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameList : MonoBehaviour
{
    public Animator _animPlayerNameList;
    //buttons
    [SerializeField] private GameObject _openBtn;
  
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
    }
}
