using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHandler : MonoBehaviour
{
    // GR: Configuration variables
    [SerializeField] Color higlightColor = new Color();

    // GR: State variables
    Color initialColor;
    bool plantGrowing = false;

    // GR: Referenced variables
    MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;
    }

    public void Highlight(bool highlight)
    {
        if (highlight)
        {
            // OnMouseEnter();
            meshRenderer.material.color = higlightColor;
        }
        else
        {
            // OnMouseExit();
            meshRenderer.material.color = initialColor;
        }
    }

    public void PlantCarrot()
    {
        if (!plantGrowing)
        {
            plantGrowing = true;
            GetComponent<Carrot>().CreateCarrot(transform.position);
        }        
    }    
}
