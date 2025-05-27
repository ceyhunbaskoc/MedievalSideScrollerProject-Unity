using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    public Slider progressBar;
    public string targetSceneName;

    void Start()
    {
        progressBar.value = 0f;
        StartCoroutine(StepwiseFillSlider());
    }

    IEnumerator StepwiseFillSlider()
    {
        // 1. Ad�m: %0 -> %30, sonra dur
        yield return StartCoroutine(FillTo(0.3f, 0.5f));
        yield return new WaitForSeconds(0.7f);

        // 2. Ad�m: %30 -> %60, sonra dur
        yield return StartCoroutine(FillTo(0.6f, 0.4f));
        yield return new WaitForSeconds(1.0f);

        // 3. Ad�m: %60 -> %85, sonra dur
        yield return StartCoroutine(FillTo(0.85f, 0.6f));
        yield return new WaitForSeconds(0.5f);

        // 4. Ad�m: %85 -> %100, bitir
        yield return StartCoroutine(FillTo(1.0f, 0.3f));
        yield return new WaitForSeconds(0.2f);

        // Sahneyi y�kle
        SceneManager.LoadScene(targetSceneName);
        // veya: SceneManager.LoadScene(SceneLoader.targetScene);
    }

    // Belirli bir de�ere kadar slider'� doldurur
    IEnumerator FillTo(float target, float speed)
    {
        while (progressBar.value < target)
        {
            progressBar.value += speed * Time.deltaTime;
            if (progressBar.value > target)
                progressBar.value = target;
            yield return null;
        }
    }
}
