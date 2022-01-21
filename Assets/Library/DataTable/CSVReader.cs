using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, string>> Read(string file)
    {
        var list = new List<Dictionary<string, string>>();
        TextAsset data = Resources.Load(file) as TextAsset;
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        var sb = new StringBuilder();
        bool isSkip = false;
        string line = string.Empty;
        for (var i = 1; i < lines.Length; i++)
        {
            // " ���Ե� ���ۺκ�
            if(!isSkip && lines[i].Contains("\""))
            {
                lines[i].Replace("\"", "");
                if (lines[i].Contains("\n"))
                {
                    sb.Append(lines[i]);
                    sb.Append("\n");
                }
                else
                    sb.Append(lines[i]);
                isSkip = true;

                continue;
            }
            // " ���Ե� ������ �κ�
            else if(isSkip && lines[i].Contains("\""))
            {
                lines[i].Replace("\"", "");
                sb.Append(lines[i]);
                line = sb.ToString();
                isSkip = false;
                sb.Clear();
            }
            else if(isSkip && !lines[i].Contains("\""))
            {
                if (lines[i].Contains("\n"))
                {
                    sb.Append(lines[i]);
                    sb.Append("\n");
                }
                else
                    sb.Append(lines[i]);
                continue;
            }
            else
            {
                line = lines[i];
            }
            
            var values = Regex.Split(line, SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, string>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                string finalvalue = value;
                if (int.TryParse(value, out int n))
                {
                    finalvalue = n.ToString();
                }
                else if (float.TryParse(value, out float f))
                {
                    finalvalue = f.ToString();
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
}