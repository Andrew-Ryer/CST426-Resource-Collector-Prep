using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * ResourceNode is a harvestable object like a tree or stone. Replicated health
 * counts down as players hit it with the right tool; at zero the server spawns
 * resource pickups and every client hides the depleted node.
 */

public class ResourceNode : Interactable
{
    [SerializeField] List<ObjectType> _toolTypeRequired = new();
    [SerializeField] NetworkObject _producedPrefab;
    [SerializeField] int _amountToSpawn = 3;
    [SerializeField] int _startingHealth = 1;
    [SerializeField] AudioClip _audioClip;
    
    readonly NetworkVariable<int> _health = new();
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // TODO Slice 8.1: on the server, set health to _startingHealth.
        // NOTE: make sure  NetworkObject.Despawn(!NetworkObject.InScenePlaced); is done
        if (IsServer)
        {
            _health.Value = _startingHealth;
        }

        // Next: Slice 8.2 CanInteract.

        // TODO Slice 8.5: subscribe to health changes and apply the current health.
        // Check: both windows hide a depleted tree. A late joiner sees it hidden.
        _health.OnValueChanged += HandleHealthChanged;
        
        HandleHealthChanged(_health.Value, _health.Value);
        
        

        // Next: Slice 8.6 OnNetworkDespawn.


    }

    public override void OnNetworkDespawn()
    {

        // TODO Slice 8.6: unsubscribe from replicated health changes.
        // </> end of Slice 8
        // Next: Slice 9.1 in World/Receptacle.cs.
        _health.OnValueChanged -= HandleHealthChanged;
        
        base.OnNetworkDespawn();
    }

    public override bool CanInteract(ObjectType heldType)
    {
        // TODO Slice 8.2:
        // 1. Require a living node.
        // 2. Require an accepted tool.
        // Check: hold the axe. The tree highlights. Empty-handed, it does not.
        // Next: Slice 8.3 Interact and HitFeedbackRpc.
        
        return (_health.Value > 0 && _toolTypeRequired.Contains(heldType));
        
    }

    protected override void Interact(PlayerHeldItem heldItem)
    {

        // TODO Slice 8.3:
        // 1. Reduce health.
        // 2. Call HitFeedbackRpc.
        // 3. Spawn _amountToSpawn copies of _producedPrefab with InstantiateAndSpawn.
        // 4. Place each with a small random XZ offset and random yaw.
        // Check: axe the tree. Wood appears. The mesh is still there until 8.4.
        // Next: Slice 8.4 HandleHealthChanged.
        
        HitFeedbackRpc();
        
        _health.Value--;
        
        if (_health.Value > 0)
            return;

        for (int i = 0; i < _amountToSpawn; i++)
        {
            Vector3 offset = new(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f));

            Quaternion rotation = Quaternion.Euler(
                0f,
                Random.Range(0f, 360f),
                0f);

            NetworkObject.InstantiateAndSpawn(_producedPrefab.gameObject, NetworkManager, position: transform.position + offset, rotation: rotation);
        }
        
        if (!NetworkObject.InScenePlaced)
        {
            NetworkObject.Despawn();
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    void HitFeedbackRpc()
    {

        // TODO Slice 8.3: play the authored hit sound on each observer.
        if (_audioClip != null)
        {
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
        }

    }

    void HandleHealthChanged(int previousValue, int newValue)
    {
        
        Debug.Log(
            $"[ResourceNode] Health changed on {name}: " +
            $"{previousValue} -> {newValue}, " +
            $"IsServer={IsServer}, IsClient={IsClient}, IsSpawned={IsSpawned}");

        // TODO Slice 8.4: make the visuals and physics match the health.
        bool isAlive = newValue > 0;

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
        {
            renderer.enabled = isAlive;
        }

        foreach (Collider collider in GetComponentsInChildren<Collider>(true))
        {
            collider.enabled = isAlive;
        }
        
        // Next: Slice 8.5 in OnNetworkSpawn — subscribe and apply.

    }

}