﻿using UnityEngine;
using System.Collections.Generic;

public class MenuLayout {

    private GUISkin gskin;
    private GUISkin _skin;

    private Texture2D bookTexture;
    private Texture2D avatarTexture;
    private Texture2D menuBackground;
    private Texture2D redArrow;

    private MenuToDraw menuToDraw;
    private delegate void MenuToDraw();

    private Rect rect;
    private Vector2 scrollPosition = Vector2.zero;
    private float def_height;

    private QuestionsDB.Question currentQuestion;
    private List<QuestionsDB.Question> questionsAnswered = new List<QuestionsDB.Question>();
    private int[] answerPower;
    private List<int> answerOrder = new List<int>();
    private int answerGiven = -1;
    private int powerClicked = -1;

    public void Start()
    {
        gskin = Resources.Load<GUISkin>("skin_default");
        _skin = Resources.Load<GUISkin>("skin_used");
        avatarTexture = Resources.Load<Texture2D>("sprites/spr_avatar");
        bookTexture = Resources.Load<Texture2D>("sprites/spr_book");
        redArrow = Resources.Load<Texture2D>("sprites/spr_red_arrow");

        // create background texture
        menuBackground = new Texture2D(61, 100);
        Color[] c = new Color[menuBackground.width * menuBackground.height];
        for (var i = 0; i < c.Length; i++) c[i] = new Color(i % 2, i % 2, i % 2, .5f);
        menuBackground.SetPixels(c); menuBackground.alphaIsTransparency = true; menuBackground.Apply();

        def_height = gskin.button.CalcSize(new GUIContent("QALDFD$%(")).y;
        _skin.button = new GUIStyle(gskin.button);
        _skin.label = new GUIStyle(gskin.label);
        _skin.box = new GUIStyle(gskin.box);

        menuToDraw = MapMenu;
    }

    public void UpperMenu()
    {
        GUI.skin = _skin;

        string[] menuOptions = new string[] { "Treinar", "Status", "Rank", "Mundo" };
        MenuToDraw[] menuFunction = new MenuToDraw[] { LearnMenu, PowerMenu, null, MapMenu };

        rect = new Rect(0, 0, Screen.width / menuOptions.Length, def_height * 1.5f);
        for (var i = 0; i < menuOptions.Length; i++) {
            if (GUI.Button(rect, menuOptions[i])) menuToDraw = menuFunction[i];
            rect.x += rect.width;
        }
        rect.x = 0;
        rect.y += rect.height;
    }

    public void PowerMenu()
    {
        DrawChessBkg();
        rect.width = Screen.width / 3;
        rect.height = (def_height + 8) * 4;
        for (var i = 0; i < 3; i++) {
            GUI.Box(rect, "");

            GUILayout.BeginArea(rect);
            GUILayout.Label(PlayerStats.power_category_name[i]);
            for (var j = 0; j < 3; j++)
                GUILayout.Label(new GUIContent(PlayerStats.power_experience[i * 3 + j] + "/" + PlayerStats.power_experience_needed[i * 3 + j], PlayerStats.power_icons[i * 3 + j]));
            GUILayout.EndArea();
            rect.x += rect.width;
        }

        rect.Set(0, rect.max.y, Screen.width, def_height);
        GUI.Box(rect, "Poder Mágico: " + PlayerStats.magic_power);
        rect.Set(0, rect.max.y, Screen.width, Screen.height - rect.max.y);
        GUI.DrawTexture(rect, avatarTexture, ScaleMode.ScaleToFit);
    }

