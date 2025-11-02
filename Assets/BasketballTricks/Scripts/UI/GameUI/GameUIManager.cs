using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;



public class GameUIManager : MonoBehaviour
{

    [Header("Sliding Panels")]
    [SerializeField] SlideInPanel _cardCatalog;
    [SerializeField] SlideInPanel _timelinePanel;
    [SerializeField] SlideInPanel _playerActionPanel;
    [SerializeField] SlideInPanel _submitPanel;


    
    void Start()
    {
       _cardCatalog.SetShown(true);
        _timelinePanel.SetShown(true);
    }

    

    
}
