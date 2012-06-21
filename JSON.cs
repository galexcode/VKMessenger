using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Text;

namespace IMV
{
	public class JSON
	{
        private const byte TOKEN_NONE = 0; // ������� ��������
		private const byte BRACE_OPEN = 1; // �������� ������ ����������� (����� ������� ������)
        private const byte BRACE_CLOSE = 2; // �������� ������ �����������
        private const byte BRACKET_OPEN = 3; // ���������� ������ ����������� (����� ������� ������)
        private const byte BRACKET_CLOSE = 4; // ���������� ������ �����������
        private const byte COLON = 5; // ��������� (��� ��� ���-�������� � �������)
        private const byte COMMA = 6; // ������� (��� ������)
        private const byte STRING = 7; // ������
        private const byte NUMBER = 8; // �����

		/// <summary>
		/// ������ JSON ������
		/// </summary>
		/// <param name="json">JSON ������</param>
		/// <param name="success">������� �� ����������</param>
		/// <returns>������. ���� Hashtable, ���� ArrayList, ���� ������, ���� �����</returns>
		public static object JsonDecode(string json)
		{
			bool success = true;
			if (json != null) // ���� �� ������
            {
                char[] charArray = json.ToCharArray(); // ����������� ������ � ������ ��������
                int index = 0; // ���������� "������" �� ������� �������
				object value = ParseValue(charArray, ref index, ref success); // ������ ������
				return value;
            } 
            else 
            {
                return null;
            }
		}

