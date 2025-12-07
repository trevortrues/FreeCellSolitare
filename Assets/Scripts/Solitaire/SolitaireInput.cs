using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SolitaireInput : MonoBehaviour
{
    private Solitaire solitaire;
    private GameObject selectedCard = null;
    void Start()
    {
        solitaire = FindAnyObjectByType<Solitaire>();   
    }

    void Update()
    {

    }
    
    void OnBurst(InputValue value)
    {
        if (CardAnimator.Instance != null && CardAnimator.Instance.IsAnimating())
            return;

        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
            return;

        Debug.Log("Burst");
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
        Debug.Log(worldPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);
        if (hit != null)
        {
            if (hit.CompareTag("Card"))
            {
                Debug.Log("Card clicked: " + hit.name);
                if (selectedCard != null)
                {
                    if (selectedCard == hit.gameObject)
                    {
                        selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                        selectedCard = null;
                        return;
                    }
                    if (solitaire.IsValidMove(selectedCard, hit.gameObject))
                    {
                        solitaire.PlaceCard(selectedCard, hit.gameObject);
                        selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                        selectedCard = null;
                        return;
                    }
                }
                Debug.Log("Card selected: " + hit.name);
                selectedCard = hit.gameObject;
                selectedCard.GetComponent<SpriteRenderer>().color = Color.gray;
            }

            if (hit.CompareTag("Tableau"))
            {
                Debug.Log("Tableau clicked: " + hit.name);
                if (selectedCard != null && solitaire.IsValidMove(selectedCard, hit.gameObject))
                {
                    solitaire.PlaceCard(selectedCard, hit.gameObject);
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
            }

            if (hit.CompareTag("Foundation"))
            {
                Debug.Log("Foundation clicked: " + hit.name);
                if (selectedCard != null && solitaire.IsValidMove(selectedCard, hit.gameObject))
                {
                    solitaire.PlaceCard(selectedCard, hit.gameObject);
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
            }

            if (hit.CompareTag("FreeCell"))
            {
                Debug.Log("FreeCell clicked: " + hit.name);
                if (selectedCard != null && solitaire.IsValidMove(selectedCard, hit.gameObject))
                {
                    solitaire.PlaceCard(selectedCard, hit.gameObject);
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
            }
        }
    }
}
