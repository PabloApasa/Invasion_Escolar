using UnityEngine;

public class Animacion_Player : MonoBehaviour
{
    // Instancia estática para acceder de forma sencilla desde otros scripts
    public static Animacion_Player Instance { get; private set; }

    private Animator animator;

    // Aquí guardamos los hashes de los parámetros. 
    // Es mucho más eficiente a nivel de rendimiento que usar Strings en cada frame.
    private int horizontalHash;
    private int verticalHash;
    private int isWalkingHash;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        animator = GetComponent<Animator>();

        // Optimizamos los nombres de los parámetros convirtiéndolos a Hashes
        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");
        isWalkingHash = Animator.StringToHash("IsWalking");
    }

    public static void UpdateMovementAnimation(Vector2 movementVector)
    {
        if (Instance == null) return;

        // Si el vector no es cero, significa que se está moviendo
        bool isWalking = movementVector.sqrMagnitude > 0.01f;

        // Enviamos el booleano para cambiar entre los estados "Idle" y "Walk"
        Instance.animator.SetBool(Instance.isWalkingHash, isWalking);

        // Si se está moviendo, actualizamos la dirección en el Blend Tree
        // Nota: Si se detiene, no actualizamos la dirección para que el personaje
        // se quede mirando hacia el último lado al que caminó.
        if (isWalking)
        {
            Instance.animator.SetFloat(Instance.horizontalHash, movementVector.x);
            Instance.animator.SetFloat(Instance.verticalHash, movementVector.y);
        }
    }
}