        /// <summary>
        /// ����� ����������, ����� ������ �������� (������, ������, �����, ��� ������) � �������� ��������������� �������
        /// </summary>
        /// <param name="json">������ ��������, �������������� JSON ������</param>
        /// <param name="index">��������� "�������" � ������</param>
        /// <param name="success">������� �� ����������</param>
        /// <returns>������. ���� Hashtable, ���� ArrayList, ���� ������, ���� �����</returns>
        protected static object ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index)) // ������� ����� ������ ���������
            {
                case JSON.STRING: // ���� �������
                    return ParseString(json, ref index, ref success);
                case JSON.NUMBER: // ���� �����
                    return ParseNumber(json, ref index);
                case JSON.BRACE_OPEN: // ���� �������� ������
                    return ParseObject(json, ref index, ref success);
                case JSON.BRACKET_OPEN: // ���� ���������� ������
                    return ParseArray(json, ref index, ref success);
                case JSON.TOKEN_NONE: // ������ ���, ������� ��������
                    break;
            }

            success = false;
            return null;
        }

        /// <summary>
        /// ����� ������ ������ (���� ���-��������)
        /// </summary>
        /// <param name="json">������ ��������, �������������� JSON ������</param>
        /// <param name="index">��������� "�������" � ������</param>
        /// <param name="success">������� �� ����������</param>
        /// <returns>Hashtable, � ������� ��� - ����</returns>

		protected static Hashtable ParseObject(char[] json, ref int index, ref bool success)
		{
			Hashtable table = new Hashtable();
			byte token;

			NextToken(json, ref index); // ���������� �� ������� ������, ��� ��� ������ �� �������� �������� ������

			bool done = false;
			while (!done) 
            {
				token = LookAhead(json, index); // ������������� ��������� ������
				if (token == JSON.TOKEN_NONE) // ���� ������ �����������
                {
					success = false;
					return null;
				} 
                else if (token == JSON.COMMA) // ���� �������
                {
					NextToken(json, ref index); // ���������� �� ������� ������
				} 
                else if (token == JSON.BRACE_CLOSE) // ���� �������� ������ �����������
                {
					NextToken(json, ref index); // ���������� �� ������� ������
					return table; // ���������� ������� ��� ���-��������
				}
                else 
                {
					string name = ParseString(json, ref index, ref success); // ������ ���
					if (!success) 
                    {
						success = false;
						return null;
					}

					token = NextToken(json, ref index); // ���������� �� ������� ������
					if (token != JSON.COLON) // ���� �� ���������, ������ ������ � ������
                    {
						success = false;
						return null;
					}

					object value = ParseValue(json, ref index, ref success); // ������ ��������, �������������� �����
					if (!success) 
                    {
						success = false;
						return null;
					}

					table[name] = value; // ��������� ����� ������
				}
			}

			return table;
		}

        /// <summary>
        /// ����� ������ ������ (��������� ��������, ������ ������)
        /// </summary>
        /// <param name="json">������ ��������, �������������� JSON ������</param>
        /// <param name="index">��������� "�������" � ������</param>
        /// <param name="success">������� �� ����������</param>
        /// <returns>ArrayList, ������ � ������� ����������� �������� � ������� �� ���������� � ������</returns>

		protected static ArrayList ParseArray(char[] json, ref int index, ref bool success)
		{
			ArrayList array = new ArrayList();

			NextToken(json, ref index); // ���������� �� ������� ������, ��� ��� ������ �� ������� ���������� ������

			bool done = false;
			while (!done)
            {
                byte token = LookAhead(json, index); // ������������� ��������� ������
                if (token == JSON.TOKEN_NONE) // ���� ������ �����������
                {
					success = false;
					return null;
				}
                else if (token == JSON.COMMA) // ���� �������
                {
                    NextToken(json, ref index); // ���������� �� ������� ������
				}
                else if (token == JSON.BRACKET_CLOSE) // ���� ���������� ������ �����������
                {
                    NextToken(json, ref index); // ���������� �� ������� ������
                    break; // ������� �� �����
				} 
                else 
                {
					object value = ParseValue(json, ref index, ref success); // ������ �������� (��������)
					if (!success) 
                    {
						return null;
					}

					array.Add(value); // ��������� �������� � ������
				}
			}

			return array;
		}

        /// <summary>
        /// ����� ������ ������
        /// </summary>
        /// <param name="json">������ ��������, �������������� JSON ������</param>
        /// <param name="index">��������� "�������" � ������</param>
        /// <param name="success">������� �� ����������</param>
        /// <returns>������, ������� ���� ���������</returns>

		protected static string ParseString(char[] json, ref int index, ref bool success)
		{
			StringBuilder s = new StringBuilder(2000);
			char c;

			EatWhitespace(json, ref index); // ���������� �������� ���������, ������� � ������
			
			c = json[index++]; // ��������� ������, ��� ��� ����� �� �������

			bool complete = false;
			while (!complete) 
            {

				if (index == json.Length) // ���� ����� �������
                {
					break;
				}

				c = json[index++]; // ���������� ������
				if (c == '"') // ���� �������, ������ ������ �����������
                {
					complete = true;
					break;
				} 
                else if (c == '\\') // ���� �������� ����, ������ �����-�� ����������
                {

					if (index == json.Length) // ���� ����� ������, �������
                    {
						break;
					}
					c = json[index++]; // ������������� ������
					if (c == '"') // ���� �������...
                    {
						s.Append('"');
					} 
                    else if (c == '\\') 
                    {
						s.Append('\\');
					} 
                    else if (c == '/') 
                    {
						s.Append('/');
					} 
                    else if (c == 'b') 
                    {
						s.Append('\b');
					} 
                    else if (c == 'f') 
                    {
						s.Append('\f');
					} 
                    else if (c == 'n') 
                    {
						s.Append('\n');
					} 
                    else if (c == 'r') 
                    {
						s.Append('\r');
					} 
                    else if (c == 't') 
                    {
						s.Append('\t');
					}
				} 
                else 
                {
					s.Append(c); // �������������� ������� ������
				}
			}

			if (!complete) 
            {
				success = false;
				return null;
			}

			return s.ToString();
		}

        /// <summary>
        /// ����� ������ �����
        /// </summary>
        /// <param name="json">������ ��������, �������������� JSON ������</param>
        /// <param name="index">��������� "�������" � ������</param>
        /// <returns>������, ������� ���� ���������</returns>

		protected static double ParseNumber(char[] json, ref int index)
		{
			EatWhitespace(json, ref index); // ���������� ������� ���������, ������� � ������

			int lastIndex = GetLastIndexOfNumber(json, index); // ���� ������ ��������� ����� ����� � ����������
			int charLength = (lastIndex - index) + 1;
			char[] numberCharArray = new char[charLength]; // ������ ��� �������� �����

			Array.Copy(json, index, numberCharArray, 0, charLength); // �������� ������� � ��������� "�������" �� ������� ��������� ����� �����
			index = lastIndex + 1;
			return Double.Parse(new string(numberCharArray), CultureInfo.InvariantCulture); // �������� ��������� � ��� double
		}

        /// <summary>
        /// ����� ���� �������, �� ������� ������������ �����
        /// </summary>
        /// <param name="json">������ ��������, �������������� JSON ������</param>
        /// <param name="index">��������� "�������" � ������</param>
        /// <returns></returns>

		protected static int GetLastIndexOfNumber(char[] json, int index)
		{
			int lastIndex;

			for (lastIndex = index; lastIndex < json.Length; lastIndex++) // ��� � ��������� "�������" �� ����� ������
            {
                if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1) // ���� ������ "0123456789+-.eE" �� �������� �������, ������������ �� ������� lastIndex, �� ������ �����������
                {
					break;
				}
			}
			return lastIndex - 1;
		}

        /// <summary>
        /// ����� ���������� ����� ���������, �������, ����������� �������
        /// </summary>
        /// <param name="json">������ ��������, �������������� JSON ������</param>
        /// <param name="index">��������� "�������" � ������</param>

		protected static void EatWhitespace(char[] json, ref int index)
		{
			for (; index < json.Length; index++) // ����������� "������" ����...
            {
                if (" \t\n\r".IndexOf(json[index]) == -1) // ������ � �������� index ���������� � ������ " \t\n\r" 
                {
					break;
				}
			}
		}

        /// <summary>
        /// ����� ����������� ��������� ���� � ������
        /// </summary>
        /// <param name="json">������ ��������, �������������� JSON ������</param>
        /// <param name="index">��������� "�������" � ������</param>
        /// <returns>��������� �������� ���������� ����� (�������, �������� ������, ���������� ������)</returns>

		protected static byte LookAhead(char[] json, int index)
		{
			int saveIndex = index;
			return NextToken(json, ref saveIndex);
		}

        /// <summary>
        /// ����� ����������� ����� ������ ��� ��������� � ������
        /// </summary>
        /// <param name="json">������ ��������, �������������� JSON ������</param>
        /// <param name="index">��������� "�������" � ������</param>
        /// <returns>���������� ����������� ��������, ������������ ����� ������ ����� � ������ ���������</returns>

		protected static byte NextToken(char[] json, ref int index)
		{
			EatWhitespace(json, ref index); // ������� �������� ��������, �������� ��������� � �������� �������

			if (index == json.Length) // ���� ������ ����� ����� ������, ������ ������ �����������, ����� �������� 
            {
				return JSON.TOKEN_NONE;
			}
			
			char c = json[index];
			index++;
			switch (c) 
            {
				case '{': // ���� �������� ������ �����������, ���������� ��������� � ��������������� ���������
					return JSON.BRACE_OPEN;
                case '}': // ���� �������� ������ �����������, ���������� ��������� � ��������������� ���������
					return JSON.BRACE_CLOSE;
                case '[': // ���� ���������� ������ �����������, ���������� ��������� � ��������������� ���������
					return JSON.BRACKET_OPEN;
                case ']': // ���� ���������� ������ �����������, ���������� ��������� � ��������������� ���������
					return JSON.BRACKET_CLOSE;
                case ',': // ���� �� ������� �������, ���������� ��������� � ��������������� ���������
					return JSON.COMMA;
                case '"': // ���� �� ������� �������, ���������� ��������� � ��������������� ���������
					return JSON.STRING;
				case '0': case '1': case '2': case '3': case '4': 
				case '5': case '6': case '7': case '8': case '9':
                case '-': // ���� �� ������� �����, ���������� ��������� � ��������������� ���������
					return JSON.NUMBER;
                case ':': // ���� �� ������� ���������, ���������� ��������� � ��������������� ���������
					return JSON.COLON;
			}
			index--;

			return JSON.TOKEN_NONE;
		}
	}
}
