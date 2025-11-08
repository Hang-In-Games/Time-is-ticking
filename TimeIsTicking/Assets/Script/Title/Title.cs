using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
   public void GoToGameScene()
   {
      SceneManager.LoadScene("Game");
   }
}
