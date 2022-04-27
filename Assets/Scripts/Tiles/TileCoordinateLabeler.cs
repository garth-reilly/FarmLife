using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class TileCoordinateLabeler : MonoBehaviour
{
    // GR: Configuration variables

    // GR: State variables
    Vector2Int tileCoordinates = new Vector2Int();

    // GR: Referenced variables
    TextMeshPro tileLabel;

    void Awake()
    {
        tileLabel = GetComponent<TextMeshPro>();
        UpdateCoordinates();
        if (Application.isPlaying)
        {
            this.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateCoordinates();
            UpdateTileName();
        }
    }

    void UpdateCoordinates()
    {
        tileCoordinates.x = (int)this.transform.parent.position.x;
        tileCoordinates.y = (int)this.transform.parent.position.z;
        tileLabel.text = tileCoordinates.x + ",\n" + tileCoordinates.y;
    }

    void UpdateTileName()
    {
        this.transform.parent.name = tileCoordinates.ToString();
    }
}
