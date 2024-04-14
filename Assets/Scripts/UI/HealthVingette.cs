using UnityEngine;

namespace UI
{
    public class HealthVignette : MonoBehaviour
    {
        [SerializeField] private Material healthVis;
        private static readonly int Radius = Shader.PropertyToID("_Radius");

        public void ChangeVisual(float hp, float maxHP)
        {
            healthVis.SetFloat(Radius, Mathf.Clamp01(hp / maxHP));
        }

        public void ClearVignette()
        {
            healthVis.SetFloat(Radius, 1);
        }
    }
}