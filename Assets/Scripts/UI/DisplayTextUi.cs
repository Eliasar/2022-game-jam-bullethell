using Ktyl.Util;
using TMPro;
using UnityEngine;

namespace Confined
{
    public class DisplayTextUi : MonoBehaviour
    {
        [SerializeField] private SerialFloat DisplayText;
        private TextMeshProUGUI text;

        void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            text.SetText(DisplayText.Value.ToString());
        }
    }
}

