﻿using UnityEngine;
using System.Collections.Generic;

public static class PlayerStats
{
    public static int magic_power = 150;
    public static int[] power_experience = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public static int[] power_experience_needed = new int[9] { 20, 20, 20, 20, 20, 20, 20, 20, 20 };
}

public static class iPower {
    public static string[] type_name = new string[] { "Humanas", "Exatas", "Ciencias" };
    public static Sprite[] icon;

    public static int GetRandom { get => Random.Range(0, 9); }

    public static void Init()
    {
        icon = Resources.LoadAll<Sprite>("sprites/powers");
    }
}

public static class QuestionsDB
{
    private static bool initialized = false;

    public class Question
    {
        public int id;
        public int type;
        public int difficulty;
        public string text;
        public string[] answer; // NOTE: the first answer in the array MUST be the right one
        public bool wasRight;

        public Question(int type, int difficulty, string text,string[] answer) {
            this.type = type; this.difficulty = difficulty; this.text = text; this.answer = answer;
        }
    }

    private static List<Question>[] questions = new List<Question>[3];

    /// <summary>nothing=random, h=humanas, e=exatas, c=ciencias</summary>
    public static Question GetRandomQuestion(char type = 'r') {
        var t = type == 'r' ? Random.Range(0, 3) : GetTypeId(type);
        if (questions[t].Count < 1) return null;
        var id = Random.Range(0, questions[t].Count);
        questions[t][id].id = id; // current id in the list
        return questions[t][id]; // random question of type t
    }

    public static void Remove(Question q) => questions[q.type].RemoveAt(q.id);
    private static int GetTypeId(char c) => "hec".IndexOf(c);

    public static void Init() // populate database
    {
        if (initialized) return;
        initialized = true;
        for (var i = 0; i < 3; i++) questions[i] = new List<Question>();

        FillDB();
    }

    // test
    public static void FillDB()
    {
        AddQuestion('h', 1, "H1 Qual é a resposta desta pergunta?", "Resposta Certa", "Resposta \nErrada", "Resposta nada a ver", "mais uma resposta errada uma frase aqui muito grande e tal");
        AddQuestion('e', 1, "E1 Qual é a resposta desta pergunta?", "Resposta Certa", "Resposta \nErrada");
        AddQuestion('c', 1, "C1 Qual é a resposta desta pergunta?", "Resposta Certa", "Resposta \nErrada", "Resposta nada a ver", "mais uma resposta errada uma frase aqui muito grande e tal", "cinco respostas \nserase 5 ta bom???? \nheiiiin? rs legal \ntrestsioasjdioaj \noi");
        AddQuestion('h', 1, "H2 Qual é a resposta desta pergunta?", "Resposta Certa", "Resposta \nErrada", "Resposta nada a ver", "mais uma resposta errada uma frase aqui muito grande e tal");
    }

    private static void AddQuestion(char type, int difficulty, string text,params string[] answer) {
        if (answer.Length > 5) Debug.LogWarning("Adding a Question with more then 5 answers may not show them correctly");
        questions[GetTypeId(type)].Add(new Question(GetTypeId(type), difficulty, text, answer));
    }
}

public class Timer {
    public float time = 0;
    public void Reset() => time = 0;

    public virtual bool this[float t] {
        get { if ((time += Time.deltaTime) >= t) {
                time -= t;
                return true;
            } else return false;
        }
    }
}

public class Cooldown : Timer { public override bool this[float t]
        { get => time >= t ? true : (time += Time.deltaTime) >= t; } }

public static class ExtensionTools {

    //gui extensions
    public static void DrawAt(this Texture2D t, float x, float y, float scale = 1)
        => GUI.DrawTexture(new Rect(x, y, t.width * scale, t.height * scale), t);

    public static bool DrawButton(this Texture2D t, float x, float y, float scale = 1)
        { var b = GUI.Button(new Rect(x, y, t.width * scale, t.height * scale), ""); t.DrawAt(x, y, scale); return b; }

    public static Rect Centralize(this Rect r) => new Rect(r.position-r.size/2,r.size);

    public static Sprite sprite(this Texture2D t) => Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);

    // list extensions
    public static T Pop<T>(this List<T> list, int id)
        { var v = list[id]; list.RemoveAt(id); return v; }

    public static T PopRandom<T>(this List<T> list) => list.Pop(Random.Range(0, list.Count));

    public static List<T> randomized<T>(this List<T> list)
        { var l = new List<T>(); while (list.Count > 0) l.Add(list.PopRandom()); return l; } 
}