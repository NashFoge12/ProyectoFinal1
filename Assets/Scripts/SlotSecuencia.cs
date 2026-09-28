using UnityEngine;
using UnityEngine.EventSystems;

public class SlotSecuencia : MonoBehaviour, IDropHandler
{
    public int indice;
    public Mision1_Algoritmos manager;

    public void OnDrop(PointerEventData eventData)
    {
        // La lógica la maneja TarjetaPaso.OnEndDrag
    }
}