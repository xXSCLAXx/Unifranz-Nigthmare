using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasilloController : MonoBehaviour
{
    public GameObject pasilloPanel;

    public void AbrirPasillo()
    {
        pasilloPanel.SetActive(true);
    }

    public void CerrarPasillo()
    {
        pasilloPanel.SetActive(false);
    }
}
