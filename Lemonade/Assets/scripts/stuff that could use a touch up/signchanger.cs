using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using globalVariables = GlobalVariables; // Alias to avoid confusion with the class name

public class signchanger : MonoBehaviour
{
    [Tooltip("Item ID the player must have collected to change this sign.")]
    public string requiredItemId = "sign_key";

    [Tooltip("Material to apply when the condition is met.")]
    public Material successMaterial;

    [Tooltip("Reference to the player PickupScript (inventory tracker).")]
    public PickupScript playerPickupScript;


    private Renderer _renderer;
    private Material _originalMaterial;
    private bool _hasChanged;

    public bool DogRanAway;

    public float requiredDistance = 2f;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _originalMaterial = _renderer.material;

        if (playerPickupScript == null)
        {
            playerPickupScript = FindObjectOfType<PickupScript>();
            if (playerPickupScript == null)
            {
                Debug.LogWarning("signchanger: playerPickupScript is not assigned and no PickupScript was found in scene.", this);
            }
            else
            {
                Debug.Log("signchanger: auto-detected PickupScript from scene.", this);
            }
        }
    }

    private void OnMouseDown()
{

    if (globalVariables.istalking == true)
            {
                
                return;
            }
    if (_hasChanged)
        return;

    if (playerPickupScript == null)
    {
        Debug.LogWarning("signchanger: no playerPickupScript at click time.", this);
        return;
    }

    if (playerPickupScript.CollectedItemIds == null)
    {
        Debug.LogWarning("signchanger: CollectedItemIds list is null in playerPickupScript.", this);
        return;
    }

    Debug.Log($"signchanger: clicked; requiredItemId='{requiredItemId}'; hasItem={playerPickupScript.hasItem}; collectedCount={playerPickupScript.CollectedItemIds.Count}", this);
    Debug.Log($"signchanger: collected items = [{string.Join(", ", playerPickupScript.CollectedItemIds)}]", this);

    if (!playerPickupScript.CollectedItemIds.Contains(requiredItemId))
    {
        Debug.Log("signchanger: required item is not in player inventory.", this);
        return;
    }

    float distance = Vector3.Distance(playerPickupScript.transform.position, transform.position);
    if (distance > requiredDistance)
        return;

    // Check the global variable
    if (!GlobalVariables.DogRanAway) // Check the global variable here
    {
        Debug.Log("signchanger: dog didn't run away, not changing sign.", this);
        return;
    }

    ApplySuccessMaterial();
}


    public void ApplySuccessMaterial()
    {
        if (_renderer == null)
        {
            Debug.LogWarning("signchanger: no renderer on object, cannot apply material.", this);
            return;
        }

        if (successMaterial == null)
        {
            Debug.LogWarning("signchanger: successMaterial is not assigned; keeping original material.", this);
            _hasChanged = true;
            return;
        }

        _renderer.material = successMaterial;
        _hasChanged = true;
        Debug.Log($"signchanger: required item '{requiredItemId}' found, material changed.", this);

        GlobalVariables.TimmyReturned= true; // Set the global variable here
    }

    private void OnDisable()
    {
        if (_renderer != null && _originalMaterial != null)
            _renderer.material = _originalMaterial;
    }
}
