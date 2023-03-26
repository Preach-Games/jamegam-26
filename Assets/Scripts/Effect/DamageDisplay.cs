using System.Collections;
using TMPro;
using UnityEngine;

namespace DungeonDraws.Effect
{
    public class DamageDisplay : MonoBehaviour
    {
        public void Init(int value)
        {
            var text = GetComponent<TMP_Text>();
            text.text = $"{value}";
            text.color = value < 0 ? Color.green : Color.red;
            StartCoroutine(WaitAndDestroy());
        }

        private void Update()
        {
            transform.Translate(Vector3.up * Time.deltaTime);
        }

        private IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}
