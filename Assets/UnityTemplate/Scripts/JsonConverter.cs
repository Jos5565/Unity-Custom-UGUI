using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO; // Newtonsoft 사용
public class JsonConverter : EditorWindow
{
    private string sourcePath;

    [MenuItem("JsonConverter/Converter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(JsonConverter), false, "Json Converter");
    }

    private void OnGUI()
    {
        DrawOpenFilePanel();
    }
    private void DrawOpenFilePanel()
    {
        EditorGUILayout.BeginHorizontal();
        sourcePath = EditorGUILayout.TextField("Json File Path", sourcePath);
        if (GUILayout.Button("Open"))
        {
            sourcePath = EditorUtility.OpenFilePanel("Select Folder", "", ".json");
        }
        EditorGUILayout.EndHorizontal();
        sourcePath = EditorGUILayout.TextField("Json String", sourcePath);
        if (string.IsNullOrEmpty(sourcePath)) return;

        if (GUILayout.Button("parse"))
        {
            Debug.Log("Parse Start");
            string json = File.ReadAllText(sourcePath);
            ParseJson(json);
            Debug.Log($"Parse Start {sourcePath}");
        }
    }

    private void ParseJson(string json)
    {
        ArbitraryJsonContainer container = ParseArbitraryJson(json);

        // 결과 출력
        Debug.Log("--- 파싱된 JSON 데이터 (Newtonsoft.Json) ---");
        foreach (var item in container.Items)
        {
            Debug.Log($"필드명: {item.FieldName,-15} | 타입: {item.DataType,-10} | 값: {item.Value}");
        }
    }
    private ArbitraryJsonContainer ParseArbitraryJson(string jsonString)
    {
        var container = new ArbitraryJsonContainer();

        // JSON 문자열을 JObject 트리 형태로 변환
        JObject parsedObj = JObject.Parse(jsonString);

        // 최상위 객체의 속성(Properties)들을 하나씩 순회
        foreach (JProperty prop in parsedObj.Properties())
        {
            var item = new JsonItem
            {
                FieldName = prop.Name,
                DataType = prop.Value.Type.ToString(), // JTokenType을 문자열로 저장
                Value = ExtractValue(prop.Value)       // 타입에 맞게 값을 추출
            };

            container.Items.Add(item);
        }

        return container;
    }

    // JToken의 타입에 맞춰 실제 C# 자료형으로 안전하게 변환해 주는 함수
    private object ExtractValue(JToken token)
    {
        // JTokenType에 따라 분기 처리
        switch (token.Type)
        {
            case JTokenType.String:
                return token.ToString();
            case JTokenType.Integer:
                return token.ToObject<int>(); // 필요하다면 long으로 변경 가능
            case JTokenType.Float:
                return token.ToObject<double>();
            case JTokenType.Boolean:
                return token.ToObject<bool>();
            case JTokenType.Array:
            case JTokenType.Object:
                // 내부 배열이나 객체는 줄바꿈 없는 순수 JSON 문자열 원본으로 저장
                return token.ToString(Newtonsoft.Json.Formatting.None);
            case JTokenType.Null:
                return null;
            default:
                return token.ToString(); // 그 외의 경우는 문자열로 안전하게 처리
        }
    }
}



public class JsonItem
{
    public string FieldName { get; set; }  // JSON의 키 (예: "squadName", "formed")
    public object Value { get; set; }      // 실제 값 (문자열, 숫자, 배열 등)
    public string DataType { get; set; }   // 값의 타입 (String, Number, Array, Object 등)
}

// 위 아이템들을 리스트로 묶어서 관리할 전체 컨테이너 클래스
public class ArbitraryJsonContainer
{
    public List<JsonItem> Items { get; set; } = new List<JsonItem>();
}