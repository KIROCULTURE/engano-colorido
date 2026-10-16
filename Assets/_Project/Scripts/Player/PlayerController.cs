using EnganoColorido.Core;
using EnganoColorido.Interaction;
using EnganoColorido.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EnganoColorido.Player
{
    /// Controlador en primera persona: caminar, correr, agacharse, linterna, interactuar (E), celular (Tab), pausa (Esc).
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController I { get; private set; }
        public Camera cam;
        public Light linterna;
        public float caminar = 2.6f, correr = 4.2f, agachado = 1.3f;
        public float alto = 1.7f, altoAgachado = 1.0f, alcance = 2.6f;

        CharacterController cc;
        float pitch, vy;
        bool crouch;
        Interactable mirando;

        public bool Agachado => crouch;
        public bool Corriendo { get; private set; }
        public static bool Frozen => UIManager.I == null || UIManager.I.AnyModal || !GameFlow.InGameplay;

        void Awake()
        {
            I = this;
            cc = GetComponent<CharacterController>();
            gameObject.layer = 2; // Ignore Raycast: el rayo de interacción no se pega al jugador
            ApplySettings();
            Settings.Changed += ApplySettings;
        }

        void OnDestroy() { Settings.Changed -= ApplySettings; }

        void ApplySettings() { if (cam != null) cam.fieldOfView = Settings.FOV; }

        void Update()
        {
            var k = Keyboard.current;
            var m = Mouse.current;
            if (k == null) return;

            if (k.escapeKey.wasPressedThisFrame) OnEscape();
            if (!Frozen && k.tabKey.wasPressedThisFrame && !UIManager.I.JustClosed) { PhoneUI.Open(); return; }

            Vector3 move = Vector3.zero;
            Corriendo = false;
            if (!Frozen)
            {
                if (m != null)
                {
                    Vector2 d = m.delta.ReadValue() * 0.08f * Settings.Sensibilidad;
                    transform.Rotate(0, d.x, 0);
                    pitch = Mathf.Clamp(pitch - d.y, -85f, 85f);
                    cam.transform.localEulerAngles = new Vector3(pitch, 0, 0);
                }
                float x = (k.dKey.isPressed ? 1 : 0) - (k.aKey.isPressed ? 1 : 0);
                float z = (k.wKey.isPressed ? 1 : 0) - (k.sKey.isPressed ? 1 : 0);
                if (k.rightArrowKey.isPressed) x += 1; if (k.leftArrowKey.isPressed) x -= 1;
                if (k.upArrowKey.isPressed) z += 1; if (k.downArrowKey.isPressed) z -= 1;
                move = Vector3.ClampMagnitude(transform.right * x + transform.forward * z, 1f);

                if (k.cKey.wasPressedThisFrame || k.leftCtrlKey.wasPressedThisFrame) SetCrouch(!crouch);
                Corriendo = k.leftShiftKey.isPressed && !crouch && move.sqrMagnitude > 0.01f;
                float speed = crouch ? agachado : Corriendo ? correr : caminar;
                move *= speed;

                if (k.fKey.wasPressedThisFrame && linterna != null) linterna.enabled = !linterna.enabled;
            }

            vy = cc.isGrounded ? -1f : vy - 9.81f * Time.deltaTime;
            move.y = vy;
            cc.Move(move * Time.deltaTime);

            UpdateInteraction(k);
        }

        void SetCrouch(bool c)
        {
            crouch = c;
            float h = c ? altoAgachado : alto;
            cc.height = h;
            cc.center = new Vector3(0, h / 2f, 0);
            cam.transform.localPosition = new Vector3(0, h - 0.1f, 0);
        }

        void UpdateInteraction(Keyboard k)
        {
            mirando = null;
            if (Frozen) { UIManager.I?.SetPrompt(""); return; }
            var ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out var hit, alcance, ~(1 << 2), QueryTriggerInteraction.Collide))
            {
                var it = hit.collider.GetComponentInParent<Interactable>();
                if (it != null && it.enabled && it.CanInteract) mirando = it;
            }
            UIManager.I.SetPrompt(mirando != null ? "<color=#EBC261>[E]</color> " + mirando.Prompt : "");
            if (mirando != null && k.eKey.wasPressedThisFrame && !UIManager.I.JustClosed) mirando.Interact();
        }

        void OnEscape()
        {
            if (UIManager.I.JustClosed) return;
            if (PhoneUI.IsOpen) { PhoneUI.Close(); return; }
            if (!UIManager.I.AnyModal && GameFlow.InGameplay) PauseMenu.Show();
        }

        public void Teleport(Vector3 pos, float yaw)
        {
            cc.enabled = false;
            transform.position = pos;
            transform.rotation = Quaternion.Euler(0, yaw, 0);
            cc.enabled = true;
        }
    }
}