    public void LearnMenu()
    {
        DrawChessBkg();
        if (currentQuestion != null) {
            // draw question
            var pad = Screen.width * .2f;
            rect.Set(pad, rect.y, Screen.width - pad * 2, 1);
            rect.height = GUI.skin.box.CalcHeight(new GUIContent(currentQuestion.text), rect.width);
            GUI.Box(rect, currentQuestion.text);
            rect.y += rect.height;

            // draw answers
            rect.Set(0, rect.y, Screen.width, Screen.height / 2);
            GUI.Box(rect, "");
            GUILayout.BeginArea(rect);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(Screen.height / 2));
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            for (var i = 0; i < currentQuestion.answer.Length; i++) {
                GUI.skin.label.normal.textColor = answerPower[answerOrder[i]] < 0 ? Color.gray : Color.black;
                var ico = answerPower[answerOrder[i]] < 0 ? null : PlayerStats.power_icons[answerPower[answerOrder[i]]];
                GUILayout.Label(new GUIContent(currentQuestion.answer[answerOrder[i]], ico));
            }
            GUI.skin.label.normal.textColor = gskin.label.normal.textColor;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            rect.y += rect.height;

            // draw book and power symbols
            float x = Screen.width - bookTexture.width, y = Screen.height - bookTexture.height;
            float x2 = bookTexture.width / 4, y2 = bookTexture.height / 3;
            float s = PlayerStats.power_icons[0].width * 2.3f;
            bookTexture.DrawAt(x, y);

            if (PlayerStats.power_icons[currentQuestion.type * 3 + 0].DrawButton(x + x2 * 1 - s / 2, y + y2 - s / 2, 2.3f))
                powerClicked = currentQuestion.type * 3 + 0;
            if (PlayerStats.power_icons[currentQuestion.type * 3 + 1].DrawButton(x + x2 * 3 - s / 2, y + y2 - s / 2, 2.3f))
                powerClicked = currentQuestion.type * 3 + 1;
            if (PlayerStats.power_icons[currentQuestion.type * 3 + 2].DrawButton(x + x2 * 2 - s / 2, y + y2 * 2 - s / 2, 2.3f))
                powerClicked = currentQuestion.type * 3 + 2;
            // debug draw reroll button
            //if (GUI.Button(new Rect(0,-def_height,Screen.width,def_height), "Next Question")) currentQuestion = null;
        }
        // draw avatar
        avatarTexture.DrawAt(0, Screen.height - avatarTexture.height);
    }

    private void MapMenu()
    {
        /*
        var eTrans = GameObject.Find("AllEnemies").GetComponentsInChildren<Transform>();
        rect.Set(0, rect.y, Screen.width, Screen.height - rect.y);
        foreach (Transform t in eTrans where ) {
            var e = Camera.main.WorldToScreenPoint(t.position);
            print(e.ToString());
            if (!rect.Contains(e)) {
                GUI.matrix = Matrix4x4.Rotate(Quaternion.Euler(Vector2.Angle(rect.center, e), 0, 0));
                GUI.DrawTexture(new Rect(e.x < rect.x ? rect.x : rect.max.x, e.y < rect.y ? rect.y : rect.max.y, 16, 16), redArrow);
            }
        }
        GUI.matrix = Matrix4x4.identity;
        GUI.DrawTexture(new Rect(rect.x,rect.y,16,16), redArrow);
        */
        BattleController.OnGUI();
    }

    public void OnGUI() {
        UpperMenu();
        if (menuToDraw != null) {
            
            menuToDraw.Invoke();
        }
    }

    private void DrawChessBkg() => GUI.DrawTexture(new Rect(0, rect.y, Screen.width, Screen.height - rect.y), menuBackground);

    public void Update()
    {
        if (powerClicked >= 0) {
            var answerGiven = GetAnswer(ref answerPower, powerClicked);
            if (answerGiven == -2) answerPower = GiveAnswerPower(currentQuestion.type); // no answer corresponds to the choosen symbol
            if (answerGiven >= 0) {
                if (answerGiven == 1) {
                    PlayerStats.power_experience[answerPower[0]] += currentQuestion.difficulty;
                    currentQuestion.wasRight = true;
                } else currentQuestion.wasRight = false;
                questionsAnswered.Add(currentQuestion);
                QuestionsDB.Remove(currentQuestion);
                currentQuestion = null;
            }
            powerClicked = -1;
        }
        if (currentQuestion == null) { // get new question and randomize answer power icon
            currentQuestion = QuestionsDB.GetRandomQuestion();
            if (currentQuestion != null) {
                answerPower = GiveAnswerPower(currentQuestion.type);
                answerOrder.Clear();
                for (var i = 0; i < currentQuestion.answer.Length; i++) answerOrder.Add(i);
                answerOrder = answerOrder.randomized();
            }
        }
    }

    /// <summary>If no powerid is given, all answers will have new powers given</summary>
    private int[] GiveAnswerPower(int type)
    {
        var l = new List<int>() { type * 3, type * 3 + 1, type * 3 + 2 };
        l = l.randomized();
        l.AddRange(new int[] { type * 3 + Random.Range(0, 3), type * 3 + Random.Range(0, 3) });
        return l.randomized().ToArray();
    }

    /// <summary>
    /// Returned values
    /// -2: No answer match that symbol
    /// -1: erease all non-matching symbols from answer and randomize the remaining
    ///  0: Got the wrong answer
    ///  1: Got the right answer
    /// </summary>
    private int GetAnswer(ref int[] currAPowers, int powerid)
    {
        var type = Mathf.FloorToInt(powerid / 3) * 3;
        var togive = new List<int>() { type, type + 1, type + 2 };
        for (var i = 0; i < currAPowers.Length; i++) currAPowers[i] = currAPowers[i] == powerid ? togive.PopRandom() : -1;
        return togive.Count == 2 ? currAPowers[0] >= 0 ? 1 : 0 : togive.Count == 3 ? -2 : -1; // return true if there was only one answer with said power
    }

        
}