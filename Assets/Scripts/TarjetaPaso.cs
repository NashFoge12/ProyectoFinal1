using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TarjetaPaso : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Mision1_Algoritmos.Paso paso;
    public int slotActual = -1;

    private Mision1_Algoritmos manager;
    private CanvasGroup canvasGroup;
    private Transform padreOriginal;
    private RectTransform rectTransform;
    private Canvas canvas;

    public void Configurar(Mision1_Algoritmos.Paso p, Mision1_Algoritmos m, CanvasGroup cg)
    {
        paso = p;
        manager = m;
        canvasGroup = cg;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        padreOriginal = transform.parent;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        SlotSecuencia slotDetectado = null;
        foreach (var resultado in eventData.hovered)
        {
            SlotSecuencia s = resultado.GetComponent<SlotSecuencia>();
            if (s != null) { slotDetectado = s; break; }
        }

        if (slotDetectado != null)
        {
            transform.SetParent(slotDetectado.transform, false);
            rectTransform.anchoredPosition = Vector2.zero;
            manager.AlSoltarTarjeta(this, slotDetectado.indice);
        }
        else
        {
            transform.SetParent(padreOriginal, false);
            rectTransform.anchoredPosition = Vector2.zero;
            slotActual = -1;
        }
    }
}