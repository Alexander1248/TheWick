using NPC;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Dialogue = Plugins.DialogueSystem.Scripts.DialogueGraph.Dialogue;

public class CSManager : MonoBehaviour
{
    private int cntr;
    [SerializeField] private PlayableDirector playableDirector;
     [SerializeField] private Animator father;

    [SerializeField] private Animator npc2_1;


    [SerializeField] private Animator[] npcs_room1;
    [SerializeField] private Animator[] npcs_room2;
    [SerializeField] private Animator[] npcs_room3;

    [SerializeField] private Dialogue dialogue;

    void Start(){
        Invoke("startGame", 0.5f);

        for(int i = 0; i < npcs_room1.Length; i++){
            playIdleNpc(npcs_room1[i]);
        }
        for(int i = 0; i < npcs_room2.Length; i++){
            playIdleNpc(npcs_room2[i]);
        }
        for(int i = 0; i < npcs_room3.Length; i++){
            playIdleNpc(npcs_room3[i]);
        }
    }

    void playIdleNpc(Animator animatorNpc){
        if (animatorNpc.name.Contains("Npc1")) animatorNpc.CrossFade("FatherIdle", 0.5f, -1, 0);
        else if (animatorNpc.name.Contains("Npc2")) animatorNpc.CrossFade("FatherSitting", 0.5f, -1, 0);
        else if (animatorNpc.name.Contains("Npc3")) animatorNpc.CrossFade("FatherHalfSitting", 0.5f, -1, 0);
    }
    
    void playTalkNpc(Animator animatorNpc){
        if (animatorNpc.name.Contains("Npc1")) animatorNpc.CrossFade("FatherTalk", 0.5f, -1, 0);
        else if (animatorNpc.name.Contains("Npc2")) animatorNpc.CrossFade("FatherSittingDialogue", 0.5f, -1, 0);
        else if (animatorNpc.name.Contains("Npc3")) animatorNpc.CrossFade("FatherHalfSittingTalk", 0.5f, -1, 0);
    }

    void startGame()
    {
        Invoke(nameof(PlayDirector), 4);
        dialogue.StartDialogue("Intro");
        //Debug.Log("(Отец)-И вот мы пришли! Главное ничего не трогай, а то я знаю, какая ты у меня. Один раз мне все гайки из лестницы выкрутила, так папаня и полетел с неё, хаха.");
    }

    private void PlayDirector()
    {
        playableDirector.Play();
    }

    public void getSignal(int id){
        if (cntr == 0){
            dialogue.StartDialogue("Enter");
            //Debug.Log("(Мужик)- Доброго дня! Костя, иж ты, дочку взял с собой?  (Отец)-Доброго! Просилась ужасно, вот и решил показать. (Мужик)- Проходите тогда, коль не шутишь");
        }
        else if (cntr == 1){
            dialogue.StartDialogue("Mono");
            //Debug.Log("(Отец)- А вот и Великий Царицынский Цех, построенный по указу Царя Всея Руси Николая Второго в 1930 году!(Отец)- Здесь и собирают все свечи, озаряющие наш Царицын! (Отец)- Коли желаешь посмотреть, понаблюдать за работой, можешь погулять. Как насмотришься, скажи мне.(Отец)- Только не отвлекай никого!");
            npc2_1.CrossFade("FatherSitting", 0.5f, -1, 0);
        }
        else if (cntr == 2){
            //Debug.Log("(Отец)- Если хочешь, можешь походить по кабинетам, только не мешай");

        }
        else if (cntr == 3){
            // конец катсцены
            // с отцом можно поговорить (да / нет)
            father.gameObject.tag = "InteractMe";

            father.CrossFade("FatherIdle", 0.5f, -1, 0);
        }
        cntr++;
    }

    public void playerEnteredRoom(int idroom){
        if (idroom == 1){
            for(int i = 0; i < npcs_room1.Length; i++){
                playTalkNpc(npcs_room1[i]);
            }
            dialogue.StartDialogue("Room 1");
            // Debug.Log("(Мужики)- Эх, представьте, Алёше, цесаревичу нашему, коль поди 30 наступило!(Мужики)- Боже! Цесаревича храни! (Мужики)- Надеюсь, что будет таким же, как и папка. Авось, братцы, Аляску вернёт!");
        
        }
        else if (idroom == 2){
            for(int i = 0; i < npcs_room2.Length; i++){
                playTalkNpc(npcs_room2[i]);
            }
            dialogue.StartDialogue("Room 2");
            // Debug.Log("(Мужики)- Девочка, а ну кыш отсюда, у нас тут сановитые дела");

        }
        else if (idroom == 3){
            for(int i = 0; i < npcs_room3.Length; i++){
                playTalkNpc(npcs_room3[i]);
            }
            dialogue.StartDialogue("Room 3");
            // Debug.Log("(Мужики)- Ох, представь, как получка придёт, поедем в бричке кутить по центральным улочкам!(Мужики)- Да уж... В прошлый раз после такой ночи мы оказались где-то в Вятке...(Мужики)- Ой, не начинай...");
        
        }
    }

    public void TryJumpToNextLevel(string tag)
    {
        if (tag != "next") return;
        SceneManager.LoadScene("KOSTYA/FACTORY_2");
    }
}
