using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private LayerMask collisionLayers = -1; // What can the bullet hit?

    [Header("Explosion Effect")]
    [SerializeField] private GameObject explosionPrefab; // Assign your explosion prefab
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private bool useAreaDamage = true; // Damage everything in radius?

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private float soundVolume = 1f;

    private void Start()
    {
        // Ensure Rigidbody settings are correct
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.useGravity = false;
        }
        else
        {
            Debug.LogWarning(" Bullet has no Rigidbody! Add one for collisions to work.");
        }

        // Ensure collider is NOT a trigger
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Debug.LogWarning(" Bullet collider is set to 'Is Trigger'. Turn it OFF for physical collisions!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if we should collide with this layer
        if (((1 << collision.gameObject.layer) & collisionLayers) == 0)
        {
            return; // Ignore this collision
        }

        Vector3 explosionPosition = collision.contacts[0].point;

        Debug.Log($" Bullet hit: {collision.gameObject.name} at {explosionPosition}");

        
        // SPAWN EXPLOSION EFFECT
        
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
            
            // Auto-destroy explosion after 3 seconds (adjust as needed)
            Destroy(explosion, 3f);
            
            Debug.Log(" Explosion spawned!");
        }
        else
        {
            Debug.LogWarning(" No explosion prefab assigned! Assign one in the Inspector.");
        }

        
        // PLAY EXPLOSION SOUND
        
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, explosionPosition, soundVolume);
        }

        
        // AREA DAMAGE & PHYSICS FORCE
        
        if (useAreaDamage)
        {
            ApplyExplosionDamage(explosionPosition);
        }
        else
        {
            // Single target damage only
            var enemy = collision.gameObject.GetComponent<IEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"🩸 Dealt {damage} damage to {collision.gameObject.name}");
            }
        }

        
        // DESTROY BULLET
        
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    
    // EXPLOSION AREA DAMAGE 
    
    private void ApplyExplosionDamage(Vector3 explosionPosition)
    {
        // Find all colliders in explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        Debug.Log($"💥 Explosion hit {hitColliders.Length} objects");

        foreach (Collider hit in hitColliders)
        {
            // Apply physics force
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, 1f, ForceMode.Impulse);
            }

            // Apply damage to enemies
            var enemy = hit.GetComponent<IEnemy>();
            if (enemy != null)
            {
                // Calculate damage falloff based on distance
                float distance = Vector3.Distance(explosionPosition, hit.transform.position);
                float damageFalloff = 1f - (distance / explosionRadius);
                float finalDamage = explosionDamage * Mathf.Clamp01(damageFalloff);

                enemy.TakeDamage(finalDamage);
                Debug.Log($"🩸 Explosion dealt {finalDamage:F1} damage to {hit.gameObject.name}");
            }
        }
    }

    
    // DEBUG VISUALIZATION (In Scene View)
    
    private void OnDrawGizmosSelected()
    {
        // Draw explosion radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

// Interface for any damageable entity
public interface IEnemy
{
    void TakeDamage(float damage);
}