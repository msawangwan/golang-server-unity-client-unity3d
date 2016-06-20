using UnityEngine;
using madMeesh.Cards;
using System.Collections;

public class PlayerComponent : MonoBehaviour {
    public Player PlayerReference;

    public DeckComponent DeckGO;
    public HandComponent HandGO;
    public CardViewComponent CardViewGO;

    private madMeesh.Cards.MasterControllerComponent cardController;

    private void Start() {
        DeckGO = FindObjectOfType<DeckComponent> ( );
        HandGO = FindObjectOfType<HandComponent> ( );
        CardViewGO = FindObjectOfType<CardViewComponent> ( );

        cardController = FindObjectOfType<MasterControllerComponent> ( );
        cardController.SetOwner ( this );
    }
}