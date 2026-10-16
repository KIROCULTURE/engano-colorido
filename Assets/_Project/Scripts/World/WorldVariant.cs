using UnityEngine;

namespace EnganoColorido.World
{
    /// Color del objeto en cada mundo. En el Dorado todo se tiñe de pan de oro (sin cambiar geometría).
    [RequireComponent(typeof(Renderer))]
    public class WorldVariant : MonoBehaviour
    {
        public Color dorado = new Color(0.85f, 0.65f, 0.25f);
        Color niebla;
        Renderer r;
        MaterialPropertyBlock mpb;
        static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        void Awake()
        {
            r = GetComponent<Renderer>();
            niebla = r.sharedMaterial != null && r.sharedMaterial.HasProperty(BaseColor) ? r.sharedMaterial.GetColor(BaseColor) : Color.gray;
            mpb = new MaterialPropertyBlock();
        }

        public void Apply(float t)
        {
            if (r == null) return;
            r.GetPropertyBlock(mpb);
            mpb.SetColor(BaseColor, Color.Lerp(niebla, dorado, t));
            r.SetPropertyBlock(mpb);
        }
    }
}
