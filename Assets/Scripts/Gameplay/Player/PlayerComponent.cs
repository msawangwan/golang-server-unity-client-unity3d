using UnityEngine;
using madMeesh.Cards;
using System.Collections;

public class PlayerComponent : MonoBehaviour {
    public Player PlayerReference { get; private set; }

    public DeckComponent DeckGameObject { get; private set; }
    public HandComponent HandGameObject { get; private set; }
    public CardViewComponent CardViewGameObject { get; private set; }
    public TableComponent TableGameObject { get; private set; }

    public void RegisterPlayerReference ( Player player ) {
        PlayerReference = player;
    }

    private void Start() {
        RegisterPlayerGameObjects ( );
    }

    private void RegisterPlayerGameObjects() {
        DeckGameObject = FindObjectOfType<DeckComponent> ( );
        HandGameObject = FindObjectOfType<HandComponent> ( );
        CardViewGameObject = FindObjectOfType<CardViewComponent> ( );
        TableGameObject = FindObjectOfType<TableComponent> ( );

        DeckGameObject.RegisterComponentWithOwner ( this );
        HandGameObject.RegisterComponentWithOwner ( this );
        CardViewGameObject.RegisterComponentWithOwner ( this );
        TableGameObject.RegisterComponentWithOwner ( this );
    }
}