using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solitaire : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite emptyPlace;
    public String[] suits = { "C", "D", "H", "S" };
    public String[] ranks = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"};
    public Sprite[] cardFaces;
    public Sprite cardBack;
    public GameObject[] foundationPositions;
    public GameObject[] tableauPositions;
    public GameObject[] freeCellPositions;
    public List<string>[] foundations;
    public List<string>[] tableaus;
    public List<string>[] freeCells;
    public List<string> foundation0 = new List<string>();
    public List<string> foundation1 = new List<string>();
    public List<string> foundation2 = new List<string>();
    public List<string> foundation3 = new List<string>();
    public List<string> tableau0 = new List<string>();
    public List<string> tableau1 = new List<string>();
    public List<string> tableau2 = new List<string>();
    public List<string> tableau3 = new List<string>();
    public List<string> tableau4 = new List<string>();
    public List<string> tableau5 = new List<string>();
    public List<string> tableau6 = new List<string>();
    public List<string> tableau7 = new List<string>();
    public List<string> freeCell0 = new List<string>();
    public List<string> freeCell1 = new List<string>();
    public List<string> freeCell2 = new List<string>();
    public List<string> freeCell3 = new List<string>();
    private System.Random rng = new System.Random();
    private Vector3 cardOffset = new Vector3(0f, -.3f, -0.1f);
    private float zOffset = -.3f;
    void Start()
    {
        tableaus = new List<string>[] { tableau0, tableau1, tableau2, tableau3, tableau4, tableau5, tableau6, tableau7 };
        foundations = new List<string>[] { foundation0, foundation1, foundation2, foundation3 };
        freeCells = new List<string>[] { freeCell0, freeCell1, freeCell2, freeCell3 };
        PlayGame();
    }

    void Update()
    {

    }

    void PlayGame()
    {
        List<string> allCards = GenerateDeck();
        foreach (string card in allCards)
        {
            Debug.Log(card);
        }
        Deal(allCards);
    }

    List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string suit in suits)
        {
            foreach (string rank in ranks)
            {
                newDeck.Add(suit + rank);
            }
        }
        newDeck = newDeck.OrderBy(x => rng.Next()).ToList();
        return newDeck;
    }

    void Deal(List<string> allCards)
    {
        Debug.Log("Dealing FreeCell cards...");

        int cardIndex = 0;
        for (int col = 0; col < 8; col++)
        {
            int cardsInColumn = (col < 4) ? 7 : 6;
            for (int row = 0; row < cardsInColumn; row++)
            {
                string card = allCards[cardIndex++];
                tableaus[col].Add(card);
            }
        }

        foreach (GameObject tabPosition in tableauPositions)
        {
            Debug.Log("Dealing to tableau position " + tabPosition.name);
            int index = Array.IndexOf(tableauPositions, tabPosition);
            Vector3 currentPosition = tabPosition.transform.position + new Vector3(0, 0, -.1f);
            foreach (string card in tableaus[index])
            {
                Debug.Log("Dealing card " + card + " to tableau " + index);
                CreateCard(card, currentPosition, tabPosition.transform, true);
                currentPosition += cardOffset;
            }
        }
    }

    void CreateCard(string cardName, Vector3 position, Transform parent, bool isFaceUp)
    {
        Debug.Log("Creating card " + cardName + " at " + position);
        GameObject newCard = Instantiate(cardPrefab, position, Quaternion.identity, parent);
        newCard.name = cardName;
        Sprite cardFace = cardFaces.FirstOrDefault(s => s.name == cardName);
        newCard.GetComponent<CardSprite>().cardFace = cardFace;
        newCard.GetComponent<CardSprite>().isFaceUp = isFaceUp;
    }

    public bool CanPlaceOnFreeCell(int freeCellIndex)
    {
        return freeCells[freeCellIndex].Count == 0;
    }

    public int GetMaxMovableCards()
    {
        int emptyFreeCells = freeCells.Count(fc => fc.Count == 0);
        int emptyTableaus = tableaus.Count(t => t.Count == 0);
        return (1 + emptyFreeCells) * (1 + emptyTableaus);
    }

    private int GetCardsToMoveCount(GameObject cardObject)
    {
        if (!cardObject.transform.parent.CompareTag("Tableau")) return 1;

        foreach (List<string> tableau in tableaus)
        {
            int cardIndex = tableau.IndexOf(cardObject.name);
            if (cardIndex >= 0)
            {
                return tableau.Count - cardIndex;
            }
        }
        return 1;
    }

    public bool CanMoveSequence(GameObject cardObject, int cardCount)
    {
        if (cardCount > GetMaxMovableCards()) return false;

        if (cardCount <= 1) return true;

        if (!cardObject.transform.parent.CompareTag("Tableau")) return false;

        foreach (List<string> tableau in tableaus)
        {
            int cardIndex = tableau.IndexOf(cardObject.name);
            if (cardIndex >= 0)
            {

                for (int i = cardIndex; i < tableau.Count - 1; i++)
                {
                    string currentCard = tableau[i];
                    string nextCard = tableau[i + 1];
                    if (!IsAlternatingColor(currentCard, nextCard) || !IsOneRankHigher(currentCard, nextCard))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }

    public bool IsValidMove(GameObject cardObject, GameObject targetObject)
    {
        if (cardObject == targetObject || cardObject == null || targetObject == null) return false;
        ResolveTarget(targetObject, out GameObject clickedTag, out int foundationIndex, out int tabIndex, out int freeCellIndex);

        int cardsToMove = GetCardsToMoveCount(cardObject);

        if (cardObject.transform.parent.CompareTag("FreeCell"))
        {
            if (clickedTag.CompareTag("Tableau") && tabIndex >= 0)
            {
                Debug.Log("FreeCell->Tab: " + CanPlaceOnTableau(cardObject.name, tabIndex));
                return CanPlaceOnTableau(cardObject.name, tabIndex);
            }
            if (clickedTag.CompareTag("Foundation") && foundationIndex >= 0)
            {
                Debug.Log("FreeCell->Foundation: " + CanPlaceOnFoundation(cardObject.name, foundationIndex));
                return CanPlaceOnFoundation(cardObject.name, foundationIndex);
            }
            return false;
        }

        if (cardObject.transform.parent.CompareTag("Foundation"))
        {
            if (clickedTag.CompareTag("Tableau") && tabIndex >= 0)
            {
                Debug.Log("Foundation->Tab: " + CanPlaceOnTableau(cardObject.name, tabIndex));
                return CanPlaceOnTableau(cardObject.name, tabIndex);
            }
            return false;
        }

        if (cardObject.transform.parent.CompareTag("Tableau"))
        {
            if (clickedTag.CompareTag("FreeCell") && freeCellIndex >= 0)
            {
                if (IsBlocked(cardObject))
                {
                    Debug.Log("Blocked: can't move to FreeCell");
                    return false;
                }
                Debug.Log("Tab->FreeCell: " + CanPlaceOnFreeCell(freeCellIndex));
                return CanPlaceOnFreeCell(freeCellIndex);
            }

            if (clickedTag.CompareTag("Tableau") && tabIndex >= 0)
            {
                if (!CanMoveSequence(cardObject, cardsToMove))
                {
                    Debug.Log("Too many cards to move: " + cardsToMove + " > " + GetMaxMovableCards());
                    return false;
                }
                Debug.Log("Tab->Tab: " + CanPlaceOnTableau(cardObject.name, tabIndex));
                return CanPlaceOnTableau(cardObject.name, tabIndex);
            }

            if (clickedTag.CompareTag("Foundation") && foundationIndex >= 0)
            {
                if (IsBlocked(cardObject))
                {
                    Debug.Log("Blocked: can't move to Foundation");
                    return false;
                }
                Debug.Log("Tab->Foundation: " + CanPlaceOnFoundation(cardObject.name, foundationIndex));
                return CanPlaceOnFoundation(cardObject.name, foundationIndex);
            }
            return false;
        }

        Debug.Log("Nothing matched, returning false");
        return false;
    }

    public void MoveCardsAbove(GameObject origParent, int originalTabIndex, int destTabIndex, int cardsToMoveCount, GameObject clickedTag, GameObject cardObject)
    {
        if (originalTabIndex == -1 || cardsToMoveCount <= 1) return;
        List<string> origTab = tableaus[originalTabIndex];
        int origCount = origTab.Count;
        int origIndex = origCount - cardsToMoveCount + 1;
        for (int i = 0; i < cardsToMoveCount -1 ; i++)
        {
            string movingCardName = origTab[origIndex];
            origTab.RemoveAt(origIndex);
            tableaus[destTabIndex].Add(movingCardName);
            GameObject movingCardObj = null;
            foreach (Transform child in origParent.transform)
            {
                if (child.gameObject.name == movingCardName)
                {
                    movingCardObj = child.gameObject;
                    break;
                }
            }
            if(movingCardObj!=null)
            {
                movingCardObj.transform.parent = clickedTag.transform;
                movingCardObj.transform.position = cardObject.transform.position + (cardOffset * (i + 1));
            }
        }
    }
    
    public void PlaceCard(GameObject cardObject, GameObject targetObject)
    {
        if (cardObject == targetObject || cardObject == null || targetObject == null) return;
        int originalTabIndex = -1;
        int cardsToMoveCount = 1;
        ResolveTarget(targetObject, out GameObject clickedTag, out int foundationIndex, out int tabIndex, out int freeCellIndex);
        GameObject originalParent = cardObject.transform.parent.gameObject;

        if (cardObject.transform.parent.CompareTag("Tableau"))
        {
            foreach (List<string> tableau in tableaus)
            {
                if (tableau.Contains(cardObject.name))
                {
                    originalTabIndex = System.Array.IndexOf(tableaus, tableau);
                    cardsToMoveCount = tableau.Count - tableau.IndexOf(cardObject.name);
                    tableau.Remove(cardObject.name);
                    break;
                }
            }
        }

        if (cardObject.transform.parent.CompareTag("FreeCell"))
        {
            foreach (List<string> freeCell in freeCells)
            {
                if (freeCell.Contains(cardObject.name))
                {
                    freeCell.Remove(cardObject.name);
                    break;
                }
            }
        }

        if (cardObject.transform.parent.CompareTag("Foundation"))
        {
            foreach (List<string> foundation in foundations)
            {
                if (foundation.Contains(cardObject.name))
                {
                    foundation.Remove(cardObject.name);
                    break;
                }
            }
        }

        if (clickedTag.transform.CompareTag("Tableau"))
        {
            int tableauIndex = System.Array.IndexOf(tableauPositions, clickedTag);
            tableaus[tableauIndex].Add(cardObject.name);
            if (tableaus[tableauIndex].Count == 1)
                cardObject.transform.position = targetObject.transform.position + new Vector3(0f, 0f, -.03f);
            else
                cardObject.transform.position = targetObject.transform.position + cardOffset;
            cardObject.transform.parent = clickedTag.transform;
            MoveCardsAbove(originalParent, originalTabIndex, tableauIndex, cardsToMoveCount, clickedTag, cardObject);
        }

        if (clickedTag.transform.CompareTag("FreeCell"))
        {
            int fcIndex = System.Array.IndexOf(freeCellPositions, clickedTag);
            freeCells[fcIndex].Add(cardObject.name);
            cardObject.transform.position = clickedTag.transform.position + new Vector3(0f, 0f, -.03f);
            cardObject.transform.parent = clickedTag.transform;
        }

        if (clickedTag.transform.CompareTag("Foundation"))
        {
            int fIndex = System.Array.IndexOf(foundationPositions, clickedTag);
            foundations[fIndex].Add(cardObject.name);
            cardObject.transform.position = targetObject.transform.position + new Vector3(0f, 0f, -.03f);
            cardObject.transform.parent = clickedTag.transform;
        }

        if (CheckWinCondition())
        {
            Debug.Log("Congratulations! You won!");
        }
    }

    public bool IsLastInTab(GameObject cardObject)
    {
        foreach(List<string> tab in tableaus)
        {
            if (tab.Count > 0 && tab.Last() == cardObject.name)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsBlocked(GameObject cardObject)
    {
        foreach (Transform child in cardObject.transform.parent)
        {
            if (child.gameObject != cardObject && child.position.z < cardObject.transform.position.z)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAlternatingColor(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        char suit1 = card1[0];
        char suit2 = card2[0];
        bool isRed1 = (suit1 == 'D' || suit1 == 'H');
        bool isRed2 = (suit2 == 'D' || suit2 == 'H');
        return isRed1 != isRed2;
    }

    public bool IsSameSuit(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        return card1[0] == card2[0];
    }

    public bool IsOneRankHigher(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        int rank1 = Array.IndexOf(ranks, card1.Substring(1));
        int rank2 = Array.IndexOf(ranks, card2.Substring(1));
        Debug.Log("rank1: " + rank1);
        Debug.Log("rank2: " + rank2);
        return rank1 == (rank2 + 1) % ranks.Length;
    }

    public bool IsOneRankLower(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        int rank1 = Array.IndexOf(ranks, card1.Substring(1));
        int rank2 = Array.IndexOf(ranks, card2.Substring(1));
        return (rank1 + 1) % ranks.Length == rank2;
    }

    public bool CanPlaceOnFoundation(string card, int foundationIndex)
    {
        if (foundations[foundationIndex].Count == 0)
        {
            return card.Substring(1) == "A";
        }
        string topCard = foundations[foundationIndex].Last();
        Debug.Log("topCard: " + topCard + ", card: " + card);
        Debug.Log("IsSameSuit: " + IsSameSuit(card, topCard));
        Debug.Log("IsOneRankHigher: " + IsOneRankHigher(card, topCard));
        return IsSameSuit(card, topCard) && IsOneRankHigher(card, topCard);
    }

    public bool CanPlaceOnTableau(string card, int tableauIndex)
    {
        if (tableaus[tableauIndex].Count == 0)
        {
            return true;  
        }
        string topCard = tableaus[tableauIndex].Last();
        return IsAlternatingColor(card, topCard) && IsOneRankLower(card, topCard);
    }

    public bool CheckWinCondition()
    {
        foreach (List<string> foundation in foundations)
        {
            if (foundation.Count != 13) return false;
        }
        return true;
    }

    void ResolveTarget(GameObject toLocation, out GameObject clickedTag, out int foundationIndex, out int tableauIndex, out int freeCellIndex)
    {
        clickedTag = toLocation.transform.CompareTag("Card") ? toLocation.transform.parent.gameObject : toLocation;
        foundationIndex = -1;
        tableauIndex = -1;
        freeCellIndex = -1;
        if (clickedTag.transform.CompareTag("Foundation"))
            foundationIndex = System.Array.IndexOf(foundationPositions, clickedTag);
        else if (clickedTag.transform.CompareTag("Tableau"))
            tableauIndex = System.Array.IndexOf(tableauPositions, clickedTag);
        else if (clickedTag.transform.CompareTag("FreeCell"))
            freeCellIndex = System.Array.IndexOf(freeCellPositions, clickedTag);
    }
}
