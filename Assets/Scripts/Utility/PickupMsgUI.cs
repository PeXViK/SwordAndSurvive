using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class PickupMsgUI : MonoBehaviour
    {
        public static PickupMsgUI Instance { get; private set; }

        [SerializeField] private GameObject prompt;

        private void Awake()
        {
            Instance = this;

            if (prompt != null)
                prompt.SetActive(false);
        }

        public void Show()
        {
            if (prompt != null)
                prompt.SetActive(true);
        }

        public void Hide()
        {
            if (prompt != null)
                prompt.SetActive(false);
        }
    }
}