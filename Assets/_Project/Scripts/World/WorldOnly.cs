using UnityEngine;

namespace EnganoColorido.World
{
    /// Objeto que solo existe en uno de los mundos (velas e inscripciones latinas del Dorado, etc.).
    public class WorldOnly : MonoBehaviour
    {
        public bool soloDorado = true;
        public void Apply(bool dorado) => gameObject.SetActive(dorado == soloDorado);
    }
}
