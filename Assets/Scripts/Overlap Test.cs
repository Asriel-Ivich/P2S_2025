using UnityEngine;

public class OverLapTest : MonoBehaviour
{
    //Las dimensiones del rectangulo
    public Vector2 dimensiones;

    private Color color = Color.white;

    //El update se ejecuta cada Frame
    private void Update()
    {
        //El centro del rectangulo va a ser la posicion actual
        Vector2 centro= transform.position;

        //El angulo en de rotacion
        float angulo = 0;

        //Creamos el filtro de la capa
        LayerMask capa = LayerMask.GetMask("Default");

        // Proyectamos un rectangulo imaginario y nos va a regresar los Colliders que esten dentro
        Collider2D[] arreglo = Physics2D.OverlapBoxAll( point: centro, size: dimensiones, angulo, capa);

        //Obtenemos el tama;o del arreglo
        //Osea..cuantos Collider/Objetos hay dentro del rectangulo
        int collidersDentro = arreglo.Length;

        //Si collidersDentro es mayor que 0: es magenta, de lo contrario es Blanco
        color = collidersDentro > 0 ? Color.magenta : Color.white;
    }

    //es un metodo de unity para dibujar gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(center:transform.position, size:dimensiones); 
    }
}
