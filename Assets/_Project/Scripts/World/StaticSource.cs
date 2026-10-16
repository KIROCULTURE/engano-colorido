using System.Collections.Generic;
using UnityEngine;

namespace EnganoColorido.World
{
    /// Algo que hace sonar estática en el celular de Ale cuando está cerca (radios, el velo, el espejo;
    /// en la Entrega 2, Las Horas).
    public class StaticSource : MonoBehaviour
    {
        public static readonly List<StaticSource> All = new();
        public float radio = 6f;
        public float fuerza = 0.6f;
        void OnEnable() => All.Add(this);
        void OnDisable() => All.Remove(this);
    }
}